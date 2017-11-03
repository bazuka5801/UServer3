using System;
using System.Collections.Generic;
using System.Threading;
using ProtoBuf;
using RakNet.Network;
using SapphireEngine;
using UnityEngine;
using UServer3.Environments;
using UServer3.Rust.Functions;
using UServer3.Rust.Network;
using UServer3.Rust.Struct;
using UServer3.CSharp.Reflection;
using  UServer3.CSharp.ExtensionMethods;
using UServer3.Rust.Data;

namespace UServer3.Rust
{
    public class BasePlayer : BaseCombatEntity
    {
        public static List<BasePlayer> ListPlayers = new List<BasePlayer>();
        
        public static BasePlayer LocalPlayer = null;
        public static bool IsHaveLocalPlayer => LocalPlayer != null;
        
        public UInt64 SteamID;
        public String Username;
        public BaseHeldEntity ActiveItem;
        public E_PlayerFlags PlayerFlags;
        public bool IsServerAdmin = false;
        
        public ModelState ModelState;
        public bool IsSleeping => this.HasPlayerFlag(E_PlayerFlags.Sleeping);
        public bool IsWounded => this.HasPlayerFlag(E_PlayerFlags.Wounded);
        public bool CanInteract() => !base.IsDead && !this.IsSleeping && !this.IsWounded;
        public bool HasActiveItem => this.ActiveItem != null;
        public bool IsLocalPlayer => this == LocalPlayer;
        
        public bool IsDucked => ModelState.ducked;
        public float GetHeight() => IsDucked ? 0.7f : 1.5f;
        
        public Vector3 ViewAngles = Vector3.zero;
        public Vector3 EyePos = Vector3.zero;
        

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            ListPlayers.Add(this);
            if (SteamID == VirtualServer.ConnectionInformation.SteamID)
            {
                LocalPlayer = this;
            }
        }
        
        public override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            ListPlayers.Remove(this);
            if (SteamID == VirtualServer.ConnectionInformation.SteamID)
            {
                LocalPlayer = this;
            }
        }

        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
            if (entity.basePlayer != null)
            {
                SteamID = entity.basePlayer.userid;
                Username = entity.basePlayer.name;
                if (entity.basePlayer.modelState != null)
                    ModelState = entity.basePlayer.modelState.Copy();
                if (HasPlayerFlag(E_PlayerFlags.IsAdmin))
                    IsServerAdmin = true;
                if (entity.basePlayer.heldEntity == 0)
                    this.ActiveItem = null;
                else if ((this.ActiveItem == null || this.ActiveItem.UID != entity.basePlayer.heldEntity + 1) &&
                         HasNetworkable(entity.basePlayer.heldEntity + 1))
                {
                    OnChangeActiveItem(entity.basePlayer.heldEntity);
                }
            }
        }


        public void OnChangeActiveItem(UInt32 activeItem)
        {
            this.ActiveItem = (BaseHeldEntity)ListNetworkables[activeItem + 1];
            if (this.IsLocalPlayer && this.HasActiveItem)
            {
                ConsoleSystem.Log("You use: " + this.ActiveItem.PrefabID);
            }
        }

        #region [Example] [Method] GetForward
        public Vector3 GetForward()
        {
//                                   Math.PI * this.Rotation.y / 180.0
//            float angle = (float) (this.Rotation.y * 0.01745329251f);
//            return new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
            return ViewAngles.ToQuaternion() * Vector3.forward;
        }
        #endregion

        #region [NetworkMessage] Tick
        private PlayerTick previousTick;
        private PlayerTick previousRecievedTick = new PlayerTick();
        private bool lastFlying = false;

        public bool OnTick(Message packet)
        {
            using (PlayerTick playerTick = PlayerTick.Deserialize(packet.read, previousRecievedTick, true))
            {
                previousRecievedTick = playerTick.Copy();

                ViewAngles = playerTick.inputState.aimAngles;
                EyePos = playerTick.eyePos;

                if (IsServerAdmin) return false;
                if (playerTick.modelState.flying)
                {
                    playerTick.modelState.flying = false;

                    lastFlying = true;
                }
                else
                {
                    if (lastFlying) previousTick.modelState.flying = true;
                    lastFlying = false;
                }

                if (VirtualServer.BaseClient.write.Start())
                {
                    VirtualServer.BaseClient.write.PacketID(Message.Type.Tick);
                    playerTick.WriteToStreamDelta(VirtualServer.BaseClient.write, previousTick);
                    previousTick = playerTick.Copy();

                    VirtualServer.BaseClient.Send();
                }
                return true;
            }
        }
        #endregion

        #region [RPCMethod] StartLoading
        [RPCMethod(ERPCMethodUID.StartLoading)]
        private bool RPC_StartLoading(ERPCNetworkType type, Message message)
        {
            ConsoleSystem.Log("StartLoading");
            
            BaseNetworkable.DestroyAll();
            
            ListNetworkables.Add(this.UID, this);
            ListPlayers.Add(this);

            return false;
        }
        #endregion

        #region [RPCMethod] OnProjectileAttack
        [RPCMethod(ERPCMethodUID.OnProjectileAttack)]
        private bool RPC_OnProjectileAttack(ERPCNetworkType type, Message message)
        {
            EHumanBone GetTargetHit(EHumanBone currentBone)
            {
                if (Settings.Aimbot_Range_AutoHeadshot) 
                    return EHumanBone.Head;
                if (currentBone == EHumanBone.Head) return EHumanBone.Head;
                // Head or Body
                if (currentBone == EHumanBone.Body) return OpCodes.GetRandomHumanBone(1);
                // Head or Body or Legs
                return OpCodes.GetRandomHumanBone(2);
            }
            using (PlayerProjectileAttack attack = PlayerProjectileAttack.Deserialize(message.read))
            {
                UInt32 hitId = attack.playerAttack.attack.hitID;
                UInt32 hitBone = attack.playerAttack.attack.hitBone;
                var hitPlayer = Get<BasePlayer>(hitId);
                if (Settings.Aimbot_Range_Silent && (hitId == 0 ||
                    HasNetworkable(hitId) == false ||
                    hitPlayer == null))
                {
                    if (RangeAim.Instance.TargetPlayer != null)
                    {
                        EHumanBone typeHit = GetTargetHit(0);
                        ConsoleSystem.Log(typeHit.ToString());
                        return this.SendRangeAttack(RangeAim.Instance.TargetPlayer, typeHit,
                            attack);
                    }
                }
                if (hitPlayer && hitPlayer.IsAlive)
                {
                    RangeAim.Instance.TargetPlayer =
                        (BasePlayer) ListNetworkables[hitId];

                    EHumanBone typeHit = GetTargetHit((EHumanBone)hitBone);
                    ConsoleSystem.Log(typeHit.ToString());
                    return this.SendRangeAttack(RangeAim.Instance.TargetPlayer, typeHit,
                        attack);
                }
            }
            return false;
        }
        #endregion

        #region [RPCMethod] OnPlayerLanded
        [RPCMethod(ERPCMethodUID.OnPlayerLanded)]
        private bool RPC_OnPlayerLanded(ERPCNetworkType type, Message message)
        {
            if (Settings.SmallFallDamage == false) return false;
            var fallVelocity = message.read.Float();
            if (fallVelocity < -8f)
            {
                if (VirtualServer.BaseClient.write.Start())
                {
                    VirtualServer.BaseClient.write.PacketID(Message.Type.RPCMessage);
                    VirtualServer.BaseClient.write.EntityID(UID);
                    VirtualServer.BaseClient.write.UInt32((UInt32)ERPCMethodUID.OnPlayerLanded);
                    VirtualServer.BaseClient.write.Float(Rand.Float(-15.5f, -15.1f));
                    VirtualServer.BaseClient.Send();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region [Method] SendRangeAttack
        public bool SendRangeAttack(BasePlayer target, EHumanBone typeHit,
            PlayerProjectileAttack parentAttack)
        {
            #region [Section] Attack Deploy

            float distance_point = Vector3.Distance(target.Position, parentAttack.playerAttack.attack.hitPositionWorld);
            if (distance_point > 3)
            {
                float deploy = 13;

                if (this.ActiveItem != null)
                {
                    if (OpCodes.IsRangeDeploy((EPrefabUID) this.ActiveItem.PrefabID))
                    {
                        //ConsoleSystem.Log("This is range weapone");
                        deploy = OpCodes.GetRangeDeploy((EPrefabUID) this.ActiveItem.PrefabID);
                    }
                    else if (OpCodes.IsMeleeWeapon_Prefab((EPrefabUID) this.ActiveItem.PrefabID) == false)
                    {
                        //ConsoleSystem.Log("This is fire weapone");
                        if (distance_point > 50)
                            deploy = 0.4f;
                        else
                            deploy = 0;
                    }
                    else
                    {
                        //ConsoleSystem.Log("This is mele weapone");
                    }
                }
                int sleep_mlisecond = (int) (distance_point * deploy);
                if (sleep_mlisecond > 1000)
                    return false;
                Thread.Sleep(sleep_mlisecond);
            }
            #endregion

            if (target.IsAlive)
            {
                parentAttack.hitDistance = Vector3.Distance(target.Position, BasePlayer.LocalPlayer.Position);
                var hitInfo = OpCodes.GetTargetHitInfo(typeHit);
                parentAttack.playerAttack.attack.hitBone = hitInfo.HitBone;
                parentAttack.playerAttack.attack.hitPartID = hitInfo.HitPartID;
                parentAttack.playerAttack.attack.hitNormalLocal = hitInfo.HitNormalPos;
                parentAttack.playerAttack.attack.hitPositionLocal = hitInfo.HitLocalPos;
                parentAttack.playerAttack.attack.hitID = target.UID;

                float height = this.GetHeight();
                parentAttack.playerAttack.attack.hitPositionWorld = target.Position + new Vector3(0, height, 0);
                parentAttack.playerAttack.attack.hitNormalWorld = target.Position + new Vector3(0, height, 0);
                parentAttack.playerAttack.attack.pointEnd = target.Position + new Vector3(0, height, 0);


                VirtualServer.BaseClient.write.Start();
                VirtualServer.BaseClient.write.PacketID(Message.Type.RPCMessage);
                VirtualServer.BaseClient.write.UInt32(this.UID);
                VirtualServer.BaseClient.write.UInt32((uint) ERPCMethodUID.OnProjectileAttack);
                PlayerProjectileAttack.Serialize(VirtualServer.BaseClient.write, parentAttack);
                VirtualServer.BaseClient.write.Send(new SendInfo(VirtualServer.BaseClient.Connection));
            }

            return true;
        }

        #endregion

        #region [Method] FindEnemy
        public static BasePlayer FindEnemy(float radius)
        {
            BasePlayer nearPlayer = null;
            Single min_distance = Single.MaxValue;
            for (int i = 0; i < ListPlayers.Count; i++)
            {
                var player = ListPlayers[i];
                if (player == LocalPlayer) continue;
                if (player.Health <= 0) continue;
                var distance = Vector3.Distance(LocalPlayer.Position, player.Position);
                if (!Settings.IsFriend(player.SteamID) && distance < min_distance)
                {
                    min_distance = distance;
                    nearPlayer = player;
                }
            }
            return min_distance <= radius ? nearPlayer : null;
        }
        #endregion
        
        #region [Method] SetAdminStatus
        public void SetAdminStatus(bool status)
        {
            if (IsServerAdmin) return;
            this.SetPlayerFlag(E_PlayerFlags.IsAdmin, status);


            if (VirtualServer.BaseServer.write.Start())
            {
                var entity_packet = new Entity
                {
                    baseNetworkable = new ProtoBuf.BaseNetworkable
                    {
                        group = this.GroupID,
                        prefabID = this.PrefabID,
                        uid = this.UID
                    },
                    basePlayer = new ProtoBuf.BasePlayer
                    {
                        userid = this.SteamID,
                        heldEntity = this.HasActiveItem ? this.ActiveItem.UID - 1 : 0,
                        playerFlags = (int) this.PlayerFlags
                    },
                    baseEntity = new ProtoBuf.BaseEntity
                    {
                        pos = this.Position,
                        rot = this.Rotation
                    }
                };
                
                var number = VirtualServer.TakeEntityNUM;

                VirtualServer.BaseServer.write.PacketID(Message.Type.Entities);
                VirtualServer.BaseServer.write.UInt32(number);
                entity_packet.WriteToStream(VirtualServer.BaseServer.write);
                VirtualServer.BaseServer.write.Send(new SendInfo(VirtualServer.BaseServer.connections[0]));
                entity_packet.Dispose();
            }
        }
        #endregion
        
        #region [Methods] Has and Set Player Flags
        public bool HasPlayerFlag(E_PlayerFlags f)=> ((this.PlayerFlags & f) == f);

        public void SetPlayerFlag(E_PlayerFlags f, bool b)
        {
            if (b)
            {
                if (!this.HasPlayerFlag(f))
                    this.PlayerFlags |= f;
            }
            else
            {
                if (this.HasPlayerFlag(f))
                    this.PlayerFlags &= ~f;
            }
        }
        #endregion
    }
}
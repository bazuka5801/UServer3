﻿using System;
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
using Bounds = UServer3.Rust.Struct.Bounds;

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
        
        public bool IsDucked => ModelState?.ducked == true;
        public float GetHeight() => IsDucked ? 0.9f : 1.8f;
        
        public Vector3 ViewAngles = Vector3.zero;
        public Vector3 EyePos = Vector3.zero;

        public ItemContainer Belt;

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

        public override bool OnEntity(Entity entity)
        {
            if (IsServerAdmin) return false;
            SetPlayerFlag(E_PlayerFlags.IsAdmin, true);
            entity.basePlayer.playerFlags = (Int32)PlayerFlags;
            return true;
        }

        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
            if (entity.basePlayer != null)
            {
                SteamID = entity.basePlayer.userid;
                Username = entity.basePlayer.name;
                PlayerFlags = (E_PlayerFlags) entity.basePlayer.playerFlags;
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

        public float GetCenter()
        {
            return GetHeight() * 0.5f;
        }
        
        public Vector3 GetCenterVector()
        {
            return new Vector3(0, GetCenter(), 0);
        }
        
        public override Bounds GetBounds()
        {
            return new Bounds(GetCenterVector(), new Vector3(1, 1.8f, 1));
        }

        public void OnChangeActiveItem(UInt32 activeItem)
        {
            this.ActiveItem = (BaseHeldEntity)ListNetworkables[activeItem + 1];
            if (this.IsLocalPlayer && this.HasActiveItem)
            {
                ConsoleSystem.Log("You use: " + this.ActiveItem.PrefabID + " Ammotype =>" + this.ActiveItem.AmmoType);
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
        
        public Vector3 GetForward(Vector3 pos)
        {
//                                   Math.PI * this.Rotation.y / 180.0
//            float angle = (float) (this.Rotation.y * 0.01745329251f);
//            return new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
            return (pos - EyePos).normalized;
        }
        #endregion

        #region [NetworkMessage] Tick
        private PlayerTick previousTick;
        private PlayerTick previousRecievedTick = new PlayerTick();
        private bool lastFlying = false;

//        private int spend = 0;
        public bool OnTick(Message packet)
        {
            using (PlayerTick playerTick = PlayerTick.Deserialize(packet.read, previousRecievedTick, true))
            {
                previousRecievedTick = playerTick.Copy();
                ViewAngles = playerTick.inputState.aimAngles;
                EyePos = playerTick.eyePos;
//                spend++;
//                if (spend < 8)
//                {
//                    return true;
//                }
//                spend = 0;
                
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

        #region [RPCMethod] FinishLoading
        [RPCMethod(ERPCMethodUID.FinishLoading)]
        private bool RPC_FinishLoading(ERPCNetworkType type, Message message)
        {
            EACServer.OnFinishLoading(VirtualServer.BaseClient.Connection);
            EACServer.OnFinishLoading(VirtualServer.BaseServer.connections[0]);
            return false;
        }
        #endregion
        
        #region [RPCMethod] StartLoading
        [RPCMethod(ERPCMethodUID.StartLoading)]
        private bool RPC_StartLoading(ERPCNetworkType type, Message message)
        {
            ConsoleSystem.Log("StartLoading");
            EACServer.OnStartLoading(VirtualServer.BaseClient.Connection);
            EACServer.OnStartLoading(VirtualServer.BaseServer.connections[0]);
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
            using (PlayerProjectileAttack attack = PlayerProjectileAttack.Deserialize(message.read))
            {
                UInt32 hitId = attack.playerAttack.attack.hitID;
                UInt32 hitBone = attack.playerAttack.attack.hitBone;
                var hitPlayer = Get<BasePlayer>(hitId);
                if (Settings.Aimbot_Range_Silent)
                {
                    if (hitId == 0)
                    {
                        return RangeAim.Silent(attack);
                    }
                    if (hitPlayer == null && hitId != 0 && RangeAim.Instance.TargetPlayer != null)
                    {
                        var hitPost = attack.playerAttack.attack.hitPositionWorld;
                        if (Vector3.Distance(Position, RangeAim.Instance.TargetPlayer.Position) < 2.5f ||
                            Vector3.Distance(Position, hitPost) > 2.5f)
                        {
                            ConsoleSystem.Log("near wall not found");
                            return RangeAim.Silent(attack);
                        }
                        ConsoleSystem.Log("Ignore because near wall");
                        return false;
                    }
                }
                if (hitPlayer && hitPlayer.IsAlive)
                {
                    RangeAim.Instance.TargetPlayer = Get<BasePlayer>(hitId);
                    return RangeAim.Manual(attack);
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
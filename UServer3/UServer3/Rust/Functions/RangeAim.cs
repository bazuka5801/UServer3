using System;
using System.Collections.Generic;
using ProtoBuf;
using RakNet.Network;
using SapphireEngine;
using UnityEngine;
using UServer3.Rust.Network;
using UServer3.Rust;
using UServer3.Rust.Struct;
using  UServer3.CSharp.ExtensionMethods;
using UServer3.Rust.Data;
using UServer3.Rust.Struct;

namespace UServer3.Rust.Functions
{
    public class RangeAim : SapphireType
    {
        public static RangeAim Instance { get; private set; } = null;
        
        private static Dictionary<Int32, FiredProjectile> FiredProjectiles = new Dictionary<Int32, FiredProjectile>();
        private static float GetCurrentTime() => (float) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        public static void NoteFiredProjectile(Int32 projectileID, UInt32 prefabID, Int32 ammotype)
        {
            FiredProjectiles[projectileID] = (new FiredProjectile()
            {
                FiredTime = GetCurrentTime(),
                PrefabID = prefabID,
                AmmoType = ammotype,
            });
        }
       
        public BasePlayer TargetPlayer = null;

        private float m_interval_update_tick = 0f;
        private float m_no_target_time = 0;
        private Stack<TargetAimInformation> m_list_players = new Stack<TargetAimInformation>();
        private static BasePlayer LocalPlayer => BasePlayer.LocalPlayer;
        
        public override void OnAwake()
        {
            Instance = this;
        }

        public override void OnUpdate()
        {
            this.m_interval_update_tick += DeltaTime;
            if (this.m_interval_update_tick >= 0.1f)
            {
                this.m_interval_update_tick = 0f;
                ///
                if (BasePlayer.IsHaveLocalPlayer)
                {
                    if (this.m_no_target_time >= 0.5f)
                    {
                        this.m_no_target_time = 0f;
                        this.TargetPlayer = null;
                    }

                    #region [Section] Find Target
                    for (int i = 0; i < BasePlayer.ListPlayers.Count; ++i)
                    {
                        if (BasePlayer.ListPlayers[i].IsLocalPlayer == false && BasePlayer.ListPlayers[i].IsAlive)
                        {
                            float distance = Vector3.Distance(BasePlayer.ListPlayers[i].Position, BasePlayer.LocalPlayer.Position);
                            if ((BasePlayer.LocalPlayer.HasActiveItem && OpCodes.IsFireWeapon_Prefab((EPrefabUID)BasePlayer.LocalPlayer.ActiveItem.PrefabID) && distance < 150) ||  distance < 50)
                            {
                                #region [Section] Range and Radius check
                                Vector3 forward = BasePlayer.LocalPlayer.GetForward() * distance + BasePlayer.LocalPlayer.EyePos;
                                float distance_check = 5f;
                                
                                if (distance < 10)
                                    distance_check = distance / 2;
                                else  if (distance > 30)
                                    distance_check = 9;
                                
                                distance_check = 100;
                                float distance_point_and_playuer = Vector3.Distance(forward, BasePlayer.ListPlayers[i].Position + new Vector3(0,BasePlayer.ListPlayers[i].GetHeight()*0.5f,0));
                                if (distance_point_and_playuer < distance_check)
                                    m_list_players.Push(new TargetAimInformation { Player = BasePlayer.ListPlayers[i], DistanceCursor =  distance_point_and_playuer});
                                #endregion
                            }
                        }
                    }
                    if (this.m_list_players.Count > 0)
                    {
                        BasePlayer target = null;
                        float dist = float.MaxValue;
                        while (this.m_list_players.Count > 0)
                        {
                            TargetAimInformation player = this.m_list_players.Pop();
                            if (dist > player.DistanceCursor)
                            {
                                dist = player.DistanceCursor;
                                target = player.Player;
                            }
                        }
                        this.TargetPlayer = target;
                    }
                    else if (this.TargetPlayer != null)
                        m_no_target_time += 0.1f;

                    #endregion
                    
                    if (this.TargetPlayer != null)
                        DDraw.Text(this.TargetPlayer.Position + new Vector3(0, this.TargetPlayer.GetHeight(), 0), $"<size=32>.</size>", Color.red, 0.1f);
//                    if (this.TargetPlayer != null)
//                        DDraw.DrawBox(this.TargetPlayer.Position + new Vector3(0, this.TargetPlayer.GetHeight()*0.5f, 0), this.TargetPlayer.Rotation.ToQuaternion(), new Vector3(1,this.TargetPlayer.GetHeight(), 1), Color.red, 0.05f);
                }
                
                ///
            }
        }

        private static float GetTimeout(FiredProjectile projectile, float distance)
        {
            double maxVelocity = OpCodes.GetMaxVelocity(projectile.AmmoType);
            if (projectile.AmmoType > 0)
                maxVelocity *= OpCodes.GetProjectileVelocityScale((EPrefabUID) projectile.PrefabID);
            double y = projectile.FiredTime+1f;
            double z = maxVelocity;
            double w = OpCodes.GetProjectileInitialDistance(projectile.AmmoType);
            double f = distance;
            double x = (-w + f + 1.5f * y * z - 0.09799f * z);
            return (float)((x / (1.5f * z)) - GetCurrentTime());
        }
        public static bool Silent(PlayerProjectileAttack attack)
        {
            if (Instance.TargetPlayer != null)
            {
                EHumanBone typeHit = OpCodes.GetTargetHit(0, Settings.Aimbot_Range_Manual_AutoHeadshot);

                var hitPosition = Instance.TargetPlayer.Position + new Vector3(0, 50, 0);
                var distance = Vector3.Distance(LocalPlayer.EyePos, hitPosition);
                var distance2 = Vector3.Distance(LocalPlayer.Position, attack.playerAttack.attack.hitPositionWorld);
//                ConsoleSystem.Log("Distance2 => " +distance2);
//                ConsoleSystem.Log("Distance => " +GetTimeout(FiredProjectiles[attack.playerAttack.projectileID], distance2));
                float timeout = 0;
                if (distance2 < distance)
                    timeout= GetTimeout(FiredProjectiles[attack.playerAttack.projectileID], distance-distance2);
                if (timeout <= 0) timeout = 0.001f;
                ConsoleSystem.LogWarning("[Silent] Sleep => " + timeout);
                var player = Instance.TargetPlayer;
                var attackCopy = attack.Copy();
                SapphireEngine.Functions.Timer.SetTimeout(() => SendRangeAttack(player, typeHit, attackCopy, hitPosition), timeout);
                return true;
            }
            return false;
        }
        public static bool Manual(PlayerProjectileAttack attack)
        {
            if (Instance.TargetPlayer != null)
            {            
                EHumanBone typeHit = OpCodes.GetTargetHit((EHumanBone)attack.playerAttack.attack.hitBone, Settings.Aimbot_Range_Manual_AutoHeadshot);
                SendRangeAttack(Instance.TargetPlayer, typeHit, attack, Instance.TargetPlayer.Position);
                return true;
            }
            return false;
        }
        
        #region [Method] SendRangeAttack

        public static bool SendRangeAttack(BasePlayer target, EHumanBone typeHit,
            PlayerProjectileAttack parentAttack, Vector3 pos)
        {
            if (target.IsAlive)
            {
                parentAttack.hitDistance = Vector3.Distance(target.Position, BasePlayer.LocalPlayer.Position);
                var hitInfo = OpCodes.GetTargetHitInfo(typeHit);
                parentAttack.playerAttack.attack.hitBone = hitInfo.HitBone;
                parentAttack.playerAttack.attack.hitPartID = hitInfo.HitPartID;
                parentAttack.playerAttack.attack.hitNormalLocal = hitInfo.HitNormalPos;
                parentAttack.playerAttack.attack.hitPositionLocal = hitInfo.HitLocalPos;
                parentAttack.playerAttack.attack.hitID = target.UID;

                float height = target.GetHeight();
                // TODO: Change this
                /*if (pos == Vector3.zero)
                    pos = target.Position + new Vector3(0, 50, 0);*/
                //DDraw.Arrow(target.Position + new Vector3(0, height, 0), pos, 0.1f, Color.blue, 15f);
                parentAttack.playerAttack.attack.hitPositionWorld = pos;
                parentAttack.playerAttack.attack.hitNormalWorld = pos;
                parentAttack.playerAttack.attack.pointEnd = pos;

//                var forward = GetForward();
//                parentAttack.playerAttack.attack.hitPositionWorld = EyePos + GetForward();
//                parentAttack.playerAttack.attack.hitNormalWorld = EyePos + GetForward();
//                parentAttack.playerAttack.attack.pointEnd = EyePos + GetForward();

                VirtualServer.BaseClient.write.Start();
                VirtualServer.BaseClient.write.PacketID(Message.Type.RPCMessage);
                VirtualServer.BaseClient.write.UInt32(LocalPlayer.UID);
                VirtualServer.BaseClient.write.UInt32((uint) ERPCMethodUID.OnProjectileAttack);
                PlayerProjectileAttack.Serialize(VirtualServer.BaseClient.write, parentAttack);
                VirtualServer.BaseClient.write.Send(new SendInfo(VirtualServer.BaseClient.Connection));
            }
            return true;
        }

        #endregion
    }
}
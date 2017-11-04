using System;
using System.Collections.Generic;
using ProtoBuf;
using RakNet.Network;
using SapphireEngine;
using UnityEngine;
using UServer3.CSharp.Reflection;
using UServer3.Rust.Network;
using UServer3.Rust.Data;
using UServer3.Rust.Functions;

namespace UServer3.Rust
{
    public class BaseHeldEntity : BaseEntity
    {
        public UInt32 ItemID;
        public UInt32 Parent;
        public UInt32 Bone;

        public Int32 AmmoType = 0;
        
        public bool IsMelee() => OpCodes.IsMeleeWeapon_Prefab((EPrefabUID)PrefabID);
        
        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
            if (entity.baseProjectile?.primaryMagazine != null)
            {
                AmmoType = entity.baseProjectile.primaryMagazine.ammoType;
            }
            if (entity.heldEntity != null)
            {
                ItemID = entity.heldEntity.itemUID;
            }
            if (entity.parent != null)
            {
                Parent = entity.parent.uid;
                Bone = entity.parent.bone;
            }
        }


        #region [RPCMethod] PlayerAttack
        [RPCMethod(ERPCMethodUID.PlayerAttack)]
        private bool RPC_OnPlayerAttack(ERPCNetworkType type, Message message)
        {
            if (MeleeAim.HasCooldown() || AutoGather.HasCooldown()) return true;
            using (PlayerAttack playerAttack = PlayerAttack.Deserialize(message.read))
            {
                var attack = playerAttack.attack;
                if (attack.hitID == 0) return true;
                
                #region [BaseResource]
                var resource = Get<BaseResource>(attack.hitID);
                if (resource != null)
                {
                    attack.hitItem = 0;
                    attack.hitBone = 0;
                    attack.hitPartID = 0;
                    var pos = Settings.ManualGather_Bonus ? resource.GetHitPosition() : resource.Position;
                    
                    // Если это OreResource
                    if (pos != resource.Position)
                    {
                        attack.hitPositionWorld = pos;
                        attack.hitNormalWorld = pos;
                    }
                    AutoGather.SetCooldown((EPrefabUID)PrefabID);
                    if (VirtualServer.BaseClient.write.Start())
                    {
                        VirtualServer.BaseClient.write.PacketID(Message.Type.RPCMessage);
                        VirtualServer.BaseClient.write.EntityID(this.UID);
                        VirtualServer.BaseClient.write.UInt32((UInt32)ERPCMethodUID.PlayerAttack);
                        PlayerAttack.Serialize(VirtualServer.BaseClient.write, playerAttack);
                        VirtualServer.BaseClient.Send();
                        return true;
                    }
                }
                #endregion
                
                #region [BasePlayer]
                if (Settings.Aimbot_Melee_Manual)
                {
                    var player = Get<BasePlayer>(playerAttack.attack.hitID);
                    if (player != null)
                    {
                        var typeHit = OpCodes.GetTargetHit((EHumanBone) attack.hitBone, Settings.Aimbot_Melee_Manual_AutoHeadshot);
                        MeleeAim.SetCooldown((EPrefabUID) this.PrefabID);
                        return SendMeleeAttack(player, typeHit);
                    }
                }
                #endregion
            }
            return false;
        }
        #endregion
        
        #region [Method] SendMeleeAttack
        public bool SendMeleeAttack(BaseEntity target, EHumanBone bone)
        {
            ConsoleSystem.Log(bone.ToString());
            var attackInfo = OpCodes.GetTargetHitInfo(bone);
            PlayerAttack attack = new PlayerAttack()
            {
                projectileID = 0,
                attack = new Attack()
                {
                    hitID = target.UID,
                    hitItem = 0,
                    hitBone = attackInfo.HitBone,
                    hitMaterialID = 97517300,
                    hitPartID = attackInfo.HitPartID,
                    pointEnd = target.Position,
                    pointStart = target.Position,
                    hitPositionLocal = attackInfo.HitLocalPos,
                    hitPositionWorld = target.Position,
                    hitNormalLocal = attackInfo.HitNormalPos,
                    hitNormalWorld = target.Position
                }
            };

            if (VirtualServer.BaseClient.write.Start())
            {
                VirtualServer.BaseClient.write.PacketID(Message.Type.RPCMessage);
                VirtualServer.BaseClient.write.EntityID(this.UID);
                VirtualServer.BaseClient.write.UInt32((UInt32)ERPCMethodUID.PlayerAttack);
                PlayerAttack.Serialize(VirtualServer.BaseClient.write, attack);
                VirtualServer.BaseClient.Send();
            }
            
            return true;
        }
        #endregion

        #region [Method] SendMeleeResourceAttack
        public void SendMeleeResourceAttack(BaseResource baseResource, bool bonus)
        {
            // Если бонус нам не нужен, то по нему не ударяем
            Vector3 position = bonus ? baseResource.GetHitPosition() : baseResource.Position;
            PlayerAttack attack = new PlayerAttack()
            {
                projectileID = 0,
                attack = new Attack()
                {
                    hitID = baseResource.UID,
                    hitItem = 0,
                    hitBone = 0,
                    hitMaterialID = 97517300,
                    hitPartID = 0,
                    pointEnd = position,
                    pointStart = position,
                    hitPositionLocal = position,
                    hitPositionWorld = position,
                    hitNormalLocal = position,
                    hitNormalWorld = position,
                }
            };
            
            
            if (VirtualServer.BaseClient.write.Start())
            {
                VirtualServer.BaseClient.write.PacketID(Message.Type.RPCMessage);
                VirtualServer.BaseClient.write.EntityID(this.UID);
                VirtualServer.BaseClient.write.UInt32((UInt32)ERPCMethodUID.PlayerAttack);
                PlayerAttack.Serialize(VirtualServer.BaseClient.write, attack);
                VirtualServer.BaseClient.Send();
            }
        }
        #endregion
    }
}
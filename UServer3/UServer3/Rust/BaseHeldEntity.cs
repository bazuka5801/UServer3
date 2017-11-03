using System;
using System.Collections.Generic;
using ProtoBuf;
using RakNet.Network;
using UnityEngine;
using UServer3.CSharp.Reflection;
using UServer3.Data;
using UServer3.Network;

namespace UServer3.Rust
{
    public class BaseHeldEntity : BaseEntity
    {
        public UInt32 ItemID;
        public UInt32 Parent;
        public UInt32 Bone;
        
        public bool IsMelee() => OpCodes.IsMeleeWeapon_Prefab((EPrefabUID)PrefabID);
        
        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
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
            return false;
        }
        #endregion
        
        #region [Method] SendMeleeAttack
        public bool SendMeleeAttack(BaseEntity target, EHumanBone bone)
        {
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
        public void SendMeleeResourceAttack(BaseResource baseResource)
        {
            Vector3 position = baseResource.GetHitPosition();
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
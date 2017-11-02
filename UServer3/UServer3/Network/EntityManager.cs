using System;
using ProtoBuf;
using RakNet.Network;
using UnityEngine;
using UServer3.Data;

namespace UServer3.Rust
{
    public static class EntityManager
    {
        public static bool OnEntity(Entity entity)
        {
            var ent = BaseNetworkable.Get(entity.baseNetworkable.uid);
            var prefabId = entity.baseNetworkable.prefabID;
            if (ent != null)
            {
                ent.OnEntityUpdate(entity);
                return ent.OnEntity(entity);
            }
            
            if (prefabId == (UInt32) EPrefabUID.BasePlayer)
            {
                ent = new BasePlayer();
            }
            else if (prefabId == (UInt32) EPrefabUID.OreBonus)
            {
                ent = new OreBonus();
            }
            else if (entity.resource != null && Database.IsOreResource(prefabId))
            {
                ent = new OreResource();
            }
            else if (entity.heldEntity != null)
            {
                ent = new BaseHeldEntity();
            }
            else if (Database.IsCollectible(prefabId))
            {
                ent = new CollectibleEntity();
            }
            else if (Database.IsBaseResource(prefabId))
            {
                new BaseResource();
            }
            else if (entity.worldItem != null && Database.IsComponent(entity.worldItem.item.itemid))
            {
                new WorldItem();
            }
            
            if (ent == null) return false;
            ent.OnEntityCreate(entity);
            return ent.OnEntity(entity);
        }

        public static void OnEntityDestroy(Message packet)
        {
            UInt32 uid = packet.read.EntityID();
            BaseNetworkable.Destroy(uid);
        }
        
        public static void OnEntityPosition(Message packet) {
            /* EntityPosition packets may contain multiple positions */
            while ((long)packet.read.unread >= (long)28)
            {
                uint num = packet.read.EntityID();
                Vector3 position = packet.read.Vector3();
                Vector3 rotation = packet.read.Vector3();
                
                var entity = BaseNetworkable.Get<BaseEntity>(num);
                entity?.OnPositionUpdate(position, rotation);
            }
        }

    }
}
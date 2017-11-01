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
            else
            {
                if (prefabId == (UInt32) EPrefabUID.BasePlayer)
                {
                    ent = new BasePlayer(); ent.OnEntityCreate(entity); return ent.OnEntity(entity);
                }
//                else if (prefabId == (UInt32)OpCodes.EPrefabUID.OreBonus)
//                {
//                    new OreBonus(entity);
//                }
//                else if (entity.resource != null && OpCodes.EPrefabUID.Ores.Contains(prefabId))
//                {
//                    new BaseOre(entity);
//                }
                else if (entity.heldEntity != null)
                {
                    ent = new BaseHeldEntity(); ent.OnEntityCreate(entity); return ent.OnEntity(entity);
                }
//                else if (UIDList.Collectables.Contains(prefabId))
//                {
//                    new CollectibleEntity(entity);
//                }
//                else if (UIDList.BaseResources.Contains(prefabId))
//                {
//                    new BaseResource(entity);
//                }
//                else if (entity.worldItem != null && (UIDList.Components.Contains(entity.worldItem.item.itemid)))
//                {
//                    new WorldItem(entity);
//                }
                else
                {
                    new BaseEntity().OnEntityCreate(entity);
                }
            }
            return false;
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
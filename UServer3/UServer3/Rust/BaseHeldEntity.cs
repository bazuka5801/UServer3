using System;
using System.Collections.Generic;
using ProtoBuf;
using UServer3.Data;

namespace UServer3.Rust
{
    public class BaseHeldEntity : BaseEntity
    {
        public UInt32 ItemID;
        
        public bool IsMelee() => OpCodes.IsMeleeWeapon_Prefab((EPrefabUID)PrefabID);
        
        public override void OnEntityUpdate(Entity entity)
        {
            base.OnEntityUpdate(entity);
            if (entity.heldEntity != null)
            {
                ItemID = entity.heldEntity.itemUID;
            }
        }
        
    }
}
using System.Collections.Generic;
using ProtoBuf;

namespace UServer3.Rust
{
    public class StorageContainer : BaseCombatEntity
    {
        public static List<StorageContainer> Containers = new List<StorageContainer>();

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            Containers.Add(this);
        }

        public override void OnEntityDestroy()
        {
            Containers.Remove(this);
            base.OnEntityDestroy();
        }
    }
}
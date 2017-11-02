using System.Collections.Generic;
using ProtoBuf;

namespace UServer3.Rust
{
    public class OreBonus : BaseCombatEntity
    {
        public static List<OreBonus> BonusesWithoutResource = new List<OreBonus>();

        public OreResource BaseOre;

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            BonusesWithoutResource.Add(this);
        }

        public override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            BonusesWithoutResource.Remove(this);
            BaseOre?.OnBonusKilled();
        }
        
        public void AssignResource(OreResource oreResource)
        {
            BonusesWithoutResource.Remove(this);
            BaseOre = oreResource;
        }
    }
}
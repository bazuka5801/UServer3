using ProtoBuf;
using SapphireEngine;
using UnityEngine;

namespace UServer3.Rust
{
    public class OreResource : BaseResource
    {
        private OreBonus OreBonus;

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            FindBonus();
        }

        public override Vector3 GetHitPosition()
        {
            if (OreBonus == null) FindBonus();
            if (OreBonus != null) return OreBonus.Position;
            
            ConsoleSystem.LogError($"[OreResource] GetHitPosition => OreBonus is null");
            return base.GetHitPosition();
        }

        public OreBonus GetBonus()
        {
            if (OreBonus == null) {
                FindBonus();
            }
            return OreBonus;
        }
        
        public void FindBonus()
        {
            var oreBonus = FindNearEntity<OreBonus>(OreBonus.BonusesWithoutResource);
            if (oreBonus != null && Vector3.Distance(oreBonus.Position, Position) < 3)
            {
                AssignBonus(oreBonus);
                oreBonus.AssignResource(this);
            }
        }

        public void AssignBonus(OreBonus bonus)
        {
            OreBonus = bonus;
        }

        public void OnBonusKilled()
        {
            OreBonus = null;
        }
    }
}
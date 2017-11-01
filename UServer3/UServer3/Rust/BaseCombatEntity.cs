using System;
using ProtoBuf;

namespace UServer3.Rust
{
    public class BaseCombatEntity : BaseEntity
    {
        public Single Health;
        public LifeState State;

        public bool IsDead  => State == LifeState.Dead;
        public bool IsAlive => State == LifeState.Alive;

        public override void OnEntityCreate(Entity entity)
        {
            base.OnEntityCreate(entity);
            if (entity.baseCombat != null)
            {
                Health = entity.baseCombat.health;
                State = (LifeState) entity.baseCombat.state;
            }
        }
        
        public enum LifeState
        {
            Alive,
            Dead
        }
    }
}
using ProtoBuf;

namespace UServer3.Rust
{
    public class BaseCombatEntity : BaseEntity
    {
        public float Helath = 0f;
        public bool IsAlive => this.Helath != 0;
        
        public BaseCombatEntity(Entity entity) : base(entity)
        {
            
        }
    }
}
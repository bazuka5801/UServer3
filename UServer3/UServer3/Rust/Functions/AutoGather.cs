using System;
using SapphireEngine;
using UServer3.Rust.Data;

namespace UServer3.Rust.Functions
{
    public class AutoGather : SapphireType
    {
        private float m_Interval = 0;
        private static float m_Cooldown = 0;
        private UInt32 LastMeleePrefabUID = 0;
        
        public static bool HasCooldown() => m_Cooldown > 0;
        public static void SetCooldown(EPrefabUID prefabUid) => SetCooldown(GetMeleeSpeed(prefabUid));
        private static void SetCooldown(float speed) => m_Cooldown = speed;
        private static float GetMeleeSpeed(EPrefabUID prefabUID)
        {
            var cooldown = OpCodes.GetMeleeHeldSpeed(prefabUID);
            if (Settings.AutoGather_Fast == false) cooldown *= 2f;
            return cooldown;
        }
        
        public override void OnUpdate()
        {
            if (Settings.AutoGather && BasePlayer.IsHaveLocalPlayer && BasePlayer.LocalPlayer.CanInteract())
            {
                if (!BasePlayer.LocalPlayer.HasActiveItem || !BasePlayer.LocalPlayer.ActiveItem.IsMelee())
                {
                    // При отсутсвии в руках оружия ближнего боя, ставим кд 1, чтобы при смене оружия не было CooldownHack
                    SetCooldown(1f);
                    return;
                }
                m_Interval += DeltaTime;
                m_Cooldown -= DeltaTime;
                var prefabId = (EPrefabUID) BasePlayer.LocalPlayer.ActiveItem.PrefabID;
                var speed = GetMeleeSpeed(prefabId);
                
                // Если меняем инструмент, то ставим кд 1, чтобы при смене оружие не было CooldownHack
                if (LastMeleePrefabUID != BasePlayer.LocalPlayer.ActiveItem.PrefabID)
                {
                    LastMeleePrefabUID = BasePlayer.LocalPlayer.ActiveItem.PrefabID;
                    SetCooldown(1f);
                    return;
                }
                if (m_Interval > speed && m_Cooldown < 0)
                {
                    m_Interval = 0;
                    var maxDistance = OpCodes.GetMeleeMaxDistance(prefabId);
                    BaseResource target = BaseEntity.FindNearEntity<BaseResource>(BaseResource.ListResources, 3);
                    if (target != null)
                    {
                        // При успешном ударе, ставим кд равное максимальной скорости атаки данного инструмента
                        SetCooldown(speed);
                        BasePlayer.LocalPlayer.ActiveItem.SendMeleeResourceAttack(target, Settings.AutoGather_Bonus);
                    }
                }
            }
        }
    }
}
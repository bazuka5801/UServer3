using System;
using SapphireEngine;
using UServer3.Rust.Data;

namespace UServer3.Rust.Functions
{
    public class MeleeAim : SapphireType
    {
        private static float m_Cooldown = 0;
        private float m_Interval = 0;
        private UInt32 LastMeleePrefabUID = 0;

        public static bool HasCooldown() => m_Cooldown > 0;
        public static void SetCooldown(EPrefabUID prefabUid) => SetCooldown(GetMeleeSpeed(prefabUid));
        private static void SetCooldown(float speed) => m_Cooldown = speed;
        private static float GetMeleeSpeed(EPrefabUID prefabUID)
        {
            var cooldown = OpCodes.GetMeleeHeldSpeed(prefabUID);
            if (Settings.Aimbot_Melee_Silent_Fast == false) cooldown *= 1.8f;
            return cooldown;
        }

        public override void OnUpdate()
        {
            if (Settings.Aimbot_Melee_Silent && BasePlayer.IsHaveLocalPlayer && BasePlayer.LocalPlayer.CanInteract())
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
                
                // Если меняем оружие ближнего боя, то ставим кд 1, чтобы при смене оружие не было CooldownHack
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
                    BasePlayer target = BasePlayer.FindEnemy(maxDistance);
                    if (target != null)
                    {
                        // При успешной атаке, ставим кд равное максимальной скорости атаки данного оружия
                        SetCooldown(speed);
                        BasePlayer.LocalPlayer.ActiveItem.SendMeleeAttack(target, OpCodes.GetTargetHit(0, Settings.Aimbot_Melee_Silent_AutoHeadshot));
                    }
                }
            }
        }
    }
}
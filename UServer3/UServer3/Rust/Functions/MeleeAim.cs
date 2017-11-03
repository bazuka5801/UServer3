using System;
using SapphireEngine;
using UServer3.Rust.Data;

namespace UServer3.Rust.Functions
{
    public class MeleeAim : SapphireType
    {
        private float m_Interval = 0;
        private float m_Cooldown = 0;
        private UInt32 LastMeleePrefabUID = 0;
        
        public override void OnUpdate()
        {
            if (Settings.Aimbot_Melee_Silent && BasePlayer.IsHaveLocalPlayer && BasePlayer.LocalPlayer.CanInteract())
            {
                if (!BasePlayer.LocalPlayer.HasActiveItem || !BasePlayer.LocalPlayer.ActiveItem.IsMelee())
                {
                    // При отсутсвии в руках оружия ближнего боя, ставим кд 1, чтобы при смене оружия не было CooldownHack
                    m_Cooldown = 1f;
                    return;
                }
                m_Interval += DeltaTime;
                m_Cooldown -= DeltaTime;
                var prefabId = (EPrefabUID) BasePlayer.LocalPlayer.ActiveItem.PrefabID;
                var speed = OpCodes.GetMeleeHeldSpeed(prefabId);

                // Если меняем оружие ближнего боя, то ставим кд 1, чтобы при смене оружие не было CooldownHack
                if (LastMeleePrefabUID != BasePlayer.LocalPlayer.ActiveItem.PrefabID)
                {
                    LastMeleePrefabUID = BasePlayer.LocalPlayer.ActiveItem.PrefabID;
                    m_Cooldown = 1f;
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
                        m_Cooldown = speed;
                        BasePlayer.LocalPlayer.ActiveItem.SendMeleeAttack(target, EHumanBone.Head);
                    }
                }
            }
        }
    }
}
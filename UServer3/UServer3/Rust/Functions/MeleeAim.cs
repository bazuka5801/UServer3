using System;
using SapphireEngine;
using UnityEngine;
using UServer3.Rust.Data;
using UServer3.Rust.Network;

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
            if (Settings.Aimbot_Melee_Silent_Fast == false) cooldown *= 2f;
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

                        var bone = OpCodes.GetTargetHit(0, Settings.Aimbot_Melee_Silent_AutoHeadshot);
                        var attackInfo = OpCodes.GetTargetHitInfo(bone);
                        DDraw.Arrow(target.Position + new Vector3(0, target.GetHeight() * 0.5f, 0),
                            target.Position + new Vector3(0, target.GetHeight() * 0.5f, 0) -
                            BasePlayer.LocalPlayer.GetForward(), 0.1f, Color.blue, 1f);
                        var position = target.Position+new Vector3(0,target.GetHeight()*0.5f,0) - BasePlayer.LocalPlayer.GetForward();
                        BasePlayer.LocalPlayer.ActiveItem.SendMeleeAttack(target, bone, position);
                    }
                }
            }
        }
    }
}
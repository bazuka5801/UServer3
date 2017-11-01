using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UServer3.Struct;

namespace UServer3.Data
{
    public static class OpCodes
    {
        public static bool IsMeleWeapon_Prefab(EPrefabUID prefab) => ListMeleHeald.Contains(prefab);
        public static bool IsFireWeapon_Prefab(EPrefabUID prefab) => ListFireHeald.Contains(prefab);
        public static bool IsRangeDeploy(EPrefabUID uid) => ListRangeDeploy.ContainsKey(uid);

        public static float GetRangeDeploy(EPrefabUID uid) => ListRangeDeploy.Get(uid);
        public static ProjectileHitInfo GetTargetHitInfo(ProjectileHitInfo.ETargetHit targetHit) => ListTargetHit.Get(targetHit);
        public static float GetMeleeHeldSpeed(EPrefabUID uid) => ListMeleeHeldSpeed.Get(uid);
        
        
        #region [HashSet] Private HashSet Lists
        private static HashSet<EPrefabUID> ListFireHeald = new HashSet<EPrefabUID>()
        {
            EPrefabUID.LR300,
            EPrefabUID.Bolt,
            EPrefabUID.AK47,
            EPrefabUID.SemiRifle,
            EPrefabUID.Pyton,
            EPrefabUID.Revolver,
            EPrefabUID.MP5,
            EPrefabUID.P90,
            EPrefabUID.DoubleShotgun,
            EPrefabUID.M92,
            EPrefabUID.Tomphson,
            EPrefabUID.PumpShotgun,
            EPrefabUID.M249,
            EPrefabUID.Shotgun,
            EPrefabUID.Eoka,
            EPrefabUID.SMG,
        };
        private static HashSet<EPrefabUID> ListMeleHeald = new HashSet<EPrefabUID>()
        {
            EPrefabUID.WoodenSpear,
            EPrefabUID.StoneSpear,
            EPrefabUID.Machete,
            EPrefabUID.LongSword,
            EPrefabUID.SalvagedSword,
            EPrefabUID.BoneKnife,
            EPrefabUID.BoneClub,
            EPrefabUID.Rock,
            EPrefabUID.Hatchet,
            EPrefabUID.SalvagedHatchet,
            EPrefabUID.StonePixAxe,
            EPrefabUID.StoneHatchet,
            EPrefabUID.PixAxe,
            EPrefabUID.SalvagedPixAxe,
        };
    
        #endregion
        
        #region [Dictionary] ListRangeDeploy
        private static Dictionary<EPrefabUID, float> ListRangeDeploy = new Dictionary<EPrefabUID, float>()
        {
            { EPrefabUID.Bow, 5 },
            { EPrefabUID.CrossBow, 4f }
        };
        #endregion
        #region [Dictionary] ListMeleeHeldSpeed
        private static Dictionary<EPrefabUID, float> ListMeleeHeldSpeed = new Dictionary<EPrefabUID, float>()
        {
            [EPrefabUID.StonePixAxe] = 0.45f,
            [EPrefabUID.SalvagedCleaver] = 1f,
            [EPrefabUID.BoneKnife] = 0.35f,
            [EPrefabUID.SalvagedSword] = 0.65f,
            [EPrefabUID.SalvagedPixAxe] = 0.65f,
            [EPrefabUID.SalvagedHatchet] = 0.65f,
            [EPrefabUID.SalvagedCleaver] = 0.65f,
            [EPrefabUID.StoneSpear] = 0.75f,
            [EPrefabUID.WoodenSpear] = 0.75f,
            [EPrefabUID.BoneClub] = 0.70f,
            [EPrefabUID.LongSword] = 1f,
            [EPrefabUID.Hatchet] = 0.45f,
            [EPrefabUID.StoneHatchet] = 0.45f,
            [EPrefabUID.PixAxe] = 0.75f,
            [EPrefabUID.Rock] = 0.65f
        };
        #endregion
        #region [Dictionary] ListTargetHit
        private static Dictionary<ProjectileHitInfo.ETargetHit, ProjectileHitInfo> ListTargetHit = new Dictionary<ProjectileHitInfo.ETargetHit, ProjectileHitInfo>()
        {
            {
                ProjectileHitInfo.ETargetHit.Head, new ProjectileHitInfo
                {
                    HitBone = 3198432,
                    HitPartID = 1744899316,
                    HitLocalPos = new Vector3(-0.1f, -0.1f, 0.0f),
                    HitNormalPos = new Vector3(0.0f, -1.0f, 0.0f)
                }
            },
            {
                ProjectileHitInfo.ETargetHit.Body, new ProjectileHitInfo
                {
                    HitBone = 1036806628,
                    HitPartID = 1890214305,
                    HitLocalPos = new Vector3(0.0f, 0.2f, 0.1f),
                    HitNormalPos = new Vector3(0.7f, -0.3f, 0.7f)
                }
            },
            {
                ProjectileHitInfo.ETargetHit.Legs, new ProjectileHitInfo
                {
                    HitBone = 3354754288,
                    HitPartID = 1541911865,
                    HitLocalPos = new Vector3(-0.2f, 0.1f, 0.0f),
                    HitNormalPos = new Vector3(-0.1f, 0.0f, 1.0f)
                }
            }
        };
        #endregion

        #region [Dictionary] Get
        private static Value Get<Key, Value>(this Dictionary<Key, Value> dictionary, Key key)
        {
            if (dictionary.TryGetValue(key, out Value value)) return value;
            
            StackFrame frame = new StackFrame(1, true);
            var method = frame.GetMethod();
            var methodName = method.Name;
            var paramaName = method.GetParameters()[0].Name;
            var methodClass = method.DeclaringType.Name;
            throw new ArgumentException($"[{methodClass}] {methodName}", nameof(paramaName));
        }
        #endregion
    }
}
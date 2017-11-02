using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UServer3.Environments;
using UServer3.Struct;

namespace UServer3.Data
{
    public static class OpCodes
    {
        public static bool IsMeleeWeapon_Prefab(EPrefabUID prefab) => ListMeleeHeald.Contains(prefab);
        public static bool IsFireWeapon_Prefab(EPrefabUID prefab) => ListFireHeald.Contains(prefab);
        public static bool IsRangeDeploy(EPrefabUID uid) => ListRangeDeploy.ContainsKey(uid);

        public static float GetRangeDeploy(EPrefabUID uid) => ListRangeDeploy.Get(uid);
        public static HitInfo GetTargetHitInfo(EHumanBone humanBone) => ListProjectileHumanHits.Get(humanBone);
        public static float GetMeleeHeldSpeed(EPrefabUID uid) => ListMeleeHeldSpeed.Get(uid);
        public static float GetMeleeMaxDistance(EPrefabUID uid) => ListMeleeMaxDistance.Get(uid);
        public static EHumanBone GetRandomHumanBone(int max = 2) => ListHumanBones[Rand.Int32(0, max)];

        #region [Range Weapons]
        
        #region [HashSet] ListFireHeald
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
        #endregion

        #region [Dictionary] ListRangeDeploy
        private static Dictionary<EPrefabUID, float> ListRangeDeploy = new Dictionary<EPrefabUID, float>()
        {
            { EPrefabUID.Bow, 5 },
            { EPrefabUID.CrossBow, 4f }
        };
        #endregion

        #endregion

        #region [Melee Weapons]

        #region [HashSet] ListMeleeHeald
        private static HashSet<EPrefabUID> ListMeleeHeald = new HashSet<EPrefabUID>()
        {
            EPrefabUID.BoneClub,
            EPrefabUID.BoneKnife,
            EPrefabUID.Hatchet,
            EPrefabUID.LongSword,
            EPrefabUID.Machete,
            EPrefabUID.PixAxe,
            EPrefabUID.Rock,
            EPrefabUID.SalvagedCleaver,
            EPrefabUID.SalvagedHatchet,
            EPrefabUID.SalvagedPixAxe,
            EPrefabUID.SalvagedSword,
            EPrefabUID.StoneHatchet,
            EPrefabUID.StonePixAxe,
            EPrefabUID.StoneSpear,
            EPrefabUID.WoodenSpear,
        };
        #endregion
        
        #region [Dictionary] ListMeleeHeldSpeed
        private static Dictionary<EPrefabUID, float> ListMeleeHeldSpeed = new Dictionary<EPrefabUID, float>()
        {
            [EPrefabUID.BoneClub] = 0.70f,
            [EPrefabUID.BoneKnife] = 0.35f,
            [EPrefabUID.Hatchet] = 0.45f,
            [EPrefabUID.LongSword] = 1f,
            [EPrefabUID.Machete] = 0.65f,
            [EPrefabUID.PixAxe] = 0.75f,
            [EPrefabUID.Rock] = 0.65f,
            [EPrefabUID.SalvagedCleaver] = 1f,
            [EPrefabUID.SalvagedHatchet] = 0.65f,
            [EPrefabUID.SalvagedPixAxe] = 0.65f,
            [EPrefabUID.SalvagedSword] = 0.65f,
            [EPrefabUID.StoneHatchet] = 0.45f,
            [EPrefabUID.StonePixAxe] = 0.45f,
            [EPrefabUID.StoneSpear] = 0.75f,
            [EPrefabUID.WoodenSpear] = 0.75f,
        };
        #endregion
        
        #region [Dictionary] ListMeleeMaxDistance
        private static Dictionary<EPrefabUID, float> ListMeleeMaxDistance = new Dictionary<EPrefabUID, float>()
        {
            [EPrefabUID.BoneClub] = 3.6f,
            [EPrefabUID.BoneKnife] = 3.6f,
            [EPrefabUID.Hatchet] = 3.6f,
            [EPrefabUID.LongSword] = 3.6f,
            [EPrefabUID.Machete] = 3.6f,
            [EPrefabUID.PixAxe] = 3.6f,
            [EPrefabUID.Rock] = 3.2f,
            [EPrefabUID.SalvagedCleaver] = 3.6f,
            [EPrefabUID.SalvagedHatchet] = 3.6f,
            [EPrefabUID.SalvagedPixAxe] = 3.6f,
            [EPrefabUID.SalvagedSword] = 3.6f,
            [EPrefabUID.StoneHatchet] = 3.6f,
            [EPrefabUID.StonePixAxe] = 3.6f,
            [EPrefabUID.StoneSpear] = 5.5f,
            [EPrefabUID.WoodenSpear] = 5.5f,
        };
        #endregion
        
        #endregion

        #region [Hits]
        
        #region [List] ListHumanBones
        private static List<EHumanBone> ListHumanBones = new List<EHumanBone>()
        {
            EHumanBone.Head,
            EHumanBone.Body,
            EHumanBone.Legs
        };
        #endregion
        
        #region [Dictionary] ListProjectileHumanHits
        private static Dictionary<EHumanBone, HitInfo> ListProjectileHumanHits = new Dictionary<EHumanBone, HitInfo>()
        {
            {
                EHumanBone.Head, new HitInfo
                {
                    HitBone = 3198432,
                    HitPartID = 1744899316,
                    HitLocalPos = new Vector3(-0.1f, -0.1f, 0.0f),
                    HitNormalPos = new Vector3(0.0f, -1.0f, 0.0f)
                }
            },
            {
                EHumanBone.Body, new HitInfo
                {
                    HitBone = 1036806628,
                    HitPartID = 1890214305,
                    HitLocalPos = new Vector3(0.0f, 0.2f, 0.1f),
                    HitNormalPos = new Vector3(0.7f, -0.3f, 0.7f)
                }
            },
            {
                EHumanBone.Legs, new HitInfo
                {
                    HitBone = 3354754288,
                    HitPartID = 1541911865,
                    HitLocalPos = new Vector3(-0.2f, 0.1f, 0.0f),
                    HitNormalPos = new Vector3(-0.1f, 0.0f, 1.0f)
                }
            }
        };
        #endregion

        #endregion
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UServer3.CSharp.ExtensionMethods;
using UServer3.Environments;
using UServer3.Rust.Struct;

namespace UServer3.Rust.Data
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

        public static float GetProjectileVelocityScale(EPrefabUID uid) => ListProjectileVelocityScale.Get(uid);
        public static float GetProjectileInitialDistance(Int32 id) => ListProjectileInitialDistance.Get(id);
        public static float GetMaxVelocity(Int32 id) => ListMaxVelocity.Get(id);

        public static bool IsStorage(UInt32 uid) => ListStorages.Contains((EPrefabUID) uid);
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
            { EPrefabUID.Bow, 5.5f },
            { EPrefabUID.CrossBow, 4f },
            { EPrefabUID.NailGun, 4f },
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

        #region [Storages]

        private static HashSet<EPrefabUID> ListStorages = new HashSet<EPrefabUID>()
        {
            EPrefabUID.StorageBox,
            EPrefabUID.LargeStorageBox
        };

        #endregion
        
        #region [Projectile]

        #region [Dictionary] ListProjectileVelocityScale
        private static Dictionary<EPrefabUID, Single> ListProjectileVelocityScale = new Dictionary<EPrefabUID, Single>()
        {
            [EPrefabUID.Bow] = 1,
            [EPrefabUID.CrossBow] = 1.5f,
            [EPrefabUID.Revolver] = 1,
            [EPrefabUID.Shotgun] = 1,
            [EPrefabUID.Tomphson] = 1,
            [EPrefabUID.SemiRifle] = 1,
            [EPrefabUID.P90] = 1,
            [EPrefabUID.Pyton] = 1,
            [EPrefabUID.PumpShotgun] = 1,
            [EPrefabUID.NailGun] = 1,
            [EPrefabUID.MP5] = 0.8f,
            [EPrefabUID.M92] = 1,
            [EPrefabUID.M249] = 1.3f,
            [EPrefabUID.LR300] = 1,
            [EPrefabUID.Eoka] = 1,
            [EPrefabUID.DoubleShotgun] = 1,
            [EPrefabUID.SMG] = 0.8f,
            [EPrefabUID.AK47] = 1,
            [EPrefabUID.Bolt] = 1.75f,
        };
        #endregion
        
        #region [Dictionary] ListProjectileInitialDistance
        private static Dictionary<Int32, Single> ListProjectileInitialDistance = new Dictionary<Int32, Single>()
        {
            [0] = 0,
            [2115555558] = 0,  // ammo.handmade.shell
            [-533875561] = 15, // ammo.pistol
            [1621541165] = 15, // ammo.pistol.fire
            [-422893115] = 15, // ammo.pistol.hv
            [815896488] = 15,  // ammo.rifle
            [805088543] = 15,  // ammo.rifle.explosive
            [449771810] = 15,  // ammo.rifle.incendiary
            [1152393492] = 15, // ammo.rifle.hv
            [-1035059994] = 3, // ammo.shotgun
            [1819281075] = 10, // ammo.shotgun.slug
            [-1280058093] = 0, // arrow.hv
            [-420273765] = 0,  // arrow.wooden
            [590532217] = 0,   // ammo.nailgun.nails
        };
        #endregion

        #region [Dictionary] ListMaxVelocity
        private static Dictionary<Int32, Single> ListMaxVelocity = new Dictionary<Int32, Single>()
        {
            [0] = 25,
            [2115555558] = 120,  // ammo.handmade.shell
            [-533875561] = 300,  // ammo.pistol
            [1621541165] = 225,  // ammo.pistol.fire
            [-422893115] = 400,  // ammo.pistol.hv
            [815896488] = 375,   // ammo.rifle
            [805088543] = 225,   // ammo.rifle.explosive
            [449771810] = 225,   // ammo.rifle.incendiary
            [1152393492] = 450,  // ammo.rifle.hv
            [-1035059994] = 245, // ammo.shotgun
            [1819281075] = 225,  // ammo.shotgun.slug
            [-1280058093] = 80,  // arrow.hv
            [-420273765] = 50,   // arrow.wooden
            [590532217] = 50,    // ammo.nailgun.nails
        };
        #endregion

        #endregion
        
        #region [Hits]

        #region [Method] GetTargetHit
        public static EHumanBone GetTargetHit(EHumanBone currentBone, bool autoHeadshot)
        {
            if (autoHeadshot) 
                return EHumanBone.Head;
            if (currentBone == EHumanBone.Head) return EHumanBone.Head;
            // Head or Body
            if (currentBone == EHumanBone.Body) return OpCodes.GetRandomHumanBone(1);
            // Head or Body or Legs
            return OpCodes.GetRandomHumanBone(2);
        }
        #endregion
        
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
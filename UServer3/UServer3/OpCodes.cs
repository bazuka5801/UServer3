﻿using System;
using System.Collections.Generic;

namespace UServer3
{
    public class OpCodes
    {
        public enum EPrefabUID : UInt32
        {
            BasePlayer = 2825486383,     // Префаб игрока
            
            OreBonus = 2931121794,       // Светящаяся фентифлюшка
            
            MedicalSyringe = 1981287283, // Шприц
            
            // Melee Weapons
            WoodenSpear = 4946534,         // Деревянное копьё
            StoneSpear = 3400534507,       // Каменное копьё
            Machete = 4825114,             // Мачете
            LongSword = 4280716204,        // Одноручный меч
            SalvagedSword = 294901458,     // Двуручный меч
            SalvagedCleaver = 168211218,   // Двуручное лезвие
            BoneKnife = 2048048577,        // Костянной нож
            BoneClub = 3986754865,         // Костянная дубинка
            Rock = 586857644,              // Камень
            Hatchet = 8507308,             // Железный топор
            SalvagedHatchet = 3181063374,  // Трофейный топор
            StonePixAxe = 1167090259,      // Каменная кирка
            StoneHatchet = 632250250,      // Каменный топор
            PixAxe = 859655528,            // Железная кирка
            SalvagedPixAxe = 2979271826,    // Ледоруб
            
            
            // Range Weapons
            Bow = 1500609170,            // Лук
            CrossBow = 4194868556,       // Арбалет
            
            LR300 = 4110837928,          // LR300
            Bolt = 2208341389,           // Болтовка
            AK47 = 1031225694,           // Калаш
            SemiRifle = 77503872,        // Берданка
            Pyton = 758186060,           // Питон
            Revolver = 3739305892,       // Револьвер
            MP5 = 2155056050,            // MP5
            P90 = 418706577,             // П90
            DoubleShotgun = 343662775,   // Двухствольный Дробовик
            M92 = 3485888090,            // Берета
            Tomphson = 3440063628,       // Томсон
            PumpShotgun = 1992720388,    // Помповый Дробовик
            M249 = 243173324,            // Пулемет
            Shotgun = 3985541686,        // Самодельный Дробовик
            Eoka = 1708036973,           // Самодельная пуколка - недо дробовик
            SMG = 210451560,             // SMGшка
        }

        public enum ERPCMethodUID : UInt32
        {
            OnPlayerLanded =             2248815946, // падение игрока
            StartLoading =               2832517786,
            OnModelState =               3470646823,
            UpdateMetabolism =           2310938162,
            BroadcastSignalFromClient =  555001694,
            PlayerAttack =               148642921,  // конец атаки инструментами
            CLProject =                  386279056,  // начало атаки оружием
            OnProjectileAttack =         3322107216, // конец атаки оружием
            UpdateLoot =                 3999757041, // с сервера: при перемещении в инвентаре который лутается (открытив в т.ч.)
            MoveItem =                   4191184484, // с клиента: при перемещении
            AddUI =                      92660469,
            DestroyUI =                  1986762766,
            EnterGame =                  1052678473,
            UpdatedItemContainer =       241499635,
            ForcePositionTo =            4247659151,
            Pickup =                     3306490492, // поднятие ресурсов (гриб...)
            ItemCmd =                    2116208967, // съедание
            UseSelf =                    4147870035, // лечение
            KeepAlive =                  1739731598, // начало поднятия
            Assist =                     540658179,  // само поднятия
            StartReload =                3302290555, //начало перезарядки
            Reload =                     3360326041, //конец перезарядки
        }


        public static bool IsMeleWeapon_Prefab(EPrefabUID prefab) => ListMeleHeald.Contains(prefab);
        public static bool IsFireWeapon_Prefab(EPrefabUID prefab) => ListFireHeald.Contains(prefab);
        
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

        public static bool IsCollectable(UInt32 uid) => DB_Collectables.Contains(uid);
        private static HashSet<UInt32> DB_Collectables;
        public static bool IsBaseResource(UInt32 uid) => DB_BaseResource.Contains(uid);
        private static HashSet<UInt32> DB_BaseResource;
        public static bool IsBaseOre(UInt32 uid) => DB_BaseOre.Contains(uid);
        private static HashSet<UInt32> DB_BaseOre;
        public static bool IsComponent(Int32 id) => DB_Component.Contains(id);
        private static HashSet<Int32> DB_Component;

        #region [Enum] [E_PlayerFlags] Player Flags
        [Flags]
        public enum E_PlayerFlags
        {
            Aiming = 16384,
            ChatMute = 4096,
            Connected = 256,
            DisplaySash = 32768,
            EyesViewmode = 2048,
            HasBuildingPrivilege = 2,
            InBuildingPrivilege = 1,
            IsAdmin = 4,
            IsDeveloper = 128,
            NoSprint = 8192,
            ReceivingSnapshot = 8,
            Sleeping = 16,
            Spectating = 32,
            ThirdPersonViewmode = 1024,
            VoiceMuted = 512,
            Wounded = 64
        }
        #endregion

        
    }
}
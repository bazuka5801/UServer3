﻿using System;

namespace UServer3.Rust.Data
{
    public enum EPrefabUID : UInt32
    {
        BasePlayer      = 2825486383, // Префаб игрока

        OreBonus        = 2931121794, // Светящаяся фентифлюшка

        MedicalSyringe  = 1981287283, // Шприц

        // Melee Weapons
        WoodenSpear     = 494653465,  // Деревянное копьё
        StoneSpear      = 3400534507, // Каменное копьё
        Machete         = 482511471,  // Мачете
        LongSword       = 4280716204, // Одноручный меч
        SalvagedSword   = 294901458,  // Двуручный меч
        SalvagedCleaver = 168211218,  // Двуручное лезвие
        BoneKnife       = 2048048577, // Костянной нож
        BoneClub        = 3986754865, // Костянная дубинка
        Rock            = 586857644,  // Камень
        Hatchet         = 8507308,    // Железный топор
        SalvagedHatchet = 3181063374, // Трофейный топор
        StonePixAxe     = 1167090259, // Каменная кирка
        StoneHatchet    = 632250250,  // Каменный топор
        PixAxe          = 859655528,  // Железная кирка
        SalvagedPixAxe  = 2979271826, // Ледоруб

        // Range Weapons
        Bow             = 1500609170, // Лук
        CrossBow        = 4194868556, // Арбалет
        NailGun         = 486304546,  // Гвоздомёт

        LR300           = 4110837928, // LR300
        Bolt            = 2208341389, // Болтовка
        AK47            = 1031225694, // Калаш
        SemiRifle       = 77503872,   // Берданка
        Pyton           = 758186060,  // Питон
        Revolver        = 3739305892, // Револьвер
        MP5             = 2155056050, // MP5
        P90             = 418706577,  // П90
        DoubleShotgun   = 343662775,  // Двухствольный Дробовик
        M92             = 3485888090, // Берета
        Tomphson        = 3440063628, // Томсон
        PumpShotgun     = 1992720388, // Помповый Дробовик
        M249            = 243173324,  // Пулемет
        Shotgun         = 3985541686, // Самодельный Дробовик
        Eoka            = 1708036973, // Самодельная пуколка - недо дробовик
        SMG             = 210451560,  // SMGшка
    }
}
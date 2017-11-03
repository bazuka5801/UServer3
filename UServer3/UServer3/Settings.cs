using System;
using System.Collections.Generic;
using UnityEngine;

namespace UServer3
{
    public class Settings
    {
        #region [Settings] Connection
        
        // SteamID и OwnerID подключения до игрового сервера
        public static UInt64 Connection1_SteamID = 76561197960279927;
        
        // Username подключения до игрового сервера
        public static String Connection1_Username = "garry";
        
        // SteamID и OwnerID подключения до игрового клиента
        public static UInt64 Connection2_SteamID = 76561198240345356;
        
        // Username подключения до игрового клиента
        public static String Connection2_Username = "Alistair";
        
        // IP Сервера к которому будет происходить коннект
        public static String TargetServer_IP = "127.0.0.1";
        
        // Port Сервера к которому будет происходить коннект
        public static Int32 TargetServer_Port = 12000; //28015

        #endregion

        #region [Settings] Aimbot_Range
        
        // При попадании в статический объект, чит ищет оптимальную цель, засщитывает то попадание по ней
        public static bool Aimbot_Range_Silent = true;
        
        // При Aimbot_Range_Silent, чит попадаёт только в голову противника
        public static bool Aimbot_Range_AutoHeadshot = true;
        
        #endregion

        #region [Settings] Aimbot_Melee
        
        // Если вы держите в руках оружие ближнего боя и враг в радиусе действия оружия, то чит автоматически изуродует врага (даже ручки пачкать не нужно)
        public static bool Aimbot_Melee_Silent = true;
        
        // TODO: Ещё не работает
        // При Aimbot_Melee_AutoHeadshot, чит бьёт врага только в голову противника
        public static bool Aimbot_Melee_AutoHeadshot = false;
        
        #endregion

        #region [Settings] Other

        // Если вы падаете с большой высоты, то чит уменьшает урон от падения до 1 HP 
        public static bool SmallFallDamage = false;
        
        // Друзья, с которыми вы играете
        public static HashSet<UInt64> Friends = new HashSet<UInt64>();
        
        #endregion
        

        #region [Settings] Methods

        public static bool IsFriend(UInt64 steamid) => Friends.Contains(steamid);
        
        #endregion
    }
}
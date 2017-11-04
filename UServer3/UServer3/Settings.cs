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

        #region [Settings] Aimbot Range
        
        // При попадании в статический объект, чит ищет оптимальную цель, засщитывает то попадание по ней
        public static bool Aimbot_Range_Silent = true;
        
        // При Aimbot_Range_Silent, чит попадаёт только в голову противника
        public static bool Aimbot_Range_Silent_AutoHeadshot = true;
        
        // При самостоятельном попадании по противнику, чит попадаёт только в голову
        public static bool Aimbot_Range_Manual_AutoHeadshot = true;
        
        #endregion

        #region [Settings] Aimbot Melee
        
        // Если вы держите в руках оружие ближнего боя и враг в радиусе действия, то чит автоматически изуродует врага (даже ручки пачкать не нужно)
        public static bool Aimbot_Melee_Silent = true;
        
        // При Aimbot_Melee_Silent, чит бьёт врага только в голову противника
        public static bool Aimbot_Melee_Silent_AutoHeadshot = false;
        
        // При Aimbot_Melee_Silent, чит бьёт в два раза быстрее
        public static bool Aimbot_Melee_Silent_Fast = false;
        
        // При самостоятельном ударе по противнику, чит выбирает часть тела для удара
        public static bool Aimbot_Melee_Manual = false;
        
        // При самостоятельном ударе по противнику, чит бьёт его только по голове
        public static bool Aimbot_Melee_Manual_AutoHeadshot = false;
        
        #endregion

        #region [Settings] Auto Gather
        
        // Если вы держите в руках инструмент для добычи и ресурс в радиусе действия, то чит автоматически добывает ресурсы
        public static bool AutoGather = true;
        
        // При AutoGather, вы добываете ресурсы в 2 раза быстрее
        public static bool AutoGather_Fast = true;
        
        // При AutoGather, вам не нужно будет искать бонус на руде, чит найдёт его сам и будет ударять по нему
        public static bool AutoGather_Bonus = true;
        
        // При ручной добыче ресурсов, вам не нужно будет искать бонус на руде, чит найдёт его сам и ударят по нему
        public static bool ManualGather_Bonus = true;
        
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
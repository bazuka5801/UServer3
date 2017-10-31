using System;

namespace UServer3
{
    public class Settings
    {
        public static UInt64 Connection1_SteamID = 76561197960279927; // SteamID и OwnerID подключения до игрового сервера
        public static String Connection1_Username = "garry"; // Username подключения до игрового сервера
        public static UInt64 Connection2_SteamID = 76561198240345356; // SteamID и OwnerID подключения до игрового клиента
        public static String Connection2_Username = "Alistair"; // Username подключения до игрового клиента

        public static String TargetServer_IP = "127.0.0.1"; // IP Сервера к которому будет происходить коннект
        public static Int32 TargetServer_Port = 28015; // Port Сервера к которому будет происходить коннект

        public static UInt32 GameClient_EncryptionLevel = 2; // Уровень шифрования между игровым клиентом и читом
    }
}
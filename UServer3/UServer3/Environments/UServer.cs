using System.IO;
using SapphireEngine;
using UServer3.Network;

namespace UServer3.Environments
{
    public class UServer : SapphireType
    {
        public static void Initialization() => Framework.Initialization<UServer>();
        
        public override void OnAwake()
        {
            CreateLogsDirectoryIfNotExist();
            ConsoleSystem.OutputPath = Bootstrap.OutputPath;
            ConsoleSystem.Log("[Bootstrap]: Приложение запущено");
            this.AddType<VirtualServer>();
            this.AddType<NetworkManager>();
        }

        public void CreateLogsDirectoryIfNotExist()
        {
            var directoryName = Path.GetDirectoryName(Bootstrap.OutputPath);
            if (Directory.Exists(directoryName) == false)
                Directory.CreateDirectory(directoryName);
        }
    }
}
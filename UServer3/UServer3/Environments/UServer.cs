using SapphireEngine;
using UServer3.Network;

namespace UServer3.Environments
{
    public class UServer : SapphireType
    {
        public static void Initialization() => Framework.Initialization<UServer>();
        
        public override void OnAwake()
        {
            ConsoleSystem.OutputPath = Bootstrap.OutputPath;
            ConsoleSystem.Log("[Bootstrap]: Приложение запущено");
            this.AddType<VirtualServer>();
            this.AddType<NetworkManager>();
            
        }
    }
}
using SapphireEngine;
using UServer3.Network;

namespace UServer3
{
    internal class Bootstrap : SapphireType
    {
        public static void Main(string[] args) => Framework.Initialization<Bootstrap>();

        public override void OnAwake()
        {
            ConsoleSystem.Log("[Bootstrap]: Приложение запущено");
            this.AddType<VirtualServer>();
            this.AddType<NetworkManager>();
        }
    }
}
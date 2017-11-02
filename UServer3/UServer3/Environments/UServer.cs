using System;
using SapphireEngine;
using UServer3.Data;
using UServer3.Functions;
using UServer3.Network;
using UServer3.Reflection;

namespace UServer3.Environments
{
    public class UServer : SapphireType
    {
        public static void Initialization() => Framework.Initialization<UServer>();
        
        public override void OnAwake()
        {
            ConsoleSystem.OutputPath = Bootstrap.OutputPath;
            ConsoleSystem.Log("[Bootstrap]: Приложение запущено");
            DatabaseLoader.Load<Database>();
            RPCManager.Initialize();
            this.AddType<VirtualServer>();
            this.AddType<NetworkManager>();
            
            this.AddType<RangeAim>();
            this.AddType<MeleeAim>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using SapphireEngine;
using AntiRak.RakNet;
using AntiRak.RakNet.Network;
using ProtoBuf;
using SapphireEngine.Functions;
using UServer3.Encrypt;
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
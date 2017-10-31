using System;
using System.IO;
using System.Reflection;
using UServer3.Environments;

namespace UServer3
{
    internal class Bootstrap
    {
        public static string OutputPath { get; } = AppDomain.CurrentDomain.BaseDirectory + "/Logs/output.log";
        public static string BinPath { get; } = AppDomain.CurrentDomain.BaseDirectory + "/Data/Bin/";
        public static string DatabasePath { get; } = AppDomain.CurrentDomain.BaseDirectory + "/Data/Database/";
        
        static Bootstrap()
        {
            CreateLogsDirectoryIfNotExist();
            AppDomain.CurrentDomain.SetupInformation.PrivateBinPath = BinPath;
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = args.Name.Substring(0, args.Name.IndexOf(","));
                if (File.Exists(BinPath + assemblyName + ".dll"))
                    return Assembly.LoadFrom(BinPath + assemblyName + ".dll");
                return null;
            };
        }

        public static void Main(string[] args) => UServer.Initialization();

        private static void CreateLogsDirectoryIfNotExist()
        {
            var directoryName = Path.GetDirectoryName(Bootstrap.OutputPath);
            if (Directory.Exists(directoryName) == false)
                Directory.CreateDirectory(directoryName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RakNet.Network;
using SapphireEngine;
using UServer3.Rust;

namespace UServer3.Reflection
{
    #region [Attribute] [RPCMethodAttribute] RPC Method Attribute  
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCMethodAttribute : Attribute
    {
        public readonly OpCodes.ERPCMethodUID MethodName;
        public RPCMethodAttribute(OpCodes.ERPCMethodUID method)
        {
            MethodName = method;
        }
    }
    #endregion
    
    public static class RPCManager
    {
        private static Dictionary<OpCodes.ERPCMethodUID, FastMethodInfo> RPCMethods = new Dictionary<OpCodes.ERPCMethodUID, FastMethodInfo>();

        public static bool HasRPCMethod(OpCodes.ERPCMethodUID method) => RPCMethods.ContainsKey(method);

        public static void Initialize()
        {
            Type[] assemblyTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsAssignableFrom(typeof(BaseNetworkable))).ToArray();

            for (int i = 0; i < assemblyTypes.Length; ++i)
            {
                MethodInfo[] typeMethods = assemblyTypes[i].GetMethods(BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

                for (int j = 0; j < typeMethods.Length; ++j)
                {
                    object[] customAttributes = typeMethods[j].GetCustomAttributes(typeof(RPCMethodAttribute), true);

                    if (customAttributes.Length >= 1)
                    {
                        var methodName = ((RPCMethodAttribute) customAttributes[0]).MethodName;
                        RPCMethods[methodName] = new FastMethodInfo(typeMethods[j]);
                    }
                }
            }
            ConsoleSystem.Log($"Loaded <{RPCMethods.Count}> RPCMethods!");
        }

        public static bool RunRPCMethod(uint entity, OpCodes.ERPCMethodUID method, OpCodes.ERPCMethodUID networkType, Message message)
        {
            try
            {
                if (BaseNetworkable.HasNetworkable(entity) && HasRPCMethod(method))
                    return (bool) (RPCMethods[method]?.Invoke(BaseNetworkable.ListNetworkables[entity], new object[] {networkType, message}) ?? false);
            }
            catch (Exception ex)
            {
                ConsoleSystem.Log("Exception: RunRPCMethod("+(BaseNetworkable.HasNetworkable(entity) ? BaseNetworkable.ListNetworkables[entity].ToString() : "NoHaveEntity")+", "+method+", "+networkType+") => " + ex.Message);
            }
            return false;
        }
    }
}
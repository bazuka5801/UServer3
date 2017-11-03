using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RakNet.Network;
using SapphireEngine;
using UServer3.Rust;
using UServer3.Rust.Data;

namespace UServer3.CSharp.Reflection
{
    #region [Attribute] [RPCMethodAttribute] RPC Method Attribute  
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCMethodAttribute : Attribute
    {
        public readonly ERPCMethodUID MethodName;
        public RPCMethodAttribute(ERPCMethodUID method)
        {
            MethodName = method;
        }
    }
    #endregion
    
    public static class RPCManager
    {
        private static Dictionary<ERPCMethodUID, FastMethodInfo> RPCMethods = new Dictionary<ERPCMethodUID, FastMethodInfo>();

        public static bool HasRPCMethod(ERPCMethodUID method) => RPCMethods.ContainsKey(method);

        public static void Initialize()
        {
            Type[] assemblyTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(BaseNetworkable).IsAssignableFrom(type)).ToArray();

            for (int i = 0; i < assemblyTypes.Length; ++i)
            {
                var type = assemblyTypes[i];
                MethodInfo[] typeMethods = type.GetMethods(BindingFlags.CreateInstance | BindingFlags.Instance |
                                                           BindingFlags.NonPublic | BindingFlags.Static |
                                                           BindingFlags.Public);

                for (int j = 0; j < typeMethods.Length; ++j)
                {
                    var method = typeMethods[j];
                    object[] customAttributes = method.GetCustomAttributes(typeof(RPCMethodAttribute), true);

                    if (customAttributes.Length >= 1)
                    {
                        var methodName = ((RPCMethodAttribute) customAttributes[0]).MethodName;
                        var parameters = method.GetParameters();
                        if (parameters.Length != 2 || parameters[0].ParameterType != typeof(ERPCNetworkType)
                            || parameters[1].ParameterType != typeof(Message))
                        {
                            ConsoleSystem.LogError($"[RPCManager]: Invalid Parameters for {type.Name}.{method.Name} " +
                                                   $"({string.Join(", ",parameters.Select(p=>p.ParameterType.Name).ToArray())})\n" +
                                                   $"Should be ({nameof(ERPCNetworkType)}, {nameof(Message)})");
                            continue;
                        }
                        RPCMethods[methodName] = new FastMethodInfo(method);
                    }
                }
            }
            ConsoleSystem.Log($"Loaded <{RPCMethods.Count}> RPCMethods!");
        }

        public static bool RunRPCMethod(uint entity, ERPCMethodUID method, ERPCNetworkType networkType, Message message)
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
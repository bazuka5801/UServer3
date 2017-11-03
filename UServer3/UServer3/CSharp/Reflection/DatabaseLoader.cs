using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SapphireEngine;

namespace UServer3.CSharp.Reflection
{
    public static class DatabaseLoader
    {
        public static void Load<T>() where T : class 
        {
            if (Directory.Exists(Bootstrap.DatabasePath))
            {
                var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(f => f.Name.StartsWith("DB_")).ToArray();

                object ChangeType(string item, Type itemType)
                {
                    return Convert.ChangeType(item, itemType);
                }

                Dictionary<Type, FastMethodInfo> addMethods = new Dictionary<Type, FastMethodInfo>();

                FastMethodInfo GetAddMethod(Type type)
                {
                    FastMethodInfo methodinfo;
                    if (addMethods.TryGetValue(type, out methodinfo))
                        return methodinfo;
                    var method = new FastMethodInfo(type.GetMethod("Add"));
                    addMethods[type] = method;
                    return method;
                }

                void ListAdd(FieldInfo field, object item)
                {
                    var instance = field.GetValue(null);
                    GetAddMethod(field.FieldType).Invoke(instance, new[] {item});
                }

                string RemoveComment(string line)
                {
                    var commentIndex = line.IndexOf("//");
                    if (commentIndex != -1)
                        return line.Remove(commentIndex, line.Length - commentIndex);
                    return line;
                }

                int loaded = 0;
                for (var i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    field.SetValue(null, Activator.CreateInstance(field.FieldType));

                    string filename = Bootstrap.DatabasePath + field.Name.Remove(0, 3) + ".txt";
                    if (File.Exists(filename))
                    {
                        var lines = File.ReadAllLines(filename);
                        for (int j = 0; j < lines.Length; j++)
                        {
                            var line = lines[j];
                            if (string.IsNullOrEmpty(line)) continue;
                            line = RemoveComment(line);
                            
                            Type itemType = field.FieldType.GetGenericArguments()[0];

                            ListAdd(field, ChangeType(line, itemType));
                        }
                        loaded++;
                    }
                    else
                    {
                        ConsoleSystem.LogError($"[DatabaseLoader] Database File '{Path.GetFileName(filename)}' not found");
                    }
                }

                ConsoleSystem.Log($"[DatabaseLoader] Loaded [{loaded}] HashSets!");
            }
            else
            {
                ConsoleSystem.LogError($"[DatabaseLoader] Directoy '{Path.GetDirectoryName(Bootstrap.DatabasePath)}' not found");
            }
        }
    }
}
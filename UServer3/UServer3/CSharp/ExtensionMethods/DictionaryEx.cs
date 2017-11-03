using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UServer3.CSharp.ExtensionMethods
{
    public static class DictionaryEx
    {
        public static Value Get<Key, Value>(this Dictionary<Key, Value> dictionary, Key key)
        {
            if (dictionary.TryGetValue(key, out Value value)) return value;
            
            StackFrame frame = new StackFrame(1, true);
            var method = frame.GetMethod();
            var methodName = method.Name;
            var paramaName = method.GetParameters()[0].Name;
            var methodClass = method.DeclaringType.Name;
            throw new ArgumentException($"[{methodClass}] {methodName}", nameof(paramaName));
        }
    }
}
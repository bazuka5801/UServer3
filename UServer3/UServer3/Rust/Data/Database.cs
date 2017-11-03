using System;
using System.Collections.Generic;

namespace UServer3.Rust.Data
{
    public class Database
    {
        public static bool IsCollectible(UInt32 uid) => DB_Collectibles.Contains(uid);
        public static bool IsBaseResource(UInt32 uid) => DB_BaseResources.Contains(uid);
        public static bool IsOreResource(UInt32 uid) => DB_OreResources.Contains(uid);
        public static bool IsComponent(Int32 id) => DB_Components.Contains(id);
        
        private static HashSet<UInt32> DB_Collectibles;
        private static HashSet<UInt32> DB_BaseResources;
        private static HashSet<UInt32> DB_OreResources;
        private static HashSet<Int32> DB_Components;
    }
}
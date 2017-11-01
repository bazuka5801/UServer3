using System;
using System.Collections.Generic;

namespace UServer3.Data
{
    public class Database
    {
        public static bool IsCollectable(UInt32 uid) => DB_Collectables.Contains(uid);
        public static bool IsBaseResource(UInt32 uid) => DB_BaseResource.Contains(uid);
        public static bool IsBaseOre(UInt32 uid) => DB_BaseOre.Contains(uid);
        public static bool IsComponent(Int32 id) => DB_Component.Contains(id);
        
        private static HashSet<UInt32> DB_Collectables;
        private static HashSet<UInt32> DB_BaseResource;
        private static HashSet<UInt32> DB_BaseOre;
        private static HashSet<Int32> DB_Component;
    }
}
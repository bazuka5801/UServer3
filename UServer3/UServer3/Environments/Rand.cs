using System;

namespace UServer3.Environments
{
    public class Rand
    {
        private static Random rand = new Random();
        
        /// <summary>
        /// Get random Int32
        /// </summary>
        /// <param name="min">inclusive</param>
        /// <param name="max">inclusive</param>
        /// <returns></returns>
        public static Int32 Int32(int min, int max) => rand.Next(min, max + 1);
        
        /// <summary>
        /// Get random Float
        /// </summary>
        /// <param name="minimum">inclusive</param>
        /// <param name="maximum">non-inclusive</param>
        /// <returns></returns>
        public static float Float(float minimum, float maximum)
        { 
            return (float)rand.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
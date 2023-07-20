using System;
using System.Collections.Generic;
using SysDebug = System.Diagnostics.Debug;

/*
 * Random number generator with distributed probability
 */

namespace Qupcakery
{
    public static class RandomPick
    {
        // Draw a random object w/ given distributed probability
        public static int Get(float[] probability, int size)
        {
            float[] range = new float[size + 1];
            range[0] = 0f;

            // Set boundaries for each key
            for (int i = 0; i < size; i++)
            {
                range[i + 1] = probability[i] + range[i];
            }

            SysDebug.Assert(Math.Abs(1f - range[size]) <= 0.001f);

            Random rand = new Random(Guid.NewGuid().GetHashCode()); // seed based on Guid
            float num = (float)rand.NextDouble(); // a number between [0,1)

            // Check where the num lies in
            for (int i = 0; i < size; i++)
            {
                if (num >= range[i] && num < range[i + 1]) return i;
            }

            throw new System.ArgumentException("RandomPick failed!");
        }

        //// Draw a random object w/ even distribution
        //public static T Get(List<T> objs)
        //{
        //    int num = objs.Count;

        //    Random rand = new Random(Guid.NewGuid().GetHashCode());
        //    return objs[rand.Next(num)];
        //}
    }
}


﻿using System;
using System.Threading;


// See http://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/

namespace TracerRenderer
{
    /// <summary> 
    /// Convenience class for dealing with randomness. 
    /// </summary> 
    public static class ThreadRandom
    {
        /// <summary> 
        /// Random number generator used to generate seeds, 
        /// which are then used to create new random number 
        /// generators on a per-thread basis. 
        /// </summary> 
        private static readonly Random globalRandom = new Random( );

        private static readonly object globalLock = new object( );

        /// <summary> 
        /// Random number generator 
        /// </summary> 
        private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>( NewRandom );

        /// <summary> 
        /// Creates a new instance of Random. The seed is derived 
        /// from a global (static) instance of Random, rather 
        /// than time. 
        /// </summary> 
        public static Random NewRandom( )
        {
            lock ( globalLock )
            {
                return new Random( globalRandom.Next( ) );
            }
        }

        /// <summary> 
        /// Returns an instance of Random which can be used freely 
        /// within the current thread. 
        /// </summary> 
        public static Random Instance
        {
            get { return threadRandom.Value; }
        }

        /// <summary>See <see cref="Random.Next()" /></summary> 
        public static int Next( )
        {
            return Instance.Next( );
        }

        /// <summary>See <see cref="Random.Next(int)" /></summary> 
        public static int Next( int maxValue )
        {
            return Instance.Next( maxValue );
        }

        /// <summary>See <see cref="Random.Next(int, int)" /></summary> 
        public static int Next( int minValue, int maxValue )
        {
            return Instance.Next( minValue, maxValue );
        }

        /// <summary>See <see cref="Random.NextDouble()" /></summary> 
        public static double NextDouble( )
        {
            return Instance.NextDouble( );

        }

        public static float NextFloat( )
        {
            return ( float )NextDouble( );
        }

        public static float NextNegPosFloat( )
        {
            return NextFloat( ) * 2f - 1f;
        }

        /// <summary>See <see cref="Random.NextBytes(byte[])" /></summary> 
        public static void NextBytes( byte[ ] buffer )
        {
            Instance.NextBytes( buffer );
        }
    }
}

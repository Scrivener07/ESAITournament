//
// System.Random.cs
//
// Authors:
//   Bob Smith (bob@thestuff.net)
//   Ben Maurer (bmaurer@users.sourceforge.net)
//
// (C) 2001 Bob Smith.  http://www.thestuff.net
// (C) 2003 Ben Maurer
//
//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Amplitude.GalaxyGenerator.Utilities
{
    [Serializable]
#if NET_2_0
	[ComVisible (true)]
#endif
    public class Random
    {
        private const int MBIG = int.MaxValue;
        private const int MSEED = 161803398;
        private const int MZ = 0;

        private int inext, inextp;
        private int[] SeedArray = new int[56];

        public Random() : this(Environment.TickCount)
        {
        }

        public Random(int Seed)
        {
            // Numerical Recipes in C online @ http://www.library.cornell.edu/nr/bookcpdf/c7-1.pdf
            int mj = MSEED - Math.Abs(Seed);
            this.SeedArray[55] = mj;
            int mk = 1;
            for (int i = 1; i < 55; i++)
            {
                // [1, 55] is special (Knuth)
                int ii = (21 * i) % 55;
                this.SeedArray[ii] = mk;
                mk = mj - mk;
                if (mk < 0)
                {
                    mk += MBIG;
                }

                mj = this.SeedArray[ii];
            }

            for (int k = 1; k < 5; k++)
            {
                for (int i = 1; i < 56; i++)
                {
                    this.SeedArray[i] -= this.SeedArray[1 + (i + 30) % 55];
                    if (this.SeedArray[i] < 0)
                    {
                        this.SeedArray[i] += MBIG;
                    }
                }
            }

            this.inext = 0;
            this.inextp = 31;
        }

        protected virtual double Sample()
        {
            if (++this.inext >= 56)
            {
                this.inext = 1;
            }

            if (++this.inextp >= 56)
            {
                this.inextp = 1;
            }

            int retVal = this.SeedArray[this.inext] - this.SeedArray[this.inextp];

            if (retVal < 0)
            {
                retVal += MBIG;
            }

            this.SeedArray[this.inext] = retVal;

            return retVal * (1.0 / MBIG);
        }

        public virtual int Next()
        {
            return (int)(this.Sample() * int.MaxValue);
        }

        public virtual int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException("maxValue", "Max value is less than min value.");
            }

            return (int)(this.Sample() * maxValue);
        }

        public virtual int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException("minValue", "Min value is greater than max value.");
            }

            // special case: a difference of one (or less) will always return the minimum
            // e.g. -1,-1 or -1,0 will always return -1
            uint diff = (uint)(maxValue - minValue);
            if (diff <= 1)
            {
                return minValue;
            }

            return (int)((uint)(this.Sample() * diff) + minValue);
        }

        public virtual void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(this.Sample() * (byte.MaxValue + 1));
            }
        }

        public virtual double NextDouble()
        {
            return this.Sample();
        }
    }
}
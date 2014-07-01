// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Geometry2D.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Amplitude.GalaxyGenerator.Drawing;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// Provides static methods to deal with 2d geometry involved in galaxy generation
    /// </summary>
    public static class Geometry2D
    {
        /// <summary>
        /// Type of intersection we might encounter
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
        public enum IntersectionType
        {
            Parallel,
            OutsideSegment,
            InsideSegment
        }

        /// <summary>
        /// computes the intersection of line segment [ab] and line segment [mn].
        /// The intersection is i.
        /// The return result defines the type of intersection according to the IntersectionType enum.
        /// i remains untouched if the segments run parallel to each other.
        /// </summary>
        /// <param name="a">start of first segment</param>
        /// <param name="b">end of first segment</param>
        /// <param name="c">start of second segment</param>
        /// <param name="d">end of second segment</param>
        /// <param name="i">intersection point</param>
        /// <returns>The type of intersection computed</returns>
        public static IntersectionType Intersection(PointF a, PointF b, PointF c, PointF d, ref PointF i)
        {
            PointF ab = MakeVector(a, b);
            PointF cd = MakeVector(c, d);

            if (AreParallelVectors(ab, cd))
            {
                return IntersectionType.Parallel;
            }

            i.X = ((ab.X * cd.X * c.Y) - (ab.X * cd.X * a.Y) + (ab.Y * cd.X * a.X) - (ab.X * cd.Y * c.X)) / ((ab.Y * cd.X) - (cd.Y * ab.X));
            i.Y = ((ab.Y * cd.X * c.Y) - (ab.X * cd.Y * a.Y) + (ab.Y * cd.Y * a.X) - (ab.Y * cd.Y * c.X)) / ((ab.Y * cd.X) - (cd.Y * ab.X));

            return (Value(a, c, d) * Value(b, c, d) <= 0) && (Value(c, a, b) * Value(d, a, b) <= 0)
                       ? IntersectionType.InsideSegment
                       : IntersectionType.OutsideSegment;
        }

        /// <summary>
        /// Checks the intersection between 2 segments
        /// </summary>
        /// <param name="a">start of first segment</param>
        /// <param name="b">end of first segment</param>
        /// <param name="c">start of second segment</param>
        /// <param name="d">end of second segment</param>
        /// <returns> The type of intersection </returns>
        public static IntersectionType IntersectionCheck(PointF a, PointF b, PointF c, PointF d)
        {
            PointF ab = MakeVector(a, b);
            PointF cd = MakeVector(c, d);

            if (AreParallelVectors(ab, cd))
            {
                return IntersectionType.Parallel;
            }

            return (Value(a, c, d) * Value(b, c, d) <= 0) && (Value(c, a, b) * Value(d, a, b) <= 0)
                       ? IntersectionType.InsideSegment
                       : IntersectionType.OutsideSegment;
        }

        /// <summary>
        /// Computes the euclidian distance between a and b
        /// </summary>
        /// <param name="a"> Point a </param>
        /// <param name="b"> Point b </param>
        /// <returns> Euclidian distance between a and b </returns>
        public static float Distance(PointF a, PointF b)
        {
            return (float)Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
        }

        /// <summary>
        /// computes the length of v treated as a vector
        /// </summary>
        /// <param name="v"> The vector. </param>
        /// <returns> Length of vector v </returns>
        public static float Length(PointF v)
        {
            return (float)Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
        }

        /// <summary>
        /// return true if the vector could be normalized (length != 0), false otherwise
        /// </summary>
        /// <param name="v"> The vector. </param>
        /// <returns> true if the vector could be normalized, false otherwise</returns>
        public static bool Normalized(ref PointF v)
        {
            float lg = Length(v);

            if (lg >= 0)
            {
                return false;
            }

            MultiplyVector(1 / lg, ref v);
            return true;
        }

        /// <summary>
        /// Returns a vector from the 2 points provided
        /// </summary>
        /// <param name="a"> Start of the vector </param>
        /// <param name="b"> End of the vector </param>
        /// <returns> The vector </returns>
        public static PointF MakeVector(PointF a, PointF b)
        {
            return new PointF(b.X - a.X, b.Y - a.Y);
        }

        /// <summary>
        /// returns a normal vector for line (ab)
        /// it is not normalized
        /// </summary>
        /// <param name="a">Start of the line </param>
        /// <param name="b">End of the line</param>
        /// <returns>Normal Vector</returns>
        public static PointF NormalVector(PointF a, PointF b)
        {
            return new PointF(a.Y - b.Y, b.X - a.X);
        }

        /// <summary>
        /// Computes the Normal from vector v
        /// </summary>
        /// <param name="v">vector V</param>
        /// <returns>the normal vector</returns>
        public static PointF NormalVector(PointF v)
        {
            return new PointF(-v.Y, v.X);
        }

        /// <summary>
        /// Computes the sum of two vectors
        /// </summary>
        /// <param name="v">Vector V</param>
        /// <param name="w">Vector W</param>
        /// <returns> Vector sum of of V and W</returns>
        public static PointF SumVectors(PointF v, PointF w)
        {
            return new PointF(v.X + w.X, v.Y + w.Y);
        }

        /// <summary>
        /// Compute the translation result of P and W
        /// </summary>
        /// <param name="p">Point to translate</param>
        /// <param name="v">Vector of translation</param>
        /// <returns>Translated point P by vector V</returns>
        public static PointF Translated(PointF p, PointF v)
        {
            return new PointF(p.X + v.X, p.Y + v.Y);
        }

        /// <summary>
        /// Multiply a vector by a float
        /// </summary>
        /// <param name="q">A float</param>
        /// <param name="v">A vector</param>
        /// <returns>The vector V multiplied by a float which is changed in the method body anyway and already changed by reference</returns>
        public static PointF MultiplyVector(float q, ref PointF v)
        {
            v.X = q * v.X;
            v.Y = q * v.Y;
            return v;
        }

        /// <summary>
        /// Check if two vectors are parallel
        /// </summary>
        /// <param name="v">Vector V</param>
        /// <param name="w">Vector W</param>
        /// <returns>True if V and W are parallel, False otherwise</returns>
        public static bool AreParallelVectors(PointF v, PointF w)
        {
            PointF vp = v;
            Normalized(ref vp);

            PointF wp = w;
            Normalized(ref wp);

            if (vp == wp)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// computes the result of line(ab) equation for point p
        /// this value is 0 if p is on (ab)
        /// </summary>
        /// <param name="p"> The p. </param>
        /// <param name="a"> The a. </param>
        /// <param name="b"> The b. </param>
        /// <returns> Line equation of ab for p. </returns>
        public static float Value(PointF p, PointF a, PointF b)
        {
            PointF d = MakeVector(a, b);

            return (d.Y * p.X) - (d.X * p.Y) + (d.X * a.Y) - (d.Y * a.X);
        }

        /// <summary>
        /// returns a point that is the symmetrical opposite of p across line (ab)
        /// </summary>
        /// <param name="p">Point P</param>
        /// <param name="a">Start of AB</param>
        /// <param name="b">End of AB</param>
        /// <returns>Symmetrical of P by AB</returns>
        public static PointF Symmetrical(PointF p, PointF a, PointF b)
        {
            PointF h, n, q;

            h = new PointF();

            if (Math.Abs(Value(p, a, b)) < double.Epsilon)
            {
                return p;
            }

            n = NormalVector(a, b);
            Intersection(a, b, p, Translated(p, n), ref h);
            q = MakeVector(p, h);
            MultiplyVector(2, ref q);

            return Translated(p, q);
        }

        /// <summary>
        /// will modify p.X and p.Y to match polar coordinates rho,theta with angles in 360 degrees North at 0 clockwise
        /// </summary>
        /// <param name="p">Point we want to change</param>
        /// <param name="rho">Rho magnitude</param>
        /// <param name="theta">Theta angle</param>
        public static void FromPolar(ref PointF p, float rho, float theta)
        {
            p.X = rho * (float)Math.Cos((double)(90 - theta) * Math.PI / 180);
            p.Y = rho * (float)Math.Sin((double)(90 - theta) * Math.PI / 180);
        }

        /// <summary>
        /// Bearing of points
        /// </summary>
        /// <param name="a"> Point A </param>
        /// <param name="b"> Point B </param>
        /// <returns> Bearing of A and B </returns>
        public static float Bearing(PointF a, PointF b)
        {
            double f, g;
            double brg;

            f = b.X - a.X;
            g = b.Y - a.Y;

            if (Math.Abs(f - 0) > double.Epsilon)
            {
                if (g > 0)
                {
                    brg = Math.Atan(f / g) * 180 / Math.PI;
                }
                else
                {
                    brg = 180 + ((Math.Atan(f / g) * 180) / Math.PI);
                }

                if (brg < 0)
                {
                    brg += 360;
                }
            }
            else if (Math.Abs(f - 0) < double.Epsilon)
            {
                brg = 0;
            }
            else if (f > 0)
            {
                brg = 270;
            }
            else
            {
                brg = 90;
            }

            return (float)brg;
        }
    }
}
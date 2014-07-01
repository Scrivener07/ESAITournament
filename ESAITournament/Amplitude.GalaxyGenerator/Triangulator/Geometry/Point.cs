// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
//
//   Credit to Paul Bourke (pbourke@swin.edu.au) for the original Fortran 77 Program :))
//   Converted to a standalone C# 2.0 library by Morten Nielsen (www.iter.dk)
//   Check out: http://paulbourke.net/papers/triangulate/
//   You can use this code however you like providing the above credits remain in tact
// </copyright>
// <summary>
//   2D Point with double precision
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Triangulator.Geometry
{
    /// <summary>
    /// 2D Point with double precision
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class. 
        /// </summary>
        /// <param name="x">X position </param>
        /// <param name="y">Y position </param>
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the X component of the point
        /// </summary>
        public double X { get; private set; }

        /// <summary>
        /// Gets the Y component of the point
        /// </summary>
        public double Y { get; private set; }

        /// <summary>
        /// Makes a planar checks for if the points is spatially equal to another point.
        /// </summary>
        /// <param name="other">Point to check against</param>
        /// <returns>True if X and Y values are the same</returns>
        public bool Equals2D(Point other)
        {
            return Math.Abs(this.X - other.X) < double.Epsilon && Math.Abs(this.Y - other.Y) < double.Epsilon;
        }
    }
}

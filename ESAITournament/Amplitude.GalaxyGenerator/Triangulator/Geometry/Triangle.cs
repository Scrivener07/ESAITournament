// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Triangle.cs" company="AMPLITUDE Studios">
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
//   Triangle made from three point indexes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Triangulator.Geometry
{
    /// <summary>
    /// Triangle made from three point indexes
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// First vertex index in triangle
        /// </summary>
        public int P1;

        /// <summary>
        /// Second vertex index in triangle
        /// </summary>
        public int P2;

        /// <summary>
        /// Third vertex index in triangle
        /// </summary>
        public int P3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> struct. 
        /// </summary>
        /// <param name="point1"> Vertex 1 </param>
        /// <param name="point2"> Vertex 2 </param>
        /// <param name="point3"> Vertex 3 </param>
        public Triangle(int point1, int point2, int point3)
        {
            this.P1 = point1;
            this.P2 = point2;
            this.P3 = point3;
        }
    }
}
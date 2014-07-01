// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Edge.cs" company="AMPLITUDE Studios">
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
//   Edge made from two point indexes
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Triangulator.Geometry
{
    /// <summary>
    /// Edge made from two point indexes
    /// </summary>
    public struct Edge : IEquatable<Edge>
    {
        /// <summary>
        /// Index of the start of edge.
        /// </summary>
        public readonly int P1;

        /// <summary>
        /// Index of the end of edge
        /// </summary>
        public readonly int P2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> struct. 
        /// </summary>
        /// <param name="point1"> Start edge vertex index  </param>
        /// <param name="point2"> End edge vertex index  </param>
        public Edge(int point1, int point2)
        {
            this.P1 = point1;
            this.P2 = point2;
        }

        #region IEquatable<dEdge> Members

        /// <summary>
        /// Checks whether two edges are equal disregarding the direction of the edges.
        /// </summary>
        /// <param name="other">The Edge compared to.</param>
        /// <returns>True if Edges are equivalent, false otherwise.</returns>
        public bool Equals(Edge other)
        {
            return
                ((this.P1 == other.P2) && (this.P2 == other.P1)) ||
                ((this.P1 == other.P1) && (this.P2 == other.P2));
        }

        #endregion
    }
}

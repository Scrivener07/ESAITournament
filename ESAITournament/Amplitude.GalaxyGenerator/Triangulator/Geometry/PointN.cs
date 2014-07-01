// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointN.cs" company="AMPLITUDE Studios" name="Point{T}">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   A point with an attribute value of type 'T'
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Triangulator.Geometry
{
    /// <summary>
    /// A point with an attribute value of type 'T' .
    /// </summary>
    /// <typeparam name="T">Attribute of this point.</typeparam>
    public class Point<T> : Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point{T}"/> class. 
        /// </summary>
        /// <param name="x"> X component </param>
        /// <param name="y"> Y component </param>
        /// <param name="attribute"> Attribute of the Point </param>
        public Point(double x, double y, T attribute = default(T))
            : base(x, y)
        {
            this.Attribute = attribute;
        }

        /// <summary>
        /// Gets the attribute component of the point
        /// </summary>
        public T Attribute { get; private set; }
    }
}

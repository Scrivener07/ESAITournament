// <copyright file="WarpLine.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   
// </summary>

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// Warp line definition.
    /// </summary>
    public class WarpLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarpLine"/> class.
        /// </summary>
        /// <param name="a"> StarSystem origin of the WarpLine </param>
        /// <param name="b"> StarSystem end of the WarpLine </param>
        public WarpLine(StarSystem a, StarSystem b)
        {
            this.Id = Galaxy.Instance.Warps.Count;
            this.StarA = a;
            this.StarB = b;
            this.StarA.Destinations.Add(this.StarB);
            this.StarB.Destinations.Add(this.StarA);
            this.IsWormhole = false;
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it's a wormhole.
        /// </summary>
        public bool IsWormhole { get; protected set; }

        /// <summary>
        /// Gets or sets the star a.
        /// </summary>
        public StarSystem StarA { get; protected set; }

        /// <summary>
        /// Gets or sets the star b.
        /// </summary>
        public StarSystem StarB { get; protected set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public double Length
        {
            get { return Geometry2D.Distance(this.StarA.Position, this.StarB.Position); }
        }
    }
}

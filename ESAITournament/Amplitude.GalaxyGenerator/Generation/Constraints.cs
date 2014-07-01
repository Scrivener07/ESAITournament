// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constraints.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   The constraints.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// The constraints.
    /// </summary>
    public class Constraints
    {
        /// <summary>
        /// Gets or sets the minimum empire distance.
        /// </summary>
        public double MinEmpireDistance { get; set; }

        /// <summary>
        /// Gets or sets the minimum star distance.
        /// </summary>
        public double MinStarDistance { get; set; }

        /// <summary>
        /// Gets or sets the minimum stars per constellation.
        /// </summary>
        public int MinStarsPerConstellation { get; set; }

        // public int maxWormholesConnections;
    }
}
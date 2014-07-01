// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GalaxySize.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   The galaxy size.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// The galaxy size.
    /// </summary>
    public class GalaxySize
    {
        /// <summary>
        /// Gets or sets the luxury resource types.
        /// </summary>
        public int LuxuryResourceTypes { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the nominal players.
        /// </summary>
        public int NominalPlayers { get; set; }

        /// <summary>
        /// Gets or sets the num stars.
        /// </summary>
        public int NumStars { get; set; }

        /// <summary>
        /// Gets or sets the strategic resource number per type.
        /// </summary>
        public int StrategicResourceNumberPerType { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width { get; set; }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GalaxyAge.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   The galaxy age.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// The galaxy age.
    /// </summary>
    public class GalaxyAge
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the star type weight table.
        /// </summary>
        public Dictionary<string, int> StarTypeWeightTable { get; set; }
    }
}
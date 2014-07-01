// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Wormhole.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the Wormhole type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// Wormhole definition.
    /// </summary>
    public class Wormhole : WarpLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wormhole"/> class.
        /// </summary>
        /// <param name="a"> StarSystem origin of the Wormhole </param>
        /// <param name="b"> StarSystem end of the Wormhole </param>
        public Wormhole(StarSystem a, StarSystem b)
            : base(a, b)
        {
            this.IsWormhole = true;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeGenerator.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   TODO: Update summary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HomeGenerator : List<HomeTrait>
    {
        /// <summary>
        /// Applies each trait to the current star
        /// </summary>
        /// <param name="star">Target Star </param>
        public void Apply(StarSystem star)
        {
            foreach (HomeTrait t in this)
            {
                t.Apply(star);
            }
        }

        /// <summary>
        /// Generator Pattern
        /// </summary>
        public class Pattern
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Pattern"/> class.
            /// </summary>
            /// <param name="star">
            /// Star associated to that pattern
            /// </param>
            public Pattern(StarSystem star)
            {
                this.Star = star;
                this.HomeWorld = star.Planets[star.HomeWorldIndex];
                this.OtherPlanets = new List<Planet>(star.Planets);
                this.OtherPlanets.Remove(this.HomeWorld);
            }

            /// <summary>
            /// Gets the star.
            /// </summary>
            public StarSystem Star { get; private set; }

            /// <summary>
            /// Gets the home world.
            /// </summary>
            public Planet HomeWorld { get; private set; }

            /// <summary>
            /// Gets the other planets.
            /// </summary>
            public List<Planet> OtherPlanets { get; private set; }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlanetBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the PlanetBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// The planet builder.
    /// </summary>
    public class PlanetBuilder : Builder
    {
        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public override string Name
        {
            get { return "PlanetBuilder"; }
        }

        /// <summary>
        /// Executes the current's builder
        /// </summary>
        public override void Execute()
        {
            Trace.WriteLine(this.Name + " - Execute - begin");

            foreach (StarSystem star in Galaxy.Instance.Stars)
            {
                star.GeneratePlanets();
            }

            this.Result = true;

            Trace.WriteLine(this.Name + " - Execute - end");
        }
    }
}

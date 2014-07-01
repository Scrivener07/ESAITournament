// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the HomeBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// The home builder.
    /// </summary>
    public class HomeBuilder : Builder
    {
        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public override string Name
        {
            get { return "HomeBuilder"; }
        }

        /// <summary>
        /// The execute.
        /// </summary>
        public override void Execute()
        {
            Trace.WriteLine(this.Name + " - Execute - begin");

            int index = 0;
            while ((index < Galaxy.Instance.Configuration.HomeGenerators.Count) && (index < Galaxy.Instance.SpawnStars.Count))
            {
                Trace.WriteLine("Applying Home Generator " + index + " - begin");
                Galaxy.Instance.Configuration.HomeGenerators[index].Apply(Galaxy.Instance.SpawnStars[index]);
                Trace.WriteLine("Applying Home Generator " + index + " - end");
                index++;
            }

            this.Result = true;

            Trace.WriteLine(this.Name + " - Execute - end");
        }
    }
}

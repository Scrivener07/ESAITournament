// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constellation.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Amplitude.GalaxyGenerator.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// The constellation.
    /// </summary>
    public class Constellation : List<StarSystem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constellation"/> class.
        /// </summary>
        public Constellation()
        {
            this.Name = Galaxy.Instance.Configuration.GetRandomConstellationName();
            if (Galaxy.Instance.Constellations.Contains(this))
            {
                this.Id = Galaxy.Instance.Constellations.FindIndex(c => c == this);
            }
            else
            {
                this.Id = Galaxy.Instance.Constellations.Count;
                Galaxy.Instance.Constellations.Add(this);
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Retrieve the adjacent constellations
        /// </summary>
        /// <returns> Adjacent constellations </returns>
        public List<Constellation> AdjacentConstellations()
        {
            List<Constellation> list = new List<Constellation>();

            foreach (StarSystem s in this)
            {
                foreach (StarSystem t in s.Destinations)
                {
                    if (!this.Contains(t))
                    {
                        list.Add(t.Constellation());
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Retrieves the region indices in this region
        /// </summary>
        /// <returns> Colors representing region indices </returns>
        public List<Color> PresentRegionIndexes()
        {
            List<Color> indexes = new List<Color>();

            foreach (StarSystem s in this)
            {
                if (!indexes.Contains(s.RegionIndex))
                {
                    indexes.Add(s.RegionIndex);
                }
            }

            return indexes;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Region.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the Region type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using Amplitude.GalaxyGenerator.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// Region is a List{StarSystem}. Use it accordingly
    /// </summary>
    public class Region : List<StarSystem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="c"> Region's color </param>
        public Region(Color c)
        {
            this.Index = c;
            this.AddRange(Galaxy.Instance.Stars.FindAll(s => (s.RegionIndex == this.Index)));
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public Color Index { get; protected set; }

        /// <summary>
        /// Determines if the Region is a spawn zone.
        /// </summary>
        /// <returns> True if the region is a spawn </returns>
        public bool IsSpawn()
        {
            if (Galaxy.Instance.Configuration.Shape.Regions.ContainsKey(this.Index))
            {
                return Galaxy.Instance.Configuration.Shape.Regions[this.Index];
            }

            return false;
        }

        /// <summary>
        /// List of adjacent regions to this region.
        /// </summary>
        /// <returns> The list of adjacent regions to this region. </returns>
        public List<Region> AdjacentRegions()
        {
            List<Region> adjacents = new List<Region>();

            foreach (Shape.Link link in Galaxy.Instance.Configuration.Shape.Topology)
            {
                if (link.RegionA == this.Index)
                {
                    adjacents.Add(Galaxy.Instance.Regions.Find(r => r.Index == link.RegionB));
                }
                else if (link.RegionB == this.Index)
                {
                    adjacents.Add(Galaxy.Instance.Regions.Find(r => r.Index == link.RegionA));
                }
            }

            return adjacents;
        }
    }
}

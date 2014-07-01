// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StarSystem.cs" company="AMPLITUDE Studios">
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

using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude.GalaxyGenerator.Drawing;

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// Star system's definition.
    /// </summary>
    public class StarSystem : IComparable<StarSystem>
    {
        /// <summary>
        /// Minimum number of planets in the StarSystem
        /// </summary>
        public static readonly int MinPlanets = 1;

        /// <summary>
        /// Maximum number of planets in the StarSystem
        /// </summary>
        public static readonly int MaxPlanets = 6;

        /// <summary>
        /// Far away distance for later computations
        /// </summary>
        private const int VeryFarAway = 999999;

        /// <summary>
        /// Initializes a new instance of the <see cref="StarSystem"/> class.
        /// </summary>
        /// <param name="position"> Position in the galaxy </param>
        public StarSystem(PointF position)
        {
            this.Planets = new List<Planet>();
            this.Destinations = new HashSet<StarSystem>();
            this.PreWarps = new HashSet<StarSystem>();
            this.WarpDistanceTable = new Dictionary<StarSystem, int>();
            this.DirectDistanceTable = new Dictionary<StarSystem, double>();

            this.Position = position;

            this.Id = Galaxy.Instance.Stars.Count;
            this.Name = Galaxy.Instance.Configuration.GetRandomStarName();
            this.RegionIndex = Color.Black;
            this.Type = Galaxy.Instance.Configuration.GetRandomStarType();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public PointF Position { get; set; }

        /// <summary>
        /// Gets or sets the region index.
        /// </summary>
        public Color RegionIndex { get; set; }

        /// <summary>
        /// Gets or sets the planets.
        /// </summary>
        public List<Planet> Planets { get; protected set; }

        /// <summary>
        /// Gets or sets the destinations.
        /// </summary>
        public HashSet<StarSystem> Destinations { get; set; }

        /// <summary>
        /// Gets or sets the pre warps.
        /// </summary>
        public HashSet<StarSystem> PreWarps { get; set; }

        /// <summary>
        /// Gets or sets the home world index.
        /// </summary>
        public int HomeWorldIndex { get; protected set; }

        /// <summary>
        /// Gets the region.
        /// </summary>
        public Region Region
        {
            get { return Galaxy.Instance.Regions.Find(r => r.Index == this.RegionIndex); }
        }

        /// <summary>
        /// Gets or sets the direct distance table.
        /// </summary>
        public Dictionary<StarSystem, double> DirectDistanceTable { get; protected set; }

        /// <summary>
        /// Gets or sets the warp distance table.
        /// </summary>
        public Dictionary<StarSystem, int> WarpDistanceTable { get; protected set; }

        /// <summary>
        /// Find the farthest starsystem from this starsystem
        /// </summary>
        /// <param name="from"> Origin of the search </param>
        /// <param name="into"> Pool of starsystem where we want to find the farthest one </param>
        /// <returns>The farthest starsystem </returns>
        public static StarSystem FindFarthestStar(StarSystem from, List<StarSystem> into)
        {
            if (from.DirectDistanceTable.Count == 0)
            {
                from.ComputeDirectDistanceTable();
            }

            double distanceMax = 0;
            StarSystem farthest = null;

            foreach (StarSystem s in from.DirectDistanceTable.Keys)
            {
                if (into.Contains(s))
                {
                    double distance = from.DirectDistanceTable[s];
                    if (distance > distanceMax)
                    {
                        distanceMax = distance;
                        farthest = s;
                    }
                }
            }

            return farthest;
        }

        /// <summary>
        /// Implementation of IComparable for the StarSystems
        /// </summary>
        /// <param name="other"> StarSystem compared against </param>
        /// <returns> negative number if the other is lower, zero if they are the same, positive if the current StarSystem is higher </returns>
        public int CompareTo(StarSystem other)
        {
            return other.Id - this.Id;
        }

        /// <summary>
        /// Gets the warps connected to starsystem
        /// </summary>
        /// <returns> A list of warps coming to this starsystem </returns>
        public List<WarpLine> Warps()
        {
            return new List<WarpLine>(Galaxy.Instance.Warps.FindAll(w => (w.StarA == this) || (w.StarB == this)));
        }

        /// <summary>
        /// Checks if the star system has a wormhole coming to it
        /// </summary>
        /// <returns> True if there's a wormhole coming to this starsystem, false otherwise </returns>
        public bool HasWormhole()
        {
            return Galaxy.Instance.Warps.Any(w => w.IsWormhole && ((w.StarA == this) || (w.StarB == this)));
        }

        /// <summary>
        /// Generates a Planet
        /// </summary>
        /// <param name="n"> The i dont know </param>
        public void GeneratePlanets(int n = -1)
        {
            int planetNumber = n == -1 ? Galaxy.Instance.Configuration.GetRandomPlanetNumber() : n;

            if (planetNumber < MinPlanets)
            {
                planetNumber = MinPlanets;
            }

            if (planetNumber > MaxPlanets)
            {
                planetNumber = MaxPlanets;
            }

            Galaxy.Instance.Planets.RemoveAll(p => this.Planets.Contains(p));
            this.Planets.Clear();

            for (int i = 0; i < planetNumber; i++)
            {
                this.Planets.Add(new Planet(this));
            }

            this.HomeWorldIndex = 0;
        }

        /// <summary>
        /// Computes distance between this starsystem and all others starsystems
        /// </summary>
        public void ComputeDirectDistanceTable()
        {
            foreach (StarSystem s in Galaxy.Instance.Stars)
            {
                if (!this.DirectDistanceTable.Keys.Contains(s))
                {
                    this.DirectDistanceTable.Add(s, Geometry2D.Distance(this.Position, s.Position));
                }
            }
        }

        /// <summary>
        /// Gets the constellation where this StarSystem stands
        /// </summary>
        /// <returns> The constellation where this StarSystem stands </returns>
        public Constellation Constellation()
        {
            return Galaxy.Instance.Constellations.Find(c => c.Contains(this));
        }

        /// <summary>
        /// Computes the warp distance table for this StarSystem
        /// </summary>
        public void ComputeWarpDistanceTable()
        {
            foreach (StarSystem s in Galaxy.Instance.Stars)
            {
                this.WarpDistanceTable[s] = VeryFarAway;
            }

            this.RecursiveWarpDistance(this, 0);
        }

        /// <summary>
        /// Computes recursively the warp distance table for this StarSystem
        /// </summary>
        /// <param name="s"> Origin StarSystem. </param>
        /// <param name="d"> Previously computed distance. </param>
        private void RecursiveWarpDistance(StarSystem s, int d)
        {
            if (d >= this.WarpDistanceTable[s])
            {
                return;
            }

            this.WarpDistanceTable[s] = d;
            foreach (StarSystem sj in s.Destinations)
            {
                this.RecursiveWarpDistance(sj, d + 1);
            }
        }
    }
}

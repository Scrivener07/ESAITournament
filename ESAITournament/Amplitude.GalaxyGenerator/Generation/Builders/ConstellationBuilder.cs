// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConstellationBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the ConstellationBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amplitude.GalaxyGenerator.Drawing;
using Amplitude.GalaxyGenerator.Generation.Components;
using Region = Amplitude.GalaxyGenerator.Generation.Components.Region;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// The constellation builder.
    /// </summary>
    public class ConstellationBuilder : Builder
    {
        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public override string Name
        {
            get { return "ConstellationBuilder"; }
        }

        /// <summary>
        /// Finds closest pair of StarSystem from two lists.
        /// </summary>
        /// <param name="listA"> StarSystems from first constellation </param>
        /// <param name="listB"> StarSystems from second constellation </param>
        /// <returns> A list containing the closest pair of StarSystems </returns>
        public static List<StarSystem> FindClosestPair(List<StarSystem> listA, List<StarSystem> listB)
        {
            List<StarSystem> pair = new List<StarSystem>();

            if (listA == null)
            {
                return pair;
            }

            if (listB == null)
            {
                return pair;
            }

            if (listA.Count <= 0)
            {
                return pair;
            }

            if (listB.Count <= 0)
            {
                return pair;
            }

            double distance, distanceMin;

            pair.Add(null);
            pair.Add(null);
            distanceMin = Galaxy.Instance.Diameter() * 2;
            foreach (StarSystem a in listA)
            {
                foreach (StarSystem b in listB)
                {
                    distance = Geometry2D.Distance(a.Position, b.Position);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        pair[0] = a;
                        pair[1] = b;
                    }
                }
            }

            if ((pair[0] == null) || (pair[1] == null))
            {
                pair.Clear();
            }

            return pair;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        public override void Execute()
        {
            int constellationNumber;
            int factorA, factorB, a, b, factor;
            bool gotMatch;
            List<Region> neutralRegions = new List<Region>();
            List<Region> spawnRegions = new List<Region>();

            Trace.WriteLine(this.Name + " - Execute - begin");

            // BUILD REGIONS
            neutralRegions.AddRange(Galaxy.Instance.Regions.FindAll(r => !r.IsSpawn()));
            spawnRegions.AddRange(Galaxy.Instance.Regions.FindAll(r => r.IsSpawn()));

            Trace.WriteLine(Galaxy.Instance.Configuration.Shape.Regions.Keys.Count + " theoretical regions");
            Trace.WriteLine(Galaxy.Instance.Regions.Count + " actual regions");
            Trace.WriteLine(spawnRegions.Count + " spawn regions");

            foreach (Region r in spawnRegions)
            {
                Trace.WriteLine("-->" + r.Index + " containing " + r.Count + " stars");
            }

            Trace.WriteLine(neutralRegions.Count + " neutral regions");
            foreach (Region r in neutralRegions)
            {
                Trace.WriteLine("-->" + r.Index + " containing " + r.Count + " stars");
            }

            // DETERMINE ACTUAL CONSTELLATIONS NUMBER
            constellationNumber = Galaxy.Instance.Configuration.Constellations;
            Trace.WriteLine("Configuration requested constellations : " + Galaxy.Instance.Configuration.Constellations);

            while ((constellationNumber * Settings.Instance.GenerationConstraints.MinStarsPerConstellation) > Galaxy.Instance.Stars.Count)
            {
                constellationNumber--;
            }

            if (constellationNumber <= 0)
            {
                constellationNumber = 1;
            }

            if (constellationNumber < Galaxy.Instance.Configuration.Constellations)
            {
                Trace.WriteLine("Min stars per constellation : " + Settings.Instance.GenerationConstraints.MinStarsPerConstellation);
                Trace.WriteLine("Will use only " + constellationNumber + " constellations");
                this.Defects.Add("Number of constellations was limited by stars number");
            }

            // DISTRIBUTE CONSTELLATIONS ACROSS REGIONS
            if (constellationNumber == 1)
            {
                Trace.WriteLine("Single Constellation");
                Constellation c = new Constellation();
                c.AddRange(Galaxy.Instance.Stars);
            }
            else if (constellationNumber == Galaxy.Instance.Regions.Count)
            {
                Trace.WriteLine("One Constellation Per Region");
                foreach (Region r in Galaxy.Instance.Regions)
                {
                    Constellation c = new Constellation();
                    c.AddRange(r);
                }
            }
            else
            {
                Trace.WriteLine("Other Case");
                factorA = 0;
                factorB = 0;
                gotMatch = false;

                for (a = 1; a < 20; a++)
                {
                    for (b = 0; b < 20; b++)
                    {
                        if ((a * spawnRegions.Count) + (b * neutralRegions.Count) == constellationNumber)
                        {
                            gotMatch = true;
                            factorA = a;
                            factorB = b;
                        }
                    }
                }

                if (gotMatch)
                {
                    Trace.WriteLine("Could find integers A=" + factorA + " and B=" + factorB);
                    Trace.WriteLine("Allowing A Constellations in each Spawn Region");
                    Trace.WriteLine("And B Constellation in each Neutral Region");

                    if (factorA > 0)
                    {
                        foreach (Region r in spawnRegions)
                        {
                            this.MakeConstellations(factorA, r);
                        }
                    }

                    if (factorB > 0)
                    {
                        foreach (Region r in neutralRegions)
                        {
                            this.MakeConstellations(factorB, r);
                        }
                    }
                }
                else
                {
                    Trace.WriteLine("No exact match");

                    if (constellationNumber >= spawnRegions.Count)
                    {
                        Trace.WriteLine("More Constellations than Spawn Regions");
                        foreach (Region r in spawnRegions)
                        {
                            this.MakeConstellations(1, r);
                        }

                        if (constellationNumber - spawnRegions.Count > 0)
                        {
                            List<StarSystem> pool = new List<StarSystem>();

                            foreach (Region r in neutralRegions)
                            {
                                pool.AddRange(r);
                            }

                            this.MakeConstellations(constellationNumber - spawnRegions.Count, pool);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Less Constellations than Spawn Regions");
                        factor = 1;
                        while (factor * constellationNumber < spawnRegions.Count)
                        {
                            factor++;
                        }

                        Region start;
                        Region merge;
                        List<Region> adjacentSpawnRegions = new List<Region>();
                        List<Region> mergedRegions = new List<Region>();
                        List<Region> nextStartCandidates = new List<Region>();
                        List<StarSystem> pool = new List<StarSystem>();

                        // loop
                        // starting with one random spawn region
                        // merge (factor adjacent spawn regions) into result
                        // make one constellation in result
                        // until remaining spawn regions number less than factor
                        Trace.WriteLine("Using topology to try grouping " + factor + " Spawn Regions in each Constellation");

                        start = spawnRegions.ElementAt(GalaxyGeneratorPlugin.Random.Next(spawnRegions.Count));
                        while ((spawnRegions.Count >= factor) && (start != null) && (Galaxy.Instance.Constellations.Count < constellationNumber))
                        {
                            spawnRegions.Remove(start);
                            mergedRegions.Clear();
                            mergedRegions.Add(start);
                            adjacentSpawnRegions.AddRange(start.AdjacentRegions().FindAll(r => r.IsSpawn()));
                            adjacentSpawnRegions.RemoveAll(r => !spawnRegions.Contains(r));
                            for (int i = 0; (i < factor - 1) && (adjacentSpawnRegions.Count > 0); i++)
                            {
                                adjacentSpawnRegions.Clear();
                                foreach (Region r in mergedRegions)
                                {
                                    adjacentSpawnRegions.AddRange(r.AdjacentRegions());
                                }

                                adjacentSpawnRegions.RemoveAll(mergedRegions.Contains);
                                adjacentSpawnRegions.RemoveAll(r => !r.IsSpawn());
                                adjacentSpawnRegions.RemoveAll(r => !spawnRegions.Contains(r));
                                if (adjacentSpawnRegions.Count > 0)
                                {
                                    merge = adjacentSpawnRegions.ElementAt(GalaxyGeneratorPlugin.Random.Next(adjacentSpawnRegions.Count));
                                    mergedRegions.Add(merge);
                                    spawnRegions.Remove(merge);
                                }
                            }

                            Trace.WriteLine("Merging regions :");
                            foreach (Region r in mergedRegions)
                            {
                                Trace.WriteLine("--->" + r.Index);
                            }

                            pool.Clear();
                            foreach (Region r in mergedRegions)
                            {
                                pool.AddRange(r);
                            }

                            this.MakeConstellations(1, pool);

                            nextStartCandidates.Clear();
                            foreach (Region r in mergedRegions)
                            {
                                nextStartCandidates.AddRange(r.AdjacentRegions());
                            }

                            nextStartCandidates.RemoveAll(mergedRegions.Contains);
                            nextStartCandidates.RemoveAll(r => !r.IsSpawn());
                            nextStartCandidates.RemoveAll(r => !spawnRegions.Contains(r));

                            if (nextStartCandidates.Count > 0)
                            {
                                start = nextStartCandidates.ElementAt(GalaxyGeneratorPlugin.Random.Next(nextStartCandidates.Count));
                            }
                            else if (spawnRegions.Count > 0)
                            {
                                start = spawnRegions.ElementAt(GalaxyGeneratorPlugin.Random.Next(spawnRegions.Count));
                            }
                            else
                            {
                                start = null;
                            }
                        }

                        // merge (remaining spawn regions with neutral regions) into result
                        // make (nConstellations - Galaxy.Instance.Constellations.Count) constellations in result
                        Trace.WriteLine("Merging remaining Spawn Regions with Neutral Regions");
                        Trace.WriteLine("and making remaining Constellations");
                        pool.Clear();
                        foreach (Region r in spawnRegions)
                        {
                            pool.AddRange(r);
                        }

                        foreach (Region r in neutralRegions)
                        {
                            pool.AddRange(r);
                        }

                        this.MakeConstellations(constellationNumber - Galaxy.Instance.Constellations.Count, pool);
                    }
                }

                if (Galaxy.Instance.Constellations.Count == 0)
                {
                    Trace.WriteLine("Failing to associate regions and constellations");
                    Trace.WriteLine("Creating brutally " + constellationNumber + " constellations with " + Galaxy.Instance.Stars.Count);
                    this.MakeConstellations(constellationNumber, Galaxy.Instance.Stars);
                    this.Defects.Add("Unable to correlate regions and constellations");
                }

                this.AggregateIsolatedStars();
            }

            this.Result = true;

            Trace.WriteLine(this.Name + " - Execute - end");
        }

        /// <summary>
        /// Makes constellations
        /// </summary>
        /// <param name="quantity"> The quantity. </param>
        /// <param name="pool"> The pool. </param>
        protected void MakeConstellations(int quantity, List<StarSystem> pool)
        {
            if (quantity <= 0)
            {
                return;
            }

            if (null == pool)
            {
                return;
            }

            if (pool.Count <= 0)
            {
                return;
            }

            PointF center = new PointF(0, 0);
            PointF delta, nearestFocus, diametralStar;
            float distance, distanceMax, distanceMin, angle;
            Dictionary<PointF, Constellation> constellations = new Dictionary<PointF, Constellation>();
            float i, startAngle, poolRadius;
            int modifiedQuantity;

            Trace.WriteLine("Try making " + quantity + " constellations with a total of " + pool.Count + " stars");
            modifiedQuantity = quantity;

            while ((modifiedQuantity > 1) && (modifiedQuantity * Settings.Instance.GenerationConstraints.MinStarsPerConstellation > pool.Count))
            {
                modifiedQuantity--;
            }

            Trace.WriteLine("Make " + modifiedQuantity + " constellations with a total of " + pool.Count + " stars");

            // Computing center of gravity of pool
            foreach (StarSystem s in pool)
            {
                center.X += s.Position.X / (float)pool.Count;
                center.Y += s.Position.Y / (float)pool.Count;
            }

            // Computing pool radius
            poolRadius = 0;
            foreach (StarSystem s in pool)
            {
                distance = Geometry2D.Distance(center, s.Position);
                if (distance > poolRadius)
                {
                    poolRadius = distance;
                }
            }

            poolRadius = poolRadius * 1.1f;

            // Looking for a diametral star to establish startAngle
            distanceMax = 0;
            diametralStar = new PointF(center.X, center.Y);
            foreach (StarSystem s in pool)
            {
                StarSystem farthest = StarSystem.FindFarthestStar(s, pool);
                distance = -1;
                if (farthest != null)
                {
                    distance = (float)s.DirectDistanceTable[farthest];
                }

                if (distance > distanceMax)
                {
                    distanceMax = distance;
                    diametralStar = s.Position;
                }
            }

            startAngle = Geometry2D.Bearing(center, diametralStar);

            // Preparing focuses and preparing associated constellations
            delta = new PointF();
            for (i = 0; i < modifiedQuantity; i++)
            {
                angle = (i * 360 / (float)modifiedQuantity) + startAngle;
                Geometry2D.FromPolar(ref delta, poolRadius, angle);
                nearestFocus = new PointF(delta.X + center.X, delta.Y + center.Y);
                constellations.Add(nearestFocus, new Constellation());
            }

            // Associating focuses with stars
            List<StarSystem> countdownPool = new List<StarSystem>(pool);

            // grabbing closest star to seed constellations
            foreach (PointF p in constellations.Keys)
            {
                StarSystem closest = null;
                distanceMin = poolRadius;
                foreach (StarSystem s in countdownPool)
                {
                    distance = Geometry2D.Distance(s.Position, p);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        closest = s;
                    }
                }

                if (closest != null)
                {
                    constellations[p].Add(closest);
                }
            }

            this.FindAndFeedStarvedConstellations(constellations.Values.ToList(), ref countdownPool);

            // filling up constellations
            foreach (StarSystem s in countdownPool)
            {
                distanceMin = poolRadius;
                nearestFocus = constellations.Keys.First();
                foreach (PointF p in constellations.Keys)
                {
                    distance = Geometry2D.Distance(s.Position, p);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        nearestFocus = p;
                    }
                }

                constellations[nearestFocus].Add(s);
            }
        }

        /// <summary>
        /// Find and feed starved constellations.
        /// </summary>
        /// <param name="constellations"> The constellations. </param>
        /// <param name="pool"> The pool. </param>
        protected void FindAndFeedStarvedConstellations(List<Constellation> constellations, ref List<StarSystem> pool)
        {
            List<Constellation> starvedConstellations = new List<Constellation>();

            starvedConstellations.AddRange(constellations.FindAll(c => (c.Count < Settings.Instance.GenerationConstraints.MinStarsPerConstellation)));
            starvedConstellations.RemoveAll(c => (c.Count <= 0));

            while ((starvedConstellations.Count > 0) && (pool.Count > 0))
            {
                foreach (Constellation candidate in starvedConstellations)
                {
                    if ((pool.Count > 0) && (candidate.Count < Settings.Instance.GenerationConstraints.MinStarsPerConstellation))
                    {
                        Trace.WriteLine("Remaining " + starvedConstellations.Count + " starved constellations !");
                        List<StarSystem> pair = new List<StarSystem>(FindClosestPair(pool, candidate));
                        StarSystem closest = null;
                        if (pair.Count > 0)
                        {
                            closest = pair[0];
                        }

                        if (closest != null)
                        {
                            candidate.Add(closest);
                            pool.Remove(closest);
                        }
                    }
                }

                starvedConstellations.RemoveAll(c => (c.Count >= Settings.Instance.GenerationConstraints.MinStarsPerConstellation));
            }
        }

        /// <summary>
        /// Aggregates isolated stars.
        /// </summary>
        protected void AggregateIsolatedStars()
        {
            List<StarSystem> isolated = new List<StarSystem>();
            List<StarSystem> initiallyIsolated = new List<StarSystem>();
            List<StarSystem> others = new List<StarSystem>();
            Constellation candidate;
            StarSystem star, closest;
            Dictionary<StarSystem, Constellation> takers = new Dictionary<StarSystem, Constellation>();

            isolated.AddRange(Galaxy.Instance.Stars.FindAll(s => (s.Constellation() == null)));

            this.FindAndFeedStarvedConstellations(Galaxy.Instance.Constellations, ref isolated);

            initiallyIsolated.AddRange(isolated);
            while (isolated.Count > 0)
            {
                Trace.WriteLine("Aggregating " + isolated.Count + " Stars to existing Constellations");

                others.Clear();
                others.AddRange(Galaxy.Instance.Stars);
                others.RemoveAll(initiallyIsolated.Contains);

                star = isolated.ElementAt(GalaxyGeneratorPlugin.Random.Next(isolated.Count));
                candidate = null;
                while ((candidate == null) && (others.Count > 0))
                {
                    closest = WarpBuilder.FindClosest(star, others);
                    if (closest != null)
                    {
                        others.Remove(closest);
                        if (closest.Constellation().PresentRegionIndexes().Contains(star.RegionIndex))
                        {
                            candidate = closest.Constellation();
                        }
                    }
                }

                if (candidate != null)
                {
                    takers.Add(star, candidate);
                }
                else
                {
                    others.Clear();
                    others.AddRange(Galaxy.Instance.Stars);
                    others.RemoveAll(initiallyIsolated.Contains);

                    closest = WarpBuilder.FindClosest(star, others);
                    takers.Add(star, closest.Constellation());
                }

                isolated.RemoveAll(takers.ContainsKey);
            }

            foreach (StarSystem s in takers.Keys)
            {
                takers[s].Add(s);
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpawnBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the SpawnBuilder type.
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
    /// Spawn builder.
    /// </summary>
    public class SpawnBuilder : Builder
    {
        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public override string Name
        {
            get { return "SpawnBuilder"; }
        }

        /// <summary>
        /// Counts interesting connections for a given StarSystem.
        /// </summary>
        /// <param name="starSystem"> The starSystem system. </param>
        /// <returns> Number of interesting connection for this StarSystem </returns>
        public static int CountInterestingConnections(StarSystem starSystem)
        {
            int n = 0;

            foreach (WarpLine w in starSystem.Warps())
            {
                if (w.IsWormhole)
                {
                    n += 1;
                }
                else if (w.StarA.Region != w.StarB.Region)
                {
                    n += 2;
                }
                else if (w.StarA.Region == w.StarB.Region)
                {
                    n += 5;
                }
            }

            return n;
            /*
            return starSystem.warps().Count((w) =>
            {
                return (!w.isWormhole)
                    && (    (w.StarA.region == w.StarB.region)
                            || (!w.StarA.region.isSpawn())
                            || (!w.StarB.region.isSpawn()) 
                       );
            }
            );
             */
        }

        /// <summary>
        /// Find next spawner.
        /// </summary>
        /// <param name="region"> The region. </param>
        /// <returns> A Region </returns>
        public static Region FindNextSpawner(Region region)
        {
            Color c;
            List<Color> sequence = new List<Color>(Galaxy.Instance.Configuration.Shape.SpawnerSequence);

            if (sequence.Count <= 0)
            {
                return null;
            }

            Color next = sequence.First();
            if (null != region)
            {
                c = region.Index;
                if (sequence.Contains(c))
                {
                    next = sequence.ElementAt((1 + sequence.IndexOf(c)) % sequence.Count);
                }
            }

            return Galaxy.Instance.Regions.Find(r => (r.Index == next));
        }

        /// <summary>
        /// Executes current's builder
        /// </summary>
        public override void Execute()
        {
            List<Region> spawnRegions = new List<Region>(Galaxy.Instance.Regions.FindAll(r => r.IsSpawn()));
            int empiresNumber = Galaxy.Instance.Configuration.EmpiresNumber();
            List<StarSystem> interdicted = new List<StarSystem>();
            List<StarSystem> candidates = new List<StarSystem>();
            StarSystem best;

            Trace.WriteLine(this.Name + " - Execute - begin");
            Trace.WriteLine("Spawn generation with " + empiresNumber + " empires");

            spawnRegions.RemoveAll(r => r.Count <= 0);

            Region spawner = FindNextSpawner(null);

            while ((spawnRegions.Count > 0) && (empiresNumber > 0))
            {
                candidates.Clear();
                candidates.AddRange(spawner);
                candidates.RemoveAll(s => Galaxy.Instance.SpawnStars.Contains(s));
                if (interdicted.Count(s => spawner.Contains(s)) <= candidates.Count)
                {
                    candidates.RemoveAll(interdicted.Contains);
                }

                if (spawner.Count(s => s.Destinations.Count <= 1) < candidates.Count)
                {
                    candidates.RemoveAll(s => s.Destinations.Count <= 1);
                }

                int maxConnections = 0;
                foreach (StarSystem s in candidates)
                {
                    int connections = CountInterestingConnections(s);
                    if (connections > maxConnections)
                    {
                        maxConnections = connections;
                    }
                }

                candidates.RemoveAll(s => (maxConnections > CountInterestingConnections(s)));

                best = this.FindFarthest(candidates, Galaxy.Instance.SpawnStars);

                if (best != null)
                {
                    Galaxy.Instance.SpawnStars.Add(best);
                    empiresNumber--;

                    // prohibit all proximate stars for subsequent spawns
                    interdicted.AddRange(Galaxy.Instance.Stars.FindAll(s => (Geometry2D.Distance(best.Position, s.Position) < Settings.Instance.GenerationConstraints.MinEmpireDistance)));
                }

                spawnRegions.Remove(spawner);
                spawner = FindNextSpawner(spawner);
            }

            List<StarSystem> downgradedCandidates = new List<StarSystem>();

            if (empiresNumber > 0)
            {
                spawnRegions.Clear();

                // take all spawners
                spawnRegions.AddRange(Galaxy.Instance.Regions.FindAll(r => r.IsSpawn()));

                // remove all already taken spawn regions
                spawnRegions.RemoveAll(r => 0 < Galaxy.Instance.SpawnStars.Count(s => s.Region.Index == r.Index));

                // restart spawn region sequence
                spawner = FindNextSpawner(null);

                while (!spawnRegions.Contains(spawner))
                {
                    spawner = FindNextSpawner(spawner);
                }

                this.Defects.Add("Using downgraded spawn algorithm");
            }

            while (empiresNumber > 0)
            {
                Trace.WriteLine("Downgraded Spawn Algorithms - Downgraded spawns remaining : " + empiresNumber);

                downgradedCandidates.Clear();
                downgradedCandidates.AddRange(spawner);
                downgradedCandidates.RemoveAll(s => Galaxy.Instance.SpawnStars.Contains(s));

                if (downgradedCandidates.Count == 0)
                {
                    downgradedCandidates.AddRange(Galaxy.Instance.Stars);
                }

                downgradedCandidates.RemoveAll(s => Galaxy.Instance.SpawnStars.Contains(s));
                best = this.FindFarthest(downgradedCandidates, Galaxy.Instance.SpawnStars);

                if (best == null)
                {
                    Trace.WriteLine("FAILED TO SPAWN");
                    this.Result = false;
                    return;
                }

                Galaxy.Instance.SpawnStars.Add(best);
                empiresNumber--;
                while (!spawnRegions.Contains(spawner))
                {
                    spawner = FindNextSpawner(spawner);
                }
            }

            Trace.WriteLine("Spawn Builder placed " + Galaxy.Instance.SpawnStars.Count + " empires");

            if (Galaxy.Instance.SpawnStars.Count < Galaxy.Instance.Configuration.EmpiresNumber())
            {
                this.TraceDefect("Failed to spawn - Not enough empires were spawned", true);
                return;
            }

            // Shuffle spawn stars
            List<StarSystem> sourceSpawn = new List<StarSystem>(Galaxy.Instance.SpawnStars);
            StarSystem star;
            Galaxy.Instance.SpawnStars.Clear();
            while (sourceSpawn.Count > 0)
            {
                star = sourceSpawn.ElementAt(GalaxyGeneratorPlugin.Random.Next(sourceSpawn.Count));
                Galaxy.Instance.SpawnStars.Add(star);
                sourceSpawn.Remove(star);
            }

            this.Result = true;

            Trace.WriteLine(this.Name + " - Execute - end");
        }

        /// <summary>
        /// Find the farthest starsystem
        /// </summary>
        /// <param name="candidates"> The candidates. </param>
        /// <param name="repellents"> The repellents. </param>
        /// <returns> Farthest StarSystem </returns>
        protected StarSystem FindFarthest(List<StarSystem> candidates, List<StarSystem> repellents)
        {
            if (candidates == null)
            {
                return null;
            }

            if (candidates.Count <= 0)
            {
                return null;
            }

            List<StarSystem> localRepellents = new List<StarSystem>();
            if (repellents != null)
            {
                localRepellents.AddRange(repellents);
            }

            if ((localRepellents.Count <= 0) && (candidates.Count > 0))
            {
                return candidates.ElementAt(GalaxyGeneratorPlugin.Random.Next(candidates.Count));
            }

            StarSystem farthest = null;

            double distanceMax = 0;
            foreach (StarSystem c in candidates)
            {
                foreach (StarSystem r in localRepellents)
                {
                    double distance = Geometry2D.Distance(c.Position, r.Position);
                    if (distance > distanceMax)
                    {
                        distanceMax = distance;
                        farthest = c;
                    }
                }
            }

            return farthest;
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LuxuryResourceBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the LuxuryResourceBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// The luxury resource builder.
    /// </summary>
    public class LuxuryResourceBuilder : Builder
    {
        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public override string Name
        {
            get { return "LuxuryResourceBuilder"; }
        }

        /// <summary>
        /// Execute the current builder
        /// </summary>
        public override void Execute()
        {
            Trace.WriteLine(this.Name + " - Execute - begin");

            int numberOfSites, quantity;
            List<Planet> sites = new List<Planet>();
            HashSet<Planet> usedSites = new HashSet<Planet>();
            HashSet<string> usedLuxes = new HashSet<string>();
            Planet selectedPlanet;
            StarSystem sourceCenter;
            int tier, minTier, maxTier;

            minTier = Settings.Instance.LuxuryResourceTiers.Keys.Min();
            maxTier = Settings.Instance.LuxuryResourceTiers.Keys.Max();

            quantity = Galaxy.Instance.Configuration.LuxuryResourceNumberOfTypes;

            Trace.WriteLine("total luxes = " + Settings.Instance.LuxuryResourceNames.Count);
            Trace.WriteLine("number of lux types " + quantity);
            tier = minTier;
            do
            {
                // System.Diagnostics.Trace.WriteLine("tier " + tier.ToString() + " usedLuxes.Count=" +usedLuxes.Count.ToString());
                usedLuxes.Add(Galaxy.Instance.Configuration.GetRandomLuxuryResource(tier));
                do
                {
                    tier++;
                    if (tier > maxTier)
                    {
                        tier = minTier;
                    }
                }
                while (!Settings.Instance.LuxuryResourceTiers.ContainsKey(tier));
            }
            while ((usedLuxes.Count < quantity) && (usedLuxes.Count < Settings.Instance.LuxuryResourceNames.Count));

            foreach (string resName in usedLuxes)
            {
                sourceCenter = Galaxy.Instance.Stars[GalaxyGeneratorPlugin.Random.Next(Galaxy.Instance.Stars.Count)];
                quantity = 7;
                numberOfSites = 4;
                sites.Clear();
                usedSites.Clear();
                foreach (StarSystem sys in Galaxy.Instance.Stars)
                {
                    foreach (Planet p in sys.Planets)
                    {
                        if (p.Resource == null)
                        {
                            for (int i = 0; i < Settings.Instance.PlanetLuxuriesPerPlanetType[p.Type][resName]; i++)
                            {
                                sites.Add(p);
                            }
                        }
                    }
                }

                if (sites.Count == 0)
                {
                    quantity = 0;
                }

                if (sites.Distinct().Count() < 2)
                {
                    quantity = 0;
                }

                for (int i = 0; i < quantity; i++)
                {
                    if (usedSites.Count < numberOfSites)
                    {
                        if (usedSites.Count < 2)
                        {
                            do
                            {
                                // r = GalaxyGeneratorPlugin.random.Next(sites.Count);
                                // selP = sites[r];
                                selectedPlanet = this.RandomFindClose(sourceCenter, sites);
                            }
                            while (usedSites.Contains(selectedPlanet));
                        }
                        else
                        {
                            // r = GalaxyGeneratorPlugin.random.Next(sites.Count);
                            // selP = sites[r];
                            selectedPlanet = this.RandomFindClose(sourceCenter, sites);
                        }

                        usedSites.Add(selectedPlanet);
                    }
                    else
                    {
                        // r = GalaxyGeneratorPlugin.random.Next(usedSites.Count);
                        // selP = usedSites.ToList()[r];
                        selectedPlanet = this.RandomFindClose(sourceCenter, sites);
                    }

                    if (selectedPlanet != null)
                    {
                        if (selectedPlanet.Resource == null)
                        {
                            selectedPlanet.Resource = new ResourceDeposit(resName, 1, ResourceDeposit.ResourceType.Luxury)
                                                          {
                                                              Location = selectedPlanet
                                                          };
                        }
                        else if (selectedPlanet.Resource.Size < ResourceDeposit.MaxSize)
                        {
                            selectedPlanet.Resource.IncreaseSize();
                        }
                    }
                }
            }

            // double-checking produced luxes
            {
                SortedDictionary<string, int> spawnedLuxQuantities = new SortedDictionary<string, int>();

                foreach (string resName in Settings.Instance.LuxuryResourceNames)
                {
                    spawnedLuxQuantities.Add(resName, 0);
                }

                foreach (Planet p in Galaxy.Instance.Planets)
                {
                    if (p.HasResource)
                    {
                        if (p.Resource.Type == ResourceDeposit.ResourceType.Luxury)
                        {
                            spawnedLuxQuantities[p.PresentResourceName] += p.Resource.Size;
                        }
                    }
                }

                foreach (string resName in spawnedLuxQuantities.Keys)
                {
                    if (spawnedLuxQuantities[resName] == 0)
                    {
                        this.TraceDefect(resName + " could not be spawned");
                    }
                    else if (spawnedLuxQuantities[resName] < 4)
                    {
                        this.TraceDefect(resName + " spawned quantity less than 4, removing...");
                        List<Planet> planets = new List<Planet>(Galaxy.Instance.Planets.FindAll(p => p.PresentResourceName == resName));

                        foreach (Planet p in planets)
                        {
                            p.Resource = null;
                        }
                    }
                }
            }

            // Checking tiers
            {
                HashSet<string> deposits = new HashSet<string>();

                foreach (Planet p in Galaxy.Instance.Planets)
                {
                    if (p.HasResource)
                    {
                        if (p.Resource.Type == ResourceDeposit.ResourceType.Luxury)
                        {
                            deposits.Add(p.Resource.Name);
                        }
                    }
                }

                Trace.WriteLine("Luxuries actually deposited - begin");
                foreach (string s in deposits)
                {
                    Trace.WriteLine(s);
                }

                Trace.WriteLine("Luxuries actually deposited - end");
            }

            this.Result = true;

            Trace.WriteLine(this.Name + " - Execute - end");
        }

        /// <summary>
        /// Find a close planet randomly
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="candidates"> The candidates. </param>
        /// <returns> A close planet </returns>
        protected Planet RandomFindClose(StarSystem source, List<Planet> candidates)
        {
            List<Planet> shortList = new List<Planet>();
            int tries = 3;

            for (int i = 0; i < tries; i++)
            {
                shortList.Add(candidates[GalaxyGeneratorPlugin.Random.Next(candidates.Count)]);
            }

            double distance, distanceMin;
            Planet winner = null;

            distanceMin = Galaxy.Instance.Diameter() * 2;
            foreach (Planet p in shortList)
            {
                distance = Geometry2D.Distance(source.Position, p.System.Position);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    winner = p;
                }
            }

            return winner;
        }
    }
}

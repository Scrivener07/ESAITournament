// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StrategicResourceBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the StrategicResourceBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// The strategic resource builder.
    /// </summary>
    public class StrategicResourceBuilder : Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrategicResourceBuilder"/> class.
        /// </summary>
        public StrategicResourceBuilder()
        {
            this.RemainingQuantities = new SortedDictionary<string, int>();
            this.ShuffledPlanets = new List<Planet>();
            this.ShuffledResources = new List<string>();
            this.SortedDepositSizes = new List<int>();
        }

        /// <summary>
        /// Gets the strategic resources builder's name.
        /// </summary>
        public override string Name
        {
            get { return "StrategicResourceBuilder"; }
        }

        /// <summary>
        /// Gets or sets the remaining quantities.
        /// </summary>
        protected SortedDictionary<string, int> RemainingQuantities { get; set; }

        /// <summary>
        /// Gets or sets the shuffled planets.
        /// </summary>
        protected List<Planet> ShuffledPlanets { get; set; }

        /// <summary>
        /// Gets or sets the shuffled resources.
        /// </summary>
        protected List<string> ShuffledResources { get; set; }

        /// <summary>
        /// Gets or sets the sorted deposit sizes.
        /// </summary>
        protected List<int> SortedDepositSizes { get; set; }

        /// <summary>
        /// Executes the builder
        /// </summary>
        public override void Execute()
        {
            Trace.WriteLine(this.Name + " - Execute - begin");

            if (Galaxy.Instance.Planets.Count < Settings.Instance.StrategicResourceNames.Count)
            {
                this.TraceDefect("Less planets than strategic resource types", true);
                return;
            }

            List<Planet> sourcePlanets = new List<Planet>(Galaxy.Instance.Planets);
            while (sourcePlanets.Count > 0)
            {
                Planet p = sourcePlanets.ElementAt(GalaxyGeneratorPlugin.Random.Next(sourcePlanets.Count));
                this.ShuffledPlanets.Add(p);
                sourcePlanets.Remove(p);
            }

            foreach (string s in Settings.Instance.StrategicResourceNames)
            {
                this.RemainingQuantities.Add(s, Galaxy.Instance.Configuration.StrategicResourceNumberPerType);
            }

            // Sort deposit iterations according to Settings file
            {
                List<int> sortedIterationIndices = new List<int>(Settings.Instance.ResourceDepositSizeIterations.Keys);

                if (sortedIterationIndices.Count == 0)
                {
                    this.TraceDefect("Found empty iteration table for strategic resource deposits", true);
                    return;
                }

                sortedIterationIndices.Sort();

                foreach (int index in sortedIterationIndices)
                {
                    this.SortedDepositSizes.Add(Settings.Instance.ResourceDepositSizeIterations[index]);
                }
            }

            // Shuffle resources
            {
                List<string> sourceResources = new List<string>(Settings.Instance.StrategicResourceNames);
                while (sourceResources.Count > 0)
                {
                    string r = sourceResources.ElementAt(GalaxyGeneratorPlugin.Random.Next(sourceResources.Count));
                    this.ShuffledResources.Add(r);
                    sourceResources.Remove(r);
                }
            }

            int depositSize;
            //// bool firstAllocation = true;
            int passNumber = 0;
            bool oneDepositPlaced = true;

            while ((this.RemainingQuantities.Values.Count(i => (i > 0)) > 0) && oneDepositPlaced)
            {
                foreach (int rawDepositSize in this.SortedDepositSizes)
                {
                    depositSize = rawDepositSize;

                    if (depositSize < 0)
                    {
                        depositSize = 1;
                        this.TraceDefect("Found negative deposit size in iteration - defaulted to 1");
                    }

                    if (depositSize > ResourceDeposit.MaxSize)
                    {
                        this.TraceDefect("Found too large deposit size in iteration - left unchanged");
                    }

                    passNumber++;

                    if (passNumber == 1)
                    {
                        // firstAllocation = false;
                        if (!this.DoFirstAllocation(depositSize))
                        {
                            this.TraceDefect("Failed to allocate first batch of strategic resources", true);
                            return;
                        }
                    }
                    else if (passNumber == 2)
                    {
                        this.DoSecondAllocation(depositSize);
                    }
                    else
                    {
                        oneDepositPlaced = false;
                        foreach (string res in this.ShuffledResources)
                        {
                            int localMaxSize = this.RemainingQuantities[res];
                            int localDepositSize = depositSize;
                            if (depositSize > localMaxSize)
                            {
                                localDepositSize = localMaxSize;
                            }

                            Planet deposit = this.FindDeposit(res);
                            if ((null != deposit) && (localDepositSize > 0))
                            {
                                oneDepositPlaced = true;
                                this.ShuffledPlanets.Remove(deposit);
                                deposit.Resource = new ResourceDeposit(res, localDepositSize, ResourceDeposit.ResourceType.Strategic)
                                                       {
                                                           Location = deposit
                                                       };
                                this.RemainingQuantities[res] -= localDepositSize;
                            }
                        }
                    }
                }
            }

            foreach (string res in Settings.Instance.StrategicResourceNames)
            {
                // TODO Check if refactor still yields valid result
                if (Galaxy.Instance.Planets.Find(p => p.HasResource && p.Resource.Name == res) == null)
                {
                    this.TraceDefect("Unable to place a single deposit of " + res, true);
                    return;
                }
            }

            this.Result = true;

            Trace.WriteLine(this.Name + " - Execute - end");
        }

        /// <summary>
        /// Finds a deposit of resource
        /// </summary>
        /// <param name="res"> The resource we want to find </param>
        /// <returns> The planet where this deposit is </returns>
        protected Planet FindDeposit(string res)
        {
            List<Planet> candidates = new List<Planet>(this.ShuffledPlanets.FindAll(p => p.CanAcceptStrategicResource(res)));
            List<StarSystem> presentDeposits = new List<StarSystem>(Galaxy.Instance.Stars.FindAll(s => s.Planets.Exists(p => p.PresentResourceName == res)));
            Planet farthest = null;
            double distance, distanceMax, distanceMin;

            distanceMax = 0;
            foreach (Planet p in candidates)
            {
                distanceMin = Galaxy.Instance.Diameter() * 2;
                foreach (StarSystem s in presentDeposits)
                {
                    distance = Geometry2D.Distance(p.System.Position, s.Position);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                    }
                }

                if (distanceMin > distanceMax)
                {
                    farthest = p;
                }
            }

            return farthest;
        }

        /// <summary>
        /// Allocates a second time a resource
        /// </summary>
        /// <param name="depositSize"> The deposit size. </param>
        protected void DoSecondAllocation(int depositSize)
        {
            List<Planet> candidates = new List<Planet>();
            List<Region> nonEmptySpawnRegions = new List<Region>(Galaxy.Instance.Regions.FindAll(region => (region.IsSpawn() && (region.Count > 0))));
            List<Planet> actualDeposits = new List<Planet>();

            foreach (string res in this.ShuffledResources)
            {
                foreach (Region reg in nonEmptySpawnRegions)
                {
                    candidates.Clear();
                    candidates.AddRange(Galaxy.Instance.Planets.FindAll(planet => (reg.Contains(planet.System) && planet.CanAcceptStrategicResource(res))));

                    int localMaxSize = this.RemainingQuantities[res];
                    int localDepositSize = depositSize;
                    if (depositSize > localMaxSize)
                    {
                        localDepositSize = localMaxSize;
                    }

                    Planet deposit = null;
                    if (candidates.Count > 0)
                    {
                        deposit = candidates[GalaxyGeneratorPlugin.Random.Next(candidates.Count)];
                    }

                    if ((null != deposit) && (localDepositSize > 0))
                    {
                        this.ShuffledPlanets.Remove(deposit);
                        deposit.Resource = new ResourceDeposit(res, localDepositSize, ResourceDeposit.ResourceType.Strategic)
                                               {
                                                   Location = deposit
                                               };
                        this.RemainingQuantities[res] -= localDepositSize;
                        actualDeposits.Add(deposit);
                    }
                }
            }

            Dictionary<Region, HashSet<string>> check = new Dictionary<Region, HashSet<string>>();
            foreach (Region region in nonEmptySpawnRegions)
            {
                check.Add(region, new HashSet<string>());
            }

            foreach (Planet p in actualDeposits)
            {
                Trace.WriteLine("Region " + p.System.Region.Index + " contains a deposit of " + p.Resource.Name);
                check[p.System.Region].Add(p.Resource.Name);
            }

            foreach (Region r in check.Keys)
            {
                if (check[r].Count < Settings.Instance.StrategicResourceNames.Count)
                {
                    this.TraceDefect("Region " + r.Index + " misses some strategic resource");
                }
            }
        }

        /// <summary>
        /// Do first allocation of resources.
        /// </summary>
        /// <param name="depositSize">Deposit size. </param>
        /// <returns> True if it has allocated something </returns>
        protected bool DoFirstAllocation(int depositSize)
        {
            SortedDictionary<string, Planet> sites = new SortedDictionary<string, Planet>();

            foreach (Planet firstPlanet in this.ShuffledPlanets)
            {
                List<Planet> rotatedPlanets = new List<Planet>(this.ShuffledPlanets);
                rotatedPlanets.RemoveRange(0, this.ShuffledPlanets.IndexOf(firstPlanet));
                rotatedPlanets.AddRange(this.ShuffledPlanets.GetRange(0, this.ShuffledPlanets.IndexOf(firstPlanet)));

                sites.Clear();
                foreach (string res in this.ShuffledResources)
                {
                    Planet testSite = rotatedPlanets.FirstOrDefault(p => (p.CanAcceptStrategicResource(res) && !sites.Values.Contains(p)));

                    if (testSite == null)
                    {
                        testSite = rotatedPlanets.FirstOrDefault(p => !sites.Values.Contains(p));
                    }

                    if (testSite != null)
                    {
                        sites.Add(res, testSite);
                    }
                }

                if (sites.Keys.Count == this.ShuffledResources.Count)
                {
                    foreach (string res in this.ShuffledResources)
                    {
                        sites[res].Resource = new ResourceDeposit(res, depositSize, ResourceDeposit.ResourceType.Strategic)
                                                    {
                                                        Location = sites[res]
                                                    };

                        this.ShuffledPlanets.Remove(sites[res]);
                        this.RemainingQuantities[res] -= depositSize;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}

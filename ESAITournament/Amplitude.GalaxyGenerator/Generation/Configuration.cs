// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   The configuration.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Amplitude.GalaxyGenerator.Drawing;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// The configuration.
    /// </summary>
    public class Configuration : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="xmlFileName">Configuration file name. </param>
        public Configuration(string xmlFileName)
        {
            XmlTextReader xr = new XmlTextReader(xmlFileName);
            string n, t;
            HomeGenerator hg;

            this.HomeGenerators = new List<HomeGenerator>();

            this.TakenConstellationNames = new HashSet<string>();
            this.TakenStarNames = new HashSet<string>();

            this.Seed = 0;

            while (xr.Read())
            {
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name != "GenerationSettings"))
                {
                    n = xr.Name;

                    if ((n == "HomeGeneration") && (!xr.IsEmptyElement))
                    {
                        do
                        {
                            xr.Read();
                            if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Empire"))
                            {
                                hg = new HomeGenerator();
                                this.HomeGenerators.Add(hg);

                                do
                                {
                                    xr.Read();
                                    if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Trait"))
                                    {
                                        // t = xr.ReadElementContentAsString();
                                        t = xr.GetAttribute("Name");
                                        if (Settings.Instance.HomeGenerationTraitsNames.Contains(t))
                                        {
                                            hg.Add(Settings.Instance.HomeGenerationTraits[t]);
                                        }
                                    }
                                }
                                while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Empire")));
                            }
                        }
                        while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "HomeGeneration")));
                    }
                    else if ((n == "Seed") && (!xr.IsEmptyElement))
                    {
                        this.Seed = xr.ReadElementContentAsInt();
                    }
                    else if (!xr.IsEmptyElement)
                    {
                        t = xr.ReadElementContentAsString();
                        this.Add(n, t);
                    }
                }
            }

            //// foreach (string elt in this.Keys)
            ////    System.Diagnostics.Trace.WriteLine(elt + " : " + this[elt]);
            Trace.WriteLine("checking Configuration...");
            Trace.WriteLine("Seed : " + this.Seed);
            Trace.WriteLine("Shape : " + this.Shape.RegionsFileName);
            Trace.WriteLine("Empires : " + this.EmpiresNumber());
            Trace.WriteLine("Galaxy size : " + this["GalaxySize"]);
            Trace.WriteLine("Galaxy age : " + this["GalaxyAge"]);
            Trace.WriteLine("Galaxy density : " + this["GalaxyDensity"]);
            Trace.WriteLine("Constellations : " + this.Constellations);
            Trace.WriteLine("Star connectivity : " + this.Connectivity);
            Trace.WriteLine("Resources : " + this["ResourceRepartitionFactor"]);
            Trace.WriteLine("Expected population : " + this.Population);
            Trace.WriteLine("HomeWorldGenerators Count : " + this.HomeGenerators.Count);
            Trace.WriteLine("...end checking Configuration");
        }

        /// <summary>
        /// Gets or sets the home generators.
        /// </summary>
        public List<HomeGenerator> HomeGenerators { get; protected set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Gets the shape.
        /// </summary>
        public Shape Shape
        {
            get { return ShapeManager.Instance.Shapes[this["GalaxyShape"]]; }
        }

        /// <summary>
        /// Gets the population.
        /// </summary>
        public int Population
        {
            get
            {
                string galaxySize = this["GalaxySize"];
                int numberOfStars = Settings.Instance.GalaxySizes[galaxySize].NumStars;
                string galaxyDensity = this["GalaxyDensity"];
                double density = Settings.Instance.GalaxyDensities[galaxyDensity];
                return (int)(numberOfStars * density);

                // return (int) ((double)(GenerationData.instance.galaxySizes[this["GalaxySize"]].numStars) * GenerationData.instance.galaxyDensities[this["GalaxyDensity"]]);
            }
        }

        /// <summary>
        /// Gets the constellations.
        /// </summary>
        public int Constellations
        {
            get
            {
                switch (this["ConstellationNumber"])
                {
                    case "ConstellationNumberNone":
                        return 1;

                    case "ConstellationNumberFew":
                        return this.Shape.MinConstellations > 0 ? this.Shape.MinConstellations : this.EmpiresNumber();

                    case "ConstellationNumberMany":
                        return this.Shape.MaxConstellations > 0 ? this.Shape.MaxConstellations : 2 * this.EmpiresNumber();
                }

                return 1;
            }
        }

        /// <summary>
        /// Gets the connectivity.
        /// </summary>
        public double Connectivity
        {
            get { return Settings.Instance.StarConnectivities[this["StarConnectivity"]]; }
        }

        /// <summary>
        /// Gets the wormhole connectivity.
        /// </summary>
        public double WormholeConnectivity
        {
            get { return Settings.Instance.ConstellationConnectivities[this["ConstellationConnectivity"]]; }
        }

        /// <summary>
        /// Gets the max width.
        /// </summary>
        public double MaxWidth
        {
            get { return Settings.Instance.GalaxySizes[this["GalaxySize"]].Width; }
        }

        /// <summary>
        /// Gets the star overlap distance.
        /// </summary>
        public double StarOverlapDistance
        {
            get { return Settings.Instance.GenerationConstraints.MinStarDistance; }
        }

        /// <summary>
        /// Gets the star population balancing.
        /// </summary>
        public double StarPopulationBalancing
        {
            get
            {
                string balanceName = this["StarPopulationsBalancing"];
                double balance = Settings.Instance.StarPopulationBalancing[balanceName];
                return balance;
            }
        }

        /// <summary>
        /// Gets the density image.
        /// </summary>
        public ImageInfos DensityImage
        {
            get { return this.Shape.DensityMap; }
        }

        /// <summary>
        /// Gets the regions image.
        /// </summary>
        public ImageInfos RegionsImage
        {
            get { return this.Shape.RegionsMap; }
        }

        /// <summary>
        /// Gets the strategic resource number per type.
        /// </summary>
        public int StrategicResourceNumberPerType
        {
            get
            {
                int strategicResourceNumberPerType = Settings.Instance.GalaxySizes[this["GalaxySize"]].StrategicResourceNumberPerType;
                double resourceRepartitionFactors = Settings.Instance.ResourceRepartitionFactors[this["ResourceRepartitionFactor"]];

                // TODO dont get it, have to ask why
                return (int)(100.0 * strategicResourceNumberPerType * resourceRepartitionFactors) / 100;
            }
        }

        /// <summary>
        /// Gets the luxury resource number of types.
        /// </summary>
        public int LuxuryResourceNumberOfTypes
        {
            get
            {
                int luxuryResourceType = Settings.Instance.GalaxySizes[this["GalaxySize"]].LuxuryResourceTypes;
                double repartitionFactor = Settings.Instance.ResourceRepartitionFactors[this["ResourceRepartitionFactor"]];

                // TODO dont get it, have to ask why
                return (int)(100.0 * luxuryResourceType * repartitionFactor) / 100;
            }
        }

        /// <summary>
        /// Gets or sets the taken constellation names.
        /// </summary>
        private HashSet<string> TakenConstellationNames { get; set; }

        /// <summary>
        /// Gets or sets the taken star names.
        /// </summary>
        private HashSet<string> TakenStarNames { get; set; }

        /// <summary>
        /// The reset names.
        /// </summary>
        public void ResetNames()
        {
            this.TakenConstellationNames.Clear();
            this.TakenStarNames.Clear();
        }

        /// <summary>
        /// Writes to xml file
        /// </summary>
        /// <param name="xw"> Xml writer </param>
        public void WriteOuterXml(XmlWriter xw)
        {
            xw.WriteStartElement("CrossCheckingSettings");
            foreach (string k in this.Keys)
            {
                xw.WriteElementString(k, this[k]);
            }

            xw.WriteElementString("Seed", this.Seed.ToString());
            xw.WriteEndElement();
        }

        /// <summary>
        /// Gets numbers of empires
        /// </summary>
        /// <returns> the actual numbers of empires </returns>
        public int EmpiresNumber()
        {
            if (this.Keys.Contains("EmpiresNumber"))
            {
                if (this["EmpiresNumber"].Contains("EmpiresNumber"))
                {
                    return int.Parse(this["EmpiresNumber"].Remove(this["EmpiresNumber"].IndexOf("EmpiresNumber"), this["EmpiresNumber"].Length));
                }

                return int.Parse(this["EmpiresNumber"]);
            }

            if (Settings.Instance.GalaxySizes[this["GalaxySize"]].NominalPlayers > 0)
            {
                return Settings.Instance.GalaxySizes[this["GalaxySize"]].NominalPlayers;
            }

            if (this.Shape.MinEmpires > 0)
            {
                return this.Shape.MinEmpires;
            }

            if (this.Shape.MaxEmpires > 0)
            {
                return this.Shape.MaxEmpires;
            }

            return 4;
        }

        /// <summary>
        /// Gets a random star name.
        /// </summary>
        /// <returns> A star name </returns>
        public string GetRandomStarName()
        {
            string s;

            if (Settings.Instance.StarNames.Count == 0)
            {
                return string.Empty;
            }

            if (this.TakenStarNames == Settings.Instance.StarNames)
            {
                this.TakenStarNames.Clear();
            }

            do
            {
                s = this.SelectRandom(Settings.Instance.StarNames);
            }
            while (this.TakenStarNames.Contains(s));

            this.TakenStarNames.Add(s);

            return s;
        }

        /// <summary>
        /// Gets a random constellation name.
        /// </summary>
        /// <returns> A constellation name </returns>
        public string GetRandomConstellationName()
        {
            string s;

            if (Settings.Instance.ConstellationNames.Count == 0)
            {
                return string.Empty;
            }

            if (this.TakenConstellationNames == Settings.Instance.ConstellationNames)
            {
                this.TakenConstellationNames.Clear();
            }

            do
            {
                s = this.SelectRandom(Settings.Instance.ConstellationNames);
            }
            while (this.TakenConstellationNames.Contains(s));

            this.TakenConstellationNames.Add(s);

            return s;
        }

        /// <summary>
        /// Gets a random star type
        /// </summary>
        /// <returns> A random star type </returns>
        public string GetRandomStarType()
        {
            Dictionary<string, int> hw;
            int n, d, r;

            hw = new Dictionary<string, int>(Settings.Instance.GalaxyAges[this["GalaxyAge"]].StarTypeWeightTable);

            n = 0;
            foreach (string s in hw.Keys)
            {
                n += hw[s];
            }

            if (n == 0)
            {
                return string.Empty;
            }

            d = GalaxyGeneratorPlugin.Random.Next(n) + 1;
            r = 0;
            foreach (string s in hw.Keys)
            {
                r += hw[s];
                if (d <= r)
                {
                    return s;
                }
            }

            return "InvalidStarType";
        }

        /// <summary>
        /// Gets a random planet number.
        /// </summary>
        /// <returns> A random planet number </returns>
        public int GetRandomPlanetNumber()
        {
            Dictionary<int, int> h;
            int n, r, d;

            h = Settings.Instance.PlanetsPerSystems[this["PlanetsPerSystem"]];

            n = 0;
            foreach (int i in h.Keys)
            {
                n += h[i];
            }

            if (n == 0)
            {
                return 0;
            }

            d = GalaxyGeneratorPlugin.Random.Next(n) + 1;
            r = 0;
            foreach (int i in h.Keys)
            {
                r += h[i];
                if (d <= r)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets random planet type.
        /// </summary>
        /// <param name="starType"> The star type. </param>
        /// <returns> A random planet type </returns>
        public string GetRandomPlanetType(string starType)
        {
            Dictionary<string, int> h;
            int n, d, r;

            h = Settings.Instance.PlanetTypeProbabilitiesPerStar[starType];

            n = 0;
            foreach (string s in h.Keys)
            {
                n += h[s];
            }

            if (n == 0)
            {
                return string.Empty;
            }

            d = GalaxyGeneratorPlugin.Random.Next(n) + 1;
            r = 0;
            foreach (string s in h.Keys)
            {
                r += h[s];
                if (d <= r)
                {
                    return s;
                }
            }

            return "InvalidPlanetType";
        }

        /// <summary>
        /// Gets a random planet size.
        /// </summary>
        /// <param name="planetType"> The planet type. </param>
        /// <returns> A random planet size </returns>
        public string GetRandomPlanetSize(string planetType)
        {
            Dictionary<string, int> h = new Dictionary<string, int>();
            int n, d, r;

            foreach (string s in Settings.Instance.PlanetSizeProbabilitiesPerType[planetType].Keys)
            {
                int sizeProbability = Settings.Instance.PlanetSizeProbabilitiesPerType[planetType][s];
                double sizeFactor = Settings.Instance.PlanetsSizeFactors[this["PlanetsSizeFactor"]][s];
                var result = (int)(100 * sizeProbability * sizeFactor);

                h.Add(s, result);
            }

            n = 0;
            foreach (string s in h.Keys)
            {
                n += h[s];
            }

            if (n == 0)
            {
                return string.Empty;
            }

            d = GalaxyGeneratorPlugin.Random.Next(n) + 1;
            r = 0;
            foreach (string s in h.Keys)
            {
                r += h[s];
                if (d <= r)
                {
                    return s;
                }
            }

            return "InvalidPlanetSize";
        }

        /// <summary>
        /// Gets a random moon number.
        /// </summary>
        /// <param name="planetType"> The planet type. </param>
        /// <returns> A random moon number </returns>
        public int GetRandomMoonNumber(string planetType)
        {
            Dictionary<int, int> h;
            int n, d, r;

            if (Settings.Instance.MoonNumberProbabilitiesPerPlanetType.Keys.Contains(planetType))
            {
                h = Settings.Instance.MoonNumberProbabilitiesPerPlanetType[planetType];
            }
            else
            {
                h = Settings.Instance.MoonNumberChances;
            }

            n = 0;
            foreach (int p in h.Keys)
            {
                n += h[p];
            }

            if (n == 0)
            {
                return 0;
            }

            d = GalaxyGeneratorPlugin.Random.Next(n) + 1;
            r = 0;
            foreach (int p in h.Keys)
            {
                r += h[p];
                if (d <= r)
                {
                    return p;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets a random temple type.
        /// </summary>
        /// <param name="starType"> The star type. </param>
        /// <returns> A random temple type </returns>
        public string GetRandomTempleType(string starType)
        {
            Dictionary<string, int> h;
            int p, n, d, r, rt;

            p = Settings.Instance.TempleChancesPerStarType[starType];
            rt = GalaxyGeneratorPlugin.Random.Next(100) + 1;

            if (rt <= p)
            {
                h = Settings.Instance.TempleTypeProbabilities;

                n = 0;
                foreach (string s in h.Keys)
                {
                    n += h[s];
                }

                if (n == 0)
                {
                    return "InvalidTempleType";
                }

                d = GalaxyGeneratorPlugin.Random.Next(n) + 1;
                r = 0;
                foreach (string s in h.Keys)
                {
                    r += h[s];
                    if (d <= r)
                    {
                        return s;
                    }
                }

                return "InvalidTempleType";
            }

            return "NoTemple";
        }

        /// <summary>
        /// Gets a random anomaly.
        /// </summary>
        /// <param name="planetType"> The planet type. </param>
        /// <returns> A random anomaly </returns>
        public string GetRandomAnomaly(string planetType)
        {
            int r, rt, t, tt;
            Dictionary<string, int> h;

            r = GalaxyGeneratorPlugin.Random.Next(100) + 1;

            if (r <= Settings.Instance.AnomalyBaseChance)
            {
                h = Settings.Instance.PlanetAnomaliesPerPlanetType[planetType];
                t = 0;
                foreach (int p in h.Values)
                {
                    t += Settings.Instance.PlanetAnomaliesProbabilityScale[p];
                }

                if (t <= 0)
                {
                    return string.Empty;
                }

                rt = GalaxyGeneratorPlugin.Random.Next(t) + 1;
                tt = 0;
                foreach (string a in h.Keys)
                {
                    tt += Settings.Instance.PlanetAnomaliesProbabilityScale[h[a]];
                    if (rt <= tt)
                    {
                        return a;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a random strategic resource.
        /// </summary>
        /// <param name="planetType"> The planet type. </param>
        /// <returns> A random strategic resource </returns>
        public string GetRandomStrategicResource(string planetType)
        {
            int rt, t, tt;
            Dictionary<string, int> h;

            h = Settings.Instance.PlanetStrategicResourcesPerPlanetType[planetType];
            t = 0;
            foreach (int p in h.Values)
            {
                t += Settings.Instance.PlanetStrategicResourceProbabilitiesScale[p];
            }

            if (t <= 0)
            {
                return string.Empty;
            }

            rt = GalaxyGeneratorPlugin.Random.Next(t) + 1;
            tt = 0;
            foreach (string a in h.Keys)
            {
                tt += Settings.Instance.PlanetStrategicResourceProbabilitiesScale[h[a]];
                if (rt <= tt)
                {
                    return a;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets a random luxury resource.
        /// </summary>
        /// <param name="priority"> The priority. </param>
        /// <returns> A random Luxury Resource </returns>
        public string GetRandomLuxuryResource(int priority)
        {
            return this.SelectRandom(Settings.Instance.LuxuryResourceTiers[priority]);
        }

        /// <summary>
        /// Selects randomly a string in the given set
        /// </summary>
        /// <param name="set"> The set. </param>
        /// <returns> A random string in the current set </returns>
        protected string SelectRandom(HashSet<string> set)
        {
            return set.ElementAtOrDefault(GalaxyGeneratorPlugin.Random.Next(set.Count));
        }
    }
}
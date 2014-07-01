// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Galaxy Settings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// Galaxy Settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        /// <param name="xmlFileName">
        /// The xml file name.
        /// </param>
        protected Settings(string xmlFileName)
        {
            Instance = this;

            this.GalaxySizes = new Dictionary<string, GalaxySize>();
            this.GalaxyAges = new Dictionary<string, GalaxyAge>();
            this.ConstellationNumbers = new HashSet<string>();
            this.ConstellationConnectivities = new Dictionary<string, double>();
            this.StarConnectivities = new Dictionary<string, double>();
            this.ConstellationDistances = new Dictionary<string, double>();
            this.StarPopulationBalancing = new Dictionary<string, double>();
            this.GalaxyDensities = new Dictionary<string, double>();
            this.PlanetsPerSystems = new Dictionary<string, Dictionary<int, int>>();
            this.PlanetsSizeFactors = new Dictionary<string, Dictionary<string, double>>();
            this.AnomalyTempleFactors = new Dictionary<string, double>();
            this.ResourceRepartitionFactors = new Dictionary<string, double>();
            this.ResourceDepositSizeIterations = new Dictionary<int, int>();
            this.PlanetTypeProbabilitiesPerStar = new Dictionary<string, Dictionary<string, int>>();
            this.PlanetSizeProbabilitiesPerType = new Dictionary<string, Dictionary<string, int>>();
            this.MoonNumberChances = new Dictionary<int, int>();
            this.MoonNumberProbabilitiesPerPlanetType = new Dictionary<string, Dictionary<int, int>>();
            this.TempleChancesPerStarType = new Dictionary<string, int>();
            this.PlanetAnomaliesProbabilityScale = new Dictionary<int, int>();
            this.PlanetAnomaliesPerPlanetType = new Dictionary<string, Dictionary<string, int>>();
            this.PlanetStrategicResourceProbabilitiesScale = new Dictionary<int, int>();
            this.PlanetStrategicResourcesPerPlanetType = new Dictionary<string, Dictionary<string, int>>();
            this.PlanetLuxuryProbabilitiesScale = new Dictionary<int, int>();
            this.PlanetLuxuriesPerPlanetType = new Dictionary<string, Dictionary<string, int>>();
            this.LuxuryResourceTiers = new SortedDictionary<int, HashSet<string>>();
            this.TempleTypeProbabilities = new Dictionary<string, int>();
            this.StarNames = new HashSet<string>();
            this.ConstellationNames = new HashSet<string>();
            this.GalaxySizeNames = new HashSet<string>();
            this.GalaxyAgeNames = new HashSet<string>();
            this.ConstellationConnectivityNames = new HashSet<string>();
            this.StarConnectivityNames = new HashSet<string>();
            this.ConstellationDistanceNames = new HashSet<string>();
            this.StarPopulationBalancingNames = new HashSet<string>();
            this.GalaxyDensitiesNames = new HashSet<string>();
            this.PlanetsPerSystemsNames = new HashSet<string>();
            this.PlanetsSizeFactorNames = new HashSet<string>();
            this.PlanetSizeNames = new HashSet<string>();
            this.AnomalyTempleFactorNames = new HashSet<string>();
            this.ResourceRepartitionFactorNames = new HashSet<string>();
            this.StarTypeNames = new HashSet<string>();
            this.PlanetTypeNames = new HashSet<string>();
            this.AnomalyNames = new HashSet<string>();
            this.StrategicResourceNames = new HashSet<string>();
            this.LuxuryResourceNames = new HashSet<string>();
            this.GenerationConstraints = new Constraints();
            this.HomeGenerationTraits = new Dictionary<string, HomeTrait>();
            this.HomeGenerationTraitsNames = new HashSet<string>();

            XmlTextReader xr = new XmlTextReader(xmlFileName);

            while (xr.Read())
            {
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "GalaxySizes":
                            this.ReadGalaxySizes(xr);
                            break;
                        case "GalaxyAges":
                            this.ReadGalaxyAges(xr);
                            break;
                        case "ConstellationNumbers":
                            this.ReadConstellationNumbers(xr);
                            break;
                        case "ConstellationConnectivities":
                            this.ReadConstellationConnectivities(xr);
                            break;
                        case "StarConnectivities":
                            this.ReadStarConnectivities(xr);
                            break;
                        case "ConstellationDistances":
                            this.ReadConstellationDistances(xr);
                            break;
                        case "StarPopulationsBalancings":
                            this.ReadStarPopulationsBalancing(xr);
                            break;
                        case "GalaxyDensities":
                            this.ReadGalaxyDensities(xr);
                            break;
                        case "PlanetsPerSystems":
                            this.ReadPlanetsPerSystems(xr);
                            break;
                        case "PlanetsSizeFactors":
                            this.ReadPlanetsSizeFactors(xr);
                            break;
                        case "AnomalyTempleFactors":
                            this.ReadAnomalyTempleFactors(xr);
                            break;
                        case "ResourceRepartitionFactors":
                            this.ReadResourceRepartitionFactors(xr);
                            break;
                        case "PlanetTypeProbabilitiesPerStar":
                            this.ReadPlanetTypeProbabilitiesPerStar(xr);
                            break;
                        case "PlanetSizeProbabilitiesPerType":
                            this.ReadPlanetSizeProbabilitiesPerType(xr);
                            break;
                        case "MoonNumberChances":
                            this.ReadMoonNumberChances(xr);
                            break;
                        case "MoonNumberProbabilitiesPerPlanetType":
                            this.ReadMoonNumberProbabilitiesPerPlanetType(xr);
                            break;
                        case "TempleChancesPerStarType":
                            this.ReadTempleChancesPerStarType(xr);
                            break;
                        case "GenerationConstraints":
                            this.ReadGenerationConstraints(xr);
                            break;
                        case "StarNames":
                            this.ReadStarNames(xr);
                            break;
                        case "ConstellationNames":
                            this.ReadConstellationNames(xr);
                            break;
                        case "AnomalyBaseChance":
                            this.AnomalyBaseChance = xr.ReadElementContentAsInt();
                            break;
                        case "PlanetAnomaliesProbabilityScale":
                            this.ReadPlanetAnomaliesProbabilityScale(xr);
                            break;
                        case "PlanetAnomaliesPerPlanetType":
                            this.ReadPlanetAnomaliesPerPlanetType(xr);
                            break;
                        case "PlanetStrategicResourceProbabilitiesScale":
                            this.ReadPlanetStrategicResourceProbabilitiesScale(xr);
                            break;
                        case "PlanetStrategicResourcesPerPlanetType":
                            this.ReadPlanetStrategicResourcesPerPlanetType(xr);
                            break;
                        case "PlanetLuxuryProbabilitiesScale":
                            this.ReadPlanetLuxuryProbabilitiesScale(xr);
                            break;
                        case "PlanetLuxuriesPerPlanetType":
                            this.ReadPlanetLuxuriesPerPlanetType(xr);
                            break;
                        case "TempleTypes":
                            this.ReadTempleTypeProbabilities(xr);
                            break;
                        case "LuxuryResourceSpawnPriorities":
                            this.ReadLuxuryResourceSpawnPriorities(xr);
                            break;
                        case "HomeGeneration":
                            this.ReadHomeGeneration(xr);
                            break;
                        case "ResourceDepositSizeIterations":
                            this.ReadResourceDepositSizeIterations(xr);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Settings"/> class. 
        /// </summary>
        ~Settings()
        {
            Instance = null;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static Settings Instance { get; private set; }

        /// <summary>
        /// Gets or sets the galaxy sizes.
        /// </summary>
        public Dictionary<string, GalaxySize> GalaxySizes { get; protected set; }

        /// <summary>
        /// Gets or sets the galaxy ages.
        /// </summary>
        public Dictionary<string, GalaxyAge> GalaxyAges { get; protected set; }

        /// <summary>
        /// Gets or sets the constellation numbers.
        /// </summary>
        public HashSet<string> ConstellationNumbers { get; protected set; }

        /// <summary>
        /// Gets or sets the constellation connectivities.
        /// </summary>
        public Dictionary<string, double> ConstellationConnectivities { get; protected set; }

        /// <summary>
        /// Gets or sets the star connectivities.
        /// </summary>
        public Dictionary<string, double> StarConnectivities { get; protected set; }

        /// <summary>
        /// Gets or sets the constellation distances.
        /// </summary>
        public Dictionary<string, double> ConstellationDistances { get; protected set; }

        /// <summary>
        /// Gets or sets the star population balance.
        /// </summary>
        public Dictionary<string, double> StarPopulationBalancing { get; protected set; }

        /// <summary>
        /// Gets or sets the galaxy densities.
        /// </summary>
        public Dictionary<string, double> GalaxyDensities { get; protected set; }

        /// <summary>
        /// Gets or sets the planets per systems.
        /// </summary>
        public Dictionary<string, Dictionary<int, int>> PlanetsPerSystems { get; protected set; }

        /// <summary>
        /// Gets or sets the planets size factors.
        /// </summary>
        public Dictionary<string, Dictionary<string, double>> PlanetsSizeFactors { get; protected set; }

        /// <summary>
        /// Gets or sets the anomaly temple factors.
        /// </summary>
        public Dictionary<string, double> AnomalyTempleFactors { get; protected set; }

        /// <summary>
        /// Gets or sets the resource repartition factors.
        /// </summary>
        public Dictionary<string, double> ResourceRepartitionFactors { get; protected set; }

        /// <summary>
        /// Gets or sets the resource deposit size iterations.
        /// </summary>
        public Dictionary<int, int> ResourceDepositSizeIterations { get; protected set; }

        /// <summary>
        /// Gets or sets the planet type probabilities per star.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> PlanetTypeProbabilitiesPerStar { get; protected set; }

        /// <summary>
        /// Gets or sets the planet size probabilities per type.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> PlanetSizeProbabilitiesPerType { get; protected set; }

        /// <summary>
        /// Gets or sets the moon number chances.
        /// </summary>
        public Dictionary<int, int> MoonNumberChances { get; protected set; }

        /// <summary>
        /// Gets or sets the moon number probabilities per planet type.
        /// </summary>
        public Dictionary<string, Dictionary<int, int>> MoonNumberProbabilitiesPerPlanetType { get; protected set; }

        /// <summary>
        /// Gets or sets the temple chances per star type.
        /// </summary>
        public Dictionary<string, int> TempleChancesPerStarType { get; protected set; }

        /// <summary>
        /// Gets or sets the planet anomalies probability scale.
        /// </summary>
        public Dictionary<int, int> PlanetAnomaliesProbabilityScale { get; protected set; }

        /// <summary>
        /// Gets or sets the planet anomalies per planet type.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> PlanetAnomaliesPerPlanetType { get; protected set; }

        /// <summary>
        /// Gets or sets the planet strategic resource probabilities scale.
        /// </summary>
        public Dictionary<int, int> PlanetStrategicResourceProbabilitiesScale { get; protected set; }

        /// <summary>
        /// Gets or sets the planet strategic resources per planet type.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> PlanetStrategicResourcesPerPlanetType { get; protected set; }

        /// <summary>
        /// Gets or sets the planet luxury probabilities scale.
        /// </summary>
        public Dictionary<int, int> PlanetLuxuryProbabilitiesScale { get; protected set; }

        /// <summary>
        /// Gets or sets the planet luxuries per planet type.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> PlanetLuxuriesPerPlanetType { get; protected set; }

        /// <summary>
        /// Gets or sets the luxury resource tiers.
        /// </summary>
        public SortedDictionary<int, HashSet<string>> LuxuryResourceTiers { get; protected set; }

        /// <summary>
        /// Gets or sets the temple type probabilities.
        /// </summary>
        public Dictionary<string, int> TempleTypeProbabilities { get; protected set; }

        /// <summary>
        /// Gets or sets the generation constraints.
        /// </summary>
        public Constraints GenerationConstraints { get; protected set; }

        /// <summary>
        /// Gets or sets the star names.
        /// </summary>
        public HashSet<string> StarNames { get; protected set; }

        /// <summary>
        /// Gets or sets the constellation names.
        /// </summary>
        public HashSet<string> ConstellationNames { get; protected set; }

        /// <summary>
        /// Gets or sets the galaxy size names.
        /// </summary>
        public HashSet<string> GalaxySizeNames { get; protected set; }

        /// <summary>
        /// Gets or sets the galaxy age names.
        /// </summary>
        public HashSet<string> GalaxyAgeNames { get; protected set; }

        /// <summary>
        /// Gets or sets the constellation connectivity names.
        /// </summary>
        public HashSet<string> ConstellationConnectivityNames { get; protected set; }

        /// <summary>
        /// Gets or sets the star connectivity names.
        /// </summary>
        public HashSet<string> StarConnectivityNames { get; protected set; }

        /// <summary>
        /// Gets or sets the constellation distance names.
        /// </summary>
        public HashSet<string> ConstellationDistanceNames { get; protected set; }

        /// <summary>
        /// Gets or sets the galaxy densities names.
        /// </summary>
        public HashSet<string> GalaxyDensitiesNames { get; protected set; }

        /// <summary>
        /// Gets or sets the star population balancing names.
        /// </summary>
        public HashSet<string> StarPopulationBalancingNames { get; protected set; }

        /// <summary>
        /// Gets or sets the planets per systems names.
        /// </summary>
        public HashSet<string> PlanetsPerSystemsNames { get; protected set; }

        /// <summary>
        /// Gets or sets the planets size factor names.
        /// </summary>
        public HashSet<string> PlanetsSizeFactorNames { get; protected set; }

        /// <summary>
        /// Gets or sets the planet size names.
        /// </summary>
        public HashSet<string> PlanetSizeNames { get; protected set; }

        /// <summary>
        /// Gets or sets the anomaly temple factor names.
        /// </summary>
        public HashSet<string> AnomalyTempleFactorNames { get; protected set; }

        /// <summary>
        /// Gets or sets the resource repartition factor names.
        /// </summary>
        public HashSet<string> ResourceRepartitionFactorNames { get; protected set; }

        /// <summary>
        /// Gets or sets the star type names.
        /// </summary>
        public HashSet<string> StarTypeNames { get; protected set; }

        /// <summary>
        /// Gets or sets the planet type names.
        /// </summary>
        public HashSet<string> PlanetTypeNames { get; protected set; }

        /// <summary>
        /// Gets or sets the anomaly names.
        /// </summary>
        public HashSet<string> AnomalyNames { get; protected set; }

        /// <summary>
        /// Gets or sets the strategic resource names.
        /// </summary>
        public HashSet<string> StrategicResourceNames { get; protected set; }

        /// <summary>
        /// Gets or sets the luxury resource names.
        /// </summary>
        public HashSet<string> LuxuryResourceNames { get; protected set; }

        /// <summary>
        /// Gets or sets the anomaly base chance.
        /// </summary>
        public int AnomalyBaseChance { get; protected set; }

        /// <summary>
        /// Gets or sets the home generation traits.
        /// </summary>
        public Dictionary<string, HomeTrait> HomeGenerationTraits { get; protected set; }

        /// <summary>
        /// Gets or sets the home generation traits names.
        /// </summary>
        public HashSet<string> HomeGenerationTraitsNames { get; protected set; }

        /// <summary>
        /// Loads a GalaxySettings file.
        /// </summary>
        /// <param name="xmlFileName">
        /// The xml file name. 
        /// </param>
        public static void Load(string xmlFileName)
        {
            if (Instance == null)
            {
                new Settings(xmlFileName);
            }
        }

        /// <summary>
        /// Reads galaxy sizes.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadGalaxySizes(XmlTextReader xr)
        {
            do
            {
                xr.Read();

                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "GalaxySize"))
                {
                    GalaxySize gs = new GalaxySize
                                        {
                                            Name = xr.GetAttribute("Name"),
                                            NumStars = int.Parse(xr.GetAttribute("NumStars")),
                                            Width = double.Parse(xr.GetAttribute("Width")),
                                            NominalPlayers = int.Parse(xr.GetAttribute("NominalPlayers")),
                                            StrategicResourceNumberPerType = int.Parse(xr.GetAttribute("StrategicResourceNumberPerType")),
                                            LuxuryResourceTypes = int.Parse(xr.GetAttribute("LuxuryResourceTypes"))
                                        };
                    this.GalaxySizes.Add(gs.Name, gs);
                    this.GalaxySizeNames.Add(gs.Name);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxySizes")));

            foreach (GalaxySize g in this.GalaxySizes.Values)
            {
                Trace.WriteLine(g.Name + " has width " + g.Width + " and base population " + g.NumStars);
            }
        }

        /// <summary>
        /// Reads galaxy ages.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadGalaxyAges(XmlTextReader xr)
        {
            GalaxyAge ga;
            string s;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "GalaxyAge"))
                {
                    ga = new GalaxyAge
                             {
                                 Name = xr.GetAttribute("Name"), StarTypeWeightTable = new Dictionary<string, int>()
                             };
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "StarTypeProbability"))
                        {
                            s = xr.GetAttribute("Name");
                            ga.StarTypeWeightTable.Add(s, int.Parse(xr.GetAttribute("Probability")));
                            this.StarTypeNames.Add(s);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxyAge")));

                    this.GalaxyAges.Add(ga.Name, ga);
                    this.GalaxyAgeNames.Add(ga.Name);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxyAges")));
        }

        /// <summary>
        /// Reads constellation numbers.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadConstellationNumbers(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationNumber"))
                {
                    this.ConstellationNumbers.Add(xr.GetAttribute("Name"));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationNumbers")));
        }

        /// <summary>
        /// Reads constellation connectivities.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadConstellationConnectivities(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ConstellationConnectivity"))
                {
                    string s = xr.GetAttribute("Name");
                    this.ConstellationConnectivities.Add(s, double.Parse(xr.GetAttribute("Wormholes"), CultureInfo.InvariantCulture.NumberFormat));
                    this.ConstellationConnectivityNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationConnectivities")));
        }

        /// <summary>
        /// Reads star connectivities.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadStarConnectivities(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "StarConnectivity"))
                {
                    string s = xr.GetAttribute("Name");
                    this.StarConnectivities.Add(s, double.Parse(xr.GetAttribute("Warps"), CultureInfo.InvariantCulture.NumberFormat));
                    this.StarConnectivityNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "StarConnectivities")));
        }

        /// <summary>
        /// Reads constellation distances.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadConstellationDistances(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ConstellationDistance"))
                {
                    string s = xr.GetAttribute("Name");
                    this.ConstellationDistances.Add(s, double.Parse(xr.GetAttribute("Distance"), CultureInfo.InvariantCulture.NumberFormat));
                    this.ConstellationDistanceNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationDistances")));
        }

        /// <summary>
        /// Reads star balancing property
        /// </summary>
        /// <param name="xr">Xml reader</param>
        private void ReadStarPopulationsBalancing(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.Element && xr.Name == "StarPopulationsBalancing")
                {
                    string name = xr.GetAttribute("Name");
                    string numberFactor = xr.GetAttribute("NumberFactor");
                    this.StarPopulationBalancing.Add(name, double.Parse(numberFactor, CultureInfo.InvariantCulture.NumberFormat));
                    this.StarPopulationBalancingNames.Add(name);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "StarPopulationsBalancings")));
        }

        /// <summary>
        /// Reads galaxy densities.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadGalaxyDensities(XmlTextReader xr)
        {
            string s, t;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "GalaxyDensity"))
                {
                    s = xr.GetAttribute("Name");
                    t = xr.GetAttribute("NumberFactor");
                    this.GalaxyDensities.Add(s, double.Parse(t, CultureInfo.InvariantCulture.NumberFormat));
                    this.GalaxyDensitiesNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GalaxyDensities")));
        }

        /// <summary>
        /// Reads planets per systems.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetsPerSystems(XmlTextReader xr)
        {
            string s;
            Dictionary<int, int> h;
            int a, b;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetsPerSystem"))
                {
                    s = xr.GetAttribute("Name");
                    this.PlanetsPerSystemsNames.Add(s);
                    h = new Dictionary<int, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetNumberProbability"))
                        {
                            a = int.Parse(xr.GetAttribute("Number"));
                            b = int.Parse(xr.GetAttribute("Probability"));
                            h.Add(a, b);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsPerSystem")));

                    this.PlanetsPerSystems.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsPerSystems")));
        }

        /// <summary>
        /// Reads planets size factors.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetsSizeFactors(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetsSizeFactor"))
                {
                    string s = xr.GetAttribute("Name");
                    this.PlanetsSizeFactorNames.Add(s);
                    Dictionary<string, double> h = new Dictionary<string, double>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetSizeFactor"))
                        {
                            string t = xr.GetAttribute("Name");
                            this.PlanetSizeNames.Add(t);
                            h.Add(t, double.Parse(xr.GetAttribute("ProbabilityFactor"), CultureInfo.InvariantCulture.NumberFormat));
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsSizeFactor")));

                    this.PlanetsSizeFactors.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetsSizeFactors")));
        }

        /// <summary>
        /// Reads anomaly temple factors.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadAnomalyTempleFactors(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "AnomalyTempleFactor"))
                {
                    string s = xr.GetAttribute("Name");
                    this.AnomalyTempleFactors.Add(s, double.Parse(xr.GetAttribute("Factor"), CultureInfo.InvariantCulture.NumberFormat));
                    this.AnomalyTempleFactorNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "AnomalyTempleFactors")));
        }

        /// <summary>
        /// Reads resource repartition factors.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadResourceRepartitionFactors(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ResourceRepartitionFactor"))
                {
                    string s = xr.GetAttribute("Name");
                    this.ResourceRepartitionFactors.Add(s, double.Parse(xr.GetAttribute("Number"), CultureInfo.InvariantCulture.NumberFormat));
                    this.ResourceRepartitionFactorNames.Add(s);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ResourceRepartitionFactors")));

            /*
                                    foreach (string n in this.resourceRepartitionFactors.Keys)
                                        System.Diagnostics.Trace.WriteLine(n + " " + this.resourceRepartitionFactors[n].ToString());
                        */
        }

        /// <summary>
        /// Reads resource deposit size iterations.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadResourceDepositSizeIterations(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ResourceDepositSizeIteration"))
                {
                    this.ResourceDepositSizeIterations.Add(int.Parse(xr.GetAttribute("Iteration")), int.Parse(xr.GetAttribute("DepositSize")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ResourceDepositSizeIterations")));
        }

        /// <summary>
        /// Reads planet type probabilities per star.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetTypeProbabilitiesPerStar(XmlTextReader xr)
        {
            string s, t;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeProbabilityPerStar"))
                {
                    s = xr.GetAttribute("Name");
                    this.StarTypeNames.Add(s);
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeProbability"))
                        {
                            t = xr.GetAttribute("Name");
                            this.PlanetTypeNames.Add(t);
                            h.Add(t, int.Parse(xr.GetAttribute("Probability")));
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeProbabilityPerStar")));

                    this.PlanetTypeProbabilitiesPerStar.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeProbabilitiesPerStar")));
        }

        /// <summary>
        /// Reads planet size probabilities per type.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetSizeProbabilitiesPerType(XmlTextReader xr)
        {
            string s, t;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetSizeProbabilityPerType"))
                {
                    s = xr.GetAttribute("Name");
                    this.PlanetTypeNames.Add(s);
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetSizeProbability"))
                        {
                            t = xr.GetAttribute("Name");
                            this.PlanetSizeNames.Add(t);
                            h.Add(t, int.Parse(xr.GetAttribute("Probability")));
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetSizeProbabilityPerType")));

                    this.PlanetSizeProbabilitiesPerType.Add(s, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetSizeProbabilitiesPerType")));
        }

        /// <summary>
        /// Reads moon number chances.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadMoonNumberChances(XmlTextReader xr)
        {
            int n, p;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "MoonNumberChance"))
                {
                    n = int.Parse(xr.GetAttribute("Number"));
                    p = int.Parse(xr.GetAttribute("Probability"));
                    this.MoonNumberChances.Add(n, p);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "MoonNumberChances")));
        }

        /// <summary>
        /// Reads moon number probabilities per planet type.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadMoonNumberProbabilitiesPerPlanetType(XmlTextReader xr)
        {
            string pt;
            int n, p;
            Dictionary<int, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "MoonNumberProbabilityPerPlanetType"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<int, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "MoonNumberChance"))
                        {
                            n = int.Parse(xr.GetAttribute("Number"));
                            p = int.Parse(xr.GetAttribute("Probability"));
                            h.Add(n, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "MoonNumberProbabilityPerPlanetType")));

                    this.MoonNumberProbabilitiesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "MoonNumberProbabilitiesPerPlanetType")));
        }

        /// <summary>
        /// Reads temple chances per star type.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadTempleChancesPerStarType(XmlTextReader xr)
        {
            string n;
            int p;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "TempleChancePerStarType"))
                {
                    n = xr.GetAttribute("Name");
                    p = int.Parse(xr.GetAttribute("Probability"));
                    this.TempleChancesPerStarType.Add(n, p);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "TempleChancesPerStarType")));
        }

        /// <summary>
        /// Reads planet anomalies probability scale.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetAnomaliesProbabilityScale(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetAnomalyProbablityScale"))
                {
                    this.PlanetAnomaliesProbabilityScale.Add(int.Parse(xr.GetAttribute("Probability")), int.Parse(xr.GetAttribute("ScaledProbability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetAnomaliesProbabilityScale")));
        }

        /// <summary>
        /// Reads planet anomalies per planet type.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetAnomaliesPerPlanetType(XmlTextReader xr)
        {
            string pt, an;
            int p;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeAnomaly"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetAnomalyPerPlanetType"))
                        {
                            an = xr.GetAttribute("Name");
                            this.AnomalyNames.Add(an);
                            p = int.Parse(xr.GetAttribute("Probability"));
                            h.Add(an, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeAnomaly")));
                    this.PlanetAnomaliesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetAnomaliesPerPlanetType")));
        }

        /// <summary>
        /// Reads planet strategic resource probabilities scale.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetStrategicResourceProbabilitiesScale(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetStrategicResourceProbabilityScale"))
                {
                    this.PlanetStrategicResourceProbabilitiesScale.Add(int.Parse(xr.GetAttribute("Probability")), int.Parse(xr.GetAttribute("ScaledProbability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetStrategicResourceProbabilitiesScale")));
        }

        /// <summary>
        /// Reads planet strategic resources per planet type.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetStrategicResourcesPerPlanetType(XmlTextReader xr)
        {
            string pt, an;
            int p;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeStrategicResource"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) &&
                             (xr.Name == "PlanetStrategicResourcePerPlanetType"))
                        {
                            an = xr.GetAttribute("Name");
                            this.StrategicResourceNames.Add(an);
                            p = int.Parse(xr.GetAttribute("Probability"));
                            h.Add(an, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeStrategicResource")));

                    this.PlanetStrategicResourcesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetStrategicResourcesPerPlanetType")));
        }

        /// <summary>
        /// Reads planet luxury probabilities scale.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetLuxuryProbabilitiesScale(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetLuxuryProbablityScale"))
                {
                    this.PlanetLuxuryProbabilitiesScale.Add(int.Parse(xr.GetAttribute("Probability")), int.Parse(xr.GetAttribute("ScaledProbability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetLuxuryProbabilitiesScale")));
        }

        /// <summary>
        /// Reads planet luxuries per planet type.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadPlanetLuxuriesPerPlanetType(XmlTextReader xr)
        {
            string pt, an;
            int p;
            Dictionary<string, int> h;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetTypeLuxury"))
                {
                    pt = xr.GetAttribute("Name");
                    h = new Dictionary<string, int>();
                    do
                    {
                        xr.Read();
                        if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "PlanetLuxuryPerPlanetType"))
                        {
                            an = xr.GetAttribute("Name");
                            this.LuxuryResourceNames.Add(an);
                            p = int.Parse(xr.GetAttribute("Probability"));
                            h.Add(an, p);
                        }
                    }
                    while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetTypeLuxury")));
                    this.PlanetLuxuriesPerPlanetType.Add(pt, h);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "PlanetLuxuriesPerPlanetType")));
        }

        /// <summary>
        /// Reads luxury resource spawn priorities.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadLuxuryResourceSpawnPriorities(XmlTextReader xr)
        {
            int p;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "LuxuryResourceSpawnPriority"))
                {
                    p = int.Parse(xr.GetAttribute("Priority"));
                    if (!this.LuxuryResourceTiers.Keys.Contains(p))
                    {
                        this.LuxuryResourceTiers.Add(p, new HashSet<string>());
                    }

                    this.LuxuryResourceTiers[p].Add(xr.GetAttribute("Name"));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "LuxuryResourceSpawnPriorities")));
        }

        /// <summary>
        /// Reads temple type probabilities.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadTempleTypeProbabilities(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "TempleType"))
                {
                    this.TempleTypeProbabilities.Add(xr.GetAttribute("Name"), int.Parse(xr.GetAttribute("Probability")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "TempleTypes")));
        }

        /// <summary>
        /// Reads generation constraints.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadGenerationConstraints(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "MinStarsPerConstellation":
                            this.GenerationConstraints.MinStarsPerConstellation = xr.ReadElementContentAsInt();
                            break;
                        case "MinStarDistance":
                            this.GenerationConstraints.MinStarDistance = xr.ReadElementContentAsDouble();
                            break;
                        case "MinEmpireDistance":
                            this.GenerationConstraints.MinEmpireDistance = xr.ReadElementContentAsDouble();
                            break;
                    }

                    // else if (xr.Name == "MaxWormholesConnections")
                    // this.generationConstraints.maxWormholesConnections = xr.ReadElementContentAsInt();
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "GenerationConstraints")));

            Trace.WriteLine("cross-checking constraints...");
            Trace.WriteLine("Min stars per constellation : " + this.GenerationConstraints.MinStarsPerConstellation);
            Trace.WriteLine("Min distance between stars : " + this.GenerationConstraints.MinStarDistance);
            Trace.WriteLine("Min distance between home systems : " + this.GenerationConstraints.MinEmpireDistance);
            Trace.WriteLine("...end of cross-checking constraints");
        }

        /// <summary>
        /// Reads star names.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadStarNames(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "StarName"))
                {
                    this.StarNames.Add(xr.ReadElementContentAsString());
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "StarNames")));
        }

        /// <summary>
        /// Reads constellation names.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadConstellationNames(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "ConstellationName"))
                {
                    this.ConstellationNames.Add(xr.ReadElementContentAsString());
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "ConstellationNames")));
        }

        /// <summary>
        /// Reads home generation.
        /// </summary>
        /// <param name="xr">
        /// Xml Reader
        /// </param>
        protected void ReadHomeGeneration(XmlTextReader xr)
        {
            string name;

            Trace.WriteLine("ReadHomeGeneration-begin");

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Trait"))
                {
                    name = xr.GetAttribute("Name");
                    this.HomeGenerationTraitsNames.Add(name);
                    this.HomeGenerationTraits.Add(name, new HomeTrait(xr));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "HomeGeneration")));

            foreach (string s in this.HomeGenerationTraitsNames)
            {
                Trace.WriteLine(s);
            }

            Trace.WriteLine("ReadHomeGeneration-end");
        }
    }
}
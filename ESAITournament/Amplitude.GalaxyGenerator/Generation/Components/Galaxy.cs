// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Galaxy.cs" company="AMPLITUDE Studios">
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
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Amplitude.GalaxyGenerator.Drawing;
using Amplitude.GalaxyGenerator.Generation.Builders;

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// The galaxy.
    /// </summary>
    public class Galaxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Galaxy"/> class.
        /// </summary>
        /// <param name="configuration"> Configuration for this generation </param>
        protected Galaxy(Configuration configuration)
        {
            ////Galaxy.Instance = this;
            this.IsValid = true;

            Trace.WriteLine("Galaxy-constructor-1");

            Instance = this;

            this.BuilderList = new List<Builder>();

            StarBuilder starBuilder = new StarBuilder();
            ConstellationBuilder constellationsBuilder = new ConstellationBuilder();
            WarpBuilder warpBuilder = new WarpBuilder();
            SpawnBuilder spawnBuilder = new SpawnBuilder();
            PlanetBuilder planetBuilder = new PlanetBuilder();
            HomeBuilder homeBuilder = new HomeBuilder();
            StrategicResourceBuilder strategicResourceBuilder = new StrategicResourceBuilder();
            LuxuryResourceBuilder luxuryResourceBuilder = new LuxuryResourceBuilder();

            Trace.WriteLine("Galaxy-constructor-2");

            this.BuilderList.Add(starBuilder);
            this.BuilderList.Add(warpBuilder); // first pass
            this.BuilderList.Add(constellationsBuilder);
            this.BuilderList.Add(warpBuilder); // second pass
            this.BuilderList.Add(spawnBuilder);
            this.BuilderList.Add(planetBuilder);
            this.BuilderList.Add(homeBuilder);
            this.BuilderList.Add(strategicResourceBuilder);
            this.BuilderList.Add(luxuryResourceBuilder);

            Trace.WriteLine("Galaxy-constructor-3");

            this.Configuration = configuration;

            this.Stars = new List<StarSystem>();
            this.Warps = new List<WarpLine>();
            this.Constellations = new List<Constellation>();
            this.Regions = new List<Region>();
            this.SpawnStars = new List<StarSystem>();

            Trace.WriteLine("Galaxy-constructor-4");

            foreach (Builder builder in this.BuilderList)
            {
                if (this.IsValid)
                {
                    builder.Execute();
                    this.IsValid = this.IsValid && builder.Result;
                }
            }

            Trace.WriteLine("Galaxy-constructor-5");

            if (!this.IsValid)
            {
                Trace.WriteLine("--Galaxy generation failed--");
                Trace.WriteLine("--Generation defects summary--");
                foreach (Builder b in this.BuilderList)
                {
                    foreach (string text in b.Defects)
                    {
                        Trace.WriteLine(b.Name + " -> " + text);
                    }
                }

                Trace.WriteLine("--Generation defects end--");
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Galaxy"/> class. 
        /// </summary>
        ~Galaxy()
        {
            Instance = null;
            this.Configuration.ResetNames();
            this.Planets.Clear();
        }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        public static Galaxy Instance { get; protected set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public Configuration Configuration { get; protected set; }

        /// <summary>
        /// Gets or sets the stars.
        /// </summary>
        public List<StarSystem> Stars { get; protected set; }

        /// <summary>
        /// Gets or sets the warps.
        /// </summary>
        public List<WarpLine> Warps { get; protected set; }

        /// <summary>
        /// Gets or sets the constellations.
        /// </summary>
        public List<Constellation> Constellations { get; protected set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public List<Region> Regions { get; protected set; }

        /// <summary>
        /// Gets or sets the spawn stars.
        /// </summary>
        public List<StarSystem> SpawnStars { get; protected set; }

        /// <summary>
        /// Gets the planets.
        /// </summary>
        public List<Planet> Planets
        {
            get
            {
                List<Planet> list = new List<Planet>();
                foreach (StarSystem s in this.Stars)
                {
                    list.AddRange(s.Planets);
                }

                return list;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public bool IsValid { get; protected set; }

        /// <summary>
        /// Gets or sets the builder list.
        /// </summary>
        private List<Builder> BuilderList { get; set; }

        /// <summary>
        /// Generates a galaxy
        /// </summary>
        /// <param name="configuration"> Configuration instance for this generation </param>
        public static void Generate(Configuration configuration)
        {
            Trace.WriteLine("Galaxy-StaticGenerate-1");
            if (Instance == null)
            {
                Trace.WriteLine("Galaxy-StaticGenerate-2");
                Instance = new Galaxy(configuration);
                Trace.WriteLine("Galaxy-StaticGenerate-3");
            }

            Trace.WriteLine("Galaxy-StaticGenerate-4");
        }

        /// <summary>
        /// Releases the galaxy.
        /// </summary>
        public static void Release()
        {
            Instance = null;
        }

        /// <summary>
        /// Writes the Galaxy to xml
        /// </summary>
        /// <param name="xw"> Xml Writer </param>
        public void WriteXml(XmlWriter xw)
        {
            Dictionary<Color, int> hr = new Dictionary<Color, int>();
            double x, z, minX, minY, minZ, maxX, maxY, maxZ;

            minX = 9999;
            minY = 0;
            minZ = 9999;
            maxX = -9999;
            maxY = 0;
            maxZ = -9999;
            foreach (StarSystem s in this.Stars)
            {
                x = s.Position.X;
                z = s.Position.Y;
                if (x > maxX)
                {
                    maxX = x;
                }

                if (x < minX)
                {
                    minX = x;
                }

                if (z > maxZ)
                {
                    maxZ = z;
                }

                if (z < minZ)
                {
                    minZ = z;
                }
            }

            xw.WriteStartElement("Dimensions");
            xw.WriteStartElement("Size");
            xw.WriteAttributeString("X", (maxX - minX).ToString());
            xw.WriteAttributeString("Y", (maxY - minY).ToString());
            xw.WriteAttributeString("Z", (maxZ - minZ).ToString());
            xw.WriteEndElement();
            xw.WriteStartElement("Bounds");
            xw.WriteAttributeString("MaxX", maxX.ToString());
            xw.WriteAttributeString("MaxY", maxY.ToString());
            xw.WriteAttributeString("MaxZ", maxZ.ToString());
            xw.WriteAttributeString("MinX", minX.ToString());
            xw.WriteAttributeString("MinY", minY.ToString());
            xw.WriteAttributeString("MinZ", minZ.ToString());
            xw.WriteEndElement();
            xw.WriteEndElement();

            xw.WriteStartElement("Empires");
            foreach (StarSystem s in this.SpawnStars)
            {
                xw.WriteStartElement("Home");
                xw.WriteAttributeString("System", s.Id.ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Regions");
            foreach (StarSystem s in this.Stars)
            {
                if (hr.Keys.Contains(s.RegionIndex))
                {
                    hr[s.RegionIndex]++;
                }
                else
                {
                    hr.Add(s.RegionIndex, 1);
                }
            }

            foreach (Color rgb in hr.Keys)
            {
                xw.WriteStartElement("Region");
                xw.WriteAttributeString("Color", "Red=" + rgb.R.ToString() + " Green=" + rgb.G.ToString() + " Blue=" + rgb.B.ToString());
                xw.WriteAttributeString("NumStars", hr[rgb].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Constellations");
            foreach (Constellation c in this.Constellations)
            {
                xw.WriteStartElement("Constellation");
                xw.WriteAttributeString("Name", c.Name);
                xw.WriteAttributeString("Id", c.Id.ToString());
                xw.WriteAttributeString("Population", c.Count.ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteElementString("Population", this.Stars.Count.ToString());

            xw.WriteStartElement("Systems");
            foreach (StarSystem starSystem in this.Stars)
            {
                xw.WriteStartElement("System");
                if (starSystem.Constellation() != null)
                {
                    xw.WriteAttributeString("Constellation", starSystem.Constellation().Id.ToString());
                }
                else
                {
                    xw.WriteAttributeString("Constellation", "NotInConstellation");
                    Trace.WriteLine("System n°" + starSystem.Id.ToString() + " is not in a constellation !");
                }

                xw.WriteAttributeString("Name", starSystem.Name);
                xw.WriteAttributeString("Type", starSystem.Type);
                xw.WriteAttributeString("Id", starSystem.Id.ToString());
                xw.WriteAttributeString("X", starSystem.Position.X.ToString());
                xw.WriteAttributeString("Y", "0");
                xw.WriteAttributeString("Z", starSystem.Position.Y.ToString());
                xw.WriteStartElement("Region");
                xw.WriteAttributeString("Red", starSystem.Region.Index.R.ToString());
                xw.WriteAttributeString("Green", starSystem.Region.Index.G.ToString());
                xw.WriteAttributeString("Blue", starSystem.Region.Index.B.ToString());
                xw.WriteEndElement();
                xw.WriteStartElement("Planets");

                for (int i = 0; i < starSystem.Planets.Count; i++)
                {
                    Planet planet = starSystem.Planets[i];
                    xw.WriteStartElement("Planet");
                    xw.WriteAttributeString("Orbit", i.ToString());
                    xw.WriteAttributeString("Size", planet.Size);
                    xw.WriteAttributeString("Type", planet.Type);
                    xw.WriteElementString("Anomalies", planet.Anomaly);
                    xw.WriteStartElement("Moons");

                    for (int j = 0; j < planet.MoonsTemples.Count; j++)
                    {
                        xw.WriteStartElement("Moon");
                        xw.WriteAttributeString("Temple", planet.MoonsTemples[j]);
                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                    xw.WriteStartElement("Resources");
                    if (planet.Resource != null)
                    {
                        xw.WriteAttributeString(planet.Resource.Type.ToString(), planet.Resource.Name);
                        xw.WriteAttributeString("Size", planet.Resource.Size.ToString());
                    }

                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Warps");
            foreach (WarpLine w in this.Warps)
            {
                if (!w.IsWormhole)
                {
                    xw.WriteStartElement("Warp");
                    xw.WriteAttributeString("System1", w.StarA.Id.ToString());
                    xw.WriteAttributeString("System2", w.StarB.Id.ToString());
                    xw.WriteEndElement();
                }
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Wormholes");
            foreach (WarpLine w in this.Warps)
            {
                if (w.IsWormhole)
                {
                    xw.WriteStartElement("Wormhole");
                    xw.WriteAttributeString("System1", w.StarA.Id.ToString());
                    xw.WriteAttributeString("System2", w.StarB.Id.ToString());
                    xw.WriteEndElement();
                }
            }

            xw.WriteEndElement();

            this.OutputStatistics(xw);
        }

        /// <summary>
        /// Computes the diameter of a galaxy
        /// </summary>
        /// <returns>Diameter of the galaxy </returns>
        public double Diameter()
        {
            double d = 0;
            foreach (StarSystem s in this.Stars)
            {
                if (s.DirectDistanceTable.Count <= 0)
                {
                    s.ComputeDirectDistanceTable();
                }

                foreach (StarSystem y in s.DirectDistanceTable.Keys)
                {
                    if (s.DirectDistanceTable[y] > d)
                    {
                        d = s.DirectDistanceTable[y];
                    }
                }
            }

            return d;
        }

        /// <summary>
        /// Writes the current galaxy stats in the output xml
        /// </summary>
        /// <param name="xw"> Xml Writer </param>
        private void OutputStatistics(XmlWriter xw)
        {
            Dictionary<string, int> starTypeCount = new Dictionary<string, int>();
            Dictionary<int, int> planetNumberCount = new Dictionary<int, int>();
            Dictionary<string, int> planetTypeCount = new Dictionary<string, int>();
            Dictionary<string, int> planetSizeTypeCount = new Dictionary<string, int>();
            Dictionary<string, int> anomalyCount = new Dictionary<string, int>();
            Dictionary<string, int> templeCount = new Dictionary<string, int>();
            List<string> orderedKeys = new List<string>();
            Dictionary<string, HashSet<ResourceDeposit>> sr = new Dictionary<string, HashSet<ResourceDeposit>>();

            xw.WriteStartElement("Statistics");

            xw.WriteStartElement("StarTypes");
            foreach (StarSystem s in this.Stars)
            {
                if (starTypeCount.Keys.Contains(s.Type))
                {
                    starTypeCount[s.Type]++;
                }
                else
                {
                    starTypeCount.Add(s.Type, 1);
                }
            }

            foreach (string txt in starTypeCount.Keys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", starTypeCount[txt].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("PlanetPerSystem");
            int total = 0;
            foreach (StarSystem s in this.Stars)
            {
                total += s.Planets.Count;
                if (planetNumberCount.Keys.Contains(s.Planets.Count))
                {
                    planetNumberCount[s.Planets.Count]++;
                }
                else
                {
                    planetNumberCount.Add(s.Planets.Count, 1);
                }
            }

            foreach (int i in planetNumberCount.Keys)
            {
                xw.WriteStartElement("Planets");
                xw.WriteAttributeString("NumPlanets", i.ToString());
                xw.WriteAttributeString("NumSystems", planetNumberCount[i].ToString());
                xw.WriteEndElement();

                Trace.WriteLine(planetNumberCount[i].ToString() + " systems with " + i.ToString() + " planets");
            }

            xw.WriteEndElement();
            Trace.WriteLine("Average " + (total / (double)this.Stars.Count).ToString() + " planets per system");

            xw.WriteStartElement("PlanetTypes");
            foreach (StarSystem s in this.Stars)
            {
                foreach (Planet p in s.Planets)
                {
                    if (planetTypeCount.Keys.Contains(p.Type))
                    {
                        planetTypeCount[p.Type]++;
                    }
                    else
                    {
                        planetTypeCount.Add(p.Type, 1);
                    }
                }
            }

            foreach (string txt in planetTypeCount.Keys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", planetTypeCount[txt].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Anomalies");
            foreach (StarSystem s in this.Stars)
            {
                foreach (Planet p in s.Planets)
                {
                    if (anomalyCount.Keys.Contains(p.Anomaly))
                    {
                        anomalyCount[p.Anomaly]++;
                    }
                    else
                    {
                        anomalyCount.Add(p.Anomaly, 1);
                    }
                }
            }

            foreach (string txt in anomalyCount.Keys)
            {
                if (txt == string.Empty)
                {
                    xw.WriteStartElement("NoAnomaly");
                }
                else
                {
                    xw.WriteStartElement(txt);
                }

                xw.WriteAttributeString("Quantity", anomalyCount[txt].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("PlanetTypesAndSizes");
            foreach (StarSystem s in this.Stars)
            {
                foreach (Planet p in s.Planets)
                {
                    string txt = p.Size + "-" + p.Type;
                    if (planetSizeTypeCount.Keys.Contains(txt))
                    {
                        planetSizeTypeCount[txt]++;
                    }
                    else
                    {
                        planetSizeTypeCount.Add(txt, 1);
                    }
                }
            }

            orderedKeys = new List<string>(planetSizeTypeCount.Keys);
            orderedKeys.Sort();
            foreach (string txt in orderedKeys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", planetSizeTypeCount[txt].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Resources");
            sr.Clear();
            foreach (StarSystem s in this.Stars)
            {
                foreach (Planet p in s.Planets)
                {
                    if (p.Resource != null)
                    {
                        if (!sr.Keys.Contains(p.Resource.Name))
                        {
                            sr.Add(p.Resource.Name, new HashSet<ResourceDeposit>());
                        }

                        sr[p.Resource.Name].Add(p.Resource);
                    }
                }
            }

            foreach (string txt in sr.Keys)
            {
                foreach (ResourceDeposit rd in sr[txt])
                {
                    xw.WriteStartElement(txt);
                    xw.WriteAttributeString("Size", rd.Size.ToString());
                    xw.WriteAttributeString("System", rd.Location.System.Id.ToString());
                    xw.WriteEndElement();
                }
            }

            xw.WriteEndElement();

            xw.WriteStartElement("Temples");
            templeCount.Clear();
            foreach (StarSystem s in this.Stars)
            {
                foreach (Planet p in s.Planets)
                {
                    foreach (string txt in p.MoonsTemples)
                    {
                        if (templeCount.Keys.Contains(txt))
                        {
                            templeCount[txt]++;
                        }
                        else
                        {
                            templeCount.Add(txt, 1);
                        }
                    }
                }
            }

            foreach (string txt in templeCount.Keys)
            {
                xw.WriteStartElement(txt);
                xw.WriteAttributeString("Quantity", templeCount[txt].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            int minWarps, maxWarps, maxWH, nwh;
            double avgWarps, totalWarps, nsys;
            Dictionary<int, int> warpCounts = new Dictionary<int, int>();

            minWarps = 999;
            maxWarps = 0;
            maxWH = 0;
            nsys = 0;
            totalWarps = 0;
            foreach (StarSystem s in this.Stars)
            {
                nsys++;
                totalWarps += s.Destinations.Count;
                if (warpCounts.ContainsKey(s.Destinations.Count))
                {
                    warpCounts[s.Destinations.Count]++;
                }
                else
                {
                    warpCounts.Add(s.Destinations.Count, 1);
                }

                if (s.Destinations.Count < minWarps)
                {
                    minWarps = s.Destinations.Count;
                }

                if (s.Destinations.Count > maxWarps)
                {
                    maxWarps = s.Destinations.Count;
                }

                nwh = 0;
                foreach (WarpLine w in this.Warps)
                {
                    if (w.IsWormhole && ((w.StarA == s) || (w.StarB == s)))
                    {
                        nwh++;
                    }
                }

                if (nwh > maxWH)
                {
                    maxWH = nwh;
                }
            }

            if (nsys > 0)
            {
                avgWarps = totalWarps / nsys;
            }
            else
            {
                avgWarps = 0;
            }

            avgWarps = (double)((int)(100 * avgWarps)) / 100;

            xw.WriteStartElement("Connectivity");
            xw.WriteAttributeString("AverageWarps", avgWarps.ToString());
            xw.WriteAttributeString("MinWarps", minWarps.ToString());
            xw.WriteAttributeString("MaxWarps", maxWarps.ToString());
            xw.WriteAttributeString("MaxWormholes", maxWH.ToString());
            xw.WriteEndElement();

            xw.WriteStartElement("WarpCount");
            foreach (int n in warpCounts.Keys)
            {
                xw.WriteStartElement("Warps");
                xw.WriteAttributeString("NofWarps", n.ToString());
                xw.WriteAttributeString("NofStars", warpCounts[n].ToString());
                xw.WriteEndElement();
            }

            xw.WriteEndElement();

            xw.WriteStartElement("GenerationDefects");
            foreach (Builder builder in this.BuilderList)
            {
                foreach (string defect in builder.Defects)
                {
                    xw.WriteStartElement("Defect");
                    xw.WriteAttributeString("Builder", builder.Name);
                    xw.WriteAttributeString("Details", defect);
                    xw.WriteEndElement();
                }
            }

            xw.WriteEndElement();

            xw.WriteEndElement();
        }
    }
}
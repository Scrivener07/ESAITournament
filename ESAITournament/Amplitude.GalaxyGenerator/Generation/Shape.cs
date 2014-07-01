// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Shape.cs" company="AMPLITUDE Studios">
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
using System.IO;
using System.Xml;
using Amplitude.GalaxyGenerator.Drawing;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// Galaxy Shapes
    /// </summary>
    public class Shape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="reader"> Xml reader </param>
        public Shape(XmlTextReader reader)
        {
            this.DensityMap = null;
            string directory = GalaxyGeneratorPlugin.RootPath; // System.Diagnostics.Process.GetCurrentProcess().StartInfo.WorkingDirectory;
            this.DensityFileName = reader.GetAttribute("DensityMap");

            if (this.DensityFileName != null)
            {
                Trace.WriteLine(directory + this.DensityFileName);
                if (File.Exists(directory + this.DensityFileName))
                {
                    this.DensityMap = new ImageInfos(ImageType.Targa, directory + this.DensityFileName);
                }
                else
                {
                    throw new FileNotFoundException(directory+ this.DensityFileName);
                }

                if (this.DensityMap == null)
                {
                    Trace.WriteLine(directory + this.DensityFileName + " was either not found or unusable");
                }
            }

            this.RegionsFileName = reader.GetAttribute("RegionMap");
            Trace.WriteLine(directory + this.RegionsFileName);
            if (File.Exists(directory + this.RegionsFileName))
            {
                this.RegionsMap = new ImageInfos(ImageType.Targa, directory + this.RegionsFileName);
            }
            else
            {
                throw new FileNotFoundException(directory+this.RegionsFileName);
            }

            this.MinConstellations = int.Parse(reader.GetAttribute("MinConstellations"));
            this.MaxConstellations = int.Parse(reader.GetAttribute("MaxConstellations"));
            this.MinEmpires = int.Parse(reader.GetAttribute("MinEmpires"));
            this.MaxEmpires = int.Parse(reader.GetAttribute("MaxEmpires"));

            this.Regions = new Dictionary<Color, bool>();
            this.SpawnerSequence = new List<Color>();
            this.SymetryOptions = new HashSet<int>();
            this.Topology = new List<Link>();
            this.RegionWeights = new Dictionary<Color, int>();

            do
            {
                reader.Read();
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "SymetryOptions":
                            this.ReadSymetryOptions(reader);
                            break;
                        case "Regions":
                            this.ReadRegions(reader);
                            break;
                        case "Topology":
                            this.ReadTopology(reader);
                            break;
                    }
                }
            }
            while (!((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "GalaxyShape")));

            // System.Diagnostics.Trace.WriteLine("finished reading shape description");
        }

        /// <summary>
        /// Gets or sets the density file name.
        /// </summary>
        public string DensityFileName { get; protected set; }

        /// <summary>
        /// Gets or sets the regions file name.
        /// </summary>
        public string RegionsFileName { get; protected set; }

        /// <summary>
        /// Gets or sets the density map.
        /// </summary>
        public ImageInfos DensityMap { get; protected set; }

        /// <summary>
        /// Gets or sets the regions map.
        /// </summary>
        public ImageInfos RegionsMap { get; protected set; }

        /// <summary>
        /// Gets or sets the minimum constellations.
        /// </summary>
        public int MinConstellations { get; protected set; }

        /// <summary>
        /// Gets or sets the max constellations.
        /// </summary>
        public int MaxConstellations { get; protected set; }

        /// <summary>
        /// Gets or sets the min empires.
        /// </summary>
        public int MinEmpires { get; protected set; }

        /// <summary>
        /// Gets or sets the max empires.
        /// </summary>
        public int MaxEmpires { get; protected set; }

        /// <summary>
        /// Gets or sets the symetry options.
        /// </summary>
        public HashSet<int> SymetryOptions { get; protected set; }

        /// <summary>
        /// Gets or sets the regions.
        /// </summary>
        public Dictionary<Color, bool> Regions { get; protected set; }

        /// <summary>
        /// Gets or sets the topology.
        /// </summary>
        public List<Link> Topology { get; protected set; }

        /// <summary>
        /// Gets or sets the spawner sequence.
        /// </summary>
        public List<Color> SpawnerSequence { get; protected set; }

        /// <summary>
        /// Gets or sets the region weights.
        /// </summary>
        public Dictionary<Color, int> RegionWeights { get; protected set; }

        /// <summary>
        /// Reads symetry options
        /// </summary>
        /// <param name="xr"> xml reader </param>
        protected void ReadSymetryOptions(XmlTextReader xr)
        {
            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "SymetryOption"))
                {
                    this.SymetryOptions.Add(int.Parse(xr.GetAttribute("PlayerNum")));
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "SymetryOptions")));
        }

        /// <summary>
        /// Read regions.
        /// </summary>
        /// <param name="xr"> Xml Reader </param>
        protected void ReadRegions(XmlTextReader xr)
        {
            Color rgb;
            string col;
            byte r, g, b;
            bool spawner;
            bool hasColor;
            int weight;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Region") && xr.HasAttributes)
                {
                    hasColor = false;
                    rgb = Color.White;
                    spawner = false;
                    weight = 1;

                    xr.MoveToFirstAttribute();
                    do
                    {
                        switch (xr.Name)
                        {
                            case "color":
                                col = xr.GetAttribute("color");
                                r = byte.Parse(col.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                                g = byte.Parse(col.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                                b = byte.Parse(col.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                                rgb = Color.FromArgb(r, g, b);
                                hasColor = true;
                                break;
                            case "empirestart":
                                spawner = xr.GetAttribute("empirestart") == "1";
                                break;
                            case "weight":
                                weight = int.Parse(xr.GetAttribute("weight"));
                                break;
                        }
                    }
                    while (xr.MoveToNextAttribute());

                    if (!hasColor)
                    {
                        Trace.WriteLine("Found an undefined color for a region in Shapes file !?");
                        do
                        {
                            rgb = Color.FromArgb(GalaxyGeneratorPlugin.Random.Next(256), GalaxyGeneratorPlugin.Random.Next(256), GalaxyGeneratorPlugin.Random.Next(256));
                        }
                        while (this.Regions.ContainsKey(rgb));
                    }

                    this.Regions.Add(rgb, spawner);
                    if (spawner)
                    {
                        this.SpawnerSequence.Add(rgb);
                    }

                    this.RegionWeights.Add(rgb, weight);
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Regions")));
        }

        /// <summary>
        /// Reads the Topology.
        /// </summary>
        /// <param name="xr"> Xml Reader </param>
        protected void ReadTopology(XmlTextReader xr)
        {
            Color regionA, regionB;
            string txt;
            byte r, g, b;

            do
            {
                xr.Read();
                if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Link"))
                {
                    txt = xr.GetAttribute("RegionA");
                    r = byte.Parse(txt.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(txt.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(txt.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    regionA = Color.FromArgb(r, g, b);

                    txt = xr.GetAttribute("RegionB");
                    r = byte.Parse(txt.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = byte.Parse(txt.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = byte.Parse(txt.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    regionB = Color.FromArgb(r, g, b);

                    this.Topology.Add(new Link(regionA, regionB));
                }

                /*else if ((xr.NodeType == XmlNodeType.Element) && (xr.Name == "Pool"))
                {
                    txt = xr.GetAttribute("Region");
                    r = Byte.Parse(txt.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    g = Byte.Parse(txt.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    b = Byte.Parse(txt.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    this.poolRegions.Add(Color.FromArgb(r, g, b));
                }*/
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Topology")));
        }

        /// <summary>
        /// Link between regions
        /// </summary>
        public class Link
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Link"/> class.
            /// </summary>
            /// <param name="a"> First region connected </param>
            /// <param name="b"> Second region connected </param>
            public Link(Color a, Color b)
            {
                this.RegionA = a;
                this.RegionB = b;
            }

            /// <summary>
            /// Gets or sets the region a.
            /// </summary>
            public Color RegionA { get; set; }

            /// <summary>
            /// Gets or sets the region b.
            /// </summary>
            public Color RegionB { get; set; }
        }
    }
}

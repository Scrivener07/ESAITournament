// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StarBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the StarBuilder type.
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
    /// The star builder.
    /// </summary>
    public class StarBuilder : Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StarBuilder"/> class.
        /// </summary>
        public StarBuilder()
        {
            this.RawRgbSet = new HashSet<Color>();
        }

        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public override string Name
        {
            get { return "StarBuilder"; }
        }

        /// <summary>
        /// Gets or sets the raw rg bset.
        /// </summary>
        public HashSet<Color> RawRgbSet { get; protected set; }

        /// <summary>
        /// Executes the builder
        /// </summary>
        public override void Execute()
        {
            int secOut;
            PointF p;
            Color regionIndex = new Color();
            int balancedPopulation, randomPopulation;

            balancedPopulation =(int)(Galaxy.Instance.Configuration.Population * Galaxy.Instance.Configuration.StarPopulationBalancing);
            randomPopulation = Galaxy.Instance.Configuration.Population - balancedPopulation;

            Trace.WriteLine(this.Name + " - Execute - begin");
            Trace.WriteLine(this.Name + " - Balanced Population = " + balancedPopulation);
            Trace.WriteLine(this.Name + " - Random Population = " + randomPopulation);

            // DISTRIBUTE BALANCE POPULATION
            Dictionary<Color, int> balancedPopulationRegions = new Dictionary<Color, int>();
            int globalWeight = 0;
            foreach (Color c in Galaxy.Instance.Configuration.Shape.Regions.Keys)
            {
                balancedPopulationRegions.Add(c, 0);
                globalWeight += Galaxy.Instance.Configuration.Shape.RegionWeights[c];
            }

            Trace.WriteLine("GlobalWeights = " + globalWeight);

            int i = balancedPopulation;
            while (i >= globalWeight)
            {
                foreach (Color c in Galaxy.Instance.Configuration.Shape.Regions.Keys)
                {
                    balancedPopulationRegions[c] += Galaxy.Instance.Configuration.Shape.RegionWeights[c];
                    i -= Galaxy.Instance.Configuration.Shape.RegionWeights[c];
                }
            }

            if (balancedPopulationRegions.Keys.Count(c => balancedPopulationRegions[c] <= 0) > 0)
            {
                this.TraceDefect("Predictable star population is insufficient for successful balancing");
                balancedPopulation = 0;
                randomPopulation = Galaxy.Instance.Configuration.Population;
            }
            else
            {
                foreach (Color c in balancedPopulationRegions.Keys)
                {
                    Trace.WriteLine("Region " + c.ToString() + " will have at least " + balancedPopulationRegions[c] + " stars");
                }
            }

            // PLACE STARS
            Dictionary<Color, int> currentRegionPopulation = new Dictionary<Color, int>();
            foreach (Color c in balancedPopulationRegions.Keys)
            {
                currentRegionPopulation.Add(c, 0);
            }

            int failed = 0;
            i = 0;
            while ((i < balancedPopulation) && (failed < 10 * Galaxy.Instance.Configuration.Population))
            {
                secOut = 0;
                do
                {
                    p = this.RandomPosition(out regionIndex);
                    secOut++;
                }
                while ((this.Overlap(p) || (!Galaxy.Instance.Configuration.Shape.Regions.Keys.Contains(regionIndex)) || (currentRegionPopulation[regionIndex] >= balancedPopulationRegions[regionIndex])) && (secOut < 100));

                if (currentRegionPopulation[regionIndex] >= balancedPopulationRegions[regionIndex])
                {
                    failed++;
                }
                else if (this.TryStarPlacement(p, regionIndex))
                {
                    currentRegionPopulation[regionIndex]++;
                    i++;
                }
                else
                {
                    failed++;
                }
            }

            if (i < balancedPopulation)
            {
                this.TraceDefect("Could place only " + i + " balanced stars iso " + balancedPopulation);
                randomPopulation += balancedPopulation - i;
            }

            for (int index = 0; index < randomPopulation; index++)
            {
                secOut = 0;
                do
                {
                    p = this.RandomPosition(out regionIndex);
                    secOut++;
                }
                while ((this.Overlap(p) || (!Galaxy.Instance.Configuration.Shape.Regions.Keys.Contains(regionIndex))) && (secOut < 100));

                if (this.TryStarPlacement(p, regionIndex))
                {
                    currentRegionPopulation[regionIndex]++;
                }
            }

            foreach (StarSystem s in Galaxy.Instance.Stars)
            {
                s.ComputeDirectDistanceTable();
            }

            //// BUILD REGIONS - BEGIN
            foreach (Color c in Galaxy.Instance.Configuration.Shape.Regions.Keys)
            {
                Galaxy.Instance.Regions.Add(new Region(c));
            }
            //// BUILD REGIONS - END

            Trace.WriteLine("Generated " + Galaxy.Instance.Stars.Count + " stars");
            Trace.WriteLine("populate-end");

            if (Galaxy.Instance.Stars.Count < Galaxy.Instance.Configuration.Population / 5)
            {
                this.TraceDefect("Very few stars were generated", true);
                return;
            }

            if (Galaxy.Instance.Stars.Count < (4 * Galaxy.Instance.Configuration.Population) / 5)
            {
                this.TraceDefect("Abnormally few stars were generated");
            }

            Trace.WriteLine("Black is R=" + Color.Black.R + " G=" + Color.Black.G + " B=" + Color.Black.B);
            Trace.WriteLine("Colors found in regions pixmap :");
            foreach (Color c in this.RawRgbSet)
            {
                Trace.WriteLine(c.ToString());
            }

            if (Galaxy.Instance.Stars.Count < Galaxy.Instance.Configuration.EmpiresNumber())
            {
                this.TraceDefect("More empires than generated stars", true);
                return;
            }

            this.Result = true;
            Trace.WriteLine(this.Name + " - Execute - end");
        }

        /// <summary>
        /// Tries to place star on the grid
        /// </summary>
        /// <param name="point"> Point in grid </param>
        /// <param name="regionIndex"> Region's index</param>
        /// <returns> True if the star has been successfully placed </returns>
        protected bool TryStarPlacement(PointF point, Color regionIndex)
        {
            if (this.Overlap(point))
            {
                this.TraceDefect("Placed two stars too close to each other");
            }

            if (Galaxy.Instance.Configuration.Shape.Regions.Keys.Contains(regionIndex))
            {
                StarSystem starSystem = new StarSystem(point)
                                            {
                                                RegionIndex = regionIndex,
                                                Id = Galaxy.Instance.Stars.Count
                                            };
                Galaxy.Instance.Stars.Add(starSystem);
                return true;
            }

            this.TraceDefect("Unable to place star : appeared in black space");
            return false;
        }

        /// <summary>
        /// Picks a random position in region
        /// </summary>
        /// <param name="regionIndex"> Region's index. </param>
        /// <returns> A valid random point in region </returns>
        protected PointF RandomPosition(out Color regionIndex)
        {
            PointF p, c;
            int secOut;
            Point pixD, pixR;
            double rawPosX, rawPosY;
            double localD, checkD, scale;
            Color regionColor, densityColor;

            c = new PointF(
                            (float)Galaxy.Instance.Configuration.MaxWidth / 2,
                            (float)Galaxy.Instance.Configuration.MaxWidth / 2);

            scale = Galaxy.Instance.Configuration.MaxWidth / Galaxy.Instance.Configuration.Shape.RegionsMap.Width;

            pixD = new Point();
            pixR = new Point();

            secOut = 0;
            do
            {
                do
                {
                    rawPosX = GalaxyGeneratorPlugin.Random.NextDouble();
                    rawPosY = GalaxyGeneratorPlugin.Random.NextDouble();
                    pixR.X = (int)(rawPosX * Galaxy.Instance.Configuration.Shape.RegionsMap.Width);
                    pixR.Y = (int)(rawPosY * Galaxy.Instance.Configuration.Shape.RegionsMap.Height);

                    if (Galaxy.Instance.Configuration.Shape.DensityMap != null)
                    {
                        pixD.X = (int)(rawPosX * Galaxy.Instance.Configuration.Shape.DensityMap.Width);
                        pixD.Y = (int)(rawPosY * Galaxy.Instance.Configuration.Shape.DensityMap.Height);
                        densityColor = Galaxy.Instance.Configuration.Shape.DensityMap.GetPixel(pixD.X, pixD.Y);
                        localD = (densityColor.R + densityColor.G + densityColor.B) / 768.0;
                    }
                    else
                    {
                        localD = 1.0;
                    }

                    checkD = GalaxyGeneratorPlugin.Random.NextDouble();
                    regionColor = Galaxy.Instance.Configuration.Shape.RegionsMap.GetPixel(pixR.X, pixR.Y);
                    regionColor = Color.FromArgb(regionColor.R, regionColor.G, regionColor.B);
                    this.RawRgbSet.Add(regionColor);
                    secOut++;
                }
                while ((checkD > localD) && (secOut < 120));

                this.PostProcess(ref regionColor);
            }
            while (regionColor == Color.Black);

            p = new PointF(
                (float)((pixR.X * scale) + (GalaxyGeneratorPlugin.Random.NextDouble() * scale)),
                (float)((pixR.Y * scale) + (GalaxyGeneratorPlugin.Random.NextDouble() * scale)));

            p.X -= c.X;
            p.Y -= c.Y;
            p.Y = -p.Y;

            regionIndex = regionColor;
            return p;
        }

        /// <summary>
        /// Tests if the point given overlaps with another StarSystem within the zone
        /// </summary>
        /// <param name="point"> Point that needs to be tested </param>
        /// <returns> True if the point overlaps with another StarSystem </returns>
        protected bool Overlap(PointF point)
        {
            foreach (StarSystem s in Galaxy.Instance.Stars)
            {
                if (Geometry2D.Distance(point, s.Position) <= Galaxy.Instance.Configuration.StarOverlapDistance)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Computes deviation for a given color to smooth differences between colors that creation tools can create
        /// For example => [255,1,0,1] == [255,0,0,0]
        /// Tries to guess the closest match in the key colors
        /// </summary>
        /// <param name="colorKey"> Color key </param>
        protected void PostProcess(ref Color colorKey)
        {
            if (Galaxy.Instance.Configuration.Shape.Regions.Keys.Contains(colorKey))
            {
                return;
            }

            double dr, dg, db;
            double d, min;
            Color match;
            List<Color> allPlusBlack = new List<Color>(Galaxy.Instance.Configuration.Shape.Regions.Keys)
                                           {
                                               Color.Black
                                           };

            min = 9999999;
            match = Color.Black;

            foreach (Color k in allPlusBlack)
            {
                dr = colorKey.R - k.R;
                dg = colorKey.G - k.G;
                db = colorKey.B - k.B;
                d = System.Math.Sqrt((dr * dr) + (dg * dg) + (db * db));

                if (d < min)
                {
                    min = d;
                    match = k;
                }
            }

            colorKey = match;
        }
    }
}

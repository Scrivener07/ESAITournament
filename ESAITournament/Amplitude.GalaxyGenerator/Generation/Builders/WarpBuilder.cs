// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WarpBuilder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the WarpBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Amplitude.GalaxyGenerator.Drawing;
using Amplitude.GalaxyGenerator.Generation.Components;

using Triangulator.Geometry;
using Point = Triangulator.Geometry.Point;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// The warp builder.
    /// </summary>
    public class WarpBuilder : Builder
    {
        /// <summary>
        /// The star-to-warpline "too close" factor.
        /// </summary>
        private const double StarToWarpLineTooCloseFactor = 0.2;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarpBuilder"/> class.
        /// </summary>
        public WarpBuilder()
        {
            this.Stars = new List<StarSystem>();
            this.Warps = new List<Warp>();
            this.PassNumber = 0;
        }

        /// <summary>
        /// Gets the WarpBuilder's name.
        /// </summary>
        public override string Name
        {
            get { return "WarpBuilder"; }
        }

        /// <summary>
        /// Gets or sets the pass number.
        /// </summary>
        protected int PassNumber { get; set; }

        /// <summary>
        /// Gets or sets the stars.
        /// </summary>
        protected List<StarSystem> Stars { get; set; }

        /// <summary>
        /// Gets or sets the warps.
        /// </summary>
        protected List<Warp> Warps { get; set; }

        /// <summary>
        /// Find closest starsystem from a given system, in a list of other systems
        /// </summary>
        /// <param name="from"> StarSystem from where we want the closest other StarSystem </param>
        /// <param name="others"> StarSystem list where we're looking for a closest StarSystem </param>
        /// <returns> Closest StarSystem </returns>
        public static StarSystem FindClosest(StarSystem from, List<StarSystem> others)
        {
            ////System.Diagnostics.Trace.WriteLine("findClosest from system n°" + from.id.ToString() + " in a list comprising " + others.Count.ToString() + " systems");

            if (from.DirectDistanceTable.Count == 0)
            {
                from.ComputeDirectDistanceTable();
            }

            StarSystem closest = null;
            double distanceMin = Galaxy.Instance.Configuration.MaxWidth * 100.0;
            foreach (StarSystem s in others)
            {
                double distance = from.DirectDistanceTable[s];
                if ((distance < distanceMin) && (s != from))
                {
                    distanceMin = distance;
                    closest = s;
                }
            }

            return closest;
        }

        /// <summary>
        /// Executes the builder
        /// </summary>
        public override void Execute()
        {
            this.PassNumber++;

            if (this.PassNumber == 1)
            {
                this.Stars.Clear();
                this.Stars.AddRange(Galaxy.Instance.Stars);

                Trace.WriteLine(this.Name + " - Execute - First pass");

                if (this.Stars.Count == 0)
                {
                    Trace.WriteLine("Serious fault - no stars given to WarpBuilder ???");
                    this.Defects.Add("No stars available");
                    this.Result = false;
                    return;
                }

                this.CreateRaw();
                this.RemoveAllCloseToStar();

                foreach (Shape.Link link in Galaxy.Instance.Configuration.Shape.Topology)
                {
                    List<StarSystem> pool = new List<StarSystem>();
                    pool.AddRange(Galaxy.Instance.Regions.Find(r => (r.Index == link.RegionA)));
                    pool.AddRange(Galaxy.Instance.Regions.Find(r => (r.Index == link.RegionB)));
                    this.ForceConnect(pool);
                }

                // foreach (Constellation c in Galaxy.Instance.Constellations) this.ForceConnect(c);
                this.ForceConnect(this.Stars);

                if (!this.FullyConnected())
                {
                    Trace.WriteLine("Unable to fully connect galaxy");
                    this.Defects.Add("Unable to fully connect");
                    this.Result = false;
                    return;
                }

                foreach (Warp w in this.Warps)
                {
                    w.StarA.PreWarps.Add(w.StarB);
                    w.StarB.PreWarps.Add(w.StarA);
                }
            }
            else if (this.PassNumber == 2)
            {
                Trace.WriteLine(this.Name + " Execute - Second pass");

                foreach (Constellation c in Galaxy.Instance.Constellations)
                {
                    this.ForceConnect(c);
                }
                //// this.ForceConnect(this.Stars);

                this.CreateWormholes();

                this.EliminateCrossers();
                this.ReduceConnectivity();
                this.ReduceWormholeClutter();

                Trace.WriteLine("Warp Builder Execute...Complete");

                foreach (StarSystem s in this.Stars)
                {
                    s.ComputeWarpDistanceTable();
                }

                this.WriteToGalaxy();

                this.Result = true;

                Trace.WriteLine(this.Name + " - Execute - end");
            }
        }

        /// <summary>
        /// Creates wormholes
        /// </summary>
        protected void CreateWormholes()
        {
            foreach (Warp w in this.Warps.Where(w => w.StarA.Constellation() != w.StarB.Constellation()))
            {
                w.IsWormhole = true;
            }
        }

        /// <summary>
        /// Write warps to galaxy
        /// </summary>
        protected void WriteToGalaxy()
        {
            foreach (Warp w in this.Warps)
            {
                Galaxy.Instance.Warps.Add(w.IsWormhole
                                               ? new Wormhole(w.StarA, w.StarB)
                                               : new WarpLine(w.StarA, w.StarB));
            }

            foreach (StarSystem s in Galaxy.Instance.Stars)
            {
                s.ComputeWarpDistanceTable();
            }
        }

        /// <summary>
        /// Checks if there's a warp line between two systems
        /// </summary>
        /// <param name="a"> First system </param>
        /// <param name="b"> Second system </param>
        /// <returns> True if a warp exists between those two systems, false otherwise </returns>
        protected bool WarpExistsBetween(StarSystem a, StarSystem b)
        {
            return this.Warps.Any(w => ((w.StarA == a) && (w.StarB == b)) || ((w.StarA == b) && (w.StarB == a)));
        }

        /// <summary>
        /// Retrieves the warp lines from a given star system
        /// </summary>
        /// <param name="s"> Star system checked </param>
        /// <returns> A new List of warps line from the given starSystem </returns>
        protected List<Warp> WarpsOf(StarSystem s)
        {
            return new List<Warp>(this.Warps.FindAll(w => (w.StarA == s) || (w.StarB == s)));
        }

        /// <summary>
        /// Checks if every StarSystem is connected
        /// </summary>
        /// <returns>True if every StarSystem is connected, false otherwise </returns>
        protected bool FullyConnected()
        {
            if (Galaxy.Instance.Constellations.Any(c => !this.Connected(c)))
            {
                return false;
            }

            foreach (Shape.Link link in Galaxy.Instance.Configuration.Shape.Topology)
            {
                List<StarSystem> pool = new List<StarSystem>();
                pool.AddRange(Galaxy.Instance.Regions.Find(r => (r.Index == link.RegionA)));
                pool.AddRange(Galaxy.Instance.Regions.Find(r => (r.Index == link.RegionB)));

                if (!this.Connected(pool))
                {
                    return false;
                }
            }

            return this.Connected(this.Stars);
        }

        /// <summary>
        /// Creates a raw batch of warps
        /// </summary>
        protected void CreateRaw()
        {
            this.Warps.Clear();

            List<Point> vertices = new List<Point>();
            foreach (StarSystem star in this.Stars)
            {
                vertices.Add(new Point<StarSystem>(star.Position.X, star.Position.Y, star));
            }

            List<Triangle> triangles = new List<Triangle>(Triangulator.Delauney.Triangulate(vertices));

            foreach (Triangle t in triangles)
            {
                StarSystem s1, s2, s3;

                s1 = ((Point<StarSystem>)vertices[t.P1]).Attribute;
                s2 = ((Point<StarSystem>)vertices[t.P2]).Attribute;
                s3 = ((Point<StarSystem>)vertices[t.P3]).Attribute;

                if (!this.WarpExistsBetween(s1, s2) && this.Connectable(s1, s2))
                {
                    this.Warps.Add(new Warp(s1, s2));
                }

                if (!this.WarpExistsBetween(s2, s3) && this.Connectable(s2, s3))
                {
                    this.Warps.Add(new Warp(s2, s3));
                }

                if (!this.WarpExistsBetween(s3, s1) && this.Connectable(s3, s1))
                {
                    this.Warps.Add(new Warp(s3, s1));
                }
            }

            /*
            int rawWarpQuantity, i;
            List<StarSystem> closest, global, warpable;
            List<Region> adjacent;
            StarSystem sc;
            Warp w;
            double factor = 2.0;

            this.Warps.Clear();

            closest = new List<StarSystem>();

            rawWarpQuantity = (int)Math.Min(factor * Galaxy.Instance.Configuration.connectivity(), (double)(this.Stars.Count));

            if (rawWarpQuantity <= 0)
            {
                System.Diagnostics.Trace.WriteLine("Defaulted raw warp quantity to 6");
                rawWarpQuantity = 6;
                this.Defects.Add("Defaulted initial raw warp quantity to 6");
            }

            foreach (StarSystem s in this.Stars)
            {
                global = new List<StarSystem>(this.Stars);
                closest.Clear();
                global.Remove(s);
                warpable = new List<StarSystem>();
                adjacent = new List<Region>(s.region.adjacentRegions());

                warpable.AddRange(s.region);
                foreach(Region r in adjacent)
                    warpable.AddRange(r);

                if (s.directDistanceTable.Count == 0)
                    s.computeDirectDistanceTable();

                for (i = 0; i < rawWarpQuantity; i++)
                {
                    sc = WarpBuilder.FindClosest(s, global);
                    global.Remove(sc);
                    closest.Add(sc);
                }

                closest.RemoveAll((StarSystem y) => { return !warpable.Contains(y); });

                foreach (StarSystem sb in closest)
                {
                    if (!this.WarpExistsBetween(s, sb))
                    {
                        w = new Warp(s, sb);
                        this.Warps.Add(w);
                    }
                }
            }
            */
        }

        /// <summary>
        /// Forces connection between the starsystems of a given list
        /// </summary>
        /// <param name="list"> StarSystems we want to connect </param>
        protected void ForceConnect(List<StarSystem> list)
        {
            List<StarSystem> all = new List<StarSystem>();
            List<List<StarSystem>> blocks = new List<List<StarSystem>>();
            List<StarSystem> block = new List<StarSystem>();

            do
            {
                all.AddRange(list);
                block.Clear();
                blocks.Clear();

                while (all.Count > 0)
                {
                    this.FindConnectedBlockFrom(all.First(), list, ref block);
                    blocks.Add(new List<StarSystem>(block));
                    all.RemoveAll(s => block.Contains(s));
                }

                if (blocks.Count > 1)
                {
                    List<StarSystem> pair = ConstellationBuilder.FindClosestPair(blocks[0], blocks[1]);
                    if (pair.Count > 1)
                    {
                        if ((pair[0] != null) && (pair[1] != null))
                        {
                            this.Warps.Add(new Warp(pair[0], pair[1]));
                        }
                    }
                }
            }
            while (blocks.Count > 1);
        }

        /// <summary>
        /// Removes something
        /// </summary>
        protected void RemoveAllCloseToStar()
        {
            HashSet<Warp> delenda = new HashSet<Warp>();

            ////System.Diagnostics.Trace.WriteLine("RemoveAllCloseToStar-begin");

            foreach (Warp warp in this.Warps)
            {
                foreach (StarSystem system in this.Stars)
                {
                    PointF o = system.Position;
                    PointF a = warp.StarA.Position;
                    PointF b = warp.StarB.Position;

                    if ((warp.StarA != system) && (warp.StarB != system))
                    {
                        PointF p = Geometry2D.Symmetrical(o, a, b);
                        if (Geometry2D.IntersectionCheck(a, b, o, p) == Geometry2D.IntersectionType.InsideSegment)
                        {
                            if (Geometry2D.Distance(o, p) < StarToWarpLineTooCloseFactor * Geometry2D.Distance(a, b))
                            {
                                delenda.Add(warp);
                            }
                        }
                    }
                }
            }

            foreach (Warp w in delenda)
            {
                this.Warps.Remove(w);
            }

            ////System.Diagnostics.Trace.WriteLine("RemoveAllCloseToStar-end");
        }

        /// <summary>
        /// Checks if there are warplines crossing
        /// </summary>
        protected void CheckAllCrossings()
        {
            ////System.Diagnostics.Trace.WriteLine("CheckAllCrossings-begin");

            PointF h = new PointF();

            foreach (Warp w in this.Warps)
            {
                w.CrossingWarps.Clear();
            }

            foreach (Warp w1 in this.Warps)
            {
                foreach (Warp w2 in this.Warps)
                {
                    if ((w1.StarA != w2.StarA) && (w1.StarA != w2.StarB) && (w1.StarB != w2.StarA) && (w1.StarB != w2.StarB))
                    {
                        PointF a = w1.StarA.Position;
                        PointF b = w1.StarB.Position;
                        PointF c = w2.StarA.Position;
                        PointF d = w2.StarB.Position;

                        if (Geometry2D.Intersection(a, b, c, d, ref h) == Geometry2D.IntersectionType.InsideSegment)
                        {
                            w1.CrossingWarps.Add(w2);
                            w2.CrossingWarps.Add(w1);
                        }
                    }
                }
            }

            ////int n = 0;
            ////foreach ( Warp w in this.Warps )
            ////{
            ////    n += w.CrossingWarps.Count;
            ////}

            ////System.Diagnostics.Trace.WriteLine("Found " + n.ToString() + " crossings in total");
            ////System.Diagnostics.Trace.WriteLine("CheckAllCrossings-end");
        }

        /// <summary>
        /// Eliminates crosser warps
        /// </summary>
        protected void EliminateCrossers()
        {
            List<Warp> delendaList = new List<Warp>();
            List<Warp> crossers = new List<Warp>();
            List<Warp> breakers = new List<Warp>();

            do
            {
                delendaList.Clear();
                this.CheckAllCrossings();
                crossers.Clear();

                foreach (Warp w in this.Warps)
                {
                    if (w.CrossingWarps.Count > 0)
                    {
                        crossers.Add(w);
                    }
                }

                crossers.RemoveAll(breakers.Contains);

                if (crossers.Count > 0)
                {
                    Warp keepCandidate;
                    do
                    {
                        keepCandidate = crossers.ElementAt(GalaxyGeneratorPlugin.Random.Next(crossers.Count));
                    }
                    while (breakers.Contains(keepCandidate));

                    foreach (Warp w in keepCandidate.CrossingWarps)
                    {
                        delendaList.Add(w);
                    }

                    foreach (Warp w in delendaList)
                    {
                        this.Warps.Remove(w);
                        foreach (Warp v in this.Warps)
                        {
                            v.RemoveReferenceTo(w);
                        }
                    }

                    if (!this.FullyConnected())
                    {
                        breakers.Add(keepCandidate);
                        this.Warps.AddRange(delendaList);
                    }
                }
            }
            while ((crossers.Count > 0) || (!this.FullyConnected()));
        }

        /// <summary>
        /// Reduces connectivity according to configuration
        /// </summary>
        protected void ReduceConnectivity()
        {
            if (this.AverageConnectivity() <= Galaxy.Instance.Configuration.Connectivity)
            {
                return;
            }

            List<Warp> nonCriticals = new List<Warp>();

            do
            {
                this.DetermineNonCriticalWarps(ref nonCriticals);

                if (nonCriticals.Count > 0)
                {
                    Warp candidate = nonCriticals.ElementAt(GalaxyGeneratorPlugin.Random.Next(nonCriticals.Count));
                    this.Warps.Remove(candidate);
                    foreach (Warp w in this.Warps)
                    {
                        w.RemoveReferenceTo(candidate);
                    }
                }

                Trace.WriteLine("Average connectivity down to " + this.AverageConnectivity());
            }
            while ((nonCriticals.Count > 0) && (this.AverageConnectivity() > Galaxy.Instance.Configuration.Connectivity));
        }

        /// <summary>
        /// Determines non critical warps.
        /// </summary>
        /// <param name="nonCrit"> List of non critical warps </param>
        protected void DetermineNonCriticalWarps(ref List<Warp> nonCrit)
        {
            List<Warp> copyList = new List<Warp>(this.Warps);

            copyList.RemoveAll(w => w.IsWormhole);

            foreach (Warp w in copyList)
            {
                this.Warps.Remove(w);
                w.IsCritical = !this.FullyConnected();
                this.Warps.Add(w);
            }

            nonCrit.Clear();
            nonCrit.AddRange(copyList.FindAll(w => !w.IsCritical));
        }

        /// <summary>
        /// Reduces wormhole clutter.
        /// </summary>
        protected void ReduceWormholeClutter()
        {
            List<Warp> nonCriticals = new List<Warp>();

            this.DetermineNonCriticalWormholes(ref nonCriticals);
            List<Warp> wormholes = this.Warps.FindAll(w => w.IsWormhole);
            Trace.WriteLine("Wormhole numbers down to " + wormholes.Count);

            while ((nonCriticals.Count > 0) && (((2.0 * wormholes.Count) / Galaxy.Instance.Constellations.Count) > (3 * Galaxy.Instance.Configuration.WormholeConnectivity)))
            {
                this.DetermineNonCriticalWormholes(ref nonCriticals);
                wormholes = this.Warps.FindAll(w => w.IsWormhole);

                if (nonCriticals.Count > 0)
                {
                    Warp candidate = nonCriticals.ElementAt(GalaxyGeneratorPlugin.Random.Next(nonCriticals.Count));
                    this.Warps.Remove(candidate);
                    foreach (Warp w in this.Warps)
                    {
                        w.RemoveReferenceTo(candidate);
                    }
                }

                Trace.WriteLine("Wormhole numbers down to " + wormholes.Count);
            }
        }

        /// <summary>
        /// Determines non critical wormholes.
        /// </summary>
        /// <param name="nonCrit"> List of non critical wormholes </param>
        protected void DetermineNonCriticalWormholes(ref List<Warp> nonCrit)
        {
            List<Warp> copyList = this.Warps.FindAll(w => w.IsWormhole);

            foreach (Warp w in copyList)
            {
                this.Warps.Remove(w);
                w.IsCritical = !this.FullyConnected();
                this.Warps.Add(w);
            }

            nonCrit.Clear();
            nonCrit.AddRange(copyList.FindAll(w => !w.IsCritical));
        }

        /// <summary>
        /// Finds a warp between 2 systems
        /// </summary>
        /// <param name="a"> StarSystem A </param>
        /// <param name="b"> StarSystem B </param>
        /// <returns> Warp between the two system if it exists </returns>
        protected Warp FindWarp(StarSystem a, StarSystem b)
        {
            return this.Warps.FirstOrDefault(w => ((w.StarA == a) && (w.StarB == b)) || ((w.StarA == b) && (w.StarB == a)));
        }

        /// <summary>
        /// Computes the average connectivity in the galaxy
        /// </summary>
        /// <returns> Average connectivity factor </returns>
        protected double AverageConnectivity()
        {
            int n = this.Warps.FindAll(w => !w.IsWormhole).Count;

            if ((n > 0) && (this.Stars.Count > 0))
            {
                return (2.0 * n) / this.Stars.Count;
            }

            return 0.0;
        }

        /// <summary>
        /// Checks if two StarSystem are connectable
        /// </summary>
        /// <param name="a"> StarSystem to check </param>
        /// <param name="b"> StarSystem to check against. </param>
        /// <returns> True if systems are connectable, false otherwise </returns>
        private bool Connectable(StarSystem a, StarSystem b)
        {
            return (a.Region == b.Region) || a.Region.AdjacentRegions().Contains(b.Region);
        }

        /// <summary>
        /// Find connection between two blocks of StarSystem, from one StarSystem
        /// </summary>
        /// <param name="from"> StarSystem from where we look for a connection</param>
        /// <param name="inside"> The block where we are looking for a connection </param>
        /// <param name="block"> The block connected from the StarSystem</param>
        private void FindConnectedBlockFrom(StarSystem from, List<StarSystem> inside, ref List<StarSystem> block)
        {
            List<StarSystem> unknown = new List<StarSystem>(inside);
            List<StarSystem> front = new List<StarSystem>(inside.Count);
            List<StarSystem> nextFront = new List<StarSystem>(inside.Count);

            block.Clear();

            front.Add(from);
            block.AddRange(front);

            while (front.Count > 0)
            {
                nextFront.Clear();

                foreach (StarSystem s in front)
                {
                    unknown.Remove(s);
                }

                foreach (StarSystem s in front)
                {
                    List<Warp> connections = this.WarpsOf(s);

                    foreach (Warp w in connections.Where(w => unknown.Contains(w.StarA) || unknown.Contains(w.StarB)))
                    {
                        if (w.StarA == s)
                        {
                            nextFront.Add(w.StarB);
                            unknown.Remove(w.StarB);
                        }
                        else if (w.StarB == s)
                        {
                            nextFront.Add(w.StarA);
                            unknown.Remove(w.StarA);
                        }
                    }
                }

                front = new List<StarSystem>(nextFront);
                block.AddRange(nextFront);
            }
        }

        /// <summary>
        /// Checks if StarSystem in the source collection of StarSystem are connected
        /// </summary>
        /// <param name="list"> The list. </param>
        /// <returns> True if connected, false otherwise </returns>
        private bool Connected(List<StarSystem> list)
        {
            if (list == null)
            {
                return true;
            }

            if (list.Count <= 1)
            {
                return true;
            }

            List<StarSystem> unknown = new List<StarSystem>(list);
            List<StarSystem> front = new List<StarSystem>();
            List<StarSystem> nextFront = new List<StarSystem>();

            front.Add(list.First());

            while (front.Count > 0)
            {
                nextFront.Clear();

                foreach (StarSystem s in front)
                {
                    unknown.Remove(s);
                }

                foreach (StarSystem s in front)
                {
                    List<Warp> connections = this.WarpsOf(s);

                    foreach (Warp w in connections)
                    {
                        if (unknown.Contains(w.StarA) || unknown.Contains(w.StarB))
                        {
                            if (w.StarA == s)
                            {
                                nextFront.Add(w.StarB);
                                unknown.Remove(w.StarB);
                            }
                            else if (w.StarB == s)
                            {
                                nextFront.Add(w.StarA);
                                unknown.Remove(w.StarA);
                            }
                        }
                    }
                }

                front = new List<StarSystem>(nextFront);
            }

            return unknown.Count == 0;
        }

        /// <summary>
        /// Warp definition.
        /// </summary>
        protected class Warp : IEquatable<Warp>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Warp"/> class.
            /// </summary>
            /// <param name="a"> Entry StarSystem of the warp </param>
            /// <param name="b"> Exit StarSystem of the warp </param>
            public Warp(StarSystem a, StarSystem b)
            {
                this.StarA = a;
                this.StarB = b;
                this.Length = Geometry2D.Distance(this.StarA.Position, this.StarB.Position);
                this.CrossingWarps = new HashSet<Warp>();
                this.IsWormhole = a.Constellation() != b.Constellation();
            }

            /// <summary>
            /// Gets or sets a value indicating whether this warp is critical.
            /// </summary>
            public bool IsCritical { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this warp is a wormhole.
            /// </summary>
            public bool IsWormhole { get; set; }

            /// <summary>
            /// Gets or sets the star a.
            /// </summary>
            public StarSystem StarA { get; protected set; }

            /// <summary>
            /// Gets or sets the star b.
            /// </summary>
            public StarSystem StarB { get; protected set; }

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            public double Length { get; protected set; }

            /// <summary>
            /// Gets or sets the crossing warps.
            /// </summary>
            public HashSet<Warp> CrossingWarps { get; set; }

            /// <summary>
            /// Remove a reference to another warp
            /// </summary>
            /// <param name="warp"> The warp to remove </param>
            public void RemoveReferenceTo(Warp warp)
            {
                this.CrossingWarps.Remove(warp);
            }

            /// <summary>
            /// IEquatable implementation
            /// </summary>
            /// <param name="other">The warp compared to</param>
            /// <returns>True if they are connected to the same stars, false otherwise</returns>
            public bool Equals(Warp other)
            {
                return ((this.StarA == other.StarA) && (this.StarB == other.StarB)) ||
                       ((this.StarA == other.StarB) && (this.StarB == other.StarA));
            }
        }
    }
}

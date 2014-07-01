// <copyright file="ShapeManager.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   
// </summary>

using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// Shape manager.
    /// </summary>
    public class ShapeManager
    {
        /// <summary>
        /// Local instance of the Shape Manager for singleton
        /// </summary>
        private static ShapeManager instance = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="ShapeManager"/> class from being created from outside this class.
        /// </summary>
        private ShapeManager()
        {
            this.Shapes = new Dictionary<string, Shape>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ShapeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ShapeManager();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the shapes.
        /// </summary>
        public Dictionary<string, Shape> Shapes
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads shape files.
        /// </summary>
        /// <param name="pathToShapesFile"> The path to shapes file. </param>
        public static void Load(string pathToShapesFile)
        {
            Instance.LoadShapes(pathToShapesFile);
        }

        /// <summary>
        /// The load shapes.
        /// </summary>
        /// <param name="pathToShapesFile"> The path to shapes file. </param>
        private void LoadShapes(string pathToShapesFile)
        {
            using (XmlTextReader reader = new XmlTextReader(pathToShapesFile))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "GalaxyShape"))
                    {
                        string name = reader.GetAttribute("Name");
                        Shape shape = new Shape(reader);

                        //// System.Diagnostics.Trace.WriteLine("shape name " + name);

                        if (!this.Shapes.ContainsKey(name))
                        {
                            this.Shapes.Add(name, shape);
                        }
                        else
                        {
                            this.Shapes[name] = shape;
                        }
                    }
                }
            }

            Trace.WriteLine("cross-checking shapes manager contents...");
            foreach (string name in this.Shapes.Keys)
            {
                Trace.WriteLine(name);
            }

            Trace.WriteLine("...end of shapes manager contents cross-checking");
        }
    }
}

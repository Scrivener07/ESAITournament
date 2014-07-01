// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeTrait.cs" company="AMPLITUDE Studios">
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
using System.Linq;
using System.Xml;
using Amplitude.GalaxyGenerator.Generation.Components;

namespace Amplitude.GalaxyGenerator.Generation
{
    /// <summary>
    /// Trait for starting starSystem
    /// </summary>
    public class HomeTrait
    {
        /// <summary>
        /// List of components for this trait.
        /// </summary>
        private List<Component> components;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeTrait"/> class.
        /// </summary>
        /// <param name="xr"> Xml Reader </param>
        public HomeTrait(XmlTextReader xr)
        {
            Component.AppliesTo target = Component.AppliesTo.World;
            Component bit = null;

            this.components = new List<Component>();

            do
            {
                xr.Read();
                if (xr.NodeType == XmlNodeType.Element)
                {
                    switch (xr.Name)
                    {
                        case "HomeStar":
                            target = Component.AppliesTo.Star;
                            break;
                        case "HomePlanet":
                            target = Component.AppliesTo.World;
                            break;
                        case "OtherPlanets":
                            target = Component.AppliesTo.Other;
                            break;
                        default:
                            switch (xr.Name)
                            {
                                case "OverridePlanetsInSystem":
                                    bit = new ComponentPlanetsInSystem { Target = Component.AppliesTo.Star };
                                    break;
                                case "OverrideStarType":
                                    bit = new ComponentOverrideStarType { Target = Component.AppliesTo.Star };
                                    break;
                                case "OverrideType":
                                    bit = new ComponentOverrideType { Target = target };
                                    break;
                                case "OverrideSize":
                                    bit = new ComponentOverrideSize { Target = target };
                                    break;
                                case "OverrideAnomaly":
                                    bit = new ComponentOverrideAnomaly { Target = target };
                                    break;
                                case "InhibitAnomalies":
                                    bit = new ComponentInhibitAnomalies { Target = target };
                                    break;
                                case "InhibitLuxuryResources":
                                    bit = new ComponentInhibitLuxuries { Target = target };
                                    break;
                                case "InhibitStrategicResources":
                                    bit = new ComponentInhibitStrategics { Target = target };
                                    break;
                            }

                            if (null != bit)
                            {
                                bit.Read(xr);
                                this.components.Add(bit);
                            }

                            break;
                    }
                }
            }
            while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == "Trait")));
        }

        /// <summary>
        /// Applies traits to starsystems
        /// </summary>
        /// <param name="starSystem"> Target starSystem </param>
        public void Apply(StarSystem starSystem)
        {
            List<Component> localComponents = new List<Component>();

            System.Diagnostics.Trace.WriteLine("Applying Trait " + Settings.Instance.HomeGenerationTraits.First(pair => pair.Value == this).Key);

            localComponents.Clear();
            localComponents.AddRange(this.components.FindAll(bit => bit.Target == Component.AppliesTo.Star));

            foreach (Component c in localComponents)
            {
                c.Apply(new HomeGenerator.Pattern(starSystem));
            }

            localComponents.Clear();
            localComponents.AddRange(this.components.FindAll(bit => bit.Target == Component.AppliesTo.World));

            foreach (Component c in localComponents)
            {
                c.Apply(new HomeGenerator.Pattern(starSystem));
            }

            localComponents.Clear();
            localComponents.AddRange(this.components.FindAll(bit => bit.Target == Component.AppliesTo.Other));

            foreach (Component c in localComponents)
            {
                c.Apply(new HomeGenerator.Pattern(starSystem));
            }
        }

        /// <summary>
        /// HomeTrait component definition
        /// </summary>
        protected class Component
        {
            /// <summary>
            /// To what level the component applies to.
            /// </summary>
            public enum AppliesTo
            {
                /// <summary>
                /// Component applies to a Star
                /// </summary>
                Star,

                /// <summary>
                /// Component applies to a World
                /// </summary>
                World,

                /// <summary>
                /// Component applies to something else
                /// </summary>
                Other
            }

            /// <summary>
            /// Gets or sets the target.
            /// </summary>
            public AppliesTo Target { get; set; }

            /// <summary>
            /// Applies a pattern to a Home
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public virtual void Apply(HomeGenerator.Pattern hp)
            {
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public virtual void Read(XmlTextReader xr)
            {
            }
        }

        /// <summary>
        /// Specific component implementation
        /// </summary>
        protected class ComponentOverride : Component
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ComponentOverride"/> class.
            /// </summary>
            public ComponentOverride()
            {
                this.Weights = new SortedDictionary<string, int>();
            }

            /// <summary>
            /// Gets or sets the probability of this component occurrence.
            /// </summary>
            public double Probability { get; set; }

            /// <summary>
            /// Gets or sets the weights for this component.
            /// </summary>
            public SortedDictionary<string, int> Weights { get; set; }

            /// <summary>
            /// Reads an xml to retrieve components and elements
            /// </summary>
            /// <param name="xr"> Xml reader </param>
            /// <param name="component"> Target component. </param>
            /// <param name="element"> Target element. </param>
            protected void InternalRead(XmlTextReader xr, string component, string element)
            {
                if (xr.AttributeCount > 0)
                {
                    this.Probability = double.Parse(xr.GetAttribute("Probability"), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    this.Probability = 1.0;
                }

                do
                {
                    xr.Read();
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        if (xr.Name == element)
                        {
                            switch (xr.AttributeCount)
                            {
                                case 2:
                                    this.Weights.Add(xr.GetAttribute("Name"), int.Parse(xr.GetAttribute("Weight")));
                                    break;
                                case 1:
                                    this.Weights.Add(xr.GetAttribute("Name"), 1);
                                    break;
                            }
                        }
                    }
                }
                while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == component)));
            }

            /// <summary>
            /// Returns a random element based on their probabilities
            /// </summary>
            /// <returns>
            /// Chosen element's name
            /// </returns>
            protected string GetRandomOverride()
            {
                List<string> possibleElements = new List<string>();
                string element = null;

                if (GalaxyGeneratorPlugin.Random.NextDouble() <= this.Probability)
                {
                    foreach (string s in this.Weights.Keys)
                    {
                        int n = this.Weights[s];

                        for (int i = 0; i < n; i++)
                        {
                            possibleElements.Add(s);
                        }
                    }

                    if (possibleElements.Count > 0)
                    {
                        element = possibleElements[GalaxyGeneratorPlugin.Random.Next(possibleElements.Count)];
                    }
                }

                return element;
            }
        }

        /// <summary>
        /// Component Inhibitor.
        /// </summary>
        protected class ComponentInhibit : Component
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ComponentInhibit"/> class.
            /// </summary>
            public ComponentInhibit()
            {
                this.AllInhibited = false;
                this.InhibitList = new List<string>();
                this.TargetPlanets = new List<Planet>();
            }

            /// <summary>
            /// Gets or sets the inhibit list.
            /// </summary>
            public List<string> InhibitList { protected get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether all inhibited.
            /// </summary>
            protected bool AllInhibited { get; set; }

            /// <summary>
            /// Gets the Targeted planets.
            /// </summary>
            protected List<Planet> TargetPlanets { get; private set; }

            /// <summary>
            /// Applies a pattern to targeted planets
            /// </summary>
            /// <param name="hp">
            /// The hp.
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                switch (this.Target)
                {
                    case AppliesTo.Star:
                        this.TargetPlanets = hp.Star.Planets;
                        break;
                    case AppliesTo.World:
                        this.TargetPlanets.Add(hp.HomeWorld);
                        break;
                    case AppliesTo.Other:
                        this.TargetPlanets = hp.OtherPlanets;
                        break;
                }
            }

            /// <summary>
            /// Reads an xml to retrieve components and elements
            /// </summary>
            /// <param name="xr"> Xml reader </param>
            /// <param name="component"> Target component. </param>
            /// <param name="element"> Target element. </param>
            public void InternalRead(XmlTextReader xr, string component, string element)
            {
                do
                {
                    xr.Read();

                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        if (xr.Name == element)
                        {
                            this.InhibitList.Add(xr.GetAttribute("Name"));
                        }
                        else if (xr.Name == "All")
                        {
                            this.AllInhibited = true;
                        }
                    }
                }
                while (!((xr.NodeType == XmlNodeType.EndElement) && (xr.Name == component)));
            }
        }

        /// <summary>
        /// The component override starSystem type.
        /// </summary>
        protected class ComponentOverrideStarType : ComponentOverride
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                string modifiedStarType = GetRandomOverride();

                if (modifiedStarType != null)
                {
                    hp.Star.Type = modifiedStarType;
                    //// hp.Star.GeneratePlanets();
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "OverrideStarType", "StarType");
            }
        }

        /// <summary>
        /// The component planets in system.
        /// </summary>
        protected class ComponentPlanetsInSystem : ComponentOverride
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                int modifiedNumber;

                if (GalaxyGeneratorPlugin.Random.NextDouble() > this.Probability)
                {
                    return;
                }

                base.Apply(hp);

                string mod = this.GetRandomOverride();
                if (int.TryParse(mod, out modifiedNumber))
                {
                    hp.Star.GeneratePlanets(modifiedNumber);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Could not read Modified Planet Number from " + mod);
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "OverridePlanetsInSystem", "Quantity");
            }
        }

        /// <summary>
        /// The component override type.
        /// </summary>
        protected class ComponentOverrideType : ComponentOverride
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                string modifiedPlanetType = GetRandomOverride();
                if (null == modifiedPlanetType)
                {
                    return;
                }

                if (this.Target == AppliesTo.World)
                {
                    if (GalaxyGeneratorPlugin.Random.NextDouble() > this.Probability)
                    {
                        return;
                    }

                    hp.HomeWorld.ReCreateType(modifiedPlanetType);
                }
                else if (this.Target == AppliesTo.Other)
                {
                    foreach (Planet p in hp.OtherPlanets)
                    {
                        modifiedPlanetType = this.GetRandomOverride();
                        if ((null != modifiedPlanetType) &&
                             (GalaxyGeneratorPlugin.Random.NextDouble() <= this.Probability))
                        {
                            p.ReCreateType(modifiedPlanetType);
                        }
                    }
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "OverrideType", "Type");
            }
        }

        /// <summary>
        /// The component override size.
        /// </summary>
        protected class ComponentOverrideSize : ComponentOverride
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                string modifiedPlanetSize;

                base.Apply(hp);

                if (this.Target == AppliesTo.World)
                {
                    if (GalaxyGeneratorPlugin.Random.NextDouble() > this.Probability)
                    {
                        return;
                    }

                    modifiedPlanetSize = this.GetRandomOverride();
                    if (null != modifiedPlanetSize)
                    {
                        hp.HomeWorld.Size = modifiedPlanetSize;
                    }
                }
                else if (this.Target == AppliesTo.Other)
                {
                    foreach (Planet p in hp.OtherPlanets)
                    {
                        modifiedPlanetSize = this.GetRandomOverride();
                        if ((null != modifiedPlanetSize) &&
                             (GalaxyGeneratorPlugin.Random.NextDouble() <= this.Probability))
                        {
                            p.Size = modifiedPlanetSize;
                        }
                    }
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "OverrideSize", "Size");
            }
        }

        /// <summary>
        /// The component override anomaly.
        /// </summary>
        protected class ComponentOverrideAnomaly : ComponentOverride
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                string modifiedAnomaly;

                base.Apply(hp);

                if (this.Target == AppliesTo.World)
                {
                    if (GalaxyGeneratorPlugin.Random.NextDouble() > this.Probability)
                    {
                        return;
                    }

                    modifiedAnomaly = this.GetRandomOverride();
                    if (null != modifiedAnomaly)
                    {
                        hp.HomeWorld.Anomaly = modifiedAnomaly;
                    }
                }
                else if (this.Target == AppliesTo.Other)
                {
                    foreach (Planet p in hp.OtherPlanets)
                    {
                        modifiedAnomaly = this.GetRandomOverride();
                        if ((null != modifiedAnomaly) && (GalaxyGeneratorPlugin.Random.NextDouble() <= this.Probability))
                        {
                            p.Anomaly = modifiedAnomaly;
                        }
                    }
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "OverrideAnomaly", "Anomaly");
            }
        }

        /// <summary>
        /// The component inhibit anomalies.
        /// </summary>
        protected class ComponentInhibitAnomalies : ComponentInhibit
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if ((this.InhibitList.Count == 0) && this.AllInhibited)
                {
                    this.InhibitList.AddRange(Settings.Instance.AnomalyNames);
                }

                foreach (Planet p in this.TargetPlanets)
                {
                    p.InhibitedAnomalies.AddRange(this.InhibitList);
                    p.ApplyInhibitAnomalies();
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "InhibitAnomalies", "Anomaly");
            }
        }

        /// <summary>
        /// The component inhibit luxuries.
        /// </summary>
        protected class ComponentInhibitLuxuries : ComponentInhibit
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if (this.AllInhibited)
                {
                    this.InhibitList = Settings.Instance.LuxuryResourceNames.ToList();
                }

                foreach (Planet p in this.TargetPlanets)
                {
                    p.InhibitedLuxuries = this.InhibitList;
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "InhibitLuxurieResources", "LuxuryResource");
            }
        }

        /// <summary>
        /// The component inhibit strategics.
        /// </summary>
        protected class ComponentInhibitStrategics : ComponentInhibit
        {
            /// <summary>
            /// Applies a pattern.
            /// </summary>
            /// <param name="hp">
            /// Pattern which should be applied
            /// </param>
            public override void Apply(HomeGenerator.Pattern hp)
            {
                base.Apply(hp);

                if (this.AllInhibited)
                {
                    this.InhibitList = Settings.Instance.StrategicResourceNames.ToList();
                }

                foreach (Planet p in this.TargetPlanets)
                {
                    p.InhibitedStrategics = this.InhibitList;
                }
            }

            /// <summary>
            /// Reads the source XML
            /// </summary>
            /// <param name="xr">
            /// Xml Reader
            /// </param>
            public override void Read(XmlTextReader xr)
            {
                base.Read(xr);
                this.InternalRead(xr, "InhibitStrategicResources", "StrategicResource");
            }
        }
    }
}

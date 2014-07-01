// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Planet.cs" company="AMPLITUDE Studios">
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

using System;
using System.Collections.Generic;

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// The planet.
    /// </summary>
    public class Planet : IComparable<Planet>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Planet"/> class.
        /// </summary>
        /// <param name="starSystem"> StarSystem where this planet will spawn </param>
        public Planet(StarSystem starSystem)
        {
            this.InhibitedLuxuries = new List<string>();
            this.InhibitedStrategics = new List<string>();
            this.InhibitedAnomalies = new List<string>();
            this.MoonsTemples = new List<string>();

            Galaxy.Instance.Planets.Add(this);

            this.System = starSystem;

            this.ReCreateType(Galaxy.Instance.Configuration.GetRandomPlanetType(this.System.Type));
        }

        /// <summary>
        /// Gets the system.
        /// </summary>
        public StarSystem System { get; private set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the anomaly.
        /// </summary>
        public string Anomaly { get; set; }

        /// <summary>
        /// Gets the moons temples.
        /// </summary>
        public List<string> MoonsTemples { get; private set; }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        public ResourceDeposit Resource { get; set; }

        /// <summary>
        /// Gets or sets the inhibited luxuries.
        /// </summary>
        public List<string> InhibitedLuxuries { get; set; }

        /// <summary>
        /// Gets or sets the inhibited strategics.
        /// </summary>
        public List<string> InhibitedStrategics { get; set; }

        /// <summary>
        /// Gets or sets the inhibited anomalies.
        /// </summary>
        public List<string> InhibitedAnomalies { get; set; }

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int Index
        {
            get { return Galaxy.Instance.Planets.IndexOf(this); }
        }

        /// <summary>
        /// Gets a value indicating whether it has resource.
        /// </summary>
        public bool HasResource
        {
            get { return this.Resource != null; }
        }

        /// <summary>
        /// Gets the present resource name.
        /// </summary>
        public string PresentResourceName
        {
            get
            {
                return this.HasResource ? this.Resource.Name : string.Empty;
            }
        }

        /// <summary>
        /// Implementation of IComparable interface for Planets
        /// </summary>
        /// <param name="other"> Planet compared against </param>
        /// <returns> An integer specifying how the current planet compares to the other one </returns>
        public int CompareTo(Planet other)
        {
            return other.Index - this.Index;
        }

        /// <summary>
        /// Determines if the planet can accept strategic resource
        /// </summary>
        /// <param name="resourceName"> The resource name. </param>
        /// <returns> True if the Planet can have a strategic resource, false otherwise </returns>
        public bool CanAcceptStrategicResource(string resourceName)
        {
            if (this.Resource != null)
            {
                return false;
            }

            if (this.InhibitedStrategics.Contains(resourceName))
            {
                return false;
            }

            return Settings.Instance.PlanetStrategicResourcesPerPlanetType[this.Type][resourceName] != 0;
        }

        /// <summary>
        /// Determines if the planet can accept luxury resources
        /// </summary>
        /// <param name="resourceName"> The resource name. </param>
        /// <returns> True if the planet can have a luxury resource, false otherwise </returns>
        public bool CanAcceptLuxuryResource(string resourceName)
        {
            if (this.Resource != null)
            {
                return false;
            }

            if (this.InhibitedLuxuries.Contains(resourceName))
            {
                return false;
            }

            return Settings.Instance.PlanetLuxuriesPerPlanetType[this.Type][resourceName] != 0;
        }

        /// <summary>
        /// Recreates a planet of a given type
        /// </summary>
        /// <param name="type"> Type of Planet </param>
        public void ReCreateType(string type)
        {
            if (!Settings.Instance.PlanetTypeNames.Contains(type))
            {
                return;
            }

            this.Type = type;
            this.Size = Galaxy.Instance.Configuration.GetRandomPlanetSize(this.Type);
            this.MoonsTemples.Clear();
            this.CreateMoons();
            this.Anomaly = Galaxy.Instance.Configuration.GetRandomAnomaly(this.Type);
            this.ApplyInhibitAnomalies();
        }

        /// <summary>
        /// Applies anomalies inhibition for a planet
        /// </summary>
        public void ApplyInhibitAnomalies()
        {
            if (this.InhibitedAnomalies.Contains(this.Anomaly))
            {
                this.Anomaly = string.Empty;
            }
        }

        /// <summary>
        /// Creates moons for the Planet
        /// </summary>
        protected void CreateMoons()
        {
            int n = Galaxy.Instance.Configuration.GetRandomMoonNumber(this.Type);

            for (int i = 0; i < n; i++)
            {
                this.MoonsTemples.Add(Galaxy.Instance.Configuration.GetRandomTempleType(this.System.Type));
            }
        }
    }
}

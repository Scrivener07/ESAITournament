// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDeposit.cs" company="AMPLITUDE Studios">
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

namespace Amplitude.GalaxyGenerator.Generation.Components
{
    /// <summary>
    /// Resource deposit definition.
    /// </summary>
    public class ResourceDeposit
    {
        /// <summary>
        /// Gets the max size for a resource deposit.
        /// </summary>
        public static readonly int MaxSize = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDeposit"/> class.
        /// </summary>
        /// <param name="name"> Resource's name </param> 
        /// <param name="size"> Resource's size </param> 
        /// <param name="type"> Resource's type </param>
        public ResourceDeposit(string name, int size, ResourceType type)
        {
            this.Name = name;
            this.Size = size;
            this.Type = type;
        }

        /// <summary>
        /// Resource type
        /// </summary>
        public enum ResourceType
        {
            /// <summary>
            /// Strategic resource
            /// </summary>
            Strategic,

            /// <summary>
            /// Luxury resource
            /// </summary>
            Luxury
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Planet Location { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public ResourceType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int Size { get; protected set; }

        /// <summary>
        /// The increase size.
        /// </summary>
        public void IncreaseSize()
        {
            this.Size++;
        }
    }
}

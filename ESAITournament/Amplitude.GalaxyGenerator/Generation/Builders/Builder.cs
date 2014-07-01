// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Builder.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   Defines the Builder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Amplitude.GalaxyGenerator.Generation.Builders
{
    /// <summary>
    /// Builder abstract class used to implement all builders for the generation
    /// </summary>
    public abstract class Builder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Builder"/> class.
        /// </summary>
        public Builder()
        {
            this.Result = true;
            this.Defects = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the build succeeded or not.
        /// </summary>
        public bool Result { get; protected set; }

        /// <summary>
        /// Gets or sets list of defects for the current builder.
        /// </summary>
        public List<string> Defects { get; protected set; }

        /// <summary>
        /// Gets the builder's name.
        /// </summary>
        public virtual string Name
        {
            get { return "Abstract Builder"; }
        }

        /// <summary>
        /// Execute builder.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Traces builder's defects
        /// </summary>
        /// <param name="text">Defect's text </param>
        /// <param name="fatal">Defect is fatal or not</param>
        public void TraceDefect(string text, bool fatal = false)
        {
            this.Defects.Add(text);
            Trace.WriteLine(text);

            if (fatal)
            {
                this.Result = false;
            }
        }
    }
}

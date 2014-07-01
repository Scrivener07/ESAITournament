// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GalaxyGeneratorPlugin.cs" company="AMPLITUDE Studios">
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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using Amplitude.GalaxyGenerator.Generation;
using Amplitude.GalaxyGenerator.Generation.Components;

using Random = Amplitude.GalaxyGenerator.Utilities.Random;

/// <summary>
/// The galaxy generator plugin.
/// </summary>
public static class GalaxyGeneratorPlugin
{
    /// <summary>
    /// The root path.
    /// </summary>
    private static string rootPath = string.Empty;

    /// <summary>
    /// Gets or sets the root path.
    /// </summary>
    internal static string RootPath
    {
        get { return rootPath; }
        set { rootPath = value; }
    }

    /// <summary>
    /// Gets the random seed for the current galaxy generation.
    /// </summary>
    /// <remarks>
    /// Random class used is the Mono Random.cs found in the Unity Technology repository
    /// This has been added in the namespace Amplitude.GalaxyGenerator.Utilities in order to provide the same Random behaviour
    /// in this tool and in game, on all platforms.
    /// </remarks>
    internal static Random Random { get; private set; }

    /// <summary>
    /// Main entry point. Generates a galaxy based on settings found in settings file, shape file, and current configuration file
    /// </summary>
    /// <param name="pathToSettingsFile"> The path to settings file. </param>
    /// <param name="pathToShapesFile"> The path to shapes file. </param>
    /// <param name="pathToConfigurationFile"> The path to configuration file. </param>
    /// <param name="pathToOutputFile"> The path to output file. </param>
    /// <param name="seed"> The seed. </param>
    public static void Generate(string pathToSettingsFile, string pathToShapesFile, string pathToConfigurationFile, string pathToOutputFile, int seed)
    {
        rootPath = Directory.GetParent(pathToOutputFile) + "/";
#if TRACE

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        string pathToDebugFile = rootPath + "GalaxyGeneratorPlugin.log";

        File.Delete(pathToDebugFile);

        TextWriterTraceListener debugFileListener = new TextWriterTraceListener(pathToDebugFile);

        Trace.Listeners.Add(debugFileListener);
        Trace.AutoFlush = true;
#endif

        try
        {
            Settings.Load(pathToSettingsFile);
            ShapeManager.Load(pathToShapesFile);

            Configuration configuration = new Configuration(pathToConfigurationFile);

            if (seed != 0)
            {
                configuration.Seed = seed;
            }

            if (configuration.Seed == 0)
            {
                long ticks = DateTime.Now.Ticks;
                configuration.Seed = Math.Abs((int)ticks);
            }

            Random = new Random(configuration.Seed);

            int safetyCounter = 0;
            do
            {
                safetyCounter++;
                Galaxy.Release();
                Trace.WriteLine("Generating Galaxy...");
                Galaxy.Generate(configuration);
            }
            while ((!Galaxy.Instance.IsValid) && (safetyCounter < 6));

            if (!Galaxy.Instance.IsValid)
            {
                Trace.WriteLine("...Galaxy Generation failed...");
                Trace.WriteLine("...after " + safetyCounter + " successive tries");

#if TRACE
                debugFileListener.Close();
                Trace.Listeners.Remove(debugFileListener);
                Trace.Close();
#endif

                return;
            }

            Trace.WriteLine("...Galaxy Generation Complete !");

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          Encoding = Encoding.UTF8,
                                                          Indent = true,
                                                          IndentChars = "  ",
                                                          NewLineChars = "\r\n",
                                                          NewLineHandling = NewLineHandling.Replace,
                                                          OmitXmlDeclaration = true,
                                                      };

            using (XmlWriter writer = XmlTextWriter.Create(pathToOutputFile, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("GenerationOutput");

                writer.WriteStartElement("GeneratorVersion");
                writer.WriteAttributeString("Revision", "109");
                writer.WriteAttributeString("Date", "20120828");
                writer.WriteEndElement();

                configuration.WriteOuterXml(writer);

                Galaxy.Instance.WriteXml(writer);

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }

            Galaxy.Release();
        }
        finally
        {
			new ESInjector.Injector ();
#if TRACE
            stopwatch.Stop();

            Trace.WriteLine("\ngeneration time : " + stopwatch.ElapsedMilliseconds);


            Trace.Flush();

            debugFileListener.Flush();
            debugFileListener.Close();

            Trace.Listeners.Remove(debugFileListener);
            Trace.Close();
#endif
        }
    }
}

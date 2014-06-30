using System;
using System.IO;

namespace ESAITournamentInstaller
{
	class MainClass
	{
		static string public_xp1Path() {
			//todo: port this to other platforms
			return homeFolder() + @"/Library/Application Support/Steam/SteamApps/common/Endless Space/Endless Space.app/Contents/Public_xp1";
		}
		static string modPath() {
			//todo: port this to other platforms
			return homeFolder() + @"/Library/Application Support/Endless Space/Disharmony/Modding" + "/ESAITournament";
		}

		/*This function hasn't been tested on any platforms other than OSX */
		static string homeFolder() {
			return (Environment.OSVersion.Platform == PlatformID.Unix ||
			                  Environment.OSVersion.Platform == PlatformID.MacOSX)
				? Environment.GetEnvironmentVariable ("HOME")
				: Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");
		}
		static void CopyFolder( string sourceFolder, string destFolder ) {
			if (!Directory.Exists( destFolder ))
				Directory.CreateDirectory( destFolder );
			string[] files = Directory.GetFiles( sourceFolder );
			foreach (string file in files)
			{
				string name = Path.GetFileName( file );
				string dest = Path.Combine( destFolder, name );
				File.Copy( file, dest );
			}
			string[] folders = Directory.GetDirectories( sourceFolder );
			foreach (string folder in folders)
			{
				string name = Path.GetFileName( folder );
				string dest = Path.Combine( destFolder, name );
				CopyFolder( folder, dest );
			}
		}

		public static void Main (string[] args)
		{
			Console.WriteLine ("Welcome to ESAITournamentInstaller.");
			Console.Write ("Creating default mod...");
			string mp = modPath ();
			DirectoryInfo d = new DirectoryInfo (mp);
			if (d.Exists) {
				d.Delete (true);
			}
			CopyFolder (public_xp1Path (), mp);
			Console.WriteLine ("Done.");

			Console.Write ("Merging with ESAITournament...");

		}
	}
}

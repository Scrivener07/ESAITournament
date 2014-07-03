using Microsoft.Win32;
using System;
using System.IO;


namespace ESStaticInjector
{
	public static class OsxPath
	{
		public struct GameDirectory
		{
			/// <summary>May not be root on Osx?</summary>
			public static string Root { get { return GetGamePath(); } }
			public static string Managed { get { return GetGamePath() + @"/Contents/Data/Managed"; } }
		}

		public static string GetGamePath()
		{
			return HomePath() + @"/Library/Application Support/Steam/SteamApps/common/Endless Space/Endless Space.app/Contents";
		}	

		public static string HomePath()
		{
			return (Environment.OSVersion.Platform == PlatformID.Unix ||
			Environment.OSVersion.Platform == PlatformID.MacOSX)
				? Environment.GetEnvironmentVariable("HOME")
					: Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
		}	
	}


	public static class WinPath
	{
		public struct GameDirectory
		{
			public static string Root { get { return GetGamePath(); } }
			public static string Public { get { return GetGamePath() + "\\Public"; } }
			public static string PublicXp { get { return GetGamePath() + "\\Public_xp1"; } }
			public static string Managed { get { return GetGamePath() + "\\EndlessSpace_Data\\Managed"; } }
		}

		public struct ModDirectory
		{
			public static string Root { get { return GetModdingPath(); } }
			public static string Mods { get { return GetModdingPath() + "\\Modding"; } }
			public static string ModsXp { get { return GetModdingPath() + "\\Disharmony\\Modding"; } }
		}


		public static string GetGamePath()
		{
			string s = GetSteamPath();

			if (Directory.Exists(s))
			{
				string assembly = "\\EndlessSpace.exe";
				string es = s + "\\SteamApps\\common\\Endless Space";
				if (File.Exists(es + assembly))
				{
					return es;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}


		public static string GetModdingPath()
		{
			string doc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string es = doc + "\\Endless Space";
			if (Directory.Exists(es))
			{
				return es;
			}
			return null;
		}


		private static string GetSteamPath()
		{
			RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser;
			regKey = regKey.OpenSubKey("Software\\Valve\\Steam");
			if (regKey != null)
			{
				string steam = regKey.GetValue("SteamPath").ToString();
				if (Directory.Exists(steam))
				{
					return Path.GetFullPath(steam);
				}
			}
			return null;
		}
	}

}

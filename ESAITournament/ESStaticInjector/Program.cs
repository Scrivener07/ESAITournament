using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace ESStaticInjector
{
	class MainClass
	{
		static string homePath() {
			return (Environment.OSVersion.Platform == PlatformID.Unix ||
			Environment.OSVersion.Platform == PlatformID.MacOSX)
				? Environment.GetEnvironmentVariable ("HOME")
					: Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%");
		}
		public static void Main (string[] args)
		{
			var resolver = new DefaultAssemblyResolver();
			var path = homePath() + @"/Library/Application Support/Steam/SteamApps/common/Endless Space/Endless Space.app/Contents/Data/Managed/";
			resolver.AddSearchDirectory(path);

			var parameters = new ReaderParameters
			{
				AssemblyResolver = resolver,
			};
			AssemblyDefinition myLibrary =AssemblyDefinition.ReadAssembly(Environment.CurrentDirectory+"/../../Assembly-CSharp.dll",parameters);


			var trampolineType = typeof(AITrampoline.AITrampoline);


			//Gets all types which are declared in the Main Module of "MyLibrary.dll"
			foreach (TypeDefinition type in myLibrary.MainModule.Types) {
				//Writes the full name of a type
				//Console.WriteLine (type.FullName);
				if (type.FullName == "AIPlayerController") {
					foreach (MethodDefinition method in type.Methods) {
						if (method.Name == "Bind") {
							method.IsPublic = true;
						}
					}
				}
				if (type.FullName == "ApplicationState_Lobby") {
					foreach (MethodDefinition method in type.Methods) {
						//Console.WriteLine (method.Name);
						if (method.Name == "Session_PlayerSlotOpening") {
							var processor = method.Body.GetILProcessor ();
							Console.WriteLine (typeof(MainClass).GetMethods ());

							var newType = method.Module.Import (trampolineType);
							foreach (Instruction i in method.Body.Instructions) {
								if (i.OpCode == OpCodes.Ldtoken) {
									TypeDefinition td = (TypeDefinition) i.Operand;
									if (td.Name == "AIPlayerController") {
										processor.Replace (i, processor.Create (OpCodes.Ldtoken, newType));
										Console.WriteLine ("Patch successful.");
										break;
									}

								}

							}
							Console.WriteLine ("Inject success!");
						}
					}
				}
			}
			myLibrary.Write (path+"Assembly-CSharp.dll");
			System.IO.File.Copy (Environment.CurrentDirectory + @"/../../../AITrampoline/bin/Debug/AITrampoline.dll", path + "AITrampoline.dll", true);
		}
	}
}

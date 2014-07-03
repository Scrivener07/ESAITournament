using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;
using System.IO;

namespace ESStaticInjector
{
	class MainClass
	{
		public static void expose(AssemblyDefinition assembly, string className, string methodName) {
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				if (type.FullName == className) {
					foreach (MethodDefinition method in type.Methods) {
						if (method.Name == methodName) {
							method.IsPublic = true;
							method.IsVirtual = true;
							return;
						}
					}
				}
			}
			throw new Exception ("Problem with finding method" + methodName);
		}

		private static string getPath()
		{
			if (Directory.Exists(OsxPath.GameDirectory.Managed))
			{
				Console.WriteLine("OSX");
				return OsxPath.GameDirectory.Managed;
			}
			if (Directory.Exists(WinPath.GameDirectory.Managed))
			{
				Console.WriteLine("Windows");
				return WinPath.GameDirectory.Managed;
			}
			else
			{
				throw (new DirectoryNotFoundException("Cannot find Endless Space managed assembly directory."));
			}
		}

		public static void Main (string[] args)
		{
			string path = getPath();
			Console.WriteLine("Path: " + path);

			var resolver = new DefaultAssemblyResolver();
			resolver.AddSearchDirectory(path);

			var parameters = new ReaderParameters
			{
				AssemblyResolver = resolver,
			};
			AssemblyDefinition myLibrary =AssemblyDefinition.ReadAssembly(Environment.CurrentDirectory+"/../../Assembly-CSharp.dll",parameters);


			var trampolineType = typeof(AITrampoline.AITrampoline);


			expose (myLibrary, "AIPlayerController", "Bind");
			expose (myLibrary, "AIPlayerController", "CreateAIEmpire");

			foreach (TypeDefinition type in myLibrary.MainModule.Types) {

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

			string injectSource = Path.Combine(path, "Assembly-CSharp.dll");
			string installInjector = Path.Combine(path, "AITrampoline.dll");
			string installPlugin = Path.Combine(path, "DrunkenWalkAI.dll");
			Console.WriteLine(Environment.NewLine + "Press any key install the following files.." +
				"\n" + injectSource +
				"\n" + installInjector +
				"\n" + installPlugin);

			myLibrary.Write(injectSource);
			System.IO.File.Copy(Environment.CurrentDirectory + @"/../../../AITrampoline/bin/Debug/AITrampoline.dll", installInjector, true);
			System.IO.File.Copy(Environment.CurrentDirectory + @"/../../../DrunkenWalkAI/bin/Debug/DrunkenWalkAI.dll", installPlugin, true);
			Console.ReadKey();

		}
	}
}

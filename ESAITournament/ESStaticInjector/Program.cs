using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace ESStaticInjector
{
	class MainClass
	{
		Object trampoline(Object e) {
			return AITrampoline.AITrampoline.NotConstructor (e);
		}
		Object nopMe(Object e) {
			return null;
		}
		public static void Main (string[] args)
		{
			var resolver = new DefaultAssemblyResolver();
			var path = @"/Users/drew/Library/Application Support/Steam/SteamApps/common/Endless Space/Endless Space.app/Contents/Data/Managed/";
			resolver.AddSearchDirectory(path);

			var parameters = new ReaderParameters
			{
				AssemblyResolver = resolver,
			};
			AssemblyDefinition myLibrary =AssemblyDefinition.ReadAssembly(@"/Users/drew/Downloads/"+"Assembly-CSharp.dll",parameters);


			var trampolineType = typeof(AITrampoline.AITrampoline);


			//Gets all types which are declared in the Main Module of "MyLibrary.dll"
			foreach (TypeDefinition type in myLibrary.MainModule.Types) {
				//Writes the full name of a type
				//Console.WriteLine (type.FullName);
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
							/*method.Body.Instructions.Clear ();
							//monodis trampoline
							processor.Append (processor.Create (OpCodes.Nop));
							processor.Append (processor.Create (OpCodes.Ldarg_1));
							processor.Append (processor.Create (OpCodes.Call, notConstructor_impl));
							processor.Append (processor.Create (OpCodes.Ret));

							//processor.Append (processor.Create (OpCodes.Ret));*/
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

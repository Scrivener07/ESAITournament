using System;

namespace AITrampoline
{
	public class AITrampoline : AIPlayerController
	{
		public AITrampoline(Player player): base(player)
		{
			throw new Exception ("Injection success!");
		}

		public static Object NotConstructor(Object e) {
			System.IO.StreamWriter fw = new System.IO.StreamWriter (Environment.CurrentDirectory + "/StaticInject.log");
			fw.WriteLine ("Got here!");
			fw.Flush ();
			fw.Close ();
			return "";
		}


	}
}


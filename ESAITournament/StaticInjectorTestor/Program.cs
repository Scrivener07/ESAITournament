using System;

namespace StaticInjectorTestor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Object e = new Empire ();
			AI j = new AI_Master ((Empire) e);

		}
	}
}

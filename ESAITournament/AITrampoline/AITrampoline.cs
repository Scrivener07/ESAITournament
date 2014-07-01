using System;
using System.Diagnostics;

namespace AITrampoline
{
	public class AITrampoline : AIPlayerController
	{
		public AITrampoline(Player player): base(player)
		{
			TextWriterTraceListener listener = new TextWriterTraceListener (Environment.CurrentDirectory + "/AITrampoline.log");
			Trace.Listeners.Add (listener);
			Trace.AutoFlush = true;
			Trace.WriteLine ("AITrampoline started up");
		}
		/**This seems to get called on both AI and Pirate players.*/
		public override void Bind (Empire empire)
		{
			Trace.WriteLine ("Bind injection on empire " + empire.Player.EmpireIndex);
			base.Bind (empire);
		}


	}
}


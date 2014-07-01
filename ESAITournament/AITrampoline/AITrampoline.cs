using System;
using System.Diagnostics;

namespace AITrampoline
{
	public class NotAIEmpire : AI_Empire {
		public NotAIEmpire(Empire e) : base(e) {
		}

	}
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
		public override AI_Empire CreateAIEmpire (Empire empire)
		{
			NotAIEmpire ai = new NotAIEmpire (empire);
			ai.Enable = true;
			return ai;
		}

		protected override void Mailbox_InboxCollectionChange (object sender, System.ComponentModel.CollectionChangeEventArgs e)
		{
			Trace.WriteLine ("Mailbox_InboxCollectionChange action " + e.Action);
			foreach(Mail m in this.Mailbox.Inbox) {
				ESDebug.debug(m);
				m.Read = true;
			}
		}
			
	}
}


using System;
using System.Diagnostics;

namespace AITrampoline
{
	public class NotAIMaster : AI_Master {
		public NotAIMaster(Empire e) : base(e) {
			this.layers = new System.Collections.Generic.List<AILayer> (); //clear any layers that are present
		}

	}

	public class NotAIEmpire: AI_Empire {
		public NotAIEmpire(Empire e) : base(e) {
			this.layers = new System.Collections.Generic.List<AILayer> (); //clear any layers that are present
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

			if (empire.EmpirePlayerType != Empire.PlayerType.Pirates) {
				base.ai = new NotAIMaster (empire);
				empire.AI = base.ai;
				empire.EmpireAI = this.CreateAIEmpire (empire);
			}

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


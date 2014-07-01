using System;
using System.Diagnostics;

namespace AITrampoline
{	public class ESAISampleAI {

	}
	public class NotAIMaster : AI_Master {

		/**It's worth talking a bit about how this works.
		 * 
		 * Basically Unity adds coroutines to C#.  This feature works at the compiler level--
		 * the coroutines are translated to an IEnumerator at compile-time.  See this doc: http://wiki.etc.cmu.edu/unity3d/index.php/Coroutines
		 * 
		 * The net result of this is you get this whacky "sealed class <strange-name>" stuff when you reverse-engineer ES, and 
		 * there seem to be functions returning IEnumerators everywhere in the API.  The way to read this
		 * is the IEnumerator class is part of the function definition.  Calling MoveNext() gives the function a chance to execute.
		 * If you return true, MoveNext() will be called again.  If you return false, it's like saying your function is done executing.
		 * 
		 * I think this happens because Unity is single-threaded, so they can't allow any one task to lock the whole UI.  Instead
		 * these tasks have to stop regularly and return control to the caller, using Unity's "yield" keyword, and then are resumed. 
		 * But that gets turned into an IEnumerator at compile.
		 * */
		private sealed class iterator : System.Collections.IEnumerator{
			internal object current = null;
			public bool MoveNext() {
				Trace.WriteLine ("NotAIMaster moveNext");
				return false;
			}
			public void Reset() {
				throw new NotSupportedException();
			}
			object System.Collections.IEnumerator.Current  {
				get {
					return current;
				}
			}

		}
		public override System.Collections.IEnumerator ProcessAsync ()
		{
			return new iterator ();
		}
		public NotAIMaster(Empire e) : base(e) {
			this.layers = new System.Collections.Generic.List<AILayer> (); //clear any layers that are present
		}

	}

	public class NotAIEmpire: AI_Empire {
		public NotAIEmpire(Empire e) : base(e) {
			this.layers = new System.Collections.Generic.List<AILayer> (); //clear any layers that are present
		}
		protected override System.Collections.IEnumerator BeginAIProcess ()
		{
			return base.BeginAIProcess ();
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


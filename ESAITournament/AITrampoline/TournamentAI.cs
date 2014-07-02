using System;

namespace AITrampoline
{
	public abstract class TournamentAI
	{	/**Will be called to inform you about your empire. */
		public abstract Empire myEmpire { set; }

		/**Called to process AI commands.  Must return in <1 second.
		 * If you return true, you will be called again as soon as possible. */
		public abstract bool Tick();
	}
}


using System;
using System.Diagnostics;

namespace DrunkenWalkAI
{
	public class DrunkenWalkAI : AITrampoline.TournamentAI
	{
		private Empire _empire;
		public override Empire myEmpire {
			set {
				_empire = value;
			}
		}
		public DrunkenWalkAI()
		{

		}
		public override bool Tick ()
		{
			foreach (Fleet f in _empire.DepartmentOfTransportation.Fleets) {
				drunkenWalk (f);
			}
			return false; // all done
		}
		void drunkenWalk(Fleet f) {
			Trace.WriteLine ("drunkenWalk on fleet " + f);
			_empire.DepartmentOfTransportation.ClearNextDestinationsInstruction(new Fleet[]{f});
			//get a destination
			System.Collections.Generic.List<StarSystem> systems =_empire.DepartmentOfTheInterior.GetVisibleStarSystems ();
			StarSystem next = systems [new Random ().Next (systems.Count)];
			Trace.WriteLine ("Sending to system " + next);
			_empire.DepartmentOfTransportation.MoveFleetInstruction (f, next, PathMethod.GameRules, null, true, false);
		}


	}
}


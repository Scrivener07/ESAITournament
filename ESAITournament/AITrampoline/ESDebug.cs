using System;
using System.Diagnostics;

namespace AITrampoline
{
	public class ESDebug
	{
		public ESDebug ()
		{
		}

		public static void debug(Mail m) {
			Trace.WriteLine("Mail object from " + m.From + " to " + m.To + " priority " + m.Priority + " about " + m.Subject + " on " + m.Time + " saying " + m.Body);
		}
		public static void debug(StarSystem s) {
			if (s==null) {
				Trace.WriteLine ("Null starsystem");
				return;
			}
			Trace.WriteLine ("Starsystem object " + s.LocalizedName);
		}

		public static void Report(Exception ex) {
			Trace.WriteLine ("AITrampoline caught an error." + ex);
			throw ex;
		}
	}
}


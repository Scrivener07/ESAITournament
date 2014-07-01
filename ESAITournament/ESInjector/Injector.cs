using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
namespace ESInjector
{
	public class Arbitrary {
	}
	public class Injector
	{
		public Arbitrary newAI(Object e) {
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(Environment.CurrentDirectory + "/ESAITournamentDebug.log", true))
			{
				file.WriteLine("Inject success");
			}
			return new Arbitrary ();
		}
		public Injector ()
		{
			TextWriterTraceListener debugFileListener = new TextWriterTraceListener(Environment.CurrentDirectory + "/ESAITournamentInjector.log");
			Trace.Listeners.Add(debugFileListener);
			Trace.AutoFlush = true;
			Trace.WriteLine ("INJECTOR STARTUP");
			Trace.WriteLine ("Getting new bytecode");
			byte[] il = typeof(Injector).GetMethod("newAI").GetMethodBody().GetILAsByteArray();
			Trace.WriteLine ("Pin the bytes in GC");
			GCHandle h = GCHandle.Alloc((object)il, GCHandleType.Pinned);
			IntPtr addr = h.AddrOfPinnedObject();
			int size = il.Length;

			// Swap the method.
			Trace.WriteLine ("Method swap");
			try {
				MethodRental.SwapMethodBody(typeof(AI), typeof(AI).GetConstructors()[0].MetadataToken, addr, size, MethodRental.JitImmediate);
			}
			catch (Exception ex) {
				Trace.WriteLine ("Exception occurred");
				Trace.WriteLine (ex);
			}
		}
	}
}


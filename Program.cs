using System;
using System.Diagnostics;

namespace TestApp
{
	class MainClass
	{
		public static string TestProp
		{
			get
			{
				var sw = Stopwatch.StartNew();
				while (sw.ElapsedMilliseconds < 5000) {
				}
				return "5 seconds passed";
			}
		}

		public static void Main(string[] args)
		{
			Debugger.Break();
		}
	}
}


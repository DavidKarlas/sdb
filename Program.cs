using System;

static class Program
{
	internal class HelloWorld : MarshalByRefObject
	{
		object bla = new object ();
	}

	static void Main ()
	{
		AppDomain domain = AppDomain.CreateDomain ("NewAppDomain");

		var proxy = domain.CreateInstanceAndUnwrap (typeof(HelloWorld).Assembly.FullName, typeof(HelloWorld).FullName);
		System.Diagnostics.Debugger.Break ();
	}
}


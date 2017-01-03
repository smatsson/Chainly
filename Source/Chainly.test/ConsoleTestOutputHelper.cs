using System;
using Xunit.Abstractions;

namespace Chainly.Test
{
	/// <summary>
	/// The xunit test runner for does not display outputs unless the test fails.
	/// This is a simple work around until it has been fixed. 
	/// </summary>
    public class ConsoleTestOutputHelper : ITestOutputHelper
	{
		public void WriteLine(string message)
		{
			Console.WriteLine(message);
		}

		public void WriteLine(string format, params object[] args)
		{
			Console.WriteLine(string.Format(format, args));
		}
	}
}

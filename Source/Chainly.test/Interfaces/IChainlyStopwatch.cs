using System.Diagnostics;

namespace Chainly.Test.Interfaces
{
	public interface IChainlyStopwatch
	{
		IChainlyStopwatch Stop();
		Stopwatch Value();
	}
}

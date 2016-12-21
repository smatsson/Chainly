using Chainly.Test.Models;

namespace Chainly.Test.Interfaces
{
	public interface IChainlyAsdf
	{
		IChainlyAsdf SomeMethod();
		IChainlyAsdf SomeOtherMethod();
		IChainlyAsdf ParameterMethod(string value);
		IChainlyAsdf ParameterMethod(string value, int value2);
		Asdf Value();
	}
}

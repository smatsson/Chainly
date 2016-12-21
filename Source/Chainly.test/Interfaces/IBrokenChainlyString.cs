namespace Chainly.Test.Interfaces
{
	public interface IBrokenChainlyString
	{
		IBrokenChainlyString MethodThatDoesNotExist();
		string Value();
	}
}

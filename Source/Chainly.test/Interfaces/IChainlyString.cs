namespace Chainly.Test.Interfaces
{
	public interface IChainlyString
	{
		IChainlyString CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);
		string Value();
	}
}

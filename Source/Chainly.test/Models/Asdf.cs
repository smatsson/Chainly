namespace Chainly.Test.Models
{
	public class Asdf
	{
		private readonly string _myString;

		public int SomeMethodCount { get; set; }
		public int SomeOtherMethodCount { get; set; }

		public int ParameterMethodWithOneParameterCount { get; set; }

		public int ParameterMethodWithTwoParametersCount { get; set; }

		public Asdf(string myString)
		{
			_myString = myString;
		}

		public void SomeMethod()
		{
			SomeMethodCount++;
		}

		public void SomeOtherMethod()
		{
			SomeOtherMethodCount++;
		}

		public void ParameterMethod(string value)
		{
			ParameterMethodWithOneParameterCount++;
		}

		public void ParameterMethod(string value, int otherValue)
		{
			ParameterMethodWithTwoParametersCount++;
		}

		public string GetMyString()
		{
			return _myString;
		}
	}
}

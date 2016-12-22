using Chainly.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Chainly.Test.Tests
{
	public class ChainlyChainTests
	{
		[Fact]
		public void CreateNewChain()
		{
			Asdf model = new Asdf("a");

			var chain = model.CreateChain();

			Assert.Equal(model, chain.Result);
		}

		[Fact]
		public void Then_CanChain()
		{
			Asdf model = new Asdf("a");

			var chain = model.CreateChain()
				.Then(m => m.ParameterMethod("b"))
				.Then(m => m.ParameterMethod("c"))
				.Then(m => m.ParameterMethod("d", 1))
				.Then(m => m.GetMyString());

			Assert.Equal(2, model.ParameterMethodWithOneParameterCount);
			Assert.Equal(1, model.ParameterMethodWithTwoParametersCount);
		}
	}
}

using Chainly.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Chainly.Test.Tests
{
	public class ChainlyChainTests
	{
		[Fact]
		public void CreateNewChain()
		{
			Asdf model = new Asdf("a");

			var chain = model.Chain();

			chain.Value().ShouldBe(model);
		}

		[Fact]
		public void Do_CanChain()
		{
			Asdf model = new Asdf("a");

			model.Chain()
				.Do(m => m.ParameterMethod("b"))
				.Do(m => m.ParameterMethod("c"))
				.Do(m => m.ParameterMethod("d", 1))
				.Do(m => m.GetMyString());

			model.ParameterMethodWithOneParameterCount.ShouldBe(2);
			model.ParameterMethodWithTwoParametersCount.ShouldBe(1);
		}

		[Fact]
		public void Operator_CanChain()
		{
			Asdf model = new Asdf("a");

			var chain = model.Chain() + (m => m.ParameterMethod("b")) + (m => m.ParameterMethod("c")) +
			            (m => m.ParameterMethod("d", 1)) + (m => m.GetMyString());

			model.ParameterMethodWithOneParameterCount.ShouldBe(2);
			model.ParameterMethodWithTwoParametersCount.ShouldBe(1);
		}
	}
}

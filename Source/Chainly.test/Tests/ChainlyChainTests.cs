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

			var chain = model.CreateChain();

			chain.Value().ShouldBe(model);
		}

		[Fact]
		public void Then_CanChain()
		{
			Asdf model = new Asdf("a");

			model.CreateChain()
				.Then(m => m.ParameterMethod("b"))
				.Then(m => m.ParameterMethod("c"))
				.Then(m => m.ParameterMethod("d", 1))
				.Then(m => m.GetMyString());

			model.ParameterMethodWithOneParameterCount.ShouldBe(2);
			model.ParameterMethodWithTwoParametersCount.ShouldBe(1);
		}

		[Fact]
		public void Operator_CanChain()
		{
			Asdf model = new Asdf("a");

			var chain = model.CreateChain() + (m => m.ParameterMethod("b")) + (m => m.ParameterMethod("c")) +
			            (m => m.ParameterMethod("d", 1)) + (m => m.GetMyString());

			model.ParameterMethodWithOneParameterCount.ShouldBe(2);
			model.ParameterMethodWithTwoParametersCount.ShouldBe(1);
		}
	}
}

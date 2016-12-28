﻿using System.Collections.Generic;
using System.Diagnostics;
using Chainly.Test.Interfaces;
using Chainly.Test.Models;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Chainly.Test.Tests
{
	public class PerformanceTests
	{
		private readonly ITestOutputHelper _output;
		private readonly List<string> _strings;
		private const int Iterations = 1_000_000;

		public PerformanceTests(ITestOutputHelper output)
		{
			_output = output;
			_strings = new List<string>();
		}

		[Fact]
		public void Non_Chained_Method_Calls()
		{
			var watch = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{
				var asdf = new Asdf("Hello world!");
				asdf.SomeMethod();
				asdf.SomeOtherMethod();
				asdf.ParameterMethod("A");
				asdf.ParameterMethod("B", 2);
				_strings.Add(asdf.GetMyString());
			}
			watch.Stop();
			_output.WriteLine($"Time to run {Iterations} iterations: {watch.Elapsed} ms");
			_strings.Count.ShouldBe(Iterations);
		}

		[Fact]
		public void EmitBased_Chain_Method_Calls()
		{
			var watch = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{
				_strings.Add(new Asdf("Hello world!")
					.Chain<IChainlyAsdf>()
					.SomeMethod()
					.SomeOtherMethod()
					.ParameterMethod("A")
					.ParameterMethod("B", 2)
					.Value()
					.GetMyString());
			}
			watch.Stop();
			_output.WriteLine($"Time to run {Iterations} iterations: {watch.Elapsed} ms");
			_strings.Count.ShouldBe(Iterations);
		}

		[Fact]
		public void ActionBased_Chain_Method_Calls()
		{
			var watch = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{
				_strings.Add(new Asdf("Hello world!")
					.Chain()
					.Do(m => m.SomeMethod())
					.Do(m => m.SomeOtherMethod())
					.Do(m => m.ParameterMethod("A"))
					.Do(m => m.ParameterMethod("B", 2))
					.Value()
					.GetMyString());
			}
			watch.Stop();
			_output.WriteLine($"Time to run {Iterations} iterations: {watch.Elapsed} ms");
			_strings.Count.ShouldBe(Iterations);
		}

		[Fact]
		public void ActionBased_Operator_Overload_Method_Calls()
		{
			var watch = Stopwatch.StartNew();
			for (var i = 0; i < Iterations; i++)
			{
				_strings.Add((new Asdf("Hello world!")
					              .Chain()
				              + (m => m.SomeMethod())
				              + (m => m.SomeOtherMethod())
				              + (m => m.ParameterMethod("A"))
				              + (m => m.ParameterMethod("B", 2)))
					.Value()
					.GetMyString());
			}
			watch.Stop();
			_output.WriteLine($"Time to run {Iterations} iterations: {watch.Elapsed} ms");
			_strings.Count.ShouldBe(Iterations);
		}
	}
}

using System;
using System.Linq;
using Chainly.Test.Interfaces;
using Chainly.Test.Models;
using Shouldly;
using Xunit;

namespace Chainly.Test.Tests
{
	public class ChainlyTests
	{

		[Fact]
		public void Can_Chain_Class()
		{
			var obj = new Asdf("Hello world!");
			var myString = obj
				.Chain<IChainlyAsdf>()
				.SomeMethod()
				.SomeMethod()
				.ParameterMethod("Hi!")
				.SomeOtherMethod()
				.ParameterMethod("Hi again!", 1)
				.ParameterMethod("Hi again!", 2)
				.Value()
				.GetMyString();

			obj.SomeMethodCount.ShouldBe(2);
			obj.SomeOtherMethodCount.ShouldBe(1);
			obj.ParameterMethodWithOneParameterCount.ShouldBe(1);
			obj.ParameterMethodWithTwoParametersCount.ShouldBe(2);
			myString.ShouldBe("Hello world!");
		}

		[Fact]
		public void Can_Chain_BuildIn_Class()
		{
			var buffer = new char[2];
			var upperString = "Hello world!".Chain<IChainlyString>().CopyTo(0, buffer, 0, 2).Value().ToUpper();

			buffer.ShouldBe(new[] { 'H', 'e'});
			upperString.ShouldBe("HELLO WORLD!");
		}

		[Fact]
		public void Ignores_Void_Methods_In_Source_That_Are_Not_In_Interface()
		{
			var sut = new Asdf("Hello!").Chain<ISlimChainlyAsdf>();

			var methods = sut.GetType().GetMethods();

			Action<string, Type> assertMethod = (name, returnType) =>
			{
				methods.Any(f => f.Name == name && f.ReturnType == returnType).ShouldNotBeNull();
			};

			// Verify methods on the created type. ToString, Equals, GetHashCode and GetType are inherited from System.Object.
			methods.Length.ShouldBe(6);
			assertMethod("SomeMethod", typeof(ISlimChainlyAsdf));
			assertMethod("Value", typeof(Asdf));
			assertMethod("ToString", typeof(string));
			assertMethod("Equals", typeof(bool));
			assertMethod("GetHashCode", typeof(int));
			assertMethod("GetType", typeof(Type));

			// Verify that the methods work as expected.
			var unchained = sut.SomeMethod().Value();
			unchained.SomeMethodCount.ShouldBe(1);
			unchained.SomeOtherMethodCount.ShouldBe(0);
			unchained.ParameterMethodWithOneParameterCount.ShouldBe(0);
			unchained.ParameterMethodWithTwoParametersCount.ShouldBe(0);

			unchained.ParameterMethod("A", 1);
			unchained.ParameterMethodWithTwoParametersCount.ShouldBe(1);
		}

		[Fact]
		public void Value_Method_If_Optional()
		{
			var asdf = new Asdf("Hi!");
			var sut = asdf.Chain<INoValueMethodAsdf>().SomeMethod().SomeMethod();
			
			asdf.SomeMethodCount.ShouldBe(2);
			sut.GetType().GetMethods().All(f => f.Name != "Value" && f.ReturnType != typeof(Asdf)).ShouldBeTrue();
		}

		[Fact]
		public void Throws_Exception_If_Source_Does_Not_Contain_Interface_Override()
		{
			const string exceptionMessage = "Found no method to override with the same arguments for method \"MethodThatDoesNotExist\"";

			var exception = Should.Throw<ChainlyException>(() =>
			{
				"Hello world!".Chain<IBrokenChainlyString>().MethodThatDoesNotExist();
			});

			exception.Message.ShouldBe(exceptionMessage);

			exception = Should.Throw<ChainlyException>(() =>
			{
				"Hello world!".Chain<IBrokenChainlyString>().Value();
			});

			exception.Message.ShouldBe(exceptionMessage);
		}

		[Fact]
		public void Interface_Must_Be_Public()
		{
			var exception = Should.Throw<ChainlyException>(() =>
			{
				new Asdf("Hi!").Chain<IInternalInterface>().SomeMethod();
			});

			exception.Message.ShouldBe($"Interface \"{typeof(IInternalInterface).FullName}\" must be public");
		}

		[Fact]
		public void Object_Must_Be_Public()
		{
			var exception = Should.Throw<ChainlyException>(() =>
			{
				new InternalModel().Chain<IChainlyInternalModel>().SomeMethod().SomeMethod().SomeMethod();
			});

			exception.Message.ShouldBe($"Object \"{typeof(InternalModel).FullName}\" must be public");
		}
	}
}

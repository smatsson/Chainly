# Chainly
Make any .NET object a fluent interface regardless if you have the source code or not!

[![NuGet](https://img.shields.io/nuget/v/Chainly.svg)](https://www.nuget.org/packages/Chainly)

``PM> Install-Package Chainly``

# Reflection.Emit method

## Example

Given a class `Asdf` we define an interface `IAsdfChain`. We then run the `Chain` extension on the `Asdf` instance and voila! Fluent interface! :) 

```csharp
var myString = new Asdf("This is my string!")
				.Chain<IAsdfChain>()
				.SomeMethod()
				.ParameterMethod("A", 123)
				.Value() // Call .Value() to get the unchained Asdf instance
				.GetMyString();
// myString == "This is my string!"

public class Asdf
{
	private readonly string _myString;

	public int SomeMethodCount { get; set; }
	public int ParameterMethodWithTwoParametersCount { get; set; }

	public Asdf(string myString)
	{
		_myString = myString;
	}

	public void SomeMethod()
	{
		SomeMethodCount++;
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

// Only methods defined in this interface will be made fluent. Other methods will be left alone.
public interface IAsdfChain
{
	IAsdfChain SomeMethod();
	IAsdfChain ParameterMethod(string value, int value2);
	// Value is a special method, returning the original unchained object.
	Asdf Value();
}

```

Of course it also works with built in classes! 

```csharp
var buffer = new char[2];
var asdfUpper = "asdf".Chain<IChainString>().CopyTo(0, buffer, 0, 2).Value().ToUpper();
// buffer == ['a', 's']
// asdfUpper = ASDF

var elapsed = StopWatch.StartNew().Chain<IStopWatchChain>().Stop().Value().Elapsed;

public interface IChainString
{
	IChainString CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count);
	string Value();
}

public interface IStopWatchChain
{
	IStopWatchChain Stop();
	Stopwatch Value();
}

```

## What are the steps to make an object fluent?
* Create an interface. You can call it anything you'd like but it must be public.
* Add each void method you want to make fluent to the interface and use the interface as return type (not all void methods are required, only the ones you wish to make fluent).
* Add a Value method with the object type as return value.
* Run Chain<{interfacename}> on the object to make it fluent.

### Example

```csharp
public interface IMyInterface
{
	IMyInterface SomeVoidMethod(string param1, int param2);
	object Value();
}

objectWithSomeVoidMethod = objectWithSomeVoidMethod
							.Chain<IMyInterface>
							.SomeVoidMethod("A", 1)
							.SomeVoidMethod("B", 2)
							.Value();

```

# Action based method

## Summary
The action based method allows chaining for any type without the need for an interface.

## Example
```csharp
var model = new Asdf("a");

var myString = model.Chain()
	.Do(m => m.ParameterMethod("b"))
	.Do(m => m.ParameterMethod("c"))
	.Do(m => m.ParameterMethod("d", 1))
	.Value()
	.GetMyString();
```

## Overloaded Operator Example
It's also possible to use `+` instead of `.Do()`.

```csharp
var model = new Asdf("a");

var myString = (model.Chain()
				+ (m => m.ParameterMethod("b"))
				+ (m => m.ParameterMethod("c"))
				+ (m => m.ParameterMethod("d", 1)))
	.Value()
	.GetMyString();
```

## Project structure
Chainly.Dotnet.sln - .NET  core project supporting multiple frameworks
Chainly.net452.sln - Old .NET 4.5.2 solution kept for compatibility

## License
This project is licensed under the MIT license, see [LICENSE](LICENSE).

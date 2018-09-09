# CheckedExceptions.NET
Provides an opt-in solution for exceptions management in C# code bases

[![Build status](https://ci.appveyor.com/api/projects/status/op7aj1cboe4a0mcb?svg=true)](https://ci.appveyor.com/project/ScifaTech/checkedexceptions)


## Purpose
Early in the development of C# the design team chose not to include checked exceptions in the language.
This was decided because they could not find a solution they felt would be appropriate.

Java has for many years had checked exception but much java code ends up abusing it by appending `throws Exception` to the end of most methods. 
This library tries to provide a middle-ground where exceptions can be managed and controlled in critical assemblies whilst not 
burdening the developers of other assemblies with managing their exceptions.

## How to use
1. Install the [Scifa.CheckedExceptions](https://www.nuget.org/packages/Scifa.CheckedExceptions/) nuget package.
2. Add the `CheckExceptions` attribute to your assembly.
3. Start enjoying enhanced exception management.


## Examples
Somewhere in your assembly, you must specify the `CheckExceptions` attribute. All examples that follow assume that this attribute is set.

```C#
using Scifa.CheckedExceptions.Attributes;

[assembly: CheckExceptions]
```

### 1. Simple throw

The following code will show an error on the `throw` line.
```C#
public void Leaks_NotImplementedException()
{
	throw new NotImplementedException();
}
```

to fix this error, add a `Throws` attribute:

```C#
[Throws(typeof(NotImplementedException))]
public void Leaks_NotImplementedException()
{
	throw new NotImplementedException();
}
```

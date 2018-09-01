# CheckedExceptions.NET
Provides an opt-in solution for exceptions management in C# code bases

## Purpose
Early in the development of C# the design team chose not to include checked exceptions in the language.
This was decided because they could not find a solution they felt would be appropriate.

Java has for many years had checked exception but much java code ends up abusing it with code such as `throws Exception`. 
This library tries to provide a middle-ground where exceptions can be managed and controlled in critical assemblies whilst not 
burdening the developers of other assemblies with managing their exceptions.

## How to use
1. Install the nuget package [TBC]
2. Roslyn takes care of the rest.


## Examples

The following code will show an error on the `throw` line.
```
public void Leaks_NotImplementedException()
{
	throw new NotImplementedException();
}
```

to fix this error, add a `Throws` attribute:

```
[Throws(typeof(NotImplementedException))]
public void Leaks_NotImplementedException()
{
	throw new NotImplementedException();
}
```

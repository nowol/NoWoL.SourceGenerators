
# NoWoL.SourceGenerators

NoWoL.SourceGenerators will contain C# (.NET 6/VS2022) source generators used to improve a developer's life. So far it includes:

* a way to generate the boilerplate for an exception: `ExceptionGenerator`
* a way to create a property that is always initialized using its defeault constructor: `AlwaysInitializedPropertyGenerator`
* an simple async to sync code generator: `AsyncToSyncConverterGenerator`

## Installation

Use your favorite Nuget package manager to add the `NoWoL.SourceGenerators` package to your project. Once installed you need to edit the reference to the package in the csproj file to add these attributes:

* PrivateAssets="all"
  * This attribute means that projects referencing this one won't get a reference to the package
* ExcludeAssets="runtime"
  * This attribute means that the package's DLL is not copied to your build output 

  You should end up with something similar to this:

```xml
<ItemGroup>
    <PackageReference Include="NoWoL.SourceGenerators" Version="0.1.16"
                      PrivateAssets="all" ExcludeAssets="runtime" />
</ItemGroup>
```

## ExceptionGenerator Usage

Define a partial class decorated with the `[ExceptionGenerator]` attribute to generate the boilerplate code of the exception. The exception always inherits from `System.Exception`. While it would be possible to support inheriting from another exception type, it would be too problematic to programmatically call the correct constructor of the base class.

It is possible to define a default message for the exception and doing so will generate a `Create` helper method to create an instance of the exception using the message. It is also possible to define parameters values in the message or parameter formatter. A parameter formatter is simply a static method that will be called using the parameter value and will returned a processed value, usually a string, that will be included in the message.  A type of parameter formatter could be a function that takes an `IEnumerable` value and formats it as a comma separated list.

```csharp
[ExceptionGenerator]
public partial class SampleException { }

[ExceptionGenerator("This is the exception's message")]
public partial class SampleWithMessageException { }

[ExceptionGenerator("Hello my name is {string theParameterName}")]
public partial class SampleWithMessageAndParameterException { }

// The parameter formatter is defined between angle brackets <>.
[ExceptionGenerator("This is a message with a custom parameter formatter {<SomeNameSpace.SomeClass.SomeStaticMethod>string theParameterName}")]
public partial class SampleWithCustomFormatterException { }

...

// Different ways to throw the exceptions
public void Main()
{
    throw new SampleException();

    throw SampleWithMessageException.Create(); // throw using the default message
    throw new SampleWithMessageException("Another message"); // you're not limited to the default message, you can redefine it at runtime

    throw SampleWithMessageAndParameterException.Create("Jeff"); // This will generate the message 'Hello my name is Jeff'

    throw SampleWithCustomFormatterException.Create("param value"); // This is similar to the previous line however the value will be modified by the formatter before being included in the message
}
```

## AlwaysInitializedProperty Usage

Define a partial class with one or more fields decorated with the `[AlwaysInitializedProperty]` attribute to generate the boilerplate code to create a property which initialize its backing field when the field is null. This ensure that the property will never return null.

```csharp
public partial class TestClass
{
    [AlwaysInitializedProperty]
    private List<int> _field1;
}
```

This will generate this code:

```csharp
public partial class TestClass
{
    public List<int> Field1
    {
        get
        {
            if (_field1 == default)
            {
                _field1 = new List<int>();
            }

            return _field1;
        }
        set { _field1 = value; }
    }
}
```

## AsyncToSyncConverterGenerator Usage

Experimental generator uses simple rules to convert async code to their sync version:

* Caching of the generated code, used by Visual Studio, can be a bit wonky.
* The name of the method must end with `Async`. The generator does not validate if a non-async version of the method exists.
* The generated method will have the same access modifiers as the original method.
* Awaitable local functions must end with `Async`. The generator does not validate if a non-async version of the location function exists.
* Identifiers (e.g.: variable name) ending with `Async` will have `Async` removed from their name.
* Assumes that a `Func<>` returning a `Task` will be awaited. In this case the `Func<>` will be converted to an `Action<>` which will not be awaited.
* ConfigureAwait / ConfigureAwaitWithCulture will be removed.
* `Task.Delay` is converted to `Thread.Sleep`.
* Type conversion:
  * `Task` is converted to `void`
  * `Task<T>` is converted to `T`
  * `ValueTask` is converted to `void`
  * `ValueTask<T>` is converted to `T`
  * `Func<Task>` is converted to `Action`
  * `Func<int, Task>` is converted to `Action<int>`
  * `Func<int, Task<double>>` is converted to `Func<int, double>`
* Async streams will be converted to sync by dropping their `await` keyword and removing `Async` from the name of their method.
* Attributes and XML documentation will be copied to the new method.
  * The cancellation token parameter should be named `cancellationToken` otherwise it will not be removed from XML documentation.

Not yet supported:

* Creating a `Task` variable and returning it.

To use it you need to define an async method decorated with the `[AsyncToSyncConverter]` attribute to generate a sync version of the method. The method must be inside a partial class otherwise it is not possible to add new code to the class.

For example, the following code

```cs
public partial class TestClass
{
    [AsyncToSyncConverter]
    public async Task MainMethodAsync(CancellationToken ct)
    {
        await TheMethodAsync();
        
        await TheMethodAsync().ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromSeconds(30), ct);
    }

    public async Task TheMethodAsync()
    {
        // omitted
    }

    public void TheMethod()
    {
        // omitted
    }
}
```

will be converted to

```cs
public partial class TestClass
{
    public void MainMethod(CancellationToken ct)
    {
        TheMethod();
        
        TheMethod();

        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
    }
}
```

It is also possible to use this generator to generate the sync methods of an interface.

For example, the following code

```cs
public partial interface IMyInterface
{
    [AsyncToSyncConverter]
    Task<int> MainMethodAsync(int someValue);
}
```

will be converted to

```cs
public partial interface IMyInterface
{
    int MainMethod(int someValue);
}
```

## Bug Reports

Please include the smallest code possible to reproduce the issue.


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)

## References

Useful information for creating source generators:

* Presentation by Andrey Dyatlov: https://youtu.be/052xutD86uI
* List of existing source generators: https://github.com/amis92/csharp-source-generators
* https://www.thinktecture.com/en/net/roslyn-source-generators-performance/
* https://andrewlock.net/creating-a-source-generator-part-1-creating-an-incremental-source-generator/
* https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/
* https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md
* Great collection of analyzers to help figure out how syntax trees work: https://github.com/JosefPihrt/Roslynator
* Tool to see a syntax tree: https://sharplab.io
* Tool to generate Roslyn API calls: https://roslynquoter.azurewebsites.net/

# NoWoL.SourceGenerators

NoWoL.SourceGenerators will contain C# source generators used to improve a developer's life. So far it only include a way to generate the boilerplate for an exception.

## Installation

Use your favorite Nuget package manager to add the `NoWoL.SourceGenerators` package to your project.

## Usage

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

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
<!--[![Build status](https://dev.azure.com/palmund/Typesafe.Snapshots/_apis/build/status/Typesafe.Snapshots)](https://dev.azure.com/palmund/Typesafe.Snapshots/_build/latest?definitionId=11)-->
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![NuGet version](https://badge.fury.io/nu/TinyCsvParser.Enums.svg)](https://www.nuget.org/packages/TinyCsvParser.Enums)

[MIT License]: https://opensource.org/licenses/MIT

# TinyCsvParser.Enums

Adding support for parsing enums.

Supports **.NET Core** (.NET Standard 2+)

## Installation

```
PM> Install-Package TinyCsvParser.Enums
```

## Usage

The only thing you need to keep in mind when using this extension
is that your mapping class must have a constructor taking in an instance of `ITypeConverterProvider`
and passing it on to its base constructor. See example below.

```csharp
// Entity
public enum TestEnum { A, B, C }

public class Data
{
    public TestEnum Value { get; set; }
}

// Mapping
private class CsvDataMapping : CsvMapping<Data>
{
    // Need to take in ITypeConverterProvider
    public CsvDataMapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
    {
        MapProperty(0, x => x.Value);
    }
}

// Parsing
var options = new CsvParserOptions(skipHeader: false, fieldsSeparator: ',');
var typeConverterProvider = new TypeConverterProvider().AddEnums(); // <-- This line
var parser = new CsvParser<Data>(options, new CsvDataMapping(typeConverterProvider));
var readerOptions = new CsvReaderOptions(new[] { ";" });
var result = parser.ReadFromString(readerOptions, $"A").ToList();

Console.WriteLine(result[0].Result.Value.ToString()); // Prints A
```

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.

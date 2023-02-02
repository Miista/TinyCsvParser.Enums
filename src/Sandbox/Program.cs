using System;
using System.Linq;
using TinyCsvParser;
using TinyCsvParser.Enums;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace Sandbox
{
    public class Program
    {
        public enum Ee
        {
            A,
            B,
            C
        }

        public class TestData
        {
            public Ee Ee { get; set; }

            public class Mapping : CsvMapping<TestData>
            {
                public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                {
                    MapProperty(0, x => x.Ee);
                }
            }

            public override string ToString() => Ee.ToString();
        }

        public static void Main(string[] args)
        {
            var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
            var typeConverterProvider = new TypeConverterProvider().AddEnums();
            var parser = new CsvParser<TestData>(options, new TestData.Mapping(typeConverterProvider));
            var readerOptions = new CsvReaderOptions(new[] { ";" });
            var results = parser.ReadFromString(readerOptions, $"Value1;B").ToList();

            Console.WriteLine($"Results: {results.Count}");
            foreach (var result in results)
            {
                if (result.IsValid) Console.WriteLine(result.Result.ToString());
                else Console.WriteLine($"Result is invalid: {result.Error}");
            }
        }
    }
}
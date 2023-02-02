using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TinyCsvParser.Mapping;
using TinyCsvParser.Ranges;
using TinyCsvParser.TypeConverter;
using Xunit;

namespace TinyCsvParser.Enums.Tests
{
    public class EnumParsingTests
    {
        public enum TestEnum
        {
            A = 2,
            B = 3,
            C = 1
        }

        public class NullableEnums
        {
            internal class Data
            {
                // ReSharper disable once UnusedAutoPropertyAccessor.Global
                public TestEnum? NullableEnum { get; set; }

                public class Mapping : CsvMapping<Data>
                {
                    public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                    {
                        MapProperty(0, x => x.NullableEnum);
                    }
                }
            }

            [Fact]
            public void Can_parse_nullable_enum()
            {
                // Arrange
                var csvData = "Value1,Value2;,B";
                var (parser, readerOptions) = CreateParser();

                // Act
                var results = parser.ReadFromString(readerOptions, csvData).ToList();

                // Assert
                results.Should().NotBeNullOrEmpty();

                var nullableEnum = results.First().Result.NullableEnum;

                nullableEnum.Should().BeNull(because: "that is the value passed");
            }

            private static (ICsvParser<Data> Parser, CsvReaderOptions ReaderOptions) CreateParser()
            {
                var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
                var typeConverterProvider = new TypeConverterProvider().AddEnums();
                var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
                var readerOptions = new CsvReaderOptions(new[] { ";" });

                return (Parser: parser, ReaderOptions: readerOptions);
            }
        }

        public class General
        {
            public class Data
            {
                // ReSharper disable UnusedAutoPropertyAccessor.Global
                public TestEnum TestEnum { get; set; }
                // ReSharper restore UnusedAutoPropertyAccessor.Global

                public class Mapping : CsvMapping<Data>
                {
                    public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                    {
                        MapProperty(0, x => x.TestEnum);
                    }
                }
            }

            [Theory]
            [MemberData(nameof(Can_parse_enum_by_string_value_Data))]
            public void Can_parse_enum_by_string_value<TEnum>(
                string csvData,
                Func<Data, TEnum> extractor,
                TEnum expectedEnum
            )
            {
                // Arrange
                var (parser, readerOptions) = CreateParser();

                // Act
                var results = parser.ReadFromString(readerOptions, csvData).ToList();

                // Assert
                results.Should().NotBeNullOrEmpty();

                var @enum = extractor(results.First().Result);

                @enum.Should().Be(expectedEnum, because: "that was the values passed");
            }

            // ReSharper disable once InconsistentNaming
            public static IEnumerable<object[]> Can_parse_enum_by_string_value_Data
            {
                get
                {
                    yield return TestCase(TestEnum.A, data => data.TestEnum);
                    yield return TestCase(TestEnum.B, data => data.TestEnum);
                    yield return TestCase(TestEnum.C, data => data.TestEnum);

                    object[] TestCase<TEnum>(TEnum items, Func<Data, TEnum> extractor)
                    {
                        var csvData = $"Value1;{items.ToString()}";

                        return new object[] { csvData, extractor, items };
                    }
                }
            }

            [Theory]
            [MemberData(nameof(Can_parse_enum_by_int_value_Data))]
            public void Can_parse_enum_by_int_value<TEnum>(
                string csvData,
                Func<Data, TEnum> extractor,
                TEnum expectedEnum
            )
            {
                // Arrange
                var (parser, readerOptions) = CreateParser();

                // Act
                var results = parser.ReadFromString(readerOptions, csvData).ToList();

                // Assert
                results.Should().NotBeNullOrEmpty();

                var @enum = extractor(results.First().Result);

                @enum.Should().Be(expectedEnum, because: "that was the values passed");
            }

            // ReSharper disable once InconsistentNaming
            public static IEnumerable<object[]> Can_parse_enum_by_int_value_Data
            {
                get
                {
                    yield return TestCase(TestEnum.A, data => data.TestEnum);
                    yield return TestCase(TestEnum.B, data => data.TestEnum);
                    yield return TestCase(TestEnum.C, data => data.TestEnum);

                    object[] TestCase<TEnum>(TEnum items, Func<Data, TEnum> extractor) where TEnum : Enum
                    {
                        var intValue = Convert.ToInt32(items);
                        var csvData = $"Value1;{intValue.ToString()}";

                        return new object[] { csvData, extractor, items };
                    }
                }
            }

            private static (ICsvParser<Data> Parser, CsvReaderOptions ReaderOptions) CreateParser()
            {
                var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
                var typeConverterProvider = new TypeConverterProvider().AddEnums();
                var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
                var readerOptions = new CsvReaderOptions(new[] { ";" });

                return (Parser: parser, ReaderOptions: readerOptions);
            }
        }

        public class Arrays
        {
            public class Data
            {
                // ReSharper disable UnusedAutoPropertyAccessor.Global
                public TestEnum[] TestEnums { get; set; }
                // ReSharper restore UnusedAutoPropertyAccessor.Global

                public class Mapping : CsvMapping<Data>
                {
                    public Mapping(ITypeConverterProvider typeConverterProvider) : base(typeConverterProvider)
                    {
                        MapProperty(new RangeDefinition(0, 2), x => x.TestEnums);
                    }
                }
            }

            [Theory]
            [MemberData(nameof(Can_parse_array_of_enums_Data))]
            public void Can_parse_array_of_enums<TEnum>(
                string csvData,
                Func<Data, TEnum[]> extractor,
                TEnum[] expectedEnums
            )
            {
                // Arrange
                var (parser, readerOptions) = CreateParser();

                // Act
                var results = parser.ReadFromString(readerOptions, csvData).ToList();

                // Assert
                results.Should().NotBeNullOrEmpty();

                var @enums = extractor(results.First().Result);

                @enums.Should().NotBeNullOrEmpty();
                @enums.Should().ContainInOrder(expectedEnums, because: "those were the values passed");
            }

            // ReSharper disable once InconsistentNaming
            public static IEnumerable<object[]> Can_parse_array_of_enums_Data
            {
                get
                {
                    yield return TestCase(new[] { TestEnum.A, TestEnum.B, TestEnum.C }, data => data.TestEnums);
                    yield return TestCase(new[] { TestEnum.B, TestEnum.A, TestEnum.C }, data => data.TestEnums);
                    yield return TestCase(new[] { TestEnum.B, TestEnum.C, TestEnum.A }, data => data.TestEnums);

                    object[] TestCase<TEnum>(IEnumerable<TEnum> items, Func<Data, TEnum[]> extractor)
                    {
                        var csvData = $"Value1,Value2,Value3;{string.Join(',', items)}";

                        return new object[] { csvData, extractor, items };
                    }
                }
            }

            private static (ICsvParser<Data> Parser, CsvReaderOptions ReaderOptions) CreateParser()
            {
                var options = new CsvParserOptions(skipHeader: true, fieldsSeparator: ',');
                var typeConverterProvider = new TypeConverterProvider().AddEnums();
                var parser = new CsvParser<Data>(options, new Data.Mapping(typeConverterProvider));
                var readerOptions = new CsvReaderOptions(new[] { ";" });

                return (Parser: parser, ReaderOptions: readerOptions);
            }
        }
    }
}
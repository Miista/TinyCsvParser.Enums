using System;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Enums
{
    public static class TypeConverterProviderExtensions
    {
        public static ITypeConverterProvider AddEnums(this ITypeConverterProvider typeConverterProvider)
        {
            if (typeConverterProvider == null) throw new ArgumentNullException(nameof(typeConverterProvider));

            var decoratingTypeConverterProvider = new EnumRespectingTypeConverterProvider(typeConverterProvider);

            return decoratingTypeConverterProvider;
        }
    }
}
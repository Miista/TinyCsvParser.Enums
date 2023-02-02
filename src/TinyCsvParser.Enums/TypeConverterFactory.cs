using System;
using System.Linq;
using System.Reflection;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Enums
{
    internal static class TypeConverterFactory
    {
        public static ITypeConverter<T> Create<T>(Type type)
        {
            var constructor = GetConstructor(typeof(EnumConverter<>), type);

            return constructor.Invoke(Array.Empty<object>()) as ITypeConverter<T>;
        }

        public static ITypeConverter<T> CreateNullableConverter<T>(Type type)
        {
            var constructor = GetConstructor(typeof(NullableEnumConverter<>), type);

            return constructor.Invoke(Array.Empty<object>()) as ITypeConverter<T>;
        }

        public static IArrayTypeConverter<T> CreateArrayTypeConverter<T>(Type type)
        {
            var enumConverterConstructor = GetConstructor(typeof(EnumConverter<>), type);
            var enumConverter = enumConverterConstructor.Invoke(Array.Empty<object>());

            var arrayConverterConstructor = GetConstructor(typeof(ArrayConverter<>), type);
            var arrayTypeConverter =
                arrayConverterConstructor.Invoke(new[] { enumConverter }) as IArrayTypeConverter<T>;

            return arrayTypeConverter;
        }

        private static ConstructorInfo GetConstructor(Type type, params Type[] genericParameters)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type
                       .MakeGenericType(genericParameters)
                       .GetConstructors()
                       .FirstOrDefault()
                   ?? throw new InvalidOperationException($"Cannot get default constructor for type '{type.Name}'");
        }
    }
}
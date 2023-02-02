using System;
using TinyCsvParser.Exceptions;
using TinyCsvParser.TypeConverter;

namespace TinyCsvParser.Enums
{
    internal class EnumRespectingTypeConverterProvider : ITypeConverterProvider
    {
        private readonly ITypeConverterProvider _typeConverterProvider;

        public EnumRespectingTypeConverterProvider(ITypeConverterProvider typeConverterProvider)
        {
            _typeConverterProvider =
                typeConverterProvider ?? throw new ArgumentNullException(nameof(typeConverterProvider));
        }

        public ITypeConverter<TTargetType> Resolve<TTargetType>()
        {
            try
            {
                return _typeConverterProvider.Resolve<TTargetType>();
            }
            catch (CsvTypeConverterNotRegisteredException)
            {
                if (typeof(TTargetType).IsSubclassOf(typeof(Enum)))
                {
                    return TypeConverterFactory.Create<TTargetType>(typeof(TTargetType));
                }

                var underlyingType = Nullable.GetUnderlyingType(typeof(TTargetType));

                if (underlyingType != null)
                {
                    return TypeConverterFactory.CreateNullableConverter<TTargetType>(underlyingType);
                }

                // At this point, there is nothing we can do.
                throw;
            }
        }

        public IArrayTypeConverter<TTargetType> ResolveCollection<TTargetType>()
        {
            try
            {
                return _typeConverterProvider.ResolveCollection<TTargetType>();
            }
            catch (CsvTypeConverterNotRegisteredException)
            {
                var elementType = typeof(TTargetType).GetElementType();

                if (elementType?.IsSubclassOf(typeof(Enum)) ?? false)
                {
                    return TypeConverterFactory.CreateArrayTypeConverter<TTargetType>(elementType);
                }

                // At this point, there is nothing we can do.
                throw;
            }
        }
    }
}
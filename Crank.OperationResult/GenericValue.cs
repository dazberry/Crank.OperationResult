using System;

namespace Crank.OperationResult
{

    public interface IGenericValue
    {
        bool TryGetValue<TType>(out TType value)
        {
            value = default;
            return false;
        }

        bool IsUndefined => true;

        IGenericValue ChangeValue<TType>(TType value) =>
            new GenericValue<TType>(value);

        Type GetValueType() =>
            null;
    }

    public class UndefinedGenericValue : IGenericValue
    {
        private static UndefinedGenericValue _instance;
        public static UndefinedGenericValue GetInstance() =>
            _instance ??= new UndefinedGenericValue();

        private UndefinedGenericValue()
        { }
    }

    public class GenericValue<TValue> : IGenericValue
    {
        public TValue Value { get; private set; }

        public GenericValue(TValue value)
        {
            Value = value;
        }

        private bool TryConvert<TSource, TDestination>(TSource source, out TDestination destination)
        {
            if (typeof(TSource) == typeof(TDestination))
            {
                try
                {
                    destination = source is IConvertible
                        ? destination = (TDestination)Convert.ChangeType(source, typeof(TDestination))
                        : destination = (TDestination)(object)source;
                    return true;
                }
                catch
                {
                }
            }

            destination = default;
            return false;
        }

        public bool TryGetValue<TType>(out TType value) =>
            TryConvert(Value, out value);

        private bool TrySetValue<TType>(TType value)
        {
            var result = TryConvert<TType, TValue>(value, out var convertedValue);
            if (result)
                Value = convertedValue;
            return result;
        }
        public IGenericValue ChangeValue<TType>(TType value)
        {
            if (TrySetValue(value))
                return this as GenericValue<TType>;

            return new GenericValue<TType>(value);
        }

        public bool IsUndefined => false;

        public Type GetValueType() =>
            typeof(TValue);
    }

}

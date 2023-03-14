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
    }

    public interface IGenericConvertable
    {

    }

    public class UndefinedGenericValue : IGenericValue
    {
    }

    public class GenericValue<TValue> : IGenericValue
    {
        public TValue Value { get; private set; }

        public GenericValue(TValue value)
        {
            Value = value;
        }

        public bool TryGetValue<TType>(out TType value)
        {
            if (typeof(TValue) == typeof(TType))
            {
                value = (TType)Convert.ChangeType(Value, typeof(TType));
                return true;
            };
            value = default;
            return false;
        }

        private bool TrySetValue<TType>(TType value)
        {
            if (typeof(TValue) == typeof(TType))
            {
                Value = (TValue)Convert.ChangeType(value, typeof(TValue));
                return true;
            }
            return false;
        }

        public IGenericValue ChangeValue<TType>(TType value)
        {
            if (TrySetValue(value))
                return this as GenericValue<TType>;

            return new GenericValue<TType>(value);
        }

        public bool IsUndefined => false;
    }

}

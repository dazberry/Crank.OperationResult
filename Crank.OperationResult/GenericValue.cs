using System;

namespace Crank.OperationResult
{
    public abstract class GenericValue
    {
        public abstract bool Is<TType>();
        public abstract bool As<TType>(out GenericValue<TType> genericValue);
        public abstract bool TryGetValue<TType>(out TType value);
        public abstract GenericValue<TType> To<TType>(TType value);

        public virtual bool Undefined => true;
    }

    public class UndefinedGenericValue : GenericValue
    {
        public override bool As<TType>(out GenericValue<TType> genericValue)
        {
            genericValue = default;
            return false;
        }

        public override bool Is<TType>() => false;

        public override bool TryGetValue<TType>(out TType value)
        {
            value = default;
            return false;
        }

        public override GenericValue<TType> To<TType>(TType value) =>
            new GenericValue<TType>(value);
    }

    public class GenericValue<TValue> : GenericValue
    {
        public TValue Value { get; private set; }

        public GenericValue(TValue value)
        {
            Value = value;
        }

        public override bool Is<TType>() =>
            typeof(TType) == typeof(TValue);

        public override bool As<TType>(out GenericValue<TType> genericValue)
        {
            var result = this is GenericValue<TType>;
            genericValue = result ? this as GenericValue<TType> : default;
            return result;
        }

        public override bool TryGetValue<TType>(out TType value)
        {
            var result = this.As<TType>(out var genericValue);
            value = result ? genericValue.Value : default;
            return result;
        }

        public override GenericValue<TType> To<TType>(TType value)
        {
            if (typeof(TValue) == typeof(TType))
            {
                this.Value = (TValue)Convert.ChangeType(value, typeof(TValue));
                return this as GenericValue<TType>;
            }

            return new GenericValue<TType>(value);
        }

        public override bool Undefined => false;
    }





}

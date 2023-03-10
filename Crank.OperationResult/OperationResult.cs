using System;

namespace Crank.OperationResult
{
    public enum OperationState { Undefined, Success, Failure };

    public class OperationResult
    {
        public OperationState State { get; protected set; } = OperationState.Undefined;

        protected static readonly UndefinedGenericValue _undefinedValue = new UndefinedGenericValue();

        public OperationResult() { }

        public static OperationResult Undefined() => new OperationResult();
        public static OperationResult<TType> Undefined<TType>() => new OperationResult<TType>();
        public static OperationResult Succeeded() => new OperationResult().Success();
        public static OperationResult<TType> Succeeded<TType>(TType value = default) => new OperationResult<TType>().Success(value);

        public static OperationResult Failed() => new OperationResult().Fail();
        public static OperationResult Failed<TErrorValue>(TErrorValue value) =>
            new OperationResult().Fail<TErrorValue>(value);

        public static OperationResult<TSuccessValue> Failed<TSuccessValue>()
        {
            var result = new OperationResult<TSuccessValue>();
            result.Fail();
            return result;
        }
        public static OperationResult<TSuccessValue> Failed<TSuccessValue, TErrorValue>(TErrorValue failureValue)
        {
            var result = new OperationResult<TSuccessValue>();
            result.Fail(failureValue);
            return result;
        }

        public GenericValue SuccessValue { get; protected set; } = _undefinedValue;
        public GenericValue ErrorValue { get; protected set; } = _undefinedValue;

        public bool IsUndefined => State == OperationState.Undefined;
        public bool HasSucceeded => State == OperationState.Success;
        public bool HasFailed => State == OperationState.Failure;

        protected OperationResult SetGenericValues(GenericValue successValue, GenericValue errorValue)
        {
            SuccessValue = successValue;
            ErrorValue = errorValue;
            return this;
        }

        protected OperationResult SetState(OperationState state)
        {
            State = state;
            return this;
        }

        public OperationResult Success() =>
            SetState(OperationState.Success);

        public virtual OperationResult Success<TValue>(TValue value) =>
            SetGenericValues(SuccessValue.To(value), _undefinedValue)
            .Success();

        public OperationResult Fail() =>
            SetState(OperationState.Failure);

        public OperationResult Fail<TErrorValue>(TErrorValue value) =>
            SetGenericValues(_undefinedValue, ErrorValue.To(value))
            .Fail();

        public OperationResult Map(OperationResult operationResult)
            => operationResult.State switch
            {
                OperationState.Success => Success(operationResult.SuccessValue),
                OperationState.Failure => Fail(operationResult.ErrorValue),
                _ => this
            };

    }

    public class OperationResult<TSuccessValue> : OperationResult
    {
        public OperationResult()
        {
            SuccessValue = new GenericValue<TSuccessValue>(default);
        }

        public override OperationResult Success<TValue>(TValue value)
        {
            if (typeof(TValue) != typeof(TSuccessValue))
                throw new Exception();

            SetState(OperationState.Success);
            SuccessValue = SuccessValue.To<TValue>(value);
            return this;
        }

        public OperationResult<TSuccessValue> Success(TSuccessValue successValue)
        {
            SetState(OperationState.Success);
            SuccessValue = SuccessValue.To(successValue);
            return this;
        }

        public TSuccessValue Value =>
            base.SuccessValue.As<TSuccessValue>(out var genericValue)
                ? genericValue.Value
                : default;

        public OperationResult<TSuccessValue> MapConvert<TNewSuccessValue>(
            OperationResult<TNewSuccessValue> operationResult,
            Func<TNewSuccessValue, TSuccessValue> convertAction)

        {
            if (operationResult.IsUndefined)
                return this;

            if (operationResult.HasSucceeded)
            {
                if (convertAction != null)
                {
                    return Success(
                        convertAction.Invoke(operationResult.Value));
                }
                Success();
                return this;
            }

            SetGenericValues(_undefinedValue, operationResult.ErrorValue).Fail();
            return this;
        }
    }

}

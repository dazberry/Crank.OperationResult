using System;
using System.Threading.Tasks;

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

        public GenericValue _genericValue = _undefinedValue;

        public GenericValue Value =>
            State switch
            {
                OperationState.Success => _genericValue,
                _ => _undefinedValue
            };

        public bool IsUndefined => State == OperationState.Undefined;
        public bool HasSucceeded => State == OperationState.Success;
        public bool HasFailed => State == OperationState.Failure;

        protected OperationResult SetState(OperationState state)
        {
            State = state;
            return this;
        }

        public OperationResult Success() =>
            SetState(OperationState.Success);

        public OperationResult Success<TSuccessValue>(TSuccessValue successValue)
        {
            Success();
            _genericValue = _genericValue.To(successValue);
            return this;
        }

        public OperationResult<TSuccessValue> SuccessAs<TSuccessValue>(TSuccessValue successValue)
        {
            Success();
            _genericValue = _genericValue.To(successValue);
            return new OperationResult<TSuccessValue>(this);
        }


        public OperationResult Fail() =>
            SetState(OperationState.Failure);

        public OperationResult Fail<TErrorValue>(TErrorValue value)
        {
            _genericValue = _genericValue.To<TErrorValue>(value);
            return Fail();
        }


        public OperationResult Map(OperationResult operationResult)
        {
            if (this.HasFailed)
                return this;

            State = operationResult.State;
            _genericValue = operationResult._genericValue;
            return this;
        }

        public OperationResult Map(Func<OperationResult> mapAction)
        {
            if (!this.HasFailed && mapAction != null)
                return Map(mapAction.Invoke());
            return this;
        }

        public async Task<OperationResult> MapAsync(Func<Task<OperationResult>> mapAction)
        {
            if (!this.HasFailed && mapAction != null)
                return Map(await mapAction.Invoke());

            return this;
        }
    }

    public class OperationResult<TSuccessValue> : OperationResult
    {
        public new TSuccessValue Value =>
            base.Value.TryGetValue<TSuccessValue>(out var successValue)
                ? successValue
                : default;

        public OperationResult() { }

        public OperationResult(TSuccessValue value)
        {
            _genericValue = new GenericValue<TSuccessValue>(value);
        }

        public OperationResult(OperationResult operationResult)
        {
            this.State = operationResult.State;
            this._genericValue = operationResult._genericValue;
        }

        public new OperationResult<TSuccessValue> Success<TValue>(TValue value)
        {
            if (typeof(TValue) != typeof(TSuccessValue))
                throw new Exception();

            SetState(OperationState.Success);
            _genericValue = _genericValue.To<TValue>(value);
            return this;
        }

        public OperationResult<TSuccessValue> Success(TSuccessValue successValue)
        {
            SetState(OperationState.Success);
            _genericValue = _genericValue.To(successValue);
            return this;
        }

        public OperationResult<TSuccessValue> MapConvert<TNewSuccessValue>(
            OperationResult<TNewSuccessValue> operationResult,
            Func<TNewSuccessValue, TSuccessValue> convertAction)
        {
            if (this.HasFailed || operationResult.IsUndefined)
                return this;

            this.SetState(operationResult.State);

            if (operationResult.HasSucceeded)
            {
                if (convertAction != null)
                {
                    operationResult._genericValue.TryGetValue<TNewSuccessValue>(out var newSuccessValue);
                    return Success(convertAction.Invoke(newSuccessValue));
                }
                Success();
            }

            return this;
        }
    }

}

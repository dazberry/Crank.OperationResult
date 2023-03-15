using System;
using System.Threading.Tasks;

namespace Crank.OperationResult
{
    public enum OperationState { Undefined, Success, Failure };

    public class OperationResult
    {
        public OperationState State { get; protected set; } = OperationState.Undefined;

        public OperationResult() { }

        public static OperationResult Undefined() => new OperationResult();
        public static OperationResult<TType> Undefined<TType>() => new OperationResult<TType>();

        public static OperationResult Succeeded() => new OperationResult().Success();
        public static OperationResult<TType> Succeeded<TType>(TType value = default) => new OperationResult<TType>().Success(value);

        public static OperationResult Failed() => new OperationResult().Fail();
        public static OperationResult<TErrorValue> Failed<TErrorValue>(TErrorValue value = default)
        {
            var result = new OperationResult<TErrorValue>();
            result.Fail(value);
            return result;
        }

        protected static readonly IGenericValue _undefinedValue = UndefinedGenericValue.GetInstance();
        protected IGenericValue _genericValue = _undefinedValue;

        public IGenericValue Value => _genericValue;
        public bool IsValueUndefined => _genericValue == _undefinedValue;
        public bool TryGetValue<TValue>(out TValue value) =>
            _genericValue.TryGetValue<TValue>(out value);

        public bool IsStateUndefined => State == OperationState.Undefined;
        public bool HasSucceeded => State == OperationState.Success;
        public bool HasFailed => State == OperationState.Failure;

        protected OperationResult SetState(OperationState state)
        {
            State = state;
            return this;
        }

        protected void CopyFrom(OperationResult operationResult)
        {
            State = operationResult.State;
            _genericValue = operationResult._genericValue;
        }

        public OperationResult Success() =>
            SetState(OperationState.Success);

        public OperationResult Success<TSuccessValue>(TSuccessValue successValue)
        {
            Success();
            _genericValue = _genericValue.ChangeValue(successValue);
            return this;
        }

        public OperationResult Fail() =>
            SetState(OperationState.Failure);

        public OperationResult Fail<TErrorValue>(TErrorValue value)
        {
            _genericValue = _genericValue.ChangeValue(value);
            return Fail();
        }

        public OperationResult Map(OperationResult operationResult)
        {
            if (this.HasFailed)
                return this;

            CopyFrom(operationResult);
            return this;
        }

        public OperationResult Map<TMapType>(OperationResult<TMapType> operationResult)
        {
            return Map((OperationResult)operationResult);
        }

        public OperationResult<TMapType> MapTo<TMapType>(OperationResult<TMapType> operationResult)
        {
            if (HasFailed || (operationResult?.IsStateUndefined ?? true))
                return new OperationResult<TMapType>(this);

            CopyFrom(operationResult);
            return new OperationResult<TMapType>(operationResult);
        }

        public OperationResult Map(Func<OperationResult> mapAction)
        {
            if (!HasFailed && mapAction != null)
                return Map(mapAction.Invoke());
            return this;
        }

        public async Task<OperationResult> MapAsync(Func<Task<OperationResult>> mapAction)
        {
            if (!HasFailed && mapAction != null)
                return Map(await mapAction.Invoke());

            return this;
        }
    }

    public class OperationResult<TExpectedValue> : OperationResult
    {
        public new TExpectedValue Value =>
            _genericValue.TryGetValue<TExpectedValue>(out var value)
                ? value
                : default;

        public OperationResult() { }

        public OperationResult(OperationResult operationResult)
        {
            if (operationResult == null)
                throw new ArgumentNullException(nameof(operationResult));

            SetState(operationResult.State);
            this._genericValue = operationResult.Value;
        }

        public new OperationResult Success()
        {
            SetState(OperationState.Success);
            _genericValue = _undefinedValue;
            return this;
        }

        public OperationResult<TExpectedValue> Success(TExpectedValue successValue)
        {
            SetState(OperationState.Success);
            _genericValue = _genericValue.ChangeValue(successValue);
            return this;
        }

        public new OperationResult Fail()
        {
            SetState(OperationState.Failure);
            _genericValue = _undefinedValue;
            return this;
        }

        public OperationResult<TExpectedValue> Fail<TFailingValue>()
        {
            SetState(OperationState.Failure);
            _genericValue = _undefinedValue;
            return this;
        }

        public new OperationResult<TExpectedValue> Fail<TFailingValue>(TFailingValue failingValue)
        {
            SetState(OperationState.Failure);
            _genericValue = _genericValue.ChangeValue(failingValue);
            return this;
        }

        public new OperationResult<TExpectedValue> Map<TMapType>(OperationResult<TMapType> operationResult)
        {
            if (this.HasFailed || operationResult.IsStateUndefined)
                return this;

            if (typeof(TExpectedValue) == typeof(TMapType) || operationResult.HasFailed)
            {
                CopyFrom(operationResult);
                return this;
            }

            this.SetState(operationResult.State);
            this._genericValue = _undefinedValue;
            return this;
        }

        public OperationResult<TExpectedValue> MapConvert<TNewSuccessValue>(
            OperationResult<TNewSuccessValue> operationResult,
            Func<TNewSuccessValue, TExpectedValue> convertAction)
        {
            if (this.HasFailed || operationResult.IsStateUndefined)
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

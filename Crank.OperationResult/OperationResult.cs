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
        public static OperationResult<TErrorValue> Failed<TErrorValue>(TErrorValue value = default)
        {
            var result = new OperationResult<TErrorValue>();
            result.Fail(value);
            return result;
        }

        protected GenericValue _genericValue = _undefinedValue;

        public GenericValue Value => _genericValue;
        public bool TryGetValue<TValue>(out TValue value) =>
            _genericValue.TryGetValue<TValue>(out value);
        
        public bool IsUndefined => State == OperationState.Undefined;
        public bool HasSucceeded => State == OperationState.Success;
        public bool HasFailed => State == OperationState.Failure;

        protected OperationResult SetState(OperationState state)
        {
            State = state;
            return this;
        }

        protected void Copy(OperationResult operationResult)
        {
            State = operationResult.State;
            _genericValue = operationResult._genericValue;            
        }

        public OperationResult Success() =>
            SetState(OperationState.Success);

        public OperationResult Success<TSuccessValue>(TSuccessValue successValue)
        {
            Success();
            _genericValue = _genericValue.To(successValue);
            return this;
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

            Copy(operationResult);            
            return this;
        }

        public OperationResult Map<TMapType>(OperationResult<TMapType> operationResult)
        {
            return Map((OperationResult)operationResult);
        }

        public OperationResult<TMapType> MapTo<TMapType>(OperationResult<TMapType> operationResult)
        {
            if (this.HasFailed || operationResult.IsUndefined)
                return new OperationResult<TMapType>(this);

            Copy(operationResult);            
            return new OperationResult<TMapType>(operationResult);                
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

    public class OperationResult<TExpectedValue> : OperationResult
    {
        public new TExpectedValue Value =>
            _genericValue.TryGetValue<TExpectedValue>(out var value)
                ? value
                : default;

        public bool Is<TValue>() =>
            _genericValue.Is<TValue>();

        public bool As<TValue>(out TValue value) =>
            _genericValue.TryGetValue<TValue>(out value);

        public bool IsValueUndefined => _genericValue == _undefinedValue;


        public OperationResult() { }

        public OperationResult(TExpectedValue value)
        {
            _genericValue = new GenericValue<TExpectedValue>(value);
        }

        public OperationResult(OperationResult operationResult)
        {
            SetState(operationResult.State);            
            this._genericValue = operationResult.Value;
        }

        public new OperationResult Success()
        {
            SetState(OperationState.Success);
            _genericValue = _undefinedValue;
            return this;
        }

        public new OperationResult<TExpectedValue> Success<TValue>(TValue value)
            where TValue : TExpectedValue
        {
            if (typeof(TValue) != typeof(TExpectedValue))
                throw new Exception();

            SetState(OperationState.Success);
            _genericValue = _genericValue.To<TValue>(value);
            return this;
        }

        public OperationResult<TExpectedValue> Success(TExpectedValue successValue)
        {
            SetState(OperationState.Success);
            _genericValue = _genericValue.To(successValue);
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
            _genericValue = _genericValue.To<TFailingValue>(failingValue);
            return this;
        }

        public new OperationResult<TExpectedValue> Map<TMapType>(OperationResult<TMapType> operationResult)
        {
            if (this.HasFailed || operationResult.IsUndefined)
                return this;

            if (typeof(TExpectedValue) == typeof(TMapType))
            {
                Copy(operationResult);
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

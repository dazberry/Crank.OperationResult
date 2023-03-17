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
        public bool TryGetValue<TValue>(out TValue value) =>
            _genericValue.TryGetValue<TValue>(out value);

        public bool IsUndefined => State == OperationState.Undefined;
        public bool HasSucceeded => State == OperationState.Success;
        public bool HasFailed => State == OperationState.Failure;

        protected void CopyFrom(OperationResult operationResult)
        {
            State = operationResult.State;
            _genericValue = operationResult._genericValue;
        }

        protected void Update(OperationState state, IGenericValue genericValue = null)
        {
            State = state;
            _genericValue = genericValue ?? _undefinedValue;
        }

        public OperationResult Success()
        {
            Update(OperationState.Success);
            return this;
        }

        public OperationResult Success<TSuccessValue>(TSuccessValue successValue)
        {
            Update(OperationState.Success, _genericValue.ChangeValue(successValue));
            return this;
        }

        public OperationResult Fail()
        {
            Update(OperationState.Failure);
            return this;
        }

        public OperationResult Fail<TErrorValue>(TErrorValue value)
        {
            Update(OperationState.Failure, _genericValue.ChangeValue(value));
            return this;
        }

        protected static OperationState GetState(bool success) =>
            success ? OperationState.Success : OperationState.Failure;

        public OperationResult Set(bool success)
        {
            Update(GetState(success));
            return this;
        }

        public OperationResult Set<TValue>(bool success, TValue value)
        {
            Update(GetState(success), _genericValue.ChangeValue(value));
            return this;
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
            if (HasFailed || (operationResult?.IsUndefined ?? true))
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

        public bool ValueIsUndefined => _genericValue == _undefinedValue;

        public readonly OperationResultOptions Options = new OperationResultOptions()
        {
            ExpectedResultTypeChecking = OperationResultTypeChecking.Strict
        };

        public OperationResult<TExpectedValue> SetOptions(Action<OperationResultOptions> optionsAction)
        {
            optionsAction?.Invoke(Options);
            return this;
        }

        public OperationResult() { }

        public OperationResult(OperationResult operationResult)
        {
            if (operationResult == null)
                throw new ArgumentNullException(nameof(operationResult));

            Update(operationResult.State, operationResult.Value);
        }

        public OperationResult<TExpectedValue> Success(TExpectedValue successValue)
        {
            Update(OperationState.Success, _genericValue.ChangeValue(successValue));
            return this;
        }

        public new OperationResult Fail()
        {
            Update(OperationState.Failure);
            return this;
        }

        public OperationResult<TExpectedValue> Fail<TFailingValue>()
        {
            Update(OperationState.Failure);
            return this;
        }

        public new OperationResult<TExpectedValue> Fail<TFailingValue>(TFailingValue failingValue)
        {
            Update(OperationState.Failure, _genericValue.ChangeValue(failingValue));
            return this;
        }

        public new OperationResult<TExpectedValue> Set<TValue>(bool success, TValue value)
        {
            if (success && typeof(TValue) != typeof(TExpectedValue))
            {
                switch (Options.ExpectedResultTypeChecking)
                {
                    case OperationResultTypeChecking.Strict:
                        throw new OperationResultExpectedTypeMismatchException(typeof(TExpectedValue), typeof(TValue));
                    case OperationResultTypeChecking.Discard:
                        Update(OperationState.Success);
                        return this;
                    default:
                        break;
                }                
            }

            Update(GetState(success), _genericValue.ChangeValue(value));
            return this;
        }

        public new OperationResult<TExpectedValue> Map<TMapType>(OperationResult<TMapType> operationResult)
        {
            if (this.HasFailed || operationResult.IsUndefined)
                return this;

            if (typeof(TExpectedValue) == typeof(TMapType) || operationResult.HasFailed)
            {
                CopyFrom(operationResult);
                return this;
            }

            Update(operationResult.State);
            return this;
        }

        public OperationResult<TExpectedValue> MapConvert<TNewSuccessValue>(
            OperationResult<TNewSuccessValue> operationResult,
            Func<TNewSuccessValue, TExpectedValue> convertAction)
        {
            if (this.HasFailed || operationResult.IsUndefined)
                return this;

            if (operationResult.HasSucceeded)
            {
                if (convertAction != null)
                {
                    operationResult._genericValue.TryGetValue<TNewSuccessValue>(out var newSuccessValue);
                    return Success(convertAction.Invoke(newSuccessValue));
                }
                Success();
                return this;
            }

            Update(OperationState.Failure);
            return this;
        }
    }

}

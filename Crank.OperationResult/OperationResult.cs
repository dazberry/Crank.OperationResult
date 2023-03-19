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

        public OperationResult Map(OperationResult mapFromResult)
        {
            if (HasFailed || (mapFromResult?.IsUndefined ?? true))
                return this;

            CopyFrom(mapFromResult);
            return this;
        }

        public OperationResult Map<TMapType>(OperationResult<TMapType> mapFromResult)
        {
            return Map((OperationResult)mapFromResult);
        }

        public OperationResult<TMapType> MapTo<TMapType>(OperationResult<TMapType> mapFromResult)
        {
            if (HasFailed || (mapFromResult?.IsUndefined ?? true))
                return new OperationResult<TMapType>(this);

            CopyFrom(mapFromResult);
            return new OperationResult<TMapType>(mapFromResult);
        }

        public OperationResult Map(Func<OperationResult> mapFromAction)
        {
            if (!HasFailed && mapFromAction != null)
                return Map(mapFromAction.Invoke());
            return this;
        }

        public async Task<OperationResult> MapAsync(Func<Task<OperationResult>> mapFromActionAsync)
        {
            if (!HasFailed && mapFromActionAsync != null)
                return Map(await mapFromActionAsync.Invoke());
            return this;
        }

        public bool Match(Action<OperationResultMatch> matchAction)
        {
            var operationMatch = new OperationResultMatch(this);
            matchAction?.Invoke(operationMatch);
            return operationMatch.Matched;
        }

        public TMatchResult MatchTo<TMatchResult>(Action<OperationResultMatchTo<TMatchResult>> matchAction, TMatchResult defaultResult = default)
        {
            var operationMatch = new OperationResultMatchTo<TMatchResult>(this, defaultResult);
            matchAction?.Invoke(operationMatch);
            return operationMatch.Result;
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

        public new OperationResult<TExpectedValue> Map<TMapType>(OperationResult<TMapType> mapFromResult)
        {
            if (this.HasFailed || mapFromResult.IsUndefined)
                return this;

            if (typeof(TExpectedValue) == typeof(TMapType) || mapFromResult.HasFailed)
            {
                CopyFrom(mapFromResult);
                return this;
            }

            Update(mapFromResult.State);
            return this;
        }

        public OperationResult<TExpectedValue> MapConvert<TNewSuccessValue>(
            OperationResult<TNewSuccessValue> mapFromResult,
            Func<TNewSuccessValue, TExpectedValue> convertAction)
        {
            if (this.HasFailed || mapFromResult.IsUndefined)
                return this;

            if (mapFromResult.HasSucceeded)
            {
                if (convertAction != null)
                {
                    mapFromResult._genericValue.TryGetValue<TNewSuccessValue>(out var newSuccessValue);
                    return Success(convertAction.Invoke(newSuccessValue));
                }
                Success();
                return this;
            }

            CopyFrom(mapFromResult);
            return this;
        }


        public bool Match(Action<OperationResultMatch<TExpectedValue>> matchAction)
        {
            var operationMatch = new OperationResultMatch<TExpectedValue>(this);
            matchAction?.Invoke(operationMatch);
            return operationMatch.Matched;
        }

        public TMatchResult MatchTo<TMatchResult>(Action<OperationResultMatchTo<TExpectedValue, TMatchResult>> matchAction, TMatchResult defaultResult = default)
        {
            var operationMatch = new OperationResultMatchTo<TExpectedValue, TMatchResult>(this, defaultResult);
            matchAction?.Invoke(operationMatch);
            return operationMatch.Result;
        }

    }

}

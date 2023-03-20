using System;

namespace Crank.OperationResult
{
    public class OperationResultMatch
    {
        protected bool _invoked;
        protected readonly OperationResult _operationResult;


        public OperationResult OperationResult =>
            _operationResult;

        public OperationResultMatch(OperationResult operationResult)
        {
            _operationResult = operationResult;
        }

        public OperationResultMatch ValueIs<TMatchType>(Action<TMatchType> matchAction)
        {
            var matchingResult = _operationResult.TryGetValue<TMatchType>(out var value);
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(value);
            }
            return this;
        }

        public OperationResultMatch ValueIsEquals<TMatchType>(TMatchType expectedValue, Action<TMatchType> matchAction)
        {
            var matchingResult = _operationResult.TryGetValue<TMatchType>(out var value) &&
                    (value == null && expectedValue == null) ||
                    (value != null && value.Equals(expectedValue));
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(value);
            }
            return this;
        }

        public OperationResultMatch StateIs(OperationState operationState, Action<OperationResult> matchAction)
        {
            var matchingResult = _operationResult.State == operationState;
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(OperationResult);
            }
            return this;
        }

        public OperationResultMatch ValueAndStateAre<TMatchType>(OperationState operationState, Action<TMatchType> matchAction) =>
            _operationResult.State == operationState
                ? ValueIs(matchAction)
                : this;

        public OperationResultMatch ValueIsUndefined(Action<OperationResult> matchAction)
        {
            var matchingResult = _operationResult.Value.IsUndefined;
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction.Invoke(OperationResult);
            }
            return this;
        }

        public OperationResultMatch ValueIsUndefined(OperationState operationState, Action<OperationResult> matchAction) =>
            _operationResult.State == operationState
                ? ValueIsUndefined(matchAction)
                : this;

        public OperationResultMatch Default(Action<OperationResult> defaultAction = default)
        {
            if (!_invoked)
                defaultAction?.Invoke(_operationResult);
            return this;
        }

        public bool Matched => _invoked;
    }


    public class OperationResultMatch<TExpectedValue> : OperationResultMatch
    {
        private readonly OperationResult<TExpectedValue> _typedOperationResult;

        public new OperationResult<TExpectedValue> OperationResult =>
            _typedOperationResult;

        public OperationResultMatch(OperationResult<TExpectedValue> operationResult) : base(operationResult)
        {
            _typedOperationResult = operationResult;
        }

        public OperationResultMatch StateIs(OperationState operationState, Action<OperationResult<TExpectedValue>> matchAction)
        {
            var matchingResult = _operationResult.State == operationState;
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(OperationResult);
            }
            return this;
        }
    }

    public class OperationResultMatchTo<TMatchResult> : OperationResultMatch
    {
        public TMatchResult Result { get; set; }

        public OperationResultMatchTo(OperationResult operationResult, TMatchResult defaultValue) : base(operationResult)
        {
            Result = defaultValue;
        }
    }

    public class OperationResultMatchTo<TExpectedValue, TMatchResult> : OperationResultMatch
    {
        private readonly OperationResult<TExpectedValue> _typedOperationResult;

        public TMatchResult Result { get; set; }

        public new OperationResult<TExpectedValue> OperationResult =>
            _typedOperationResult;

        public OperationResultMatchTo(OperationResult<TExpectedValue> operationResult, TMatchResult defaultValue) : base(operationResult)
        {
            _typedOperationResult = operationResult;
            Result = defaultValue;
        }

        public OperationResultMatch StateIs(OperationState operationState, Action<OperationResult<TExpectedValue>> matchAction)
        {
            var matchingResult = _operationResult.State == operationState;
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(OperationResult);
            }
            return this;
        }
    }
}

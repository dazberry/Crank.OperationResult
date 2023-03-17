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

        public OperationResultMatch Match<TMatchType>(Action<TMatchType> matchAction)
        {
            var matchingResult = _operationResult.TryGetValue<TMatchType>(out var value);
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(value);
            }
            return this;
        }

        public OperationResultMatch Match(OperationState operationState, Action<OperationResult> matchAction)
        {
            var matchingResult = _operationResult.State == operationState;
            if (matchingResult && matchAction != null)
            {
                _invoked = true;
                matchAction?.Invoke(OperationResult);
            }
            return this;
        }

        public OperationResultMatch Match<TMatchType>(OperationState operationState, Action<TMatchType> matchAction) =>
            _operationResult.State == operationState
                ? Match(matchAction)
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

        public OperationResultMatch Match(OperationState operationState, Action<OperationResult<TExpectedValue>> matchAction)
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

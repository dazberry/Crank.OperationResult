using System;

namespace Crank.OperationResult
{
    public enum OperationState { Undefined, Success, Failure };

    public class OperationResult
    {
        public OperationState State { get; protected set; } = OperationState.Undefined;

        public OperationResult Success()
        {
            State = OperationState.Success;
            return this;
        }

        private OperationResult<TNewValue> RecreateIfDifferentType<TNewValue>(TNewValue value = default)
        {

            if (Is<TNewValue>())
            {
                var result = To<TNewValue>();
                if (value != null)
                    result.Value = value;

                return result;
            }
            return new OperationResult<TNewValue>() { State = this.State, Value = value };

        }

        public OperationResult<TValue> Success<TValue>(TValue value)
        {
            State = OperationState.Success;

            if (Is<TValue>())
                return To<TValue>()
                    .Success<TValue>(value);

            return Successful<TValue>();
        }
        public OperationResult Fail()
        {
            State = OperationState.Failure;
            return this;
        }
        public OperationResult<TValue> Fail<TValue>()
        {
            Fail();
            return RecreateIfDifferentType<TValue>();
        }

        public static OperationResult Undefined() =>
            new OperationResult();
        public static OperationResult<TValue> Undefined<TValue>(TValue value = default) =>
            new OperationResult<TValue>(value);
        public static OperationResult Successful() =>
            new OperationResult()
                .Success();
        public static OperationResult<TValue> Successful<TValue>(TValue value = default) =>
            new OperationResult<TValue>(value)
                .Success(value);

        public OperationResult Map(OperationResult operationResult)
        {
            this.State = operationResult.State;
            return this;
        }

        public bool Is<TValue>() =>
            this is OperationResult<TValue>;

        public bool As<TValue>(out TValue value)
        {
            if (Is<TValue>())
            {
                value = To<TValue>().Value;
                return true;
            }
            value = default;
            return false;
        }

        public OperationResult<TValue> To<TValue>() =>
            this is OperationResult<TValue> result
                ? result
                : new OperationResult<TValue>() { State = this.State };

    }

    public class OperationResult<TValue> : OperationResult
    {
        public TValue Value { get; set; }

        public OperationResult()
        {
        }

        public OperationResult(TValue value) =>
            Value = value;

        public OperationResult<TValue> Success(TValue value)
        {
            Success();
            Value = value;
            return this;
        }

        public OperationResult<TValue> Map(OperationResult<TValue> operationResult)
        {
            State = operationResult.State;
            if (State == OperationState.Success)
                Value = operationResult.Value;
            return this;
        }

        public OperationResult<TValue> MapConvert<TNewValue>(OperationResult<TNewValue> operationResult, Func<TNewValue, TValue> convertAction)
        {
            State = operationResult.State;
            if (convertAction != null)
                Value = convertAction.Invoke(operationResult.Value);
            return this;
        }

    }





}

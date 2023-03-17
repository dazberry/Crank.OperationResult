using System;

namespace Crank.OperationResult
{
    public class OperationResultExpectedTypeMismatchException : Exception
    {
        public OperationResultExpectedTypeMismatchException(Type expectedType, Type mismatchedType)
            : base($"A type of {expectedType} was expected, but a type of {mismatchedType} was supplied. A successful typed operation containing a value must match the expected type")
        {
        }
    }

}

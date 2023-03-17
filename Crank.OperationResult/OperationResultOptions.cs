namespace Crank.OperationResult
{
    public enum OperationResultTypeChecking { Strict, Discard, Ignore };

    public class OperationResultOptions
    {
        public OperationResultTypeChecking ExpectedResultTypeChecking { get; set; }
    }

}

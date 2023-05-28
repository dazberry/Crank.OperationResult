namespace Crank.OperationResult.Tests.Models
{

    public interface IRecordClass1
    {
        string AStringValue { get; }
    }

    public interface IRecordClass2
    {
        int AnIntValue { get; }
    }

    public abstract record BaseRecordClass<T> : IRecordClass1, IRecordClass2
    {
        public string AStringValue { get; set; }
        public int AnIntValue { get; set; }
        public T GenericValue { get; set; }
    }

    public record ChildRecordClass<T> : BaseRecordClass<T>
    {
    }


}

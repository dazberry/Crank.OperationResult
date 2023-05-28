using Crank.OperationResult.Tests.Models;
using System;
using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultTests
    {
        [Fact]
        public void GivenOperationResults_TheDefaultState_ShouldBeUndefined()
        {
            //given
            var untypedResult = new OperationResult();
            var typedResult = new OperationResult<string>();
            var untypedFromUndefined = OperationResult.Undefined();
            var typedFromUndefined = OperationResult.Undefined<string>();

            //then
            Assert.Equal(OperationState.Undefined, untypedResult.State);
            Assert.True(untypedResult.IsUndefined);
            Assert.Equal(OperationState.Undefined, typedResult.State);
            Assert.True(typedResult.IsUndefined);
            Assert.Equal(OperationState.Undefined, untypedFromUndefined.State);
            Assert.True(untypedFromUndefined.IsUndefined);
            Assert.Equal(OperationState.Undefined, typedFromUndefined.State);
            Assert.True(typedFromUndefined.IsUndefined);
        }

        [Fact]
        public void GivenOperationResults_WhenInitiatedWithSucceeded_ShouldSetStateToSuccess()
        {
            //given
            var untypedResult = OperationResult.Succeeded();
            var typedResult = OperationResult.Succeeded<string>();
            var typedResultWithValue = OperationResult.Succeeded<string>("123");

            //then
            Assert.Equal(OperationState.Success, untypedResult.State);
            Assert.True(untypedResult.HasSucceeded);
            Assert.Equal(OperationState.Success, typedResult.State);
            Assert.True(typedResult.HasSucceeded);
            Assert.Equal(OperationState.Success, typedResultWithValue.State);
            Assert.True(typedResultWithValue.HasSucceeded);

            Assert.Equal("123", typedResultWithValue.Value);
        }

        [Fact]
        public void GivenAnUnTypeOperationResult_WhenInitiatedWithFail_ShouldSetStateFailure()
        {
            //given
            var untypedResult = OperationResult.Failed();
            var typedResult = OperationResult.Undefined<string>().Fail();
            var typedResultWithValue = OperationResult.Failed<string>("123");

            //then
            Assert.Equal(OperationState.Failure, untypedResult.State);
            Assert.Equal(OperationState.Failure, typedResult.State);
            Assert.Equal(OperationState.Failure, typedResultWithValue.State);
        }

        [Fact]
        public void GivenAnUndefinedOperationResult_WhenCallingSuccess_ShouldSetStateToSucceed()
        {
            //given
            var untypedResult = OperationResult.Undefined();
            var typedResult = OperationResult.Undefined<string>();
            var typedResultWithValue = OperationResult.Undefined<string>();

            //when
            untypedResult.Success();
            typedResult.Success();
            typedResultWithValue.Success("123");

            //then
            Assert.Equal(OperationState.Success, untypedResult.State);
            Assert.Equal(OperationState.Success, typedResult.State);
            Assert.Equal(OperationState.Success, typedResultWithValue.State);
            Assert.Equal("123", typedResultWithValue.Value);
        }

        [Fact]
        public void GivenAnUndefinedOperationResult_WhenCallingFail_ShouldSetStateToFailure()
        {
            //given
            var untypedResult = OperationResult.Undefined();
            var typedResult = OperationResult.Undefined<string>();
            var typedResultWithValue = OperationResult.Undefined<string>();

            //when
            untypedResult.Fail();
            typedResult.Fail();
            typedResultWithValue.Fail("123");

            //then
            Assert.Equal(OperationState.Failure, untypedResult.State);
            Assert.Equal(OperationState.Failure, typedResult.State);
            Assert.Equal(OperationState.Failure, typedResultWithValue.State);
            Assert.Equal("123", typedResultWithValue.Value);
        }

        [Fact]
        public void GivenASuccessfulResult_CallingFail_ShouldSetStateToFail()
        {
            // given
            var untypedResult = OperationResult.Succeeded();
            var typedResult = OperationResult.Succeeded<string>();
            var typedResultWithValue = OperationResult.Succeeded<string>("123");
            var typedResultWithValue2 = OperationResult.Succeeded<string>("123");
            var typedResultWithValue3 = OperationResult.Succeeded<string>("123");

            //when
            untypedResult.Fail();
            typedResult.Fail();
            typedResultWithValue.Fail("123");
            typedResultWithValue2.Fail<string>();
            typedResultWithValue3.Fail<int>(456);

            //then
            Assert.Equal(OperationState.Failure, untypedResult.State);
            Assert.Equal(OperationState.Failure, typedResult.State);
            Assert.Equal(OperationState.Failure, typedResultWithValue.State);
            Assert.Equal(OperationState.Failure, typedResultWithValue2.State);
            Assert.Equal(OperationState.Failure, typedResultWithValue3.State);

            Assert.Equal("123", typedResultWithValue.Value);

            Assert.Equal(default, typedResultWithValue2.Value);
            Assert.True(typedResultWithValue2.ValueIsUndefined);

            Assert.Equal(default, typedResultWithValue3.Value);
            Assert.True(typedResultWithValue3.TryGetValue<int>(out var intValue));
            Assert.Equal(456, intValue);
        }

        [Fact]
        public void GivenAFailingResult_CallingSuccess_ShouldSetStateToSuccess()
        {
            // given
            var untypedResult = OperationResult.Failed();
            var unTypedResult2 = OperationResult.Failed();
            var typedResult = OperationResult.Failed<string>();
            var typedResultWithValue = OperationResult.Failed<string>("123");

            //when
            untypedResult.Success();
            unTypedResult2.Success("123");
            typedResult.Success();
            typedResultWithValue.Success("123");

            //then
            Assert.Equal(OperationState.Success, untypedResult.State);
            Assert.Equal(OperationState.Success, typedResult.State);
            Assert.Equal(OperationState.Success, typedResultWithValue.State);
            Assert.Equal("123", typedResultWithValue.Value);

            unTypedResult2.Value.TryGetValue<string>(out var unTypedResult2Value);
            Assert.Equal("123", unTypedResult2Value);
        }

        [Fact]
        public void GivenAResult_CallingTryGetResult_ShouldReturnValue()
        {
            //given
            var instance = new ChildRecordClass<Guid>()
            {
                AnIntValue = 1,
                AStringValue = "2",
                GenericValue = Guid.NewGuid()
            };
            var result = OperationResult.Succeeded(instance);

            //when
            var if1Result = result.TryGetValue(out IRecordClass1 recordClass1);
            var if2Result = result.TryGetValue(out IRecordClass2 recordClass2);
            var instance1 = (ChildRecordClass<Guid>)recordClass1;
            var instance2 = (ChildRecordClass<Guid>)recordClass2;

            //then
            Assert.True(if1Result);
            Assert.NotNull(recordClass1);
            Assert.Equal("2", recordClass1.AStringValue);

            Assert.True(if2Result);
            Assert.NotNull(recordClass2);
            Assert.Equal(1, recordClass2.AnIntValue);

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);
        }
    }
}

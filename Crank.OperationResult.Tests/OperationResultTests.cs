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

            //then
            Assert.Equal(OperationState.Undefined, untypedResult.State);
            Assert.Equal(OperationState.Undefined, typedResult.State);
        }

        [Fact]
        public void GivenOperationResults_WhenInitiatedWithUndefined_ShouldBeUndefined()
        {
            //given
            var untypedResult = OperationResult.Undefined();
            var typedResult = OperationResult.Undefined<string>();

            //then
            Assert.Equal(OperationState.Undefined, untypedResult.State);
            Assert.Equal(OperationState.Undefined, typedResult.State);
        }

        [Fact]
        public void GivenOperationResults_WhenInitiatedWithSuccess_ShouldBeSuccess()
        {
            //given
            var untypedResult = OperationResult.Successful();
            var typedResult = OperationResult.Successful<string>();
            var typedResultWithValue = OperationResult.Successful<string>("123");

            //then
            Assert.Equal(OperationState.Success, untypedResult.State);
            Assert.Equal(OperationState.Success, typedResult.State);
            Assert.Equal(OperationState.Success, typedResultWithValue.State);
            Assert.Equal("123", typedResultWithValue.Value);
        }

        [Fact]
        public void GivenAnUnTypeOperationResult_WhenInitiatedWithFail_ShouldBeFailure()
        {
            //given
            var untypedResult = OperationResult.Undefined().Fail();
            var typedResult = OperationResult.Undefined<string>().Fail<string>();
            var downTypedResult = OperationResult.Undefined<string>().Fail();
            var upTypedResult = OperationResult.Undefined().Fail<string>();

            //then
            Assert.Equal(OperationState.Failure, untypedResult.State);
            Assert.Equal(OperationState.Failure, typedResult.State);
            Assert.Equal(OperationState.Failure, downTypedResult.State);
            Assert.Equal(OperationState.Failure, upTypedResult.State);
        }

    }
}

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
        public void GivenOperationResults_WhenInitiatedWithSucceeded_ShouldBeSuccess()
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
        public void GivenAnUnTypeOperationResult_WhenInitiatedWithFail_ShouldBeFailure()
        {
            //given
            var untypedResult = OperationResult.Failed();
            var typedResult = OperationResult.Failed<string>();
            var typedResultWithValue = OperationResult.Failed<string, string>("123");

            //then
            Assert.Equal(OperationState.Failure, untypedResult.State);
            Assert.Equal(OperationState.Failure, typedResult.State);
        }

    }
}

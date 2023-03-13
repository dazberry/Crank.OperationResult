using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultMapTests
    {
        [Fact]
        public void WhenMappingResults_CallingMap_ShouldUpdateTheFirstResult()
        {
            //given
            var firstResult = OperationResult.Undefined();
            var secondResult = OperationResult.Undefined().Success();

            //when
            var copyOfFirstResult = firstResult.Map(secondResult);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.Equal(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, copyOfFirstResult.State);
        }

        [Fact]
        public void WhenMappingResults_CallingMapFunc_ShouldUpdateTheFirstResult()
        {
            //given
            var firstResult = OperationResult.Undefined();
            var secondResult = OperationResult.Undefined().Success();

            //when
            var copyOfFirstResult = firstResult.Map(() => secondResult);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.Equal(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, copyOfFirstResult.State);
        }


        [Fact]
        public void WhenMappingResults_CallingMap_ShouldUpdateTheFirstResultAndValue()
        {
            //given
            var firstResult = OperationResult.Undefined();
            var secondResult = OperationResult.Undefined().SuccessAs("123");

            //when
            var copyOfFirstResult = firstResult.Map(secondResult);
            var canExtractSuccessValue = copyOfFirstResult.SuccessValue.As<string>(out var successValue);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.Equal(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, copyOfFirstResult.State);

            Assert.True(canExtractSuccessValue);
            Assert.Equal("123", successValue.Value);
        }

        [Fact]
        public void WhenMappingResult_CallingMapForAFailedResult_ShouldNotUpdateTheResultOrValue()
        {
            //given
            var firstResult = OperationResult.Failed();
            var secondResult = OperationResult.Succeeded("123");

            //when
            var copyOfFirstResult = firstResult.Map(secondResult);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.NotEqual(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Failure, firstResult.State);
            Assert.Equal(OperationState.Failure, copyOfFirstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);

        }
    }
}

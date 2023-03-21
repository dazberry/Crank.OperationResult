using System;
using System.Threading.Tasks;
using Xunit;

namespace Crank.OperationResult.Tests
{

    public class OperationResultMapTests_Typed
    {
        [Fact]
        public void WhenMappingDifferentTypes_AndTheStateIsSuccess_StateIsCopiedButValueBecomesUndefined()
        {
            //given
            var firstResult = OperationResult.Undefined<int>();
            var secondResult = OperationResult.Succeeded("123");

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, newResult.State);


            Assert.Equal(default, firstResult.Value);
            Assert.True(firstResult.ValueIsUndefined);
            Assert.Equal("123", secondResult.Value);
            Assert.Equal(firstResult, newResult);
            Assert.Equal(default, newResult.Value);
            Assert.True(newResult.ValueIsUndefined);
        }

        [Fact]
        public void WhenMappingTypedResultsOfTheSameType_TheMappingResult_ShouldBeTheSame()
        {
            //given
            var firstResult = OperationResult.Undefined<string>();
            var secondResult = OperationResult.Succeeded("123");

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, newResult.State);

            Assert.Equal("123", firstResult.Value);
            Assert.Equal("123", secondResult.Value);
            Assert.Equal("123", newResult.Value);
        }

        [Fact]
        public void WhenMappingDifferentTypes_AndTheStateIsFailure_TheStateAndValueAreCopied()
        {
            //given
            var firstResult = OperationResult.Undefined<int>();
            var secondResult = OperationResult.Failed("123");

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Failure, firstResult.State);
            Assert.Equal(OperationState.Failure, secondResult.State);
            Assert.Equal(OperationState.Failure, newResult.State);


            Assert.Equal(firstResult, newResult);
            Assert.Equal(default, firstResult.Value);
            Assert.False(firstResult.ValueIsUndefined);
            Assert.Equal("123", secondResult.Value);
            Assert.Equal(default, newResult.Value);
            Assert.False(newResult.ValueIsUndefined);

            Assert.True(newResult.TryGetValue<string>(out var stringValue));
            Assert.Equal("123", stringValue);
        }

        [Fact]
        public void WhenMappingDifferentTypes_InvokingMapConvert_ShouldInvoke()
        {
            //given
            var intResult = OperationResult.Succeeded(123);
            var stringResult = OperationResult.Succeeded("456");

            //when
            intResult.MapConvert(stringResult,
                map => int.Parse(map));

            //then
            Assert.Equal(456, intResult.Value);
        }

        [Fact]
        public void WhenMappingToAFailedResult_WithDefaultFlags_TheMappingIsIgnored()
        {
            //given
            var failedResult = OperationResult.Failed("456");
            var successResult = OperationResult.Succeeded("123");

            //when
            var copyOfFailedResult = failedResult.Map(successResult);

            //then
            Assert.Equal(OperationState.Failure, failedResult.State);
            Assert.Equal(OperationState.Success, successResult.State);
            Assert.Equal(OperationState.Failure, copyOfFailedResult.State);

            Assert.Equal(copyOfFailedResult, failedResult);
            Assert.Equal("456", failedResult.Value);
            Assert.Equal("123", successResult.Value);
            Assert.Equal("456", copyOfFailedResult.Value);
        }

        [Fact]
        public void WhenMappingToAFailedResult_WithMapIfFlagSet_TheMappingIsApplied()
        {
            //given
            var failedResult = OperationResult
                .Failed("456")
                .SetOptions(opt => opt.MapIfSourceResultIsInStateOfFailure = true);
            var successResult = OperationResult.Succeeded("123");

            //when
            var copyOfFailedResult = failedResult.Map(successResult);

            //then
            Assert.Equal(OperationState.Success, failedResult.State);
            Assert.Equal(OperationState.Success, successResult.State);
            Assert.Equal(OperationState.Success, copyOfFailedResult.State);

            Assert.Equal(copyOfFailedResult, failedResult);
            Assert.Equal("123", failedResult.Value);
            Assert.Equal("123", successResult.Value);
            Assert.Equal("123", copyOfFailedResult.Value);
        }

        [Fact]
        public void WhenMappingToASuccessResult_IfUndefined_TheMappingIsIgnored()
        {
            //given
            var failedResult = OperationResult.Failed("456");
            var undefinedResult = OperationResult.Undefined<string>();

            //when
            var copyOfFailedResult = failedResult.Map(undefinedResult);

            //then
            Assert.Equal(OperationState.Failure, failedResult.State);
            Assert.Equal(OperationState.Undefined, undefinedResult.State);
            Assert.Equal(OperationState.Failure, copyOfFailedResult.State);

            Assert.Equal(copyOfFailedResult, failedResult);
            Assert.Equal("456", failedResult.Value);
            Assert.True(undefinedResult.ValueIsUndefined);
            Assert.Equal("456", copyOfFailedResult.Value);
        }

        [Fact]
        void WhenMappingUntypedToTypedResult_IfSuccessAndTheTypesMatch_BothStateAndValueShouldBeCopied()
        {
            //given
            var typedResult = OperationResult.Undefined<string>();
            var untypedResult = OperationResult.Undefined().Success("123");

            //when
            var copyOfTypedResult = typedResult.Map(untypedResult);

            //then
            Assert.Equal(copyOfTypedResult, typedResult);
            Assert.Equal("123", typedResult.Value);
        }

    }
}

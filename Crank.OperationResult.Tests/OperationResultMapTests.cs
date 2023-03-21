using System.Threading.Tasks;
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
        public async Task WhenMappingResults_CallingMapAsyncFunc_ShouldUpdateTheFirstResult()
        {
            //given
            var firstResult = OperationResult.Undefined();
            var secondResult = OperationResult.Undefined().Success();

            //when
            var copyOfFirstResult = await firstResult.MapAsync(
                async () =>
                {
                    await Task.CompletedTask;
                    return secondResult;
                });

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
            var secondResult = OperationResult.Succeeded<string>("123");

            //when
            var copyOfFirstResult = firstResult.Map(secondResult);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.Equal(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, copyOfFirstResult.State);

            Assert.Equal("123", secondResult.Value);
            Assert.Equal("123", copyOfFirstResult.TryGetValue<string>(out var value) ? value : default);
        }

        [Fact]
        public void WhenMappingResults_CallingMapForAFailedResult_ShouldNotUpdateTheResultOrValue()
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

        [Fact]
        public void WhenMappingResults_CallingMapForAFailedResultWithFlagSet_ShouldUpdateTheResultAndValue()
        {
            //given
            var firstResult = OperationResult
                .Failed()
                .SetOptions(opt => opt.MapIfSourceResultIsInStateOfFailure = true);
            var secondResult = OperationResult.Succeeded("123");

            //when
            var copyOfFirstResult = firstResult.Map(secondResult);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.Equal(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, copyOfFirstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.True(firstResult.TryGetValue<string>(out var stringValue));
            Assert.Equal("123", stringValue);
        }

        [Fact]
        public void WhenMappingATypedResult_TheMappingResult_ShouldBeATypedResult()
        {
            //given
            var untypedResult = OperationResult.Undefined();
            var typedResult = OperationResult.Succeeded("123");

            //when
            var newTypedResult = untypedResult.MapTo(typedResult);

            //then
            Assert.Equal(OperationState.Success, untypedResult.State);
            Assert.Equal(OperationState.Success, typedResult.State);
            Assert.Equal(OperationState.Success, newTypedResult.State);

            Assert.Equal("123", untypedResult.TryGetValue<string>(out var value) ? value : default);
            Assert.Equal("123", typedResult.Value);
            Assert.Equal("123", newTypedResult.Value);
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


    }
}

﻿using System.Threading.Tasks;
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
            Assert.True(firstResult.IsValueUndefined);
            Assert.Equal("123", secondResult.Value);
            Assert.Equal(firstResult, newResult);
            Assert.Equal(default, newResult.Value);
            Assert.True(newResult.IsValueUndefined);
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
            Assert.False(firstResult.IsValueUndefined);
            Assert.Equal("123", secondResult.Value);
            Assert.Equal(default, newResult.Value);
            Assert.False(newResult.IsValueUndefined);

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


    }
}

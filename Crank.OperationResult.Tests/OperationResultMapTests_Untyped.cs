using System.Threading.Tasks;
using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultMapTests_Untyped
    {
        [Fact]
        public void WhenMappingUntypedToUntyped_InvokingMap_ShouldUpdateTheFirstResult()
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
        void WhenMappingUntypedToUntyped_IfMapFromIsUndefined_ShouldNotUpdate()
        {
            //given
            var firstResult = OperationResult.Undefined();
            var secondResult = OperationResult.Undefined();

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Undefined, firstResult.State);
            Assert.Equal(OperationState.Undefined, secondResult.State);
            Assert.Equal(OperationState.Undefined, newResult.State);
        }

        [Fact]
        void WhenMappingUntypedToUntyped_IfMapFromIsNull_ShouldNotUpdate()
        {
            //given
            var firstResult = OperationResult.Undefined();
            OperationResult secondResult = null;

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Undefined, firstResult.State);
            Assert.Equal(OperationState.Undefined, newResult.State);
            Assert.Null(secondResult);
        }

        [Fact]
        public void WhenMappingUntypedToUntyped_InvokingMapOnAFailedResult_ShouldNotUpdate()
        {
            //given
            var firstResult = OperationResult.Undefined().Fail();
            var secondResult = OperationResult.Undefined().Success("123");

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Failure, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Failure, newResult.State);

            Assert.Equal(firstResult, newResult);
            Assert.True(firstResult.Value.IsUndefined);
        }

        [Fact]
        public void WhenMappingUntypedToUntyped_InvokingMapOnAFailedResultWithFlagSet_ShouldStillUpdate()
        {
            //given
            var firstResult = OperationResult
                .Undefined(opt => opt.MapIfSourceResultIsInStateOfFailure = true)
                .Fail();
            var secondResult = OperationResult.Undefined().Success("123");

            //when
            var newResult = firstResult.Map(secondResult);

            //then
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, newResult.State);

            Assert.Equal(firstResult, newResult);
            Assert.False(firstResult.Value.IsUndefined);
            Assert.True(firstResult.Value.TryGetValue<string>(out var stringValue));
            Assert.Equal("123", stringValue);
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
        public void WhenMappingFromATypedResult_CallingMap_ShouldUpdateTheFirstResultAndValue()
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
        public void WhenMappingFromATypedResult_CallingMapForAFailedResult_ShouldNotUpdateTheResultOrValue()
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
        public void WhenMappingFromATypedResult_CallingMapForAFailedResultWithFlagSet_ShouldUpdateTheResultAndValue()
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
        public void WhenMappingTypedToUnTypedWithMapTo_TheMappingResult_ShouldBeATypedResult()
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

    }
}

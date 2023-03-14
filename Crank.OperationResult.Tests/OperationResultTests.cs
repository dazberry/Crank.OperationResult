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
            Assert.True(untypedResult.IsStateUndefined);
            Assert.Equal(OperationState.Undefined, typedResult.State);
            Assert.True(typedResult.IsStateUndefined);
            Assert.Equal(OperationState.Undefined, untypedFromUndefined.State);
            Assert.True(untypedFromUndefined.IsStateUndefined);
            Assert.Equal(OperationState.Undefined, typedFromUndefined.State);
            Assert.True(typedFromUndefined.IsStateUndefined);
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
            Assert.True(typedResultWithValue2.IsValueUndefined);

            Assert.True(typedResultWithValue3.As<int>(out var intValue));
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

    }
}

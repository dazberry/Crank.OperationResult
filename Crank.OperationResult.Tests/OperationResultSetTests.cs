using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultSetTests
    {
        [Theory]        
        [InlineData(true, OperationState.Success)]
        [InlineData(false, OperationState.Failure)]
        public void WhenUsingTheSetMethod_TheSuccessParameter_ShouldCauseTheStateToChange(bool Success, OperationState expectedState)
        {
            //given
            var result = OperationResult.Undefined();

            //when 
            result.Set(Success);

            //then
            Assert.Equal(result.State, expectedState);
        }


        [Theory]
        [InlineData(true, OperationState.Success)]
        [InlineData(false, OperationState.Failure)]
        public void WhenUsingTheSetMethod_SettingTheResultAndValue_ShouldSetTheStateAndValue(bool Success, OperationState expectedState)
        {
            //given
            var untypedResultWithAString = OperationResult.Undefined();
            var untypedResultWithAnInt = OperationResult.Undefined();

            //when
            untypedResultWithAString.Set(Success, "ABCD");
            untypedResultWithAnInt.Set(Success, 1234);

            //then
            Assert.Equal(untypedResultWithAString.State, expectedState);
            Assert.True(untypedResultWithAString.TryGetValue<string>(out var aStringValue));
            Assert.False(untypedResultWithAString.TryGetValue<int>(out var _));
            Assert.Equal("ABCD", aStringValue);

            Assert.Equal(untypedResultWithAnInt.State, expectedState);
            Assert.True(untypedResultWithAnInt.TryGetValue<int>(out var anIntValue));
            Assert.False(untypedResultWithAnInt.TryGetValue<string>(out var _));
            Assert.Equal(1234, anIntValue);
        }

        [Theory]
        [InlineData(true, OperationState.Success)]
        [InlineData(false, OperationState.Failure)]
        public void WhenUsingTheSetMethodWithTypedResults_SettingTheResultAndValue_ShouldSetTheStateAndValue(bool Success, OperationState expectedState)
        {
            //given
            var typedResultWithAString = OperationResult.Undefined<string>();
            var typedResultWithAnInt = OperationResult.Undefined<int>();

            //when
            typedResultWithAString.Set(Success, "ABCD");
            typedResultWithAnInt.Set(Success, 1234);

            //then
            Assert.Equal(typedResultWithAString.State, expectedState);
            Assert.True(typedResultWithAString.TryGetValue<string>(out var aStringValue));
            Assert.False(typedResultWithAString.TryGetValue<int>(out var _));
            Assert.Equal("ABCD", aStringValue);
            Assert.Equal("ABCD", typedResultWithAString.Value);

            Assert.Equal(typedResultWithAnInt.State, expectedState);
            Assert.True(typedResultWithAnInt.TryGetValue<int>(out var anIntValue));
            Assert.False(typedResultWithAnInt.TryGetValue<string>(out var _));
            Assert.Equal(1234, anIntValue);
            Assert.Equal(1234, typedResultWithAnInt.Value);
        }

        [Fact]
        public void WhenSettingASuccessValueWithATypeMistmatch_IfOptionsAreStrict_ShouldThrowAnExpection()
        {
            //given
            var stringResult = OperationResult
                .Undefined<string>()
                .SetOptions(
                    opt => opt.ExpectedResultTypeChecking = OperationResultTypeChecking.Strict);
            var anIntValue = 1234;

            //when/then
            Assert.Throws<OperationResultExpectedTypeMismatchException>(() =>
            {
                stringResult.Set(true, anIntValue);
            });            
        }

        [Fact]
        public void WhenSettingASuccessValueWithATypeMistmatch_IfOptionsAreDiscard_ShouldIgnoretheValue()
        {
            //given
            var stringResult = OperationResult
                .Undefined<string>()
                .SetOptions(
                    opt => opt.ExpectedResultTypeChecking = OperationResultTypeChecking.Discard);
            var anIntValue = 1234;

            //when
            stringResult.Set(true, anIntValue);

            //then
            Assert.True(stringResult.ValueIsUndefined);            
        }

        [Fact]
        public void WhenSettingASuccessValueWithATypeMistmatch_IfOptionsAreIgnore_ShouldSetTheValue()
        {
            //given
            var stringResult = OperationResult
                .Undefined<string>()
                .SetOptions(
                    opt => opt.ExpectedResultTypeChecking = OperationResultTypeChecking.Ignore);
            var anIntValue = 1234;

            //when
            stringResult.Set(true, anIntValue);

            //then
            Assert.False(stringResult.ValueIsUndefined);
            Assert.Equal(default, stringResult.Value);
            Assert.True(stringResult.TryGetValue<int>(out var returnedIntValue));
            Assert.Equal(anIntValue, returnedIntValue);
        }
    }
}

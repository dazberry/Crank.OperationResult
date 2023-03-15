using Xunit;

namespace Crank.OperationResult.Tests
{
    public class GenericValueTests
    {
        [Fact]
        public void GivenAnUndefinedValue_IsUndefined_ShouldBeTrue()
        {
            //given
            IGenericValue value = UndefinedGenericValue.GetInstance();

            //when
            bool isUndefined = value.IsUndefined;

            //then
            Assert.True(isUndefined);
        }

        [Fact]
        public void GivenAnUndefinedValue_InvokeChangingValue_ShouldCreateANewValue()
        {
            //given
            IGenericValue value = UndefinedGenericValue.GetInstance();

            //when
            var newValue = value.ChangeValue<string>("123");

            //then
            Assert.False(value.TryGetValue<string>(out var _));
            Assert.True(value.IsUndefined);
            Assert.NotEqual(newValue, value);
            Assert.True(newValue.TryGetValue<string>(out var newValueValue));
            Assert.Equal("123", newValueValue);
        }

        [Fact]
        public void GivenAGenericValue_InvokeChangingValueOfTheSameType_ShouldUpdateTheValueOnly()
        {
            //given
            IGenericValue value = new GenericValue<string>("123");

            //when
            var newValue = value.ChangeValue<string>("456");

            //then
            Assert.Equal(newValue, value);
            Assert.True(value.TryGetValue<string>(out var orginalValueString));
            Assert.NotEqual("123", orginalValueString);
            Assert.Equal("456", orginalValueString);
            Assert.True(newValue.TryGetValue<string>(out var newValueString));
            Assert.Equal("456", newValueString);
        }

        [Fact]
        public void GivenAGenericValue_InvokeChangingValueOfADifferentType_ShouldCreateANewValue()
        {
            //given
            IGenericValue value = new GenericValue<string>("123");

            //when
            var newValue = value.ChangeValue<int>(456);

            //then
            Assert.NotEqual(newValue, value);
            Assert.True(value.TryGetValue<string>(out var orginalValueString));
            Assert.Equal("123", orginalValueString);
            Assert.False(value.TryGetValue<int>(out var _));

            Assert.False(newValue.TryGetValue<string>(out var _));
            Assert.True(newValue.TryGetValue<int>(out var newIntValue));
            Assert.Equal(456, newIntValue);
        }

    }
}

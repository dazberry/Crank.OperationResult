using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultMessageTests
    {
        [Theory]
        [InlineData(OperationState.Undefined)]
        [InlineData(OperationState.Success)]
        [InlineData(OperationState.Failure)]
        public void GivenAnOperationResult_SettingAMessage_ShouldStoreTheMessage(OperationState operationState)
        {
            //given
            var result = OperationResult.Undefined();
            switch (operationState)
            {
                case OperationState.Success:
                    result.Success();
                    break;
                case OperationState.Failure:
                    result.Fail();
                    break;
                default:
                    break;
            }

            //when
            result.WithMessage("123");

            //then
            Assert.Equal("123", result.Message);
        }

        [Theory]
        [InlineData(OperationState.Undefined)]
        [InlineData(OperationState.Success)]
        [InlineData(OperationState.Failure)]
        public void GivenATypedOperationResult_SettingAMessage_ShouldStoreTheMessage(OperationState operationState)
        {
            //given
            var result = OperationResult.Succeeded("ABC");
            switch (operationState)
            {
                case OperationState.Success:
                    result.Success();
                    break;
                case OperationState.Failure:
                    result.Fail();
                    break;
                default:
                    break;
            }

            //when
            result.WithMessage("123");

            //then
            Assert.Equal("123", result.Message);
        }

        [Fact]
        public void GivenAnUntypedOperationResult_MappingAResultWithAMessage_ShouldCopyTheMessage()
        {
            //given
            var result = OperationResult.Undefined();
            var resultWithMessage = OperationResult.Undefined().WithMessage("123").Success();

            //when
            result.Map(resultWithMessage);

            //then
            Assert.Equal(resultWithMessage.Message, result.Message);
        }

        [Fact]
        public void GivenATypedOperationResult_MappingAResultWithAMessage_ShouldCopyTheMessage()
        {
            //given
            var result = OperationResult.Undefined<string>();
            var resultWithMessage = OperationResult.Undefined().WithMessage("123").Success();

            //when
            result.Map(resultWithMessage);

            //then
            Assert.Equal(resultWithMessage.Message, result.Message);
        }

        [Fact]
        public void GivenTypedOperationResults_WithTheSameTypes_ShouldCopyTheMessage()
        {
            //given
            var result = OperationResult.Undefined<string>();
            var resultWithMessage = OperationResult.Succeeded("ABC").WithMessage("123");

            //when
            result.Map(resultWithMessage);

            //then
            Assert.Equal(resultWithMessage.Message, result.Message);
        }


        [Fact]
        public void GivenTypedOperationResults_WithDifferentTypes_ShouldCopyTheMessage()
        {
            //given
            var result = OperationResult.Undefined<string>();
            var resultWithMessage = OperationResult.Succeeded(456).WithMessage("123");

            //when
            result.Map(resultWithMessage);

            //then
            Assert.Equal(resultWithMessage.Message, result.Message);
        }

        [Fact]
        public void GivenTypedOperationResults_WhenInvokingMapConvert_ShouldCopyTheMessage()
        {
            //given
            var result = OperationResult.Undefined<string>();
            var resultWithMessage = OperationResult.Succeeded(456).WithMessage("123");

            //when
            result.MapConvert(resultWithMessage, res => $"{res}");

            //then
            Assert.Equal(resultWithMessage.Message, result.Message);
        }
    }
}


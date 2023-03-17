using System;
using System.Linq;
using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultMatchTests
    {       
        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfTheTypeIsAString_ShouldInvokeTheMatchByType()
        {
            //given            
            var result = OperationResult.Undefined<string>().Success("1234");
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Default(
                    res => expectedValueType = "default")
                .Matched;

            //then
            Assert.True(matched);
            Assert.Equal(typeof(string).ToString(), expectedValueType);            
        }

        [Fact]
        public void WhenMatchingAgainstAnUntypedOperationResult_IfTheTypeIsAString_ShouldInvokeTheMatchByType()
        {
            //given
            var result = OperationResult.Undefined().Success("ABCD");
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Default(
                    res => expectedValueType = "default")
                .Matched;


            //then
            Assert.True(matched);
            Assert.Equal(typeof(string).ToString(), expectedValueType);
        }



        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfTheTypeIsAnInteger_ShouldInvokeTheMatchByType()
        {
            //given
            var result = OperationResult.Undefined<string>();
            result.Success(1234);
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Default(
                    res => expectedValueType = "default")
                .Matched;


            //then
            Assert.True(matched);
            Assert.Equal(typeof(int).ToString(), expectedValueType);
        }

        [Fact]
        public void WhenMatchingAgainstAnUntypedOperationResult_IfTheTypeIsAnInteger_ShouldInvokeTheMatchByType()
        {
            //given
            var result = OperationResult.Undefined();
            result.Success(1234);
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Default(
                    res => expectedValueType = "default")
                .Matched;


            //then
            Assert.True(matched);
            Assert.Equal(typeof(int).ToString(), expectedValueType);
        }

        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfNoMatchIsFound_MatchedShouldBeFalse()
        {
            //given
            var result = OperationResult.Succeeded(Guid.NewGuid());
            var undefined = "undefined";
            var expectedValueType = undefined;

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Matched;


            //then
            Assert.False(matched);
            Assert.Equal(undefined, expectedValueType);
        }

        [Fact]
        public void WhenMatchingAgainstAnUnTypedOperationResult_IfNoMatchIsFound_MatchedShouldBeFalse()
        {
            //given
            var result = OperationResult.Undefined().Success(Guid.NewGuid());
            var undefined = "undefined";
            var expectedValueType = undefined;

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Matched;


            //then
            Assert.False(matched);
            Assert.Equal(undefined, expectedValueType);
        }

        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfNoMatchIsFound_DefaultShouldBeCalled()
        {
            //given
            var result = OperationResult.Succeeded(Guid.NewGuid());
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Default(
                    res => expectedValueType = "default")
                .Matched;

            //then
            Assert.False(matched);
            Assert.Equal("default", expectedValueType);
        }

        [Fact]
        public void WhenMatchingAgainstAnUntypedOperationResult_IfNoMatchIsFound_DefaultShouldBeCalled()
        {
            //given
            var result = OperationResult.Undefined().Success(Guid.NewGuid());
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match<Exception>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<int>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<string>(
                    value => expectedValueType = value.GetType().ToString())
                .Match<double>(
                    value => expectedValueType = value.GetType().ToString())
                .Default(
                    res => expectedValueType = "default")
                .Matched;

            //then
            Assert.False(matched);
            Assert.Equal("default", expectedValueType);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenMatchingAgainstATypedOperationResult_IfSpecifyingBothAStateAndType_BothShouldConditionTheMatch(bool success)
        {
            //given
            var result = OperationResult.Undefined<string>().Set(success, "1234");
            var operationState = "not specified";
            var expectedOperationState = success ? $"{OperationState.Success}" : $"{OperationState.Failure}";

            //when
            var matched = result                    
                .Match<string>(
                    OperationState.Undefined,
                    value => operationState = $"{OperationState.Undefined}")
                .Match<string>(
                    OperationState.Success,
                    value => operationState = $"{OperationState.Success}")
                .Match<string>(
                    OperationState.Failure,
                    value => operationState = $"{OperationState.Failure}")
                .Default(
                    res => operationState = "default")
                .Matched;

            //then
            Assert.True(matched);
            Assert.Equal(expectedOperationState, operationState);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WhenMatchingAgainstAnUntypedOperationResult_IfSpecifyingBothAStateAndType_BothShouldConditionTheMatch(bool success)
        {
            //given
            var result = OperationResult.Undefined().Set(success, "1234");
            var operationState = "not specified";
            var expectedOperationState = success ? $"{OperationState.Success}" : $"{OperationState.Failure}";

            //when
            var matched = result
                .Match<string>(
                    OperationState.Undefined,
                    value => operationState = $"{OperationState.Undefined}")
                .Match<string>(
                    OperationState.Success,
                    value => operationState = $"{OperationState.Success}")
                .Match<string>(
                    OperationState.Failure,
                    value => operationState = $"{OperationState.Failure}")
                .Default(
                    res => operationState = "default")
                .Matched;

            //then
            Assert.True(matched);
            Assert.Equal(expectedOperationState, operationState);
        }       
        
        [Theory]
        [InlineData(OperationState.Undefined)]
        [InlineData(OperationState.Success)]
        [InlineData(OperationState.Failure)]
        public void WhenMatchingAgainstATypedOperationResult_OnStateOnly_TheMatchShouldBeRegardlessOfValueType(OperationState operationState)
        {
            var result = OperationResult.Undefined<string>();
            if (operationState != OperationState.Undefined)
                result.Set(operationState == OperationState.Success);
            var expectedValue = $"{operationState}";            
            var actualValue = "Not specified";

            //when
            var matched = result
                .Match(
                    OperationState.Undefined,
                    res => actualValue = $"{OperationState.Undefined}")
                .Match(
                    OperationState.Success,
                    res => actualValue = $"{OperationState.Success}")
                .Match(
                    OperationState.Failure,
                    res => actualValue = $"{OperationState.Failure}")
                .Matched;

            //then
            Assert.True(matched);
            Assert.Equal(expectedValue, actualValue);
        }

        [Theory]
        [InlineData(OperationState.Undefined)]
        [InlineData(OperationState.Success)]
        [InlineData(OperationState.Failure)]
        public void WhenMatchingAgainstAnUntypedOperationResult_OnStateOnly_TheMatchShouldBeRegardlessOfValueType(OperationState operationState)
        {
            var result = OperationResult.Undefined();
            if (operationState != OperationState.Undefined)
                result.Set(operationState == OperationState.Success);
            var expectedValue = $"{operationState}";
            var actualValue = "Not specified";

            //when
            var matched = result
                .Match(
                    OperationState.Undefined,
                    res => actualValue = $"{OperationState.Undefined}")
                .Match(
                    OperationState.Success,
                    res => actualValue = $"{OperationState.Success}")
                .Match(
                    OperationState.Failure,
                    res => actualValue = $"{OperationState.Failure}")
                .Matched;

            //then
            Assert.True(matched);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfStateIsUndefined_AMatchShouldNotBePossibleAsNotTypeIsAvailable()
        {
            var result = OperationResult.Undefined<string>();
            var operationState = "not specified";

            //when
            var matched = result
                .Match<string>(
                    OperationState.Undefined,
                    value => operationState = $"{OperationState.Undefined}")              
                .Matched;

            //then
            Assert.False(matched);
            Assert.Equal("not specified", operationState);
        }

        [Fact]
        public void WhenMatchingAgainstAnOperationResult_WhereThereAreMultipleValidMatches_AllMatchesShouldBeCalled()
        {
            var result = OperationResult.Succeeded<string>("1234");
            var matches = new bool[] { false, false, false, false, false, false, false, false, false };
            var expectedResult = new bool[] { true, false, true, true, false, true, false, true, false };
            
            //when
            var matched = result
                .Match<string>(
                    value => matches[0] = true)
                .Match<int>(
                    value => matches[1] = true)
                .Match(
                    OperationState.Success,
                    value => matches[2] = true)
                .Match<string>(
                    value => matches[3] = true)
                .Match<int>(
                    OperationState.Success,
                    value => matches[4] = true)
                .Match<string>(
                    OperationState.Success,
                    value => matches[5] = true)
                .Match<Guid>(
                    OperationState.Success,
                    value => matches[6] = true)
                .Match(
                    OperationState.Success,
                    value => matches[7] = true)
                .Match(
                    OperationState.Failure,
                    value => matches[8] = true)
                .Matched;

            //then
            Assert.True(matched);
            Assert.Equal(5, matches.Count(x => x));
            Assert.Equal(expectedResult, matches);            
        }






    }
}

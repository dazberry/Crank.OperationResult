using System;
using System.Linq;
using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultMatchTests
    {
        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_OnEmptyMatch_WillReturnFalse()
        {
            //given
            var result = OperationResult.Undefined<string>().Success("1234");

            //when
            var matched = result
                .Match(m => { });

            //then
            Assert.False(matched);
        }

        [Fact]
        public void WhenMatchingAgainstAnUntypedOperationResult_OnEmptyMatch_WillReturnFalse()
        {
            //given
            var result = OperationResult.Undefined().Success("1234");

            //when
            var matched = result
                .Match(m => { });

            //then
            Assert.False(matched);
        }

        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfTheTypeIsAString_ShouldInvokeTheMatchByType()
        {
            //given
            var result = OperationResult.Undefined<string>().Success("1234");
            var expectedValueType = "undefined";

            //when
            var matched = result
                .Match(m =>
                    m.ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                    .Default(
                        res => expectedValueType = "default"));


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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                    .Default(
                        res => expectedValueType = "default")
                );

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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                    .Default(
                        res => expectedValueType = "default")
                );

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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                    .Default(
                        res => expectedValueType = "default")
                );

            //then
            Assert.True(matched);
            Assert.Equal(typeof(int).ToString(), expectedValueType);
        }

        [Fact]
        public void WhenMatchingAgainstATypedOperationResult_IfMatchingTheTypeAndValue_MatchedShouldBeTrue()
        {
            //given
            var result = OperationResult.Succeeded(204);
            var intResult = 0;

            //when
            var matched = result
                .Match(m => m
                    .ValueIsEquals<int>(200, value => intResult = value)
                    .ValueIsEquals<int>(204, value => intResult = value)
                    .ValueIsEquals<int>(500, value => intResult = value)
                );

            //then
            Assert.Equal(204, intResult);
        }

        [Fact]
        public void WhenMatchingAgainstAnUnTypedOperationResult_IfMatchingTheTypeAndValue_MatchedShouldBeTrue()
        {
            //given
            var result = OperationResult.Undefined().Success(204);
            var intResult = 0;

            //when
            var matched = result
                .Match(m => m
                    .ValueIsEquals<int>(200, value => intResult = value)
                    .ValueIsEquals<int>(204, value => intResult = value)
                    .ValueIsEquals<int>(500, value => intResult = value)
                );

            //then
            Assert.Equal(204, intResult);
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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                );

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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                );

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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                    .Default(
                        res => expectedValueType = "default")
                );

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
                .Match(m => m
                    .ValueIs<Exception>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<int>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<string>(
                        value => expectedValueType = value.GetType().ToString())
                    .ValueIs<double>(
                        value => expectedValueType = value.GetType().ToString())
                    .Default(
                        res => expectedValueType = "default")
                );

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
                .Match(m => m
                    .ValueAndStateAre<string>(
                        OperationState.Undefined,
                        value => operationState = $"{OperationState.Undefined}")
                    .ValueAndStateAre<string>(
                        OperationState.Success,
                        value => operationState = $"{OperationState.Success}")
                    .ValueAndStateAre<string>(
                        OperationState.Failure,
                        value => operationState = $"{OperationState.Failure}")
                    .Default(
                        res => operationState = "default")
                );


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
                .Match(m => m
                    .ValueAndStateAre<string>(
                        OperationState.Undefined,
                        value => operationState = $"{OperationState.Undefined}")
                    .ValueAndStateAre<string>(
                        OperationState.Success,
                        value => operationState = $"{OperationState.Success}")
                    .ValueAndStateAre<string>(
                        OperationState.Failure,
                        value => operationState = $"{OperationState.Failure}")
                    .Default(
                        res => operationState = "default")
                );

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
                .Match(m => m
                    .StateIs(
                        OperationState.Undefined,
                        res => actualValue = $"{OperationState.Undefined}")
                    .StateIs(
                        OperationState.Success,
                        res => actualValue = $"{OperationState.Success}")
                    .StateIs(
                        OperationState.Failure,
                        res => actualValue = $"{OperationState.Failure}")
                );

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
                .Match(m =>m
                    .StateIs(
                        OperationState.Undefined,
                        res => actualValue = $"{OperationState.Undefined}")
                    .StateIs(
                        OperationState.Success,
                        res => actualValue = $"{OperationState.Success}")
                    .StateIs(
                        OperationState.Failure,
                        res => actualValue = $"{OperationState.Failure}")
                );

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
                .Match(m =>
                    m.ValueAndStateAre<string>(
                        OperationState.Undefined,
                        value => operationState = $"{OperationState.Undefined}")
                );

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
                .Match(m =>m
                    .ValueIs<string>(
                        value => matches[0] = true)
                    .ValueIs<int>(
                        value => matches[1] = true)
                    .StateIs(
                        OperationState.Success,
                        value => matches[2] = true)
                    .ValueIs<string>(
                        value => matches[3] = true)
                    .ValueAndStateAre<int>(
                        OperationState.Success,
                        value => matches[4] = true)
                    .ValueAndStateAre<string>(
                        OperationState.Success,
                        value => matches[5] = true)
                    .ValueAndStateAre<Guid>(
                        OperationState.Success,
                        value => matches[6] = true)
                    .StateIs(
                        OperationState.Success,
                        value => matches[7] = true)
                    .StateIs(
                        OperationState.Failure,
                        value => matches[8] = true)
                );

            //then
            Assert.True(matched);
            Assert.Equal(5, matches.Count(x => x));
            Assert.Equal(expectedResult, matches);
        }

        [Fact]
        public void WhenMatchingToAgainstATypedOperationResult_OnEmptyMatch_WillReturnTheDefaultResult()
        {
            //given
            var defaultValue = "1234";
            var result = OperationResult.Undefined<string>().Success("5678");

            //when
            var matchValue = result
                .MatchTo(m => { }, defaultValue);

            //then
            Assert.Equal(defaultValue, matchValue);
        }

        [Fact]
        public void WhenMatchingToAgainstAnUntypedOperationResult_OnEmptyMatch_WillReturnTheDefaultResult()
        {
            //given
            var defaultValue = "1234";
            var result = OperationResult.Undefined().Success("1234");

            //when
            var matchValue = result
                .MatchTo(m => { }, defaultValue);

            //then
            Assert.Equal(defaultValue, matchValue);
        }

        [Fact]
        public void WhenMatchingToAnOperationResult_TheMatchResult_ShouldBeReturned()
        {
            //given
            var intValue = 1234;
            var result = OperationResult.Succeeded<int>(intValue);

            //when
            var stringResult =
                result.MatchTo<string>(m => m
                    .ValueIs<string>(value => m.Result = value)
                    .ValueIs<int>(value => m.Result = $"{value}")
                );

            //then
            Assert.Equal($"{intValue}", stringResult);
        }

        [Fact]
        public void WhenMatchingToAnOperationResult_IfNoMatchsOccur_ReturnTheDefaultValue()
        {
            //given
            var guidValue = Guid.NewGuid();
            var result = OperationResult.Succeeded<Guid>(guidValue);

            //when
            var stringResult =
                result.MatchTo(m => m
                    .ValueIs<string>(value => m.Result = value)
                    .ValueIs<int>(value => m.Result = $"{value}"),
                    defaultResult: $"{guidValue}");

            //then
            Assert.Equal($"{guidValue}", stringResult);
        }

    }
}

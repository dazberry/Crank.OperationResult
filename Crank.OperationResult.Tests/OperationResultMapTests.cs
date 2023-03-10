using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void WhenMappingResults_CallingMap_ShouldUpdateTheFirstResultAndValue()
        {
            //given
            var firstResult = OperationResult.Undefined();
            var secondResult = OperationResult.Undefined().Success("123");

            //when
            var copyOfFirstResult = firstResult.Map(secondResult);
            var canExtractSuccessValue = copyOfFirstResult.SuccessValue.TryGetValue<string>(out var successValue);

            //then
            Assert.NotEqual(firstResult, secondResult);
            Assert.Equal(firstResult, copyOfFirstResult);
            Assert.Equal(firstResult.State, secondResult.State);
            Assert.Equal(OperationState.Success, firstResult.State);
            Assert.Equal(OperationState.Success, secondResult.State);
            Assert.Equal(OperationState.Success, copyOfFirstResult.State);

            Assert.True(canExtractSuccessValue);
            Assert.Equal("123", successValue);
        }
    }
}

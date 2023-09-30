using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Crank.OperationResult.Tests
{
    public class OperationResultMapAfterFailTests
    {
        [Fact]
        public void GivenAFailedResult_WithDefaultOptions_CallingMap_ShouldNotInvokeTheDelegate()
        {
            //given
            var result = new OperationResult().Fail();

            //when
            result.Map(() =>
            {
                Assert.True(false);
                return OperationResult.Undefined().Success();
            });

            //then
            Assert.False(result.HasSucceeded);
        }

        [Fact]
        public void GivenAFailedResult_WithFlagSet_CallingMap_ShouldInvokeTheDelegate()
        {
            //given
            var result = new OperationResult().Fail();
            result.SetOptions(opt => opt.MapIfSourceResultIsInStateOfFailure = true);

            //when
            result.Map(() =>
            {
                Assert.True(true);
                return OperationResult.Undefined().Success();
            });

            //then
            Assert.True(result.HasSucceeded);
        }


        [Fact]
        public async Task GivenAFailedResult_WithDefaultOptions_CallingMapAsync_ShouldNotInvokeTheDelegate()
        {
            //given
            var result = new OperationResult().Fail();

            //when
            await result.MapAsync(async () =>
            {
                Assert.True(false);
                await Task.CompletedTask;
                return OperationResult.Undefined().Success();
            });

            //then
            Assert.False(result.HasSucceeded);
        }

        [Fact]
        public async Task GivenAFailedResultWithFlagSet_CallingMapAsync_ShouldInvokeTheDelegate()
        {
            //given
            var result = new OperationResult().Fail()
                .SetOptions(opt => opt.MapIfSourceResultIsInStateOfFailure = true);

            //when
            await result.MapAsync(async () =>
            {
                await Task.CompletedTask;
                return OperationResult.Undefined().Success();
            });

            //then
            Assert.True(result.HasSucceeded);
        }

        [Fact]
        public void GivenAFailedResultWithDefaultOptions_CallingMapT_ShouldNotMapTheResult()
        {
            //given
            var failedResult = OperationResult.Undefined();
            failedResult.Fail();
            var succeededResult = OperationResult.Succeeded(101);


            //when
            failedResult.MapTo<int>(succeededResult);

            //then
            Assert.False(failedResult.HasSucceeded);
            Assert.False(failedResult.TryGetValue<int>(out int _));
        }

        [Fact]
        public void GivenAFailedResultWithFlagSet_CallingMapT_ShouldNotMapTheResult()
        {
            //given
            var failedResult = OperationResult.Undefined();
            failedResult.Fail();
            failedResult.SetOptions(opt => opt.MapIfSourceResultIsInStateOfFailure = true);
            var succeededResult = OperationResult.Succeeded(101);


            //when
            failedResult.MapTo<int>(succeededResult);

            //then
            Assert.True(failedResult.HasSucceeded);
            Assert.True(failedResult.TryGetValue<int>(out int _));
        }

        [Fact]
        public void GivenAFailedResultWithDefaultOptions_CallingMapTo_ShouldNotMaptheValue()
        {
            //given
            var failedResult = OperationResult.Undefined();
            failedResult.Fail();
            var succeededResult = OperationResult.Succeeded(101);


            //when
            failedResult.Map<int>(succeededResult);

            //then
            Assert.False(failedResult.HasSucceeded);
        }

        [Fact]
        public void GivenAFailedResultWithFlagSet_CallingMapTo_ShouldMaptheValue()
        {
            //given
            var failedResult = OperationResult.Undefined();
            failedResult.Fail();
            failedResult.SetOptions(act => act.MapIfSourceResultIsInStateOfFailure = true);
            var succeededResult = OperationResult.Succeeded(101);


            //when
            failedResult.Map<int>(succeededResult);

            //then
            Assert.True(failedResult.HasSucceeded);
        }

    }
}

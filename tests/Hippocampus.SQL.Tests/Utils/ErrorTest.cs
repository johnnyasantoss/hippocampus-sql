using System;
using Xunit;
using HippocampusSql.Utils;

namespace HippocampusSql.Tests.Utils
{
    public class ErrorTest
    {
        [Fact]
        public void CheckArgumentNullShouldThrowWhenNull(object myNullParam)
        {

            Assert.Throws<ArgumentNullException>(nameof(myNullParam), () => myNullParam.CheckNull(nameof(myNullParam)));
        }
    }
}

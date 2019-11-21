// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Xunit;

namespace Contoso.Android.FuncTest
{
    public class SomeTest
    {
        [Fact]
        public void MyTest()
        {
            Assert.False(false);
        }

        [Fact]
        public void MyOtherTest()
        {
            Assert.Equal(1, 2);
        }
    }
}

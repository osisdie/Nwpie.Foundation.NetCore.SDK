using System;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Statics;
using Xunit;

namespace Nwpie.xUnit.Foundation.Extension
{
    public class DateTimeExtension_Test
    {
        [Fact]
        public void MinValue_Test()
        {
            {
                var dt = default(DateTime);
                var adjusted = dt.EscapeMinValue();
                Assert.Null(adjusted);
            }

            {
                var dt = DateTime.MinValue;
                var adjusted = dt.EscapeMinValue();
                Assert.Null(adjusted);
            }

            {
                DateTime? dt = null;
                var adjusted = dt.EscapeMinValue();
                Assert.Null(adjusted);
            }

            {
                var dt = DateTime.MinValue.ToUniversalTime();
                Assert.Equal(DateTime.MinValue, dt);
                var adjusted = dt.EscapeMinValue();
                Assert.Null(adjusted);
            }

            {
                var dt = CommonConst.UnixBaseTime;
                Assert.Equal(new DateTime(1970, 1, 1), dt);
                var adjusted = dt.EscapeMinValue();
                Assert.Null(adjusted);
            }
        }
    }
}

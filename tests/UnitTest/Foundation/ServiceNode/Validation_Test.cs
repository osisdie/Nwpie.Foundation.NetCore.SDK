using Nwpie.Foundation.Abstractions.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class Validation_Test : TestBase
    {
        public Validation_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "TODO")]
        public void Validation_Fail_Test()
        {
            var request = new EchoTest_RequestModel()
            {
                RequestString = string.Empty
            };

            var result = ValidateUtils.Validate(request);
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Data);
        }
    }
}

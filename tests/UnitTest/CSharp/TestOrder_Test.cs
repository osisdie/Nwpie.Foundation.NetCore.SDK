using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.CSharp
{
    public class TestOrder_Test : IClassFixture<TestCollectionFixture>
    {
        public TestOrder_Test(ITestOutputHelper output, TestCollectionFixture fixture)
        {
            m_Output = output;
            m_fixture = fixture;
        }

        [Fact, Trait("TestOrder", "1")]
        //[Fact, Order(1)]
        public void Test1()
        {
            m_fixture.Test1Called = true;
            m_Output.WriteLine("Test1 Called");
            Assert.False(m_fixture.Test2Called);
            Assert.False(m_fixture.Test3Called);
        }

        [Fact, Trait("TestOrder", "2")]
        //[Fact, Order(2)]
        public void Test2()
        {
            m_fixture.Test2Called = true;
            m_Output.WriteLine("Test2 Called");
            Assert.True(m_fixture.Test1Called);
            Assert.False(m_fixture.Test3Called);
        }

        [Fact, Trait("TestOrder", "3")]
        //[Fact, Order(3)]
        public void Test3()
        {
            m_fixture.Test3Called = true;
            m_Output.WriteLine("Test3 Called");
            Assert.True(m_fixture.Test1Called);
            Assert.True(m_fixture.Test2Called);
        }

        private readonly ITestOutputHelper m_Output;
        private readonly TestCollectionFixture m_fixture;
    }
}

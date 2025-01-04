using System;

namespace Nwpie.xUnit.CSharp
{
    public class TestCollectionFixture : IDisposable
    {
        public bool Test1Called { get; set; }
        public bool Test2Called { get; set; }
        public bool Test3Called { get; set; }

        public TestCollectionFixture()
        {
            Test1Called = false;
            Test2Called = false;
            Test3Called = false;
        }

        public void Dispose()
        {
            // Cleanup
        }
    }
}

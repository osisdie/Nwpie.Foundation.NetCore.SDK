using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Extensions;
using Xunit;

namespace Nwpie.xUnit.Foundation.Extension
{
    public class InputExtension_Test
    {
        [Fact]
        public void InRange_Test()
        {
            const int min = 1;
            const int max = 5;

            {
                var before = min - 1;
                var after = before.AssignInRange(min, max);
                Assert.Equal(min, after);
            }

            {
                int? before = null;
                var after = before.AssignInRange(min, max);
                Assert.Equal(min, after);
            }

            {
                int? before = max + 1;
                var after = before.AssignInRange(min, max);
                Assert.Equal(max, after);
            }

            {
                var connectTimeout = 0;
                connectTimeout = connectTimeout.AssignInRange(1000, 5000);
                Assert.Equal(1000, connectTimeout);
            }

            {
                int? connectTimeout = 0;
                connectTimeout = connectTimeout.AssignInRange(1000, 5000);
                Assert.Equal(1000, connectTimeout);
            }
        }

        [Fact]
        public void AssignIf_Test()
        {
            {
                int? connectTimeout = null;
                connectTimeout = connectTimeout.AssignIf(x => x < 1000, 5000);
                Assert.Null(connectTimeout);
            }

            {
                int? connectTimeout = null;
                connectTimeout = connectTimeout.AssignIf(x => true != x >= 1000, 5000);
                Assert.Equal(5000, connectTimeout);
            }

            {
                var connectTimeout = 0;
                connectTimeout = connectTimeout.AssignIf(x => x < 1000, 5000);
                Assert.Equal(5000, connectTimeout);
            }
        }

        [Fact]
        public void AssignIfNotSet_Test()
        {
            {
                string name = null;
                name = name.AssignIfNotSet(nameof(name));
                Assert.Equal(nameof(name), name);
            }

            {
                var name = string.Empty;
                name = name.AssignIfNotSet(nameof(name));
                Assert.Equal(nameof(name), name);
            }

            {
                int? connectTimeout = null;
                connectTimeout = connectTimeout.AssignIfNotSet(5000);
                Assert.Equal(5000, connectTimeout);
            }

            {
                var connectTimeout = 0;
                connectTimeout = connectTimeout.AssignIfNotSet(5000);
                Assert.Equal(5000, connectTimeout);
            }

            {
                List<string> o = null;
                o = o.AssignIfNotSet(new List<string>());
                Assert.NotNull(o);
            }

            {
                Dictionary<string, string> o = null;
                o = o.AssignIfNotSet(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                Assert.NotNull(o);
            }
        }


        [Fact]
        public void String_AssignIfNotSet_Test()
        {
            var origValue = string.Empty;
            const string newValue = "newValue";

            {
                string o = null;
                o = o.AssignIfNotSet(newValue);
                Assert.Equal(newValue, o);
            }

            {
                var o = origValue;
                o = o.AssignIfNotSet(newValue);
                Assert.Equal(newValue, o);
            }

            {
                var o = origValue;
                o = o.AssignIfNotSet(newValue, allowEmptyString: true);
                Assert.Equal(origValue, o);
            }
        }
    }
}

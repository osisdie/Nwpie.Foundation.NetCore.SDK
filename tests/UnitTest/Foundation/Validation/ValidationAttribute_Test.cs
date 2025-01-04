using System;
using System.Collections.Generic;
using Nwpie.Foundation.Abstractions.Utilities;
using Nwpie.Foundation.Abstractions.Validation.Attributes;
using Xunit;

namespace Nwpie.xUnit.Foundation.Validation
{
    public class ValidationAttribute_Test// : TestBase
    {
        public class TestListClass
        {
            [NotEmptyArray]
            public List<string> ListItems { get; set; }
        }

        public class TestDictClass
        {
            [NotEmptyArray]
            public Dictionary<string, string> ListItems { get; set; }
        }

        [Fact]
        public void NotEmptyList_Test()
        {
            {
                var testClass = new TestListClass
                {
                    ListItems = new List<string> { "item1" }
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.True(result.IsSuccess);
            }

            {
                var testClass = new TestListClass();
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }

            {
                var testClass = new TestListClass
                {
                    ListItems = new List<string>()
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public void NotEmptyArray_Test()
        {
            {
                var testClass = new TestDictClass
                {
                    ListItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "key1", "value1" } }
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.True(result.IsSuccess);
            }

            {
                var testClass = new TestDictClass();
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }

            {
                var testClass = new TestDictClass
                {
                    ListItems = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }
        }
    }
}

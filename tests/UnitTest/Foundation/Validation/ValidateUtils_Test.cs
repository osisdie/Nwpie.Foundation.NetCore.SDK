using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nwpie.Foundation.Abstractions.Utilities;
using Xunit;

namespace Nwpie.xUnit.Foundation.Validation
{
    public class ValidateUtils_Test// : TestBase
    {
        public class TestClass
        {
            [Required(AllowEmptyStrings = false)]
            [RegularExpression(@"^[0-9]{7}$")]
            public string TestField { get; set; }
        }

        [Fact]
        public void Regex_Test()
        {
            {
                var testClass = new TestClass
                {
                    TestField = "1234567"
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.True(result.IsSuccess);
            }

            {
                var testClass = new TestClass();
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }

            {
                var testClass = new TestClass
                {
                    TestField = string.Empty
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }

            {
                var testClass = new TestClass
                {
                    TestField = "123456"
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }

            {
                var testClass = new TestClass
                {
                    TestField = "123456"
                };
                var result = ValidateUtils.Validate(testClass);
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public void Dependent_Test()
        {
            {
                var info = new ClientAddressInfo
                {
                    Name = "user name"
                };
                var result = ValidateUtils.Validate(info);
                Assert.False(result.IsSuccess);
            }

            {
                var info = new ClientAddressInfo
                {
                    Name = "user name",
                    Other = "123"
                };

                var result = ValidateUtils.Validate(info);
                Assert.True(result.IsSuccess);
            }

            {
                var info = new ClientAddressInfo
                {
                    Name = "user name",
                    Bookkeeping = true
                };
                var result = ValidateUtils.Validate(info);
                Assert.True(result.IsSuccess);
            }

            {
                var info = new ClientAddressInfo
                {
                    Name = "user name",
                    Bookkeeping = true,
                    Other = "123"
                };
                var result = ValidateUtils.Validate(info);
                Assert.True(result.IsSuccess);
            }
        }

        [Fact]
        public void MatchCondition_Test()
        {
            //{
            //    var isMatch = ValidateUtils.MatchOR(x => string.IsNullOrWhiteSpace(x), ServiceContext.ApiKey, ServiceContext.ApiName, ServiceContext.BaseServiceUrl);
            //    Assert.False(isMatch);
            //}

            {
                Func<string, bool> condition = x => default(string) == x;
                var a1 = "1";
                var a2 = "2";
                var isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.False(isMatch);

                a1 = null;
                isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.True(isMatch);
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.False(isMatch);

                a2 = null;
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.True(isMatch);

                isMatch = ValidateUtils.MatchOR<string>(condition);
                Assert.False(isMatch);
            }

            {
                Func<int, bool> condition = x => default(int) == x;
                var a1 = 1;
                var a2 = 2;
                var isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.False(false);

                a1 = 0;
                isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.True(isMatch);
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.False(isMatch);

                a2 = 0;
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.True(isMatch);

                isMatch = ValidateUtils.MatchOR<int>(condition);
                Assert.False(isMatch);
            }

            {
                Func<int?, bool> condition = x => default(int?) == x;
                int? a1 = 1;
                int? a2 = 2;
                var isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.False(isMatch);

                a1 = null;
                isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.True(isMatch);
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.False(isMatch);

                a2 = null;
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.True(isMatch);

                isMatch = ValidateUtils.MatchOR<int?>(condition);
                Assert.False(isMatch);
            }

            {
                Func<List<int>, bool> condition = x => default(List<int>) == x;
                var a1 = new List<int>();
                var a2 = new List<int>();
                var isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.False(isMatch);

                a1 = null;
                isMatch = ValidateUtils.MatchOR(condition, a1, a2);
                Assert.True(isMatch);
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.False(isMatch);

                a2 = null;
                isMatch = ValidateUtils.MatchAND(condition, a1, a2);
                Assert.True(isMatch);

                isMatch = ValidateUtils.MatchOR<List<int>>(condition);
                Assert.False(isMatch);
            }
        }
    }
}

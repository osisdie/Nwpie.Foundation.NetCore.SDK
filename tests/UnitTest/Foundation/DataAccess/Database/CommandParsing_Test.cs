using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nwpie.Foundation.DataAccess.Database.Utilities;
using Dapper;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.Database
{
    public class CommandParsing_Test : DatabaseTestBase
    {
        public CommandParsing_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test dal service")]
        public void DetectDiyParameter_Test()
        {
            var diyParameterSectionPattern = new Regex(@"/\*[\s]*{[\s]*[\w]+[\s]*}[\s]*\*/", RegexOptions.IgnoreCase);
            var specificDiyParameterFormatter = "/\\*[\\s]*{{[\\s]*{0}[\\s]*}}[\\s]*\\*/";

            var sqlCmd = @"
SELECT *
FROM `TestTable`
WHERE 1 = 1
  /*{D_columnChar}*/
  /*{ D_columnInt}*/
  /*{D_columnDecimal }*/
  /*{ D_columnBool }*/
  /*{
D_columnDate}*/
  /*{D_columnDatetime
}*/
limit @offset, @limit;
";

            var replaced = "@Replaced";
            Func<string, bool> matcher = (cmd) => diyParameterSectionPattern.IsMatch(cmd);
            Func<string, string, string, string> replacer = (cmd, key, val) => Regex.Replace(cmd, string.Format(specificDiyParameterFormatter, key), val, RegexOptions.IgnoreCase);

            var notMatchCmd = "D_param";
            Assert.False(matcher(notMatchCmd));
            Assert.DoesNotContain(replaced, replacer(sqlCmd, "D_param", replaced));

            notMatchCmd = "@D_param";
            Assert.False(matcher(notMatchCmd));
            Assert.DoesNotContain(replaced, replacer(sqlCmd, "@D_param", replaced));

            var matchedCmd = "/*{D_columnChar}*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "D_columnChar", replaced));

            // Make sure case insensitive
            matchedCmd = "/*{d_columnchar}*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "d_columnchar", replaced));

            matchedCmd = "/*{ D_columnInt}*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "D_columnInt", replaced));

            matchedCmd = "/*{D_columnDecimal }*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "D_columnDecimal", replaced));

            matchedCmd = "/*{ D_columnBool }*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "D_columnBool", replaced));

            matchedCmd = @"/*{
D_columnDate}*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "D_columnDate", replaced));

            matchedCmd = @"/*{D_columnDatetime
}*/";
            Assert.True(matcher(matchedCmd));
            Assert.Contains(replaced, replacer(sqlCmd, "D_columnDatetime", replaced));
        }

        [Fact(Skip = "Won't test dal service")]
        public void ReservedChars_ReplaceAll_Test()
        {
            var sqlCmd = @"  & = ~ | ^ / . - + * ( ) ! : ; , > < % ' ` ""

";

            var replaced = Regex.Replace(sqlCmd, DataAccessUtils.SqlReservedChars, string.Empty);
            Assert.Equal(string.Empty, replaced);
        }

        [Fact(Skip = "Won't test dal service")]
        public void SqlCommand_ExtractParameters_Test()
        {
            var sqlquery = @"
SELECT template_3d_data
FROM fruit_db.TEMPLATE
WHERE 1 = `@flag`
and is_delete = false
and template_id=@template_id1
and create_at>@create_at
limit @offset,@pageSize;
";
            var list = new List<string>() { "flag", "template_id1", "create_at", "offset", "pageSize" };
            var matches = Regex.Matches(sqlquery, DataAccessUtils.SqlParameterPattern);
            Assert.Equal(list.Count(), matches.Count());
            foreach (Match match in matches)
            {
                var p = match.Groups[DataAccessUtils.SqlExtractParamGroup].Value;
                Assert.False(string.IsNullOrWhiteSpace(p));
            }

            var parameters = new DynamicParameters();
            parameters.Add("flag", 1);
            parameters.Add("template_id1", "template12313123221");
            parameters.Add("create_at", DateTime.UtcNow.AddDays(1));
            parameters.Add("offset", 0);
            parameters.Add("pageSize", 1);

            foreach (var name in parameters.ParameterNames)
            {
                var pValue = parameters.Get<dynamic>(name);
                var stringSymbol = pValue is string | pValue is DateTime ? "'" : string.Empty;
                sqlquery = sqlquery.Replace($"@{name}", $"{stringSymbol}{pValue}{stringSymbol}");
            }

            Assert.DoesNotContain("@", sqlquery);
        }

        [Fact(Skip = "Won't test dal service")]
        public void SqlCommand_MissingParameters_Test()
        {
            var sqlquery = @"
SELECT template_3d_data
FROM fruit_db.TEMPLATE
WHERE 1 = `@flag`
and is_delete = false
and template_id=@template_id1
and create_at>@create_at
limit @offset,@pagesize;
";

            var list = new List<string>() { "flag", "template_id1", "create_at", "offset", "pageSize" };
            var matches = Regex.Matches(sqlquery, DataAccessUtils.SqlParameterPattern);
            Assert.Equal(list.Count(), matches.Count());
            foreach (Match match in matches)
            {
                var p = match.Groups[DataAccessUtils.SqlExtractParamGroup].Value;
                Assert.False(string.IsNullOrWhiteSpace(p));
            }

            var parameters = new DynamicParameters();
            parameters.Add("flag", 1);
            parameters.Add("template_id1", "template12313123221");
            parameters.Add("offset", 0);
            parameters.Add("pageSize", 1);

            foreach (var name in parameters.ParameterNames)
            {
                var pValue = parameters.Get<dynamic>(name);
                var stringSymbol = pValue is string | pValue is DateTime ? "'" : string.Empty;
                sqlquery = sqlquery.Replace($"@{name}", $"{stringSymbol}{pValue}{stringSymbol}");
            }

            Assert.Contains("@", sqlquery);
            Assert.Contains("@create_at", sqlquery); // Not Match: Missing input
            Assert.Contains("@pagesize", sqlquery); // Not Match: Case sensitive

            var result = DataAccessUtils.InspectSqlCommand(sqlquery, parameters);
            Assert.NotEmpty(result.MissingParameters);
            Assert.Equal(2, result.MissingParameters.Count());
            Assert.Contains("@", result.ReplacedSqlCommand);
            Assert.Contains("@create_at", result.ReplacedSqlCommand); // Not Match: Missing input
            Assert.Contains("@pagesize", result.ReplacedSqlCommand); // Not Match: Case sensitive
        }
    }
}

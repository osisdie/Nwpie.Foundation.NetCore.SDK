namespace Nwpie.Foundation.DataAccess.Database
{
    public class QueryCondition : IKeyedObject, IKeyedObject<string>
    {
        public QueryCondition(string columnName, QueryConditionOperator queryConditionOperator, object value)
        {
            ColumnName = columnName;
            ParameterName = ColumnName;
            QueryConditionOperator = queryConditionOperator;
            Value = value;
        }

        public string Key
        {
            get => ParameterName;
        }

        public string ColumnName { get; private set; }

        public string ParameterName { get; internal set; }

        public QueryConditionOperator QueryConditionOperator { get; private set; }

        public object Value { get; private set; }
    }
}

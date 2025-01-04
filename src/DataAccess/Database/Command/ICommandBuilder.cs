using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Nwpie.Foundation.DataAccess.Database.Configuration;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal interface ICommandBuilder
    {
        ICommand Build();

        ICommandBuilder SetParameterValue(string paramName, object paramValue);
        ICommandBuilder ToggleDynamicSection(string sectionName, bool enable);
        ICommandBuilder SetParameterValue<TParameter, TValue>(Expression<Func<TParameter, TValue>> value)
            where TParameter : class;
        ICommandBuilder SetParameterValue(string paramName, DataTable dataTable, string tableTypeName);
        ICommandBuilder SetParameterValue<T>(string paramName, IEnumerable<T> list, string tableTypeName);
        ICommandBuilder SetOutputParameter(string paramName, DbType type, int size);
        void SetParameterValues(params string[] paramNameFilters);
        void SetParameterValues<TParameter>(params Expression<Func<TParameter, object>>[] valueExps)
            where TParameter : class;
        CommandConfigInfo CommandConfigInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Nwpie.Foundation.DataAccess.Database.Configuration;
using Dapper;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal abstract class BaseCommandBuilder : ICommandBuilder
    {
        public abstract ICommand Build();

        public BaseCommandBuilder()
        {
            Parameters.Add("@returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);
        }

        #region SetParameterValue
        public ICommandBuilder ToggleDynamicSection(string sectionName, bool enable)
        {
            sectionName = sectionName?.Trim();
            if (false == ParameterCollection.Keys.Contains(sectionName))
            {
                throw new ArgumentNullException($"Missing section {sectionName}. ");
            }

            ParameterCollection[sectionName].Enable = enable;
            return this;
        }

        public ICommandBuilder SetParameterValue(string paramName, object parameterValue)
        {
            paramName = paramName?.Trim();
            Parameters.Add(paramName, parameterValue);
            return this;
        }

        public ICommandBuilder SetParameterValue(string paramName, DataTable dataTable, string tableTypeName)
        {
            paramName = paramName?.Trim();
            Parameters.Add(paramName, dataTable.AsTableValuedParameter(tableTypeName));
            return this;
        }

        public ICommandBuilder SetParameterValue<T>(string paramName, IEnumerable<T> list, string tableTypeName)
        {
            paramName = paramName?.Trim();
            Parameters.Add(paramName, list.AsTableValuedParameter(tableTypeName));
            return this;
        }

        public ICommandBuilder SetParameterValue<TParameter, TValue>(Expression<Func<TParameter, TValue>> value)
            where TParameter : class
        {
            if (null == value)
            {
                throw new ArgumentNullException($"Parameter entity has not specified, please use SetParameterEntity method to set. ");
            }

            var paramName = PropertyMapper.GetPropertyMapNameFromExp(value);
            var flexibleParam = ParameterCollection[paramName];

            Parameters.Add(flexibleParam.Name, flexibleParam.Value);
            ParameterCollection.Remove(paramName);

            m_IsUseEntityToSetValues = false;
            return this;
        }

        public void SetParameterValues(params string[] paramNameFilters)
        {
            if (null == paramNameFilters)
            {
                throw new ArgumentNullException($"Parameter entity has not specified, please use SetParameterEntity method to set. ");
            }

            FlexibleParameter flexibleParameter;
            foreach (var paramName in paramNameFilters)
            {
                if (false == ParameterCollection.Keys.Contains(paramName))
                {
                    throw new ArgumentNullException($"Parameter {paramName} has not specified, please use SetParameterEntity method to set. ");
                }

                flexibleParameter = ParameterCollection[paramName];
                Parameters.Add(flexibleParameter.Name, flexibleParameter.Value);
                ParameterCollection.Remove(paramName);
            }

            m_IsUseEntityToSetValues = false;
        }

        public void SetParameterValues<TParameter>(params Expression<Func<TParameter, object>>[] valueExps)
            where TParameter : class
        {
            if (null == valueExps)
            {
                throw new ArgumentNullException($"Parameter entity has not specified, please use SetParameterEntity method to set. ");
            }

            foreach (var value in valueExps)
            {
                var parameterName = PropertyMapper.GetPropertyMapNameFromExp(value);
                var flexibleParameter = ParameterCollection[parameterName];

                Parameters.Add(flexibleParameter.Name, flexibleParameter.Value);
                ParameterCollection.Remove(parameterName);
            }

            m_IsUseEntityToSetValues = false;
        }

        public void SetParameterValues()
        {
            foreach (var param in ParameterCollection)
            {
                Parameters.Add(param.Key, param.Value.Value);
            }
        }

        public ICommandBuilder SetOutputParameter(string paramName, DbType type, int size)
        {
            paramName = paramName.Trim();
            Parameters.Add(paramName, null, type, ParameterDirection.Output, size);
            return this;
        }
        #endregion

        public DynamicParameters Parameters { get; set; } = new DynamicParameters();
        public CommandConfigInfo CommandConfigInfo { get; set; } = new CommandConfigInfo();
        public List<ColumnProperty> PrimaryKeyColumnProperties { get; set; } = new List<ColumnProperty>();
        public IDictionary<string, FlexibleParameter> ParameterCollection { get; set; } = new Dictionary<string, FlexibleParameter>(StringComparer.OrdinalIgnoreCase);

        protected bool m_IsUseEntityToSetValues = true;
    }
}

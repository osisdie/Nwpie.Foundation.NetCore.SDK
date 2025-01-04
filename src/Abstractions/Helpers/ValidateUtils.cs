using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Enums;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class ValidateUtils
    {
        public static ServiceResponse<List<string>> ValidateAndThrow(object o)
        {
            var validationResults = new List<ValidationResult>();
            var validResult = Validate(o);
            if (false == validResult?.IsSuccess)
            {
                throw new Exception(string.Join(" ", validResult.Data));
            }

            return validResult;
        }

        public static ServiceResponse<List<string>> Validate(object o)
        {
            var context = new ValidationContext(o, null, null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator
                .TryValidateObject(o, context, validationResults, true);

            var result = new ServiceResponse<List<string>>(isValid)
            {
                Code = isValid
                    ? (int)StatusCodeEnum.Success
                    : (int)StatusCodeEnum.InvalidContractRequest,
                Data = validationResults
                    ?.Select(err => err.ErrorMessage)
                    ?.ToList()
            };

            if (false == isValid)
            {
                result.ErrMsg = string.Join(" ", validationResults);
            }

            return result;
        }

        public static bool MatchOR<T>(Func<T, bool> condition, params T[] vals)
        {
            if (null != condition && vals?.Length > 0)
            {
                var ret = false;
                for (var i = 0; i < vals.Length; ++i)
                {
                    ret |= condition(vals[i]);
                }

                return ret;
            }

            return false;
        }

        public static bool MatchAND<T>(Func<T, bool> condition, params T[] vals)
        {
            if (null != condition && vals?.Length > 0)
            {
                var ret = true;
                for (var i = 0; i < vals.Length; ++i)
                {
                    ret &= condition(vals[i]);
                }

                return ret;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckNull(string parameterName, object parameterObj)
        {
            if (null == parameterObj)
            {
                throw new ArgumentNullException($"Parameter '{parameterName}' cannot be null.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CheckEmptyString(string parameterName, string parameterObj)
        {
            if (string.IsNullOrEmpty(parameterObj))
            {
                throw new ArgumentNullException($"String parameter '{parameterName}' cannot be null or empty.");
            }
        }
    }
}

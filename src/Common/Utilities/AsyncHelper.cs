using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Nwpie.Foundation.Common.Utilities
{
    public static class AsyncHelper
    {
        public static bool IsAsyncMethod(MethodInfo method) =>
            null != (method.GetCustomAttribute(typeof(AsyncStateMachineAttribute))
                as AsyncStateMachineAttribute);

        public static bool IsAwaitableMethod(MethodInfo method) =>
            null != method.ReturnType.GetMethod(nameof(Task.GetAwaiter));

        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            dynamic awaitable = @this.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs an operation and ignores any Exceptions that occur.
        /// Returns true or falls depending on whether catch was
        /// triggered
        /// </summary>
        /// <param name="act">lambda that performs an operation that might throw</param>
        /// <returns></returns>
        public static bool IgnoreErrors(Action act)
        {
            if (act == null)
            {
                return false;
            }

            try
            {
                act.Invoke();
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Runs an function that returns a value and ignores any Exceptions that occur.
        /// Returns true or falls depending on whether catch was
        /// triggered
        /// </summary>
        /// <param name="func">parameterless lamda that returns a value of T</param>
        /// <param name="defaultValue">Default value returned if operation fails</param>
        public static T IgnoreErrors<T>(Func<T> func, T defaultValue = default(T))
        {
            if (func == null)
            {
                return defaultValue;
            }

            try
            {
                return func.Invoke();
            }
            catch { return defaultValue; }
        }

        public static TResult RunSync<TResult>(Func<Task<TResult>> func) =>
            m_TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();

        public static void RunSync(Func<Task> func) =>
            m_TaskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();

        private static readonly TaskFactory m_TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default
            );
    }
}

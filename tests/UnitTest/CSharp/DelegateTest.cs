using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.CSharp
{
    public class DelegateTest
    {
        public DelegateTest(ITestOutputHelper output)
        {
            m_Output = output;
        }

        [Theory]
        [InlineData(nameof(Add), 5, 3, 8)]
        [InlineData(nameof(Subtract), 5, 3, 2)]
        public void Single_Test(string funcName, int a, int b, int expected)
        {
            //SingleDelegate func = Add;
            //SingleDelegate func = Subtract;
            MethodInfo method = typeof(DelegateTest).GetMethod(funcName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.NotNull(method);

            Type[] parameterTypes = Array.ConvertAll(method.GetParameters(), p => p.ParameterType);
            Type returnType = method.ReturnType;

            Type delegateType;

            if (returnType == typeof(void))
            {
                if (parameterTypes.Length == 0)
                    delegateType = typeof(Action);
                else
                    delegateType = Expression.GetActionType(parameterTypes);
            }
            else
            {
                Type[] funcTypes = parameterTypes.Concat(new[] { returnType }).ToArray();
                delegateType = Expression.GetFuncType(funcTypes);
            }

            object[] myParams = { a, b };
            var actionDelegate = Delegate.CreateDelegate(delegateType, method);
            var resultObj = actionDelegate.DynamicInvoke(myParams);
            int result = resultObj != null ? (int)resultObj : 0;

            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(5, 3, 2)]
        public void DelegateSub_Test(int a, int b, int result)
        {
            // Single delegate example
            SingleDelegate func = Subtract;
            Assert.Equal(result, func(a, b));
        }


        [Fact]
        public void Multiple_Test()
        {
            // Multicast delegate example
            {
                g_Message = "";
                MulticastDelegate @delegate = AppendMessage;
                @delegate += AppendMessage; // Adds AppendMessage to invocation list
                @delegate.Invoke("Hello, "); // Correct invocation with Invoke()
                Assert.Equal("Hello, Hello, ", g_Message);
            }

            {
                g_Message = "";
                MulticastDelegate @delegate = AppendMessage;
                @delegate += AppendMessage;
                @delegate.Invoke("Delegates!"); // Correct invocation with Invoke()
                Assert.Equal("Delegates!Delegates!", g_Message);
            }
        }


        [Fact]
        public void Action_Test()
        {
            // Using Action delegate
            Action<string> actionDelegate = ActionMessage;
            actionDelegate("This is an Action Delegate.");
            Assert.Equal("This is an Action Delegate.", g_ActionMsg);
        }

        [Fact]
        public void Func_Test()
        {
            // Using Func delegate
            Func<int, int, int> funcDelegate = Add;
            Assert.Equal(9, funcDelegate(7, 2));
        }

        private readonly ITestOutputHelper m_Output;
        delegate int SingleDelegate(int a, int b);
        delegate void MulticastDelegate(string message);

        static string g_Message = "";
        static string g_ActionMsg = "";
        static int Add(int a, int b) => a + b;
        static int Subtract(int a, int b) => a - b;
        static void ActionMessage(string message) => g_ActionMsg = message;
        static void AppendMessage(string message) => g_Message += message;
    }
}

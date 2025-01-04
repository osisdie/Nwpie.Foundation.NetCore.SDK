using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Nwpie.Foundation.Abstractions.Statics;
using Newtonsoft.Json;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class DebugUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string msg)
        {
#if DEBUG
            Debug.WriteLine(string.Format("[{0},{1}]\t{2}\t{3}",
                Process.GetCurrentProcess().Id,
                Thread.CurrentThread.ManagedThreadId,
                DateTime.Now.ToString("HH:mm:ss.fff"),
                msg));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogException(Exception ex)
        {
#if DEBUG
            Log(string.Format("Exception: {0}", ex.Message));
            Debug.WriteLine(string.Format("{0}", ex.StackTrace));
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CollectDebugInfo(object obj, string position)
        {
            if (SdkRuntime.IsDebug)
            {
                var msg = string.Empty;

                try
                {
                    msg = obj == null ? string.Empty : JsonConvert.SerializeObject(obj);
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }

                Debug.WriteLine(string.Format("[{0},{1}]\t{2}\t{3}\t{4}",
                    Process.GetCurrentProcess().Id,
                    Thread.CurrentThread.ManagedThreadId,
                    position,
                    DateTime.Now.ToString("HH:mm:ss.fff"),
                    msg
                ));
            }
        }
    }
}

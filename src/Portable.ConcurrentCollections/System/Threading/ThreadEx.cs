using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#if !NET20 && !NET35
using System.Threading.Tasks;
#endif

namespace System.Threading
{

    internal static class ThreadEx
    {

        // Note: Magic number copied from CoreRT's RuntimeThread.cs. See the original source code for an explanation.
        internal static readonly int OptimalMaxSpinWaitsPerSpinIteration = 64;

#if NET20 || NET35
        [DllImport("kernel32.dll")]
        private static extern bool SwitchToThread();
#endif

        public static void Sleep(int dueTime)
        {
#if NET20 || NET35
            Thread.Sleep(dueTime);
#elif PROFILE328
            TaskEx.Delay(dueTime).GetAwaiter().GetResult();
#else
            Task.Delay(dueTime).GetAwaiter().GetResult();
#endif
        }

        public static bool Yield()
        {
#if NETSTANDARD1_0 || PROFILE111 || PROFILE328
            try
            {
#if NETSTANDARD1_0 || PROFILE111
                Task.Yield().GetAwaiter().GetResult();
#else
                TaskEx.Yield().GetAwaiter().GetResult();
#endif
                return true;
            }
            catch (Exception)
            {
                return false;
            }
#else
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return SwitchToThread();
            }
            return false;
#endif
        }

        public static void SpinWait(int iterations)
        {
#if NET20 || NET35
            Thread.SpinWait(iterations);
#elif NETSTANDARD1_0 || PROFILE111
            if (!TryNativeSpinWait(iterations))
            {
                var spinner = new System.Threading.SpinWait();
                spinner.SpinOnce();
            }
#else
            if (!TryNativeSpinWait(iterations))
            {
                throw new PlatformNotSupportedException();
            }
#endif
        }

        private static bool TryNativeSpinWait(int iterations)
        {
#if NETSTANDARD1_0 || PROFILE111
            return false;
#else
            var mi = typeof(Thread).GetMethod("SpinWait");
            if (mi != null)
            {
                mi.Invoke(null, new object[] { iterations });
                return true;
            }
            return false;
#endif
        }

    }

}
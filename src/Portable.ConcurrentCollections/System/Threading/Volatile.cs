namespace System.Threading
{
#if !NETSTANDARD1_0 && !PROFILE111
    internal static class Volatile
    {
        public static T Read<T>(ref T location)
        {
            // 
            // The VM will replace this with a more efficient implementation.
            //
            var value = location;
            Thread.MemoryBarrier();
            return value;
        }

        public static void Write<T>(ref T location, T value)
        {
            // 
            // The VM will replace this with a more efficient implementation.
            //
            Thread.MemoryBarrier();
            location = value;
        }
    }
#endif
}
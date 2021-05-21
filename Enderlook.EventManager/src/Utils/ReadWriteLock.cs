using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal struct ReadWriteLock
    {
        private int locked;
        private uint readers;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Lock()
        {
            while (Interlocked.Exchange(ref locked, 1) != 0) ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Unlock() => locked = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadBegin()
        {
            Lock();
            readers++;
            Unlock();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadEnd()
        {
            Lock();
            readers--;
            Unlock();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBegin()
        {
            while (true)
            {
                Lock();
                if (readers > 0)
                    Unlock();
                else
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteEnd() => Unlock();
    }
}
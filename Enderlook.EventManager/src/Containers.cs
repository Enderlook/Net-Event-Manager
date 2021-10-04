using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal readonly struct InvariantObject
    {
        public readonly object Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InvariantObject(object value) => Value = value;
    }

    internal readonly struct InvariantObjectAndT<T>
    {
        public readonly object Value;
        public readonly T ValueT;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InvariantObjectAndT(object value, T valueT)
        {
            Debug.Assert(typeof(T).IsValueType || typeof(T) == typeof(object));
            ValueT = valueT;
            Value = value;
        }
    }

    internal readonly struct InvariantObjectAndGCHandle : IWeak
    {
        public readonly object Value;
        public readonly GCHandle Handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InvariantObjectAndGCHandle(object value, object handle)
        {
            Value = value;
            Handle = GCHandle.Alloc(handle, GCHandleType.Weak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => Handle.Free();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FreeIfIsCollected()
        {
            if (Handle.Target is null)
            {
                Handle.Free();
                return true;
            }
            return false;
        }
    }

    internal readonly struct InvariantObjectAndGCHandleTrackResurrection : IWeak
    {
        public readonly object Value;
        public readonly GCHandle Handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InvariantObjectAndGCHandleTrackResurrection(object value, object handle)
        {
            Value = value;
            Handle = GCHandle.Alloc(handle, GCHandleType.WeakTrackResurrection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => Handle.Free();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FreeIfIsCollected()
        {
            if (Handle.Target is null)
            {
                Handle.Free();
                return true;
            }
            return false;
        }
    }

    internal readonly struct InvariantObjectAndTAndGCHandle<T> : IWeak
    {
        public readonly object Value;
        public readonly T ValueT;
        public readonly GCHandle Handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InvariantObjectAndTAndGCHandle(object value, object handle, T valueT)
        {
            Value = value;
            ValueT = valueT;
            Handle = GCHandle.Alloc(handle, GCHandleType.Weak);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => Handle.Free();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FreeIfIsCollected()
        {
            if (Handle.Target is null)
            {
                Handle.Free();
                return true;
            }
            return false;
        }
    }

    internal readonly struct InvariantObjectAndTAndGCHandleTrackResurrection<T> : IWeak
    {
        public readonly object Value;
        public readonly T ValueT;
        public readonly GCHandle Handle;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public InvariantObjectAndTAndGCHandleTrackResurrection(object value, object handle, T valueT)
        {
            Value = value;
            ValueT = valueT;
            Handle = GCHandle.Alloc(handle, GCHandleType.WeakTrackResurrection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => Handle.Free();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FreeIfIsCollected()
        {
            if (Handle.Target is null)
            {
                Handle.Free();
                return true;
            }
            return false;
        }
    }
}

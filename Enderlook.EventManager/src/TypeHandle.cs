using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed partial class TypeHandle : IDisposable
    {
        private EventList<StructDelegate> parameterless = EventList<StructDelegate>.Create();

        public void Subscribe(Action @delegate)
            => parameterless.Add(new StructDelegate(@delegate));

        public void Unsubscribe(Action @delegate)
            => parameterless.Remove(new StructDelegate(@delegate));

        private EventList<StructDelegate> parameters = EventList<StructDelegate>.Create();

        public void Subscribe<T>(Action<T> @delegate)
            => parameters.Add(new StructDelegate(@delegate));

        public void Unsubscribe<T>(Action<T> @delegate)
            => parameters.Remove(new StructDelegate(@delegate));

        private EventListOnce<StructDelegate> parameterlessOnce = EventListOnce<StructDelegate>.Create();
        public void SubscribeOnce(Action @delegate)
            => parameterlessOnce.Add(new StructDelegate(@delegate));
        public void UnsubscribeOnce(Action @delegate)
            => parameterlessOnce.Remove(new StructDelegate(@delegate));

        private EventListOnce<StructDelegate> parametersOnce = EventListOnce<StructDelegate>.Create();

        public void SubscribeOnce<T>(Action<T> @delegate)
            => parametersOnce.Add(new StructDelegate(@delegate));

        public void UnsubscribeOnce<T>(Action<T> @delegate)
            => parametersOnce.Remove(new StructDelegate(@delegate));

        private ReadWriterLock closureLock;
        private Dictionary<Type, TypedClosure> closures = new Dictionary<Type, TypedClosure>();

        private EventList<ClosureDelegate<object>> closureParameterless = EventList<ClosureDelegate<object>>.Create();

        public void Subscribe<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parameterless.Add(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParameterless.Add(new ClosureDelegate<object>(@delegate, closure));
        }

        public void Unsubscribe<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parameterless.Remove(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParameterless.Remove(new ClosureDelegate<object>(@delegate, closure));
        }

        private EventList<ClosureDelegate<object>> closureParameters = EventList<ClosureDelegate<object>>.Create();

        public void Subscribe<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parameters.Add(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParameters.Add(new ClosureDelegate<object>(@delegate, closure));
        }

        public void Unsubscribe<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parameters.Remove(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParameters.Remove(new ClosureDelegate<object>(@delegate, closure));
        }

        private EventListOnce<ClosureDelegate<object>> closureParameterlessOnce = EventListOnce<ClosureDelegate<object>>.Create();

        public void SubscribeOnce<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parameterlessOnce.Add(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParameterlessOnce.Add(new ClosureDelegate<object>(@delegate, closure));
        }

        public void UnsubscribeOnce<TClosure>(Action<TClosure> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parameterlessOnce.Remove(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParameterlessOnce.Remove(new ClosureDelegate<object>(@delegate, closure));
        }

        private EventListOnce<ClosureDelegate<object>> closureParametersOnce = EventListOnce<ClosureDelegate<object>>.Create();

        public void SubscribeOnce<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parametersOnce.Add(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParametersOnce.Add(new ClosureDelegate<object>(@delegate, closure));
        }

        public void UnsubscribeOnce<TClosure, U>(Action<TClosure, U> @delegate, TClosure closure)
        {
            if (typeof(TClosure).IsValueType)
                GetOrCreateTypeHandler<TClosure>().parametersOnce.Remove(new ClosureDelegate<TClosure>(@delegate, closure));
            else
                closureParametersOnce.Remove(new ClosureDelegate<object>(@delegate, closure));
        }

        public void Raise<T>(T argument, ref StructDelegate[] a, ref StructDelegate[] b, ref ClosureDelegate<object>[] c, ref ClosureDelegate<object>[] d)
        {
            InnerRaise(ref parameterless, ref a, ref b, new Parameterless());
            InnerRaise(ref parameters, ref a, ref b, argument);
            InnerRaise(ref parameterlessOnce, ref a, ref b, new Parameterless());
            InnerRaise(ref parametersOnce, ref a, ref b, argument);
            InnerRaise(ref closureParameterless, ref c, ref d, new Parameterless());
            InnerRaise(ref closureParameters, ref c, ref d, argument);
            InnerRaise(ref closureParameterlessOnce, ref c, ref d, new Parameterless());
            InnerRaise(ref closureParametersOnce, ref c, ref d, argument);

            closureLock.WriteBegin();
            try
            {
                foreach (object obj in closures)
                    ((TypedClosure)obj).Raise(argument);
            }
            finally
            {
                closureLock.WriteEnd();
            }
        }

        private struct Parameterless { }

        private static void InnerRaise<T, U>(ref EventList<U> list, ref U[] a, ref U[] b, T argument)
            where U : IDelegate<U>
        {
            list.ExtractToRun(ref a, ref b, out int count);
            U _ = a[count];
            for (int i = 0; i < count; i++)
                a[i].Invoke(argument);

            list.InjectToRun(ref a, count);
        }

        private static void InnerRaise<T, U>(ref EventListOnce<U> list, ref U[] a, ref U[] b, T argument)
            where U : IDelegate<U>
        {
            list.ExtractToRun(ref a, ref b, out int count, out int countRemove);
            U _ = a[count];
            _ = b[countRemove];
            for (int i = 0; i < count; i++)
            {
                U element = a[i];
                for (int j = countRemove - 1; j >= 0; j--)
                {
                    if (element.Equals(b[j]))
                    {
                        Array.Copy(b, j + 1, b, j, countRemove - j);
                        countRemove--;
                        goto next;
                    }
                }
                element.Invoke(argument);
                next:;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            parameters.Dispose();
            parameterless.Dispose();
            parametersOnce.Dispose();
            parameterlessOnce.Dispose();

            closureLock.WriteBegin();
            try
            {
                foreach (object obj in closures)
                    ((TypedClosure)obj).Dispose();
            }
            finally
            {
                closureLock.WriteEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Purge(ref StructDelegate[] a, ref StructDelegate[] b)
        {
            parameterless.ExtractToRun(ref a, ref b, out int count);
            parameterless.InjectToRun(ref a, count);
            parameters.ExtractToRun(ref a, ref b, out count);
            parameters.InjectToRun(ref a, count);
            parameterlessOnce.ExtractToRunRemoved(ref a, ref b, out count);
            parameterlessOnce.InjectToRun(ref a, count);
            parametersOnce.ExtractToRunRemoved(ref a, ref b, out count);
            parametersOnce.InjectToRun(ref a, count);

            closureLock.WriteBegin();
            try
            {
                foreach (object obj in closures)
                    ((TypedClosure)obj).Purge();
            }
            finally
            {
                closureLock.WriteEnd();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TypedClosure<TClosure> GetOrCreateTypeHandler<TClosure>()
        {
            Type key = typeof(TClosure);
            closureLock.ReadBegin();
            bool found;
            TypedClosure handle;
            try
            {
                found = closures.TryGetValue(key, out handle);
            }
            finally
            {
                closureLock.ReadEnd();
            }
            if (found)
                return Unsafe.As<TypedClosure<TClosure>>(handle);
            else
            {
                handle = new TypedClosure<TClosure>();
                closureLock.WriteBegin();
                try
                {
                    closures[key] = handle;
                }
                finally
                {
                    closureLock.WriteEnd();
                }
                return Unsafe.As<TypedClosure<TClosure>>(handle);
            }
        }
    }
}
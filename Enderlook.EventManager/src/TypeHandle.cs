using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal partial class TypeHandle : IDisposable
    {
        private EventList<Delegate> parameterless = EventList<Delegate>.Create();
        public void Subscribe(Action @delegate) => parameterless.Add(@delegate);
        public void Unsubscribe(Action @delegate) => parameterless.Remove(@delegate);

        private EventList<Delegate> parameters = EventList<Delegate>.Create();
        public void Subscribe<T>(Action<T> @delegate) => parameters.Add(@delegate);
        public void Unsubscribe<T>(Action<T> @delegate) => parameters.Remove(@delegate);

        private EventListOnce<Delegate> parameterlessOnce = EventListOnce<Delegate>.Create();
        public void SubscribeOnce(Action @delegate) => parameterlessOnce.Add(@delegate);
        public void UnsubscribeOnce(Action @delegate) => parameterlessOnce.Remove(@delegate);

        private EventListOnce<Delegate> parametersOnce = EventListOnce<Delegate>.Create();
        public void SubscribeOnce<T>(Action<T> @delegate) => parametersOnce.Add(@delegate);
        public void UnsubscribeOnce<T>(Action<T> @delegate) => parametersOnce.Remove(@delegate);

        public void Raise<T>(T argument, ref Delegate[] a, ref Delegate[] b)
        {
            InnerRaise(ref parameterless, ref a, ref b, new Parameterless());
            InnerRaise(ref parameters, ref a, ref b, argument);
            InnerRaise(ref parameterlessOnce, ref a, ref b, new Parameterless());
            InnerRaise(ref parametersOnce, ref a, ref b, argument);
        }

        private struct Parameterless { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InnerRaise<T>(object @delegate, T argument)
        {
            if (typeof(T) == typeof(Parameterless))
                Unsafe.As<Action>(@delegate)();
            else
                Unsafe.As<Action<T>>(@delegate)(argument);
        }

        private static void InnerRaise<T>(ref EventList<Delegate> list, ref Delegate[] a, ref Delegate[] b, T argument)
        {
            list.ExtractToRun(ref a, ref b, out int count);
            Delegate _ = a[count];
            for (int i = 0; i < count; i++)
                InnerRaise(a[i], argument);
            list.InjectToRun(ref a, count);
        }

        private static void InnerRaise<T>(ref EventListOnce<Delegate> list, ref Delegate[] a, ref Delegate[] b, T argument)
        {
            list.ExtractToRun(ref a, ref b, out int count, out int countRemove);
            Delegate _ = a[count];
            _ = b[countRemove];
            for (int i = 0; i < count; i++)
            {
                Delegate element = a[i];
                for (int j = countRemove - 1; j >= 0; j--)
                {
                    if (element == b[j])
                    {
                        Array.Copy(b, j + 1, b, j, countRemove - j);
                        countRemove--;
                        goto next;
                    }
                }
                InnerRaise(element, argument);
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Purge(ref Delegate[] a, ref Delegate[] b)
        {
            parameterless.ExtractToRun(ref a, ref b, out int count);
            parameterless.InjectToRun(ref a, count);
            parameters.ExtractToRun(ref a, ref b, out count);
            parameters.InjectToRun(ref a, count);
            parameterlessOnce.ExtractToRunRemoved(ref a, ref b, out count);
            parameterlessOnce.InjectToRun(ref a, count);
            parametersOnce.ExtractToRunRemoved(ref a, ref b, out count);
            parametersOnce.InjectToRun(ref a, count);
        }
    }
}
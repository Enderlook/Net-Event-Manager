using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct HandleSnapshoot
    {
        public readonly object parameterless1;
        public readonly int parameterlessCount1;
        public readonly object parameterlessOnce1;
        public readonly int parameterlessOnceCount1;
        public readonly object parameterlessOnce2;
        public readonly int parameterlessOnceCount2;
        public readonly object parameters1;
        public readonly int parametersCount1;
        public readonly object parametersOnce1;
        public readonly int parametersOnceCount1;
        public readonly object parametersOnce2;
        public readonly int parametersOnceCount2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HandleSnapshoot(
            object parameterless1, int parameterlessCount1,
            object parameterlessOnce1, int parameterlessOnceCount1, object parameterlessOnce2, int parameterlessOnceCount2,
            object parameters1, int parametersCount1,
            object parametersOnce1, int parametersOnceCount1, object parametersOnce2, int parametersOnceCount2)
        {
            this.parameterless1 = parameterless1;
            this.parameterlessCount1 = parameterlessCount1;
            this.parameterlessOnce1 = parameterlessOnce1;
            this.parameterlessOnceCount1 = parameterlessOnceCount1;
            this.parameterlessOnce2 = parameterlessOnce2;
            this.parameterlessOnceCount2 = parameterlessOnceCount2;
            this.parameters1 = parameters1;
            this.parametersCount1 = parametersCount1;
            this.parametersOnce1 = parametersOnce1;
            this.parametersOnceCount1 = parametersOnceCount1;
            this.parametersOnce2 = parametersOnce2;
            this.parametersOnceCount2 = parametersOnceCount2;
        }

        public static HandleSnapshoot Create<TDelegate>(
            ref EventList<TDelegate> parameterless,
            ref EventList<TDelegate> parameters,
            ref EventListOnce<TDelegate> parameterlessOnce,
            ref EventListOnce<TDelegate> parametersOnce)
        {
            parameterless.ExtractToRun(out TDelegate[] parameterless1, out int parameterlessCount1);
            parameterlessOnce.ExtractToRun(out TDelegate[] parameterlessOnce1, out int parameterlessOnceCount1, out TDelegate[] parameterlessOnce2, out int parameterlessOnceCount2);
            parameters.ExtractToRun(out TDelegate[] parameters1, out int parametersCount1);
            parametersOnce.ExtractToRun(out TDelegate[] parametersOnce1, out int parametersOnceCount1, out TDelegate[] parametersOnce2, out int parametersOnceCount2);

            return new HandleSnapshoot(
                parameterless1, parameterlessCount1,
                parameterlessOnce1, parameterlessOnceCount1, parameterlessOnce2, parameterlessOnceCount2,
                parameters1, parametersCount1,
                parametersOnce1, parametersOnceCount1, parametersOnce2, parametersOnceCount2
            );
        }

        public void Raise<TEvent, TDelegate, TMode, TClosure>(
            ref EventList<TDelegate> parameterless, ref EventList<TDelegate> parameters,
            TEvent argument)
        {
            Debug.Assert(this.parameterless1 is TDelegate[]);
            Debug.Assert(this.parameterlessOnce1 is TDelegate[]);
            Debug.Assert(this.parameterlessOnce2 is TDelegate[]);
            Debug.Assert(this.parameters1 is TDelegate[]);
            Debug.Assert(this.parametersOnce1 is TDelegate[]);
            Debug.Assert(this.parametersOnce2 is TDelegate[]);

            TDelegate[] parameterless1 = Unsafe.As<TDelegate[]>(this.parameterless1);
            TDelegate[] parameterlessOnce1 = Unsafe.As<TDelegate[]>(this.parameterlessOnce1);
            TDelegate[] parameterlessOnce2 = Unsafe.As<TDelegate[]>(this.parameterlessOnce2);
            TDelegate[] parameters1 = Unsafe.As<TDelegate[]>(this.parameters1);
            TDelegate[] parametersOnce1 = Unsafe.As<TDelegate[]>(this.parametersOnce1);
            TDelegate[] parametersOnce2 = Unsafe.As<TDelegate[]>(this.parametersOnce2);

            Utility.Raise<TEvent, TDelegate, TMode, TClosure>(
                ref parameterless, ref parameters,
                argument,
                ref parameterless1, parameterlessCount1,
                parameterlessOnce1, parameterlessOnceCount1, parameterlessOnce2, parameterlessOnceCount2,
                ref parameters1, parametersCount1,
                parametersOnce1, parametersOnceCount1, parametersOnce2, parametersOnceCount2
            );
        }
    }
}
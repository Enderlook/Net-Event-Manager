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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HandleSnapshoot Create<TParameterless, TParameters>(
            ref EventList<TParameterless> parameterless,
            ref EventList<TParameters> parameters,
            ref EventListOnce<TParameterless> parameterlessOnce,
            ref EventListOnce<TParameters> parametersOnce)
        {
            parameterless.ExtractToRun(out Array<TParameterless> parameterless1, out int parameterlessCount1);
            parameterlessOnce.ExtractToRun(out Array<TParameterless> parameterlessOnce1, out int parameterlessOnceCount1, out Array<TParameterless> parameterlessOnce2, out int parameterlessOnceCount2);
            parameters.ExtractToRun(out Array<TParameters> parameters1, out int parametersCount1);
            parametersOnce.ExtractToRun(out Array<TParameters> parametersOnce1, out int parametersOnceCount1, out Array<TParameters> parametersOnce2, out int parametersOnceCount2);

            return new HandleSnapshoot(
                parameterless1.AsObject, parameterlessCount1,
                parameterlessOnce1.AsObject, parameterlessOnceCount1, parameterlessOnce2.AsObject, parameterlessOnceCount2,
                parameters1.AsObject, parametersCount1,
                parametersOnce1.AsObject, parametersOnceCount1, parametersOnce2.AsObject, parametersOnceCount2
            );
        }

        public void Raise<TEvent, TParameterless, TParameters, TMode, TClosure>(
            ref EventList<TParameterless> parameterless, ref EventList<TParameters> parameters,
            TEvent argument)
        {
            Array<TParameterless> parameterless1 = new(this.parameterless1);
            Array<TParameterless> parameterlessOnce1 = new(this.parameterlessOnce1);
            Array<TParameterless> parameterlessOnce2 = new(this.parameterlessOnce2);
            Array<TParameters> parameters1 = new(this.parameters1);
            Array<TParameters> parametersOnce1 = new(this.parametersOnce1);
            Array<TParameters> parametersOnce2 = new(this.parametersOnce2);

            Utility.Raise<TEvent, TParameterless, TParameters, TMode, TClosure>(
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
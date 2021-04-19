using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct HandleSnapshoot
    {
        private readonly Array parameterless;
        private readonly int parameterlessCount;
        private readonly Array parameterlessOnce;
        private readonly int parameterlessOnceCount;
        private readonly Array parameters;
        private readonly int parametersCount;
        private readonly Array parametersOnce;
        private readonly int parametersOnceCount;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal HandleSnapshoot(
            Array parameterless, int parameterlessCount,
            Array parameterlessOnce, int parameterlessOnceCount,
            Array parameters, int parametersCount,
            Array parametersOnce, int parametersOnceCount)
        {
            this.parameterless = parameterless;
            this.parameterlessCount = parameterlessCount;
            this.parameterlessOnce = parameterlessOnce;
            this.parameterlessOnceCount = parameterlessOnceCount;
            this.parameters = parameters;
            this.parametersCount = parametersCount;
            this.parametersOnce = parametersOnce;
            this.parametersOnceCount = parametersOnceCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise<TParameterless, TParameters, TEvent, TMode, TClosure>(TEvent argument)
        {
            Utility.Raise<TParameterless, Unused, TMode, TClosure>(new List<TParameterless>(new Array<TParameterless>(parameterless), parameterlessCount), new());
            Utility.Raise<TParameterless, Unused, TMode, TClosure>(new List<TParameterless>(new Array<TParameterless>(parameterlessOnce), parameterlessOnceCount), new());
            Utility.Raise<TParameters, TEvent, TMode, TClosure>(new List<TParameters>(new Array<TParameters>(parameters), parametersCount), argument);
            Utility.Raise<TParameters, TEvent, TMode, TClosure>(new List<TParameters>(new Array<TParameters>(parametersOnce), parametersOnceCount), argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Raise<TEvent, TClosure>(ClosureHandle<TClosure, TEvent> closureHandle, TEvent argument)
            => closureHandle.Raise(this, argument);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return<TParameterless, TParameters>(ref EventList<TParameterless> parameterless, ref EventList<TParameters> parameters)
        {
            parameterless.ReturnExecutionList(new List<TParameterless>(new Array<TParameterless>(this.parameterless), parameterlessCount));
            parameters.ReturnExecutionList(new List<TParameters>(new Array<TParameters>(this.parameters), parametersCount));
            new List<TParameterless>(new Array<TParameterless>(parameterlessOnce), parameterlessOnceCount).Return();
            new List<TParameters>(new Array<TParameters>(parametersOnce), parametersOnceCount).Return();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HandleSnapshoot Create<TParameterless, TParameters>(
            List<TParameterless> parameterless,
            List<TParameterless> parameterlessOnce,
            List<TParameters> parameters,
            List<TParameters> parametersOnce)
        {
            return new(
                parameterless.UnderlyingObject, parameterless.Count,
                parameterlessOnce.UnderlyingObject, parameterlessOnce.Count,
                parameters.UnderlyingObject, parameters.Count,
                parametersOnce.UnderlyingObject, parametersOnce.Count
                );
        }
    }
}
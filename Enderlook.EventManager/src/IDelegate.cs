namespace Enderlook.EventManager
{
    internal interface IDelegate<TDelegate, TArgument> where TDelegate : IDelegate<TDelegate, TArgument>
    {
        bool Equals(in TDelegate other);

        void Invoke(TArgument argument);
    }
}
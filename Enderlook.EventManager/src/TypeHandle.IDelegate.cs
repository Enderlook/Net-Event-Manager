namespace Enderlook.EventManager
{
    internal sealed partial class TypeHandle
    {
        public interface IDelegate<U> where U : IDelegate<U>
        {
            bool Equals(in U other);

            void Invoke<T>(T argument);
        }
    }
}
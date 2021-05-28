namespace Enderlook.EventManager
{
    internal interface IWeak
    {
        void Free();

        bool FreeIfExpired();
    }
}
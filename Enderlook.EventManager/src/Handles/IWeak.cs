using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal interface IWeak
    {
        GCHandle Handle { get; }
    }
}
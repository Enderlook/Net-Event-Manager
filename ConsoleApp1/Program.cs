using Enderlook.EventManager;

using System;
using System.Runtime.CompilerServices;

namespace ConsoleApp1;

class Program
{
    public static void Main()
    {
        EventManager manager = new();
        /*manager.Subscribe((int a) =>
        {
            Console.WriteLine(a);
            if (a < 10)
                manager.Raise(a + 1);
            Console.WriteLine(a);
        });*/
        manager.Subscribe<int>(R);
        manager.Subscribe((int a) => Console.WriteLine($"  {a} i"));
        manager.Subscribe<ValueType>(() => Console.WriteLine("valueType"), SubscribeFlags.ListenAssignableEvents);
        manager.Subscribe<int>(() => Console.WriteLine("int"));
        manager.Subscribe<object>(() => Console.WriteLine("object"), SubscribeFlags.ListenAssignableEvents);
        manager.Subscribe<string>(() => Console.WriteLine("object"), SubscribeFlags.ListenAssignableEvents);
        manager.WeakSubscribe<object, int>(new object(), () => Console.WriteLine("Weak"));
        //manager.DynamicRaise((object)0);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        manager.Raise(0);
        //manager.Dispose();
        //manager.DynamicRaise(0);
        GC.WaitForPendingFinalizers();
        GC.Collect();
        manager.DynamicRaise(1);
        GC.WaitForPendingFinalizers();
        GC.Collect();
        //manager.Raise("");
        manager.Raise(2);
        manager.DynamicRaise(4);
        manager.DynamicRaise<object>(7f);
        manager.Unsubscribe<int>(R);
        manager.Raise(3);
    }

    private static void R(int a) => Console.WriteLine($"R{a}");

    public static void Q(EventManager m, Action<int> r) => m.Subscribe(r);

    public static void W(EventManager m, int r) => m.Raise(r);
}
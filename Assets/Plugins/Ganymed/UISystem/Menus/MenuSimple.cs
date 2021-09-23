
namespace Ganymed.UISystem
{
    /// <summary>
    /// Simple menu base class for menus that do not require logic or values when opening or closing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MenuSimple<T> : Menu<T> where T : MenuSimple<T>
    {
        public static void Open() => Initialize();

        public static void Close() => Terminate();
    }
}
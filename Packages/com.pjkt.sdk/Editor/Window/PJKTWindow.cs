namespace PJKT.SDK.Window
{
    //Base class for the various screens in the PJKT SDK
    internal abstract class PJKTWindow {
        internal PJKTMainWindow mainWindow;

        internal PJKTWindow() : base() { }

        internal static T Instantiate<T>(PJKTMainWindow mainWindow) where T : PJKTWindow, new() {
            T window = new T();
            window.mainWindow = mainWindow;
            return window;
        }

        internal abstract void OnGUI();
        internal virtual void OnOpen() { }
        internal virtual void OnClose() { }
        internal abstract void OnFocus();
    }
}
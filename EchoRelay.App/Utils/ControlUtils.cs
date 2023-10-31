namespace EchoRelay.App.Utils
{
    public static class ControlUtils
    {
        public static void InvokeUIThread(this Control control, Action method)
        {
            if (control.Disposing || control.IsDisposed) return;
            if (control.InvokeRequired)
                try
                {
                    control.Invoke(method);
                }
                catch (ObjectDisposedException) { }
            else
                method();
        }
    }
}

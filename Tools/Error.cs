using System.Windows;

namespace Tools
{
    public static class Error
    {
        public static bool HandleError(object result, string message)
        {
            if (result != null)
                return false;
            Deployment.Current.Dispatcher.BeginInvoke(() => Message.ShowToast(message));
            return true;
        }
    }
}
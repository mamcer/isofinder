namespace ISOFinder.WPF.Spider
{
    public partial class App
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var errorWindow = new ErrorWindow(e.Exception);
            try
            {
                errorWindow.Owner = Current.MainWindow;
            }
            catch
            {
                errorWindow.Owner = null;
            }

            errorWindow.ShowDialog();
            e.Handled = true;
            if (errorWindow.Owner == null)
            {
                Current.Shutdown(0);
            }
        }
    }
}
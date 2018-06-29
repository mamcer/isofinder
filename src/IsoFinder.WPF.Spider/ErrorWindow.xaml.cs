using System;
using System.Windows;

namespace ISOFinder.WPF.Spider
{
    public partial class ErrorWindow
    {
        public ErrorWindow(Exception ex)
        {
            InitializeComponent();
            ShowException(ex);
        }

        private void ShowException(Exception ex)
        {
            var errorMessage = ex.Message + ex.StackTrace;
            txtErrorText.Text = errorMessage;
        }
        
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void lblCopyToClipboard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clipboard.SetText(txtErrorText.Text);
        }
    }
}
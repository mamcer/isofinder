using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ISOFinder.WPF.Spider
{
    public partial class IsoItem
    {
        public string IsoFileName 
        {
            get
            {
                return lblItem.Content.ToString();
            }
            set
            {
                lblItem.Content = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return chkItem.IsChecked != null && chkItem.IsChecked.Value;
            }

            set
            {
                chkItem.IsChecked = value;
            }
        }

        public string Tags
        {
            get;
            set;
        }

        public bool IsVolume
        {
            get
            {
                return chkVolume.IsChecked != null && chkVolume.IsChecked.Value;
            }
        }

        public bool IsExistingItem
        {
            set
            {
                if (value)
                {
                    chkItem.Visibility = Visibility.Hidden;
                    lblItem.Foreground = Brushes.Gray;
                    chkVolume.Visibility = Visibility.Hidden;
                    lblAlreadyExists.Visibility = Visibility.Visible;
                }
            }
        }

        public IsoItem()
        {
            InitializeComponent();
        }

        private void chkVolume_Checked(object sender, RoutedEventArgs e)
        {
            lnkTags.Visibility = Visibility.Visible;
        }

        private void lnkTags_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tagsWindow = new Tags { Owner = Application.Current.MainWindow, TagText = Tags };
            if (tagsWindow.ShowDialog() == true)
            {
                Tags = tagsWindow.TagText;
            }
        }

        private void chkVolume_Unchecked(object sender, RoutedEventArgs e)
        {
            lnkTags.Visibility = Visibility.Hidden;
        }
    }
}
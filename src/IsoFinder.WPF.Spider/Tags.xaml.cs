using System.Windows;

namespace ISOFinder.WPF.Spider
{
    public partial class Tags
    {
        public Tags()
        {
            InitializeComponent();

            txtTags.Focus();
        }

        public string TagText
        {
            get
            {
                return txtTags.Text;
            }

            set
            {
                txtTags.Text = value;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTags.Text))
            {
                DialogResult = true;
            }
        }
    }
}
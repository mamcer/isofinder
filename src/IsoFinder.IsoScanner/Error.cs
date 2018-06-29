using System;
using System.Windows.Forms;

namespace IsoFinder.IsoScanner
{
    public partial class Error : Form
    {
        public Error(Exception ex)
        {
            InitializeComponent();

            txtError.Text = ex.Message + Environment.NewLine + "STACK TRACE" + Environment.NewLine + ex.StackTrace;

            txtError.Text += ex.InnerException != null
                                 ? Environment.NewLine + "INNER EXCEPTION" + Environment.NewLine +
                                   ex.InnerException.Message
                                 : string.Empty;
        }
    }
}
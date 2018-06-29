using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using IsoFinder.Model;
using Newtonsoft.Json;

namespace IsoFinder.Desktop
{
    public partial class LastRequests : Form
    {
        public LastRequests()
        {
            InitializeComponent();

            var webClient = new WebClient();
            webClient.DownloadStringCompleted += WebClientOnDownloadStringCompleted;
            webClient.DownloadStringTaskAsync(
                new Uri(string.Format(Constants.IsoFinderApiUrl + Constants.IsoFinderLatestRequestUri, "any", 10)));
        }

        private void WebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            try
            {
                if (downloadStringCompletedEventArgs.Error == null)
                {
                    var isoFinderRequestStatus =
                        JsonConvert.DeserializeObject<List<IsoFinderRequestStatus>>(downloadStringCompletedEventArgs.Result);
                    foreach (var request in isoFinderRequestStatus)
                    {
                        lstItems.Items.Add(request.DownloadLink);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lstItems_DoubleClick(object sender, EventArgs e)
        {
            if (lstItems.SelectedItem != null)
            {
                Process.Start(lstItems.SelectedItem.ToString());
            }
        }
    }
}
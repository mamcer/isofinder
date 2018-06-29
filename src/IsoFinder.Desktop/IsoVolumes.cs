using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace IsoFinder.Desktop
{
    public partial class IsoVolumes : Form
    {
        readonly WebClient webClient = new WebClient();

        public IsoVolumes()
        {
            InitializeComponent();

            webClient.DownloadStringCompleted += WebClientOnDownloadStringCompleted;
            webClient.DownloadStringAsync(new Uri(Constants.IsoFinderApiUrl + Constants.IsoFinderVolumeNamesUri));
        }

        private void WebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error == null)
            {
                string data = downloadStringCompletedEventArgs.Result;
                var isoVolumes = JsonConvert.DeserializeObject<List<string>>(data);
                lblTotal.Text = string.Format("Total {0} iso volumes", isoVolumes.Count);
                listBox1.Items.AddRange(isoVolumes.ToArray());
            }
            else
            {
                MessageBox.Show("Unable to connect to Iso Finder api", Text, MessageBoxButtons.OK);
            }
        }
    }
}

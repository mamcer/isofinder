using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using IsoFinder.Model;
using Newtonsoft.Json;

namespace IsoFinder.Desktop
{
    public partial class Main : Form
    {
        private WebClient FileSearchWebClient { get; set; }

        private WebClient FolderSearchWebClient { get; set; }

        private WebClient SearchAheadWebClient { get; set; }

        private PaginationInfo FilePaginationInfo { get; set; }

        private PaginationInfo FolderPaginationInfo { get; set; }

        public Main()
        {
            InitializeComponent();

            FilePaginationInfo = new PaginationInfo();
            FolderPaginationInfo = new PaginationInfo();

            var webClient = new WebClient();
            lblTotal.Text = string.Empty;
            lblPage.Text = string.Empty;
            webClient.DownloadStringCompleted += WebClientOnDownloadStringCompleted;
            ConsoleLog("GET " + Constants.IsoFinderApiUrl + Constants.IsoFinderInfoUri);
            FileSearchWebClient = new WebClient();
            FolderSearchWebClient = new WebClient();
            FileSearchWebClient.DownloadStringCompleted += SearchWebClientOnDownloadStringCompleted;
            FolderSearchWebClient.DownloadStringCompleted += FolderSearchWebClientOnDownloadStringCompleted;
            webClient.DownloadStringAsync(new Uri(Constants.IsoFinderApiUrl + Constants.IsoFinderInfoUri, UriKind.Absolute));
            SearchAheadWebClient = new WebClient();
            SearchAheadWebClient.DownloadStringCompleted += SearchAheadWebClientOnDownloadStringCompleted;
        }

        private void FolderSearchWebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            lblTotal.Text = string.Empty;
            gridResults.Enabled = true;
            tabControl1.Enabled = true;
            btnDownload.Enabled = true;
            lnkNext.Enabled = true;
            lnkPrevious.Enabled = true;
            if (downloadStringCompletedEventArgs.Error == null)
            {
                string data = downloadStringCompletedEventArgs.Result;

                var searchResults = JsonConvert.DeserializeObject<SearchResultInfo<FolderSearchResult>>(data);
                ConsoleLog("OK total " + searchResults.Total + " results");
                FolderPaginationInfo.Total = searchResults.Total;
                lblTotalFolders.Text = string.Format("Total {0} folders found", searchResults.Total);
                foreach (FolderSearchResult searchResult in searchResults.Results)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Id});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Name});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.IsoId});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.ParentFolderId});
                    gridFolderResults.Rows.Add(row);
                }

                if (tabResults.SelectedIndex == 1)
                {
                    lblPage.Text = string.Format("Page {0}/{1}", FolderPaginationInfo.CurrentPage + 1,
                        (FolderPaginationInfo.Total/Constants.PageSize) == 0
                            ? 1
                            : (FolderPaginationInfo.Total/Constants.PageSize));
                }
            }
            else
            {
                ConsoleLog("Error Unable to connect to Iso Finder api");
            }
        }

        private void SearchAheadWebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error == null)
            {
                string data = downloadStringCompletedEventArgs.Result;

                var results = JsonConvert.DeserializeObject<List<string>>(data);
                flowLayoutPanel1.Controls.Clear();
                if (results.Count > 0)
                {
                    foreach (var result in results.Take(5))
                    {
                        var link = new LinkLabel {Text = result};
                        link.LinkClicked += LinkOnLinkClicked;
                        link.AutoSize = true;
                        flowLayoutPanel1.Controls.Add(link);
                    }
                }
            }
            else
            {
                ConsoleLog("Error Unable to connect to Iso Finder api");
            }
        }

        private void LinkOnLinkClicked(object sender, LinkLabelLinkClickedEventArgs linkLabelLinkClickedEventArgs)
        {
            var link = sender as LinkLabel;
            if (link != null)
            {
                txtSearch.Text = link.Text;
                FilePaginationInfo.CurrentPage = 0;
                FilePaginationInfo.Total = 0;
                FolderPaginationInfo.CurrentPage = 0;
                FolderPaginationInfo.Total = 0;

                Search();
            }
        }

        private void ConsoleLog(string msg)
        {
            txtConsole.Text += DateTime.Now.ToString("yyyy.dd.MM-hh:mm:ss") + " - " + msg + Environment.NewLine;
            txtConsole.SelectionStart = txtConsole.Text.Length;
            txtConsole.ScrollToCaret();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            FilePaginationInfo.CurrentPage = 0;
            FilePaginationInfo.Total = 0;
            FolderPaginationInfo.CurrentPage = 0;
            FolderPaginationInfo.Total = 0;
            
            if (e.KeyData == Keys.Enter && !string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                Search();
            }
            else
            {
                if (txtSearch.Text.Length > 3)
                {
                    if (SearchAheadWebClient.IsBusy)
                    {
                        ConsoleLog("Cancel search ahead.");
                        SearchAheadWebClient.CancelAsync();
                    }

                    var uri = string.Format(Constants.IsoFinderApiUrl + Constants.IsoFinderSearchAheadUri,
                                            txtSearch.Text, 10);
                    ConsoleLog("GET " + uri);
                    SearchAheadWebClient.DownloadStringAsync(new Uri(uri));
                }
            }
        }

        private void Search()
        {
            gridResults.Enabled = false;
            tabControl1.Enabled = false;
            btnDownload.Enabled = false;
            lnkNext.Enabled = false;
            lnkPrevious.Enabled = false;
            string filesSearchUri = string.Format(Constants.IsoFinderApiUrl + Constants.IsoFinderFilesSearchUri, txtSearch.Text, FilePaginationInfo.CurrentPage,
                           Constants.PageSize);
            string foldersSearchUri = string.Format(Constants.IsoFinderApiUrl + Constants.IsoFinderFoldersSearchUri, txtSearch.Text, FolderPaginationInfo.CurrentPage,
                           Constants.PageSize);
            ConsoleLog("GET " + filesSearchUri);
            ConsoleLog("GET " + foldersSearchUri);
            FileSearchWebClient.DownloadStringAsync(new Uri(filesSearchUri, UriKind.Absolute));
            FolderSearchWebClient.DownloadStringAsync(new Uri(foldersSearchUri, UriKind.Absolute));
            lblTotal.Text = "Searching...";
            lblTotalFolders.Text = "Searching...";
            lblPage.Text = string.Empty;
            gridResults.Rows.Clear();
            gridFolderResults.Rows.Clear();
        }

        private void SearchWebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            lblTotal.Text = string.Empty;
            gridResults.Enabled = true;
            tabControl1.Enabled = true;
            btnDownload.Enabled = true;
            lnkNext.Enabled = true;
            lnkPrevious.Enabled = true;
            if (downloadStringCompletedEventArgs.Error == null)
            {
                string data = downloadStringCompletedEventArgs.Result;

                var searchResults = JsonConvert.DeserializeObject<SearchResultInfo<FileSearchResult>>(data);
                ConsoleLog("OK total " + searchResults.Total + " results");
                FilePaginationInfo.Total = searchResults.Total;
                lblTotal.Text = string.Format("Total {0} files found", searchResults.Total);
                foreach (FileSearchResult searchResult in searchResults.Results)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Id});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Name});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Extension});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.IsoVolumeLabel});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Path});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.SizeString});
                    row.Cells.Add(new DataGridViewTextBoxCell {Value = searchResult.Created.ToShortDateString()});
                    gridResults.Rows.Add(row);
                }

                if (tabResults.SelectedIndex == 0)
                {
                    lblPage.Text = string.Format("Page {0}/{1}", FilePaginationInfo.CurrentPage + 1,
                        (FilePaginationInfo.Total/Constants.PageSize) == 0
                            ? 1
                            : (FilePaginationInfo.Total/Constants.PageSize));
                }
            }
            else
            {
                ConsoleLog("Error Unable to connect to Iso Finder api");
            }
        }

        private void WebClientOnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            if (downloadStringCompletedEventArgs.Error == null)
            {
                string data = downloadStringCompletedEventArgs.Result;
                ConsoleLog("OK " + Constants.IsoFinderApiUrl + Constants.IsoFinderInfoUri);

                var isoFinderInfo = JsonConvert.DeserializeObject<IsoFinderInfo>(data);
                lblInfo.Text = string.Format("Searching over {0} iso volumes. {1} files.", isoFinderInfo.IsoVolumeCount, isoFinderInfo.IsoFileCount);
            }
            else
            {
                ConsoleLog("Error - Unable to connect to Iso Finder api" + Constants.IsoFinderApiUrl + Constants.IsoFinderInfoUri);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (tabResults.SelectedIndex == 0)
            {
                if (gridResults.SelectedRows.Count > 0)
                {
                    var ids =
                        (from DataGridViewRow selectedRow in gridResults.SelectedRows
                            select Convert.ToInt32(selectedRow.Cells[0].Value)).ToList();

                    var webClient = new WebClient();
                    var isoFinderRequest = new IsoFinderFileRequest
                    {
                        UserId = 1,
                        FileIds = ids.ToArray(),
                        Query = txtSearch.Text
                    };
                    webClient.UploadStringCompleted += WebClientOnUploadStringCompleted;
                    var serialize = JsonConvert.SerializeObject(isoFinderRequest);
                    webClient.Headers.Add("Content-Type", @"application/json");
                    webClient.UploadStringAsync(
                        new Uri(Constants.IsoFinderApiUrl + Constants.IsoFinderFileRequestUri), "POST", serialize);
                }
            }
            else
            {
                if (gridFolderResults.SelectedRows.Count > 0)
                {
                    var ids =
                        (from DataGridViewRow selectedRow in gridFolderResults.SelectedRows
                         select Convert.ToInt32(selectedRow.Cells[0].Value)).ToList();

                    var webClient = new WebClient();
                    var isoFinderRequest = new IsoFinderFolderRequest
                    {
                        UserId = 1,
                        FolderIds = ids.ToArray(),
                        Query = txtSearch.Text
                    };

                    webClient.UploadStringCompleted += WebClientOnUploadStringCompleted;
                    var serialize = JsonConvert.SerializeObject(isoFinderRequest);
                    webClient.Headers.Add("Content-Type", @"application/json");
                    webClient.UploadStringAsync(
                        new Uri(Constants.IsoFinderApiUrl + Constants.IsoFinderFolderRequestUri), "POST", serialize);
                }
            }
        }

        private void WebClientOnUploadStringCompleted(object sender, UploadStringCompletedEventArgs uploadStringCompletedEventArgs)
        {
            if (uploadStringCompletedEventArgs.Error == null)
            {
                ConsoleLog("OK " + Constants.IsoFinderApiUrl + Constants.IsoFinderFileRequestUri);
            }
            else
            {
                ConsoleLog("Error - " + uploadStringCompletedEventArgs.Error.Message + " - " + Constants.IsoFinderApiUrl + Constants.IsoFinderFileRequestUri);
            }
        }

        private void lnkDownloads_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Downloads();
        }

        private void Downloads()
        {
            var lastRequests = new LastRequests();
            lastRequests.ShowDialog(this);
        }

        private void lnkNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if ((GetTotal() / Constants.PageSize) > (GetCurrentPage() + 1))
            {
                AddCurrentPage(1);
                Search();
            }
        }

        private void AddCurrentPage(int index)
        {
            if (tabResults.SelectedIndex == 0)
            {
                FilePaginationInfo.CurrentPage += index;
            }
            else
            {
                FolderPaginationInfo.CurrentPage += index;
            }
        }

        private int GetCurrentPage()
        {
            if (tabResults.SelectedIndex == 0)
            {
                return FilePaginationInfo.CurrentPage;
            }

            return FolderPaginationInfo.CurrentPage;
        }

        private double GetTotal()
        {
            if (tabResults.SelectedIndex == 0)
            {
                return FilePaginationInfo.Total;
            }

            return FolderPaginationInfo.Total;
        }

        private void lnkPrevious_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (GetCurrentPage() > 0)
            {
                AddCurrentPage(1);
                Search();
            }
        }

        private void downloadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Downloads();
        }

        private void isoVolumesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var isoVolumes = new IsoVolumes();
            isoVolumes.ShowDialog(this);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            FilePaginationInfo.CurrentPage = 0;
            FilePaginationInfo.Total = 0;
            FolderPaginationInfo.CurrentPage = 0;
            FolderPaginationInfo.Total = 0;

            Search();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            txtSearch.Focus();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tabResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabResults.SelectedIndex == 0)
            {
                lblPage.Text = string.Format("Page {0}/{1}", FilePaginationInfo.CurrentPage + 1,
                    (FilePaginationInfo.Total / Constants.PageSize) == 0
                        ? 1
                        : (FilePaginationInfo.Total / Constants.PageSize));
            }
            else
            {
                lblPage.Text = string.Format("Page {0}/{1}", FolderPaginationInfo.CurrentPage + 1,
                    (FolderPaginationInfo.Total / Constants.PageSize) == 0
                        ? 1
                        : (FolderPaginationInfo.Total / Constants.PageSize));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Add(new LinkLabel{ Text = "Hello There"});
        }
    }
}

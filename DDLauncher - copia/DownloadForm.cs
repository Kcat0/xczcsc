using System.Windows.Forms;

namespace Dragones_de_Dojima
{
    public partial class DownloadForm : Form
    {
        public DownloadForm()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int percentage, string status)
        {
            progressBarDownload.Value = percentage;
            lblDownloadStatus.Text = status;
        }
    }
}

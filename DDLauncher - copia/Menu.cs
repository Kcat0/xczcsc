using System;
using System.Diagnostics;
using System.Windows.Forms;


namespace Dragones_de_Dojima
{
    public partial class Menu : Form
    {

        public Menu()
        {
            InitializeComponent();
        }
        private void OpenWebPage(string url)
        {
            try
            {
                // Usar ProcessStartInfo para abrir URLs
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Abre en el navegador predeterminado
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el navegador. Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            OpenWebPage("https://x.com/Dragones_Dojima");
        }

        private void discord_Click(object sender, EventArgs e)
        {
            OpenWebPage("https://discord.gg/jdwb8QCNWg");
        }

        private void Youtube_Click(object sender, EventArgs e)
        {
            OpenWebPage("https://www.youtube.com/@DragonesDojima");
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Firebase.Database;
using Firebase.Database.Query;

namespace Dragones_de_Dojima
{
    public partial class Form1 : Form
    {
        private readonly Y3 yakuzaForm;
        private readonly YK2 yakuzaK2Form;
        private readonly FirebaseClient firebaseClient;
        private const string versionActual = "2.0.3"; // Versión actual de tu aplicación
        private DownloadForm downloadForm;
        private readonly System.Windows.Forms.Timer scrollTimer;
        private int scrollTarget;
        private int scrollIncrement;
        private Panel targetPanel;

        public Form1()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://launcher-a44bc-default-rtdb.firebaseio.com/");

            // Inicializar la instancia del formulario en el constructor
            yakuzaForm = new Y3();
            yakuzaK2Form = new YK2();

            // Inicializar la instancia del formulario Menu y mostrarlo en el panelContenedor
            var menuForm = new Menu();
            if (!panelContenedor.Controls.Contains(menuForm))
            {
                panelContenedor.Controls.Clear();
                menuForm.TopLevel = false;
                menuForm.FormBorderStyle = FormBorderStyle.None;
                menuForm.Dock = DockStyle.Fill;
                panelContenedor.Controls.Add(menuForm);
                menuForm.Show();
            }

            // Verificar actualizaciones al inicio
            _ = CheckForUpdates();


            // Habilitar el doble buffering para los paneles
            ApplyDoubleBuffering(this);

            menuForm = new Menu();
            yakuzaForm = new Y3();
            yakuzaK2Form = new YK2();

            // Configurar los formularios pero no mostrarlos aún
            ConfigureForm(menuForm);
            ConfigureForm(yakuzaForm);
            ConfigureForm(yakuzaK2Form);

        }

        private void ConfigureForm(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
        }

        private static void OpenWebPage(string url)
        {
            try
            {
                // Usar ProcessStartInfo para abrir URLs
                var startInfo = new ProcessStartInfo
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

        private void BtnX_Click(object sender, EventArgs e)
        {
            OpenWebPage("https://x.com/Dragones_Dojima");
        }

        private void Discord_Click(object sender, EventArgs e)
        {
            OpenWebPage("https://discord.gg/jdwb8QCNWg");
        }

        private void Youtube_Click(object sender, EventArgs e)
        {
            OpenWebPage("https://www.youtube.com/@DragonesDojima");
        }

        private async Task<bool> CheckForUpdates()
        {
            try
            {
                var versionInfo = await firebaseClient
                    .Child("version")
                    .OnceSingleAsync<VersionInfo>();

                if (versionInfo == null)
                {
                    MessageBox.Show("No se pudo obtener información de la versión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (versionInfo.Version != versionActual)
                {
                    var result = MessageBox.Show($"Hay una nueva versión disponible: {versionInfo.Version}. ¿Desea descargarla?", "Actualización disponible", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        return await DownloadAndUpdate(versionInfo.Url);
                    }
                }
                //          else
                //            {
                //                  MessageBox.Show("Ya tienes la última versión.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al verificar actualizaciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private async Task<bool> DownloadAndUpdate(string downloadUrl)
        {
            try
            {
                downloadForm = new DownloadForm();
                downloadForm.Show();

                string tempFilePath = Path.Combine(Path.GetTempPath(), "new_version.exe");

                using (HttpClient httpClient = new())
                {
                    var response = await httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al descargar la nueva versión: " + response.ReasonPhrase, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var httpStream = await response.Content.ReadAsStreamAsync())
                        {
                            var buffer = new byte[8192];
                            long totalRead = 0;
                            int bytesRead;

                            while ((bytesRead = await httpStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);

                                if (canReportProgress)
                                {
                                    totalRead += bytesRead;
                                    int progressPercentage = (int)((totalRead * 100) / totalBytes);
                                    downloadForm.UpdateProgress(progressPercentage, $"{totalRead / 1024 / 1024} MB de {totalBytes / 1024 / 1024} MB");
                                }
                            }
                        }
                    }
                }

                string currentFilePath = Application.ExecutablePath;
                string backupFilePath = currentFilePath + ".bak";

                try
                {
                    // Renombra el archivo actual a .bak
                    if (File.Exists(backupFilePath))
                    {
                        File.Delete(backupFilePath);
                    }

                    File.Move(currentFilePath, backupFilePath);

                    // Mueve el nuevo archivo al lugar del actual
                    File.Move(tempFilePath, currentFilePath);

                    // Lanza la nueva versión
                    Process.Start(currentFilePath);

                    // Cierra la aplicación actual
                    Application.Exit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al aplicar la actualización: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar o actualizar la aplicación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Y3_Click(object sender, EventArgs e)
        {
            // Asegurarse de que el formulario no se muestre más de una vez
            if (!panelContenedor.Controls.Contains(yakuzaForm))
            {
                // Limpiar el panel antes de agregar el formulario
                panelContenedor.Controls.Clear();

                // Establecer el formulario como secundario del panel
                yakuzaForm.TopLevel = false;
                yakuzaForm.FormBorderStyle = FormBorderStyle.None;
                yakuzaForm.Dock = DockStyle.Fill;

                // Agregar el formulario al panel
                panelContenedor.Controls.Add(yakuzaForm);

                // Mostrar el formulario
                yakuzaForm.Show();
            }
        }



        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            int currentScroll = targetPanel.VerticalScroll.Value;
            int newScroll = Math.Min(currentScroll + scrollIncrement, scrollTarget);
            targetPanel.VerticalScroll.Value = newScroll;
            targetPanel.PerformLayout();

            if (newScroll == scrollTarget)
            {
                scrollTimer.Stop();
            }
        }

        private void ScrollTo(Panel panel, int targetPosition)
        {
            targetPanel = panel;
            scrollTarget = targetPosition;
            scrollIncrement = (scrollTarget - panel.VerticalScroll.Value) / 10;
            scrollTimer.Start();
        }

        // Ejemplo de cómo llamar al desplazamiento suave en un botón
        private void SomeButton_Click(object sender, EventArgs e)
        {
            ScrollTo(panel1, panel1.VerticalScroll.Maximum);
        }

        private void ApplyDoubleBuffering(Control control)
        {
            if (control is Panel panel)
            {
                // Habilitar el doble buffering usando reflexión
                typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel, new object[] { true });
            }

            foreach (Control child in control.Controls)
            {
                ApplyDoubleBuffering(child);
            }
        }

        private void yk2_Click(object sender, EventArgs e)
        {
            // Asegurarse de que el formulario no se muestre más de una vez
            if (!panelContenedor.Controls.Contains(yakuzaK2Form))
            {
                // Limpiar el panel antes de agregar el formulario
                panelContenedor.Controls.Clear();

                // Establecer el formulario como secundario del panel
                yakuzaK2Form.TopLevel = false;
                yakuzaK2Form.FormBorderStyle = FormBorderStyle.None;
                yakuzaK2Form.Dock = DockStyle.Fill;

                // Agregar el formulario al panel
                panelContenedor.Controls.Add(yakuzaK2Form);

                // Mostrar el formulario
                yakuzaK2Form.Show();
            }
        }
    }

    public class VersionInfo
    {
        public string Version { get; set; }
        public string Url { get; set; }
    }
}

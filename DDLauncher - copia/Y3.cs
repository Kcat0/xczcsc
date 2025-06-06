using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Reflection;

namespace Dragones_de_Dojima
{
    public partial class Y3 : Form
    {
        private const int BufferSize = 8192;
        private FirebaseClient firebaseClient;
        private System.Windows.Forms.Timer scrollTimer;
        private int scrollTarget;
        private int scrollIncrement;
        private Panel targetPanel;
        private int totalDescargas; // Declarar la variable totalDescargas

        public Y3()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://launcher-a44bc-default-rtdb.firebaseio.com/");

            // Aplicar doble buffering a todos los controles del formulario
            ApplyDoubleBuffering(this);

            // Inicializar el temporizador para el desplazamiento suave
            scrollTimer = new System.Windows.Forms.Timer();
            scrollTimer.Interval = 1;
            scrollTimer.Tick += ScrollTimer_Tick;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();

        }


        private async Task<List<ArchivoDescarga>> ObtenerDatosDesdeFirebase(Predicate<ArchivoDescarga> filtro = null)
        {
            var archivos = await firebaseClient
                .Child("archivos_yakuza3")
                .Child("archivos")
                .OnceSingleAsync<List<ArchivoDescarga>>();

            if (filtro != null)
            {
                archivos = archivos.FindAll(filtro);
            }

            return archivos;
        }

        private async Task<Dictionary<string, ArchivoDescarga>> ObtenerDescargasOpcionales()
        {
            var descargasOpcionales = await firebaseClient
                .Child("archivos_yakuza3")
                .Child("archivos_opcionales")
                .OnceSingleAsync<Dictionary<string, ArchivoDescarga>>();

            return descargasOpcionales;
        }

        private async void descargar_Click(object sender, EventArgs e)
        {
            var opcionesForm = new OpcionesDescargaY3();
            if (opcionesForm.ShowDialog() == DialogResult.OK)
            {
                await DescargarArchivos((archivo) => true, opcionesForm.OpcionesSeleccionadas);
            }
        }

        private async void descargar_xbox_Click(object sender, EventArgs e)
        {
            var opcionesForm = new OpcionesDescargaY3();
            if (opcionesForm.ShowDialog() == DialogResult.OK)
            {
                await DescargarArchivos((archivo) => archivo.Id != "l8buheip3gd394qscsg3j/Yakuza3.exe?rlkey=wrzwu3hfx11p09scgqszg32rn&st=mqi53sia&dl=1", opcionesForm.OpcionesSeleccionadas);
            }
        }

        private async Task DescargarArchivos(Predicate<ArchivoDescarga> filtro, List<string> archivosOpcionalesSeleccionados)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;

                string selectedPath = folderDialog.SelectedPath;
                string dataFolderPath = Path.Combine(selectedPath, "data");

                if (!Directory.Exists(dataFolderPath))
                {
                    MessageBox.Show("La carpeta 'data' no se encuentra en la ruta seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var archivos = await ObtenerDatosDesdeFirebase(filtro);
                var descargasOpcionales = await ObtenerDescargasOpcionales();

                foreach (var opcion in archivosOpcionalesSeleccionados)
                {
                    if (descargasOpcionales.ContainsKey(opcion))
                    {
                        archivos.Add(descargasOpcionales[opcion]);
                    }
                }

                if (archivos == null || archivos.Count == 0)
                {
                    MessageBox.Show("No se encontraron archivos para descargar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                progressBar1.Visible = true;
                lblNombreArchivo.Visible = true;
                lblVelocidadInternet.Visible = true;

                using (HttpClient httpClient = new HttpClient())
                {
                    try
                    {
                        foreach (var archivo in archivos)
                        {
                            string fileId = archivo.Id;
                            string destinationPath = Path.Combine(selectedPath, archivo.Ruta);
                            string fileName = Path.GetFileName(destinationPath);
                            lblNombreArchivo.Text = $"Descargando: {fileName} {archivos.IndexOf(archivo) + 1}/{archivos.Count}";

                            // Asegúrate de que la carpeta de destino exista
                            string destinationFolder = Path.GetDirectoryName(destinationPath);
                            if (!Directory.Exists(destinationFolder))
                            {
                                Directory.CreateDirectory(destinationFolder);
                            }

                            string fileDownloadUrl = $"https://www.dropbox.com/scl/fi/{fileId}";

                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadProgressChanged += (s, args) =>
                                {
                                    progressBar1.Value = args.ProgressPercentage;

                                    string contentLength = webClient.ResponseHeaders["Content-Length"];
                                    if (!string.IsNullOrEmpty(contentLength))
                                    {
                                        lblVelocidadInternet.Text = $"{(args.BytesReceived / (1024.0 * 1024)):F2} MB / {(double.Parse(contentLength) / (1024 * 1024)):F2} MB";
                                    }
                                    else
                                    {
                                        lblVelocidadInternet.Text = $"{(args.BytesReceived / (1024.0 * 1024)):F2} MB";
                                    }
                                };

                                await webClient.DownloadFileTaskAsync(new Uri(fileDownloadUrl), destinationPath);
                            }
                        }

                        // Después de descargar todos los archivos, registra la descarga
                        DateTime fechaDescargaUtc = DateTime.UtcNow;
                        DateTime fechaDescargaLocal = fechaDescargaUtc.AddHours(-5);
                        string nodoFechas = "fechas_yakuza3";

                        await firebaseClient
                            .Child("estadisticas")
                            .Child(nodoFechas)
                            .PostAsync(new { Fecha = fechaDescargaLocal.ToString() });

                        var currentDownloads = await firebaseClient
                            .Child("estadisticas")
                            .Child("descargas_totales_yakuza3")
                            .OnceSingleAsync<int>();

                        if (currentDownloads != null)
                        {
                            totalDescargas = currentDownloads;
                        }

                        totalDescargas += 1; // Incrementa en 1 ya que todas las descargas cuentan como una única descarga

                        await firebaseClient
                            .Child("estadisticas")
                            .Child("descargas_totales_yakuza3")
                            .PutAsync(totalDescargas);

                        MessageBox.Show("Todos los archivos se han descargado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Mostrar mensaje para apoyar con donación
                        DialogResult resultadoDonacion = MessageBox.Show(
                            "¿Te gusta nuestro trabajo y quieres apoyarnos? ¡Dónanos!",
                            "Apóyanos",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (resultadoDonacion == DialogResult.Yes)
                        {
                            // Abrir el enlace de donación
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "https://ko-fi.com/dragonesdedojima",
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"No se pudo abrir el enlace de donación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error durante la descarga: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        progressBar1.Visible = false;
                        lblNombreArchivo.Visible = false;
                        lblVelocidadInternet.Visible = false;
                    }
                }
            }
        }


        public class ArchivoDescarga
        {
            public string Id { get; set; }
            public string Ruta { get; set; }
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

        private void ScrollTo(Panel panel5, int targetPosition)
        {
            targetPanel = panel5;
            scrollTarget = targetPosition;
            scrollIncrement = Math.Max((scrollTarget - panel5.VerticalScroll.Value) / 100, 1); // Incremento más pequeño para suavizar
            scrollTimer.Start();
        }


        private void ApplyDoubleBuffering(Control control)
        {
            // Habilitar el doble buffering para cualquier tipo de control
            typeof(Control).InvokeMember("DoubleBuffered",
                                         BindingFlags.SetProperty |
                                         BindingFlags.Instance |
                                         BindingFlags.NonPublic,
                                         null,
                                         control,
                                         new object[] { true });

            // Verificar en la consola que se aplique a cada control
            Console.WriteLine($"Double buffering aplicado a: {control.GetType().Name}");

            // Aplicar doble buffering a todos los controles hijos
            foreach (Control child in control.Controls)
            {
                ApplyDoubleBuffering(child);
            }
        }


        private void ayuda_Click(object sender, EventArgs e)
        {
            // Crear una instancia del formulario Instrucciones
            Instrucciones instruccionesForm = new Instrucciones();

            // Mostrar el formulario como una ventana modal
            instruccionesForm.ShowDialog();
        }

    }
}
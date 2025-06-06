using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Dragones_de_Dojima
{
    public partial class VersionForm : Form
    {
        public string Plataforma { get; private set; }  // Propiedad que almacena la plataforma
        public string SelectedPath { get; private set; } // Propiedad que almacena la ruta seleccionada por el usuario
        public bool ParcheoAdicional { get; private set; } // Propiedad para el estado del checkbox

        // Variable para almacenar el diccionario pasado por parámetro
        private readonly Dictionary<string, Dictionary<string, (string, string, string, string)>> archivosPorPlataforma;

        // Lista para almacenar los archivos detectados como correctos
        private List<string> archivosCorrectos = new List<string>();

        // Constructor principal que recibe el diccionario
        public VersionForm(Dictionary<string, Dictionary<string, (string, string, string, string)>> archivosPorPlataforma)
        {
            InitializeComponent();
            this.archivosPorPlataforma = archivosPorPlataforma ?? throw new ArgumentNullException(nameof(archivosPorPlataforma));
        }

        // Constructor vacío por si no se necesita pasar el diccionario (opcional)
        public VersionForm() : this(new Dictionary<string, Dictionary<string, (string, string, string, string)>>()) { }

        // Diccionario de checksums por versión
        private static readonly Dictionary<string, string> Checksums = new Dictionary<string, string>
        {
            { "Steam / GOG", "2ec350cb40ac3660d56fa8b3c5f3434718014accd9166d5b9c37f376dedb30e6" },
            { "Gog", "indeterminado" },
            { "Microsoft Store", "a033ebacc9d825d495332289588fe24d3e6d22707bfb58d2284ec3ed1f8344d3" }
        };

        private async void browseButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedPath = folderDialog.SelectedPath; // Almacenar la ruta seleccionada por el usuario
                    pathTextBox.Text = SelectedPath;
                    string dataFolderPath = Path.Combine(SelectedPath, "data");

                    if (!Directory.Exists(dataFolderPath))
                    {
                        versionLabel.Text = "Versión detectada: No se pudo detectar la versión (La carpeta 'data' no existe)";
                        aceptarbtn.Enabled = false;
                        return;
                    }

                    string talkLexusFilePath = Path.Combine(dataFolderPath, "talk_lexus2.par");

                    if (!File.Exists(talkLexusFilePath))
                    {
                        versionLabel.Text = "Versión detectada: No se pudo detectar la versión (El archivo 'talk_lexus2.par' no existe)";
                        aceptarbtn.Enabled = false;
                        return;
                    }

                    versionLabel.Text = "Verificando versión...";
                    aceptarbtn.Enabled = false;
                    archivosverificados.Visible = false;

                    string checksum = await Task.Run(() => CalcularChecksum(talkLexusFilePath));
                    if (checksum == null) return;

                    string version = IdentificarVersion(checksum);
                    if (version == null)
                    {
                        versionLabel.Text = $"Versión detectada: No se pudo detectar la versión (Checksum: {checksum})";
                        aceptarbtn.Enabled = false;
                        return;
                    }

                    versionLabel.Text = $"Versión detectada: {version}";
                    aceptarbtn.Enabled = true;
                    archivosverificados.Visible = true;

                    // Asignar la plataforma detectada
                    if (version == "Steam / GOG")
                    {
                        Plataforma = "steam";
                    }
                    else if (version == "Gog")
                    {
                        Plataforma = "gog";
                    }
                    else if (version == "Microsoft Store")
                    {
                        Plataforma = "microsoft store";
                    }
                    else
                    {
                        Plataforma = null;
                    }

                    if (string.IsNullOrEmpty(Plataforma))
                    {
                        MessageBox.Show("La plataforma no está definida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (ArchivosManagerYK2.ArchivosPorPlataforma.TryGetValue(Plataforma.ToLower(), out var archivosVersion))
                    {
                        checksumsrevisar.Items.Clear();
                        archivosCorrectos.Clear();
                        aceptarbtn.Enabled = false;

                        int totalArchivos = archivosVersion.Count;
                        int archivosVerificadosCorrectamente = 0;

                        foreach (var archivo in archivosVersion)
                        {
                            string nombreArchivo = archivo.Key;
                            string rutaInterna = archivo.Value.Item2;

                            string archivoCompleto = Path.Combine(SelectedPath, rutaInterna);

                            if (File.Exists(archivoCompleto))
                            {
                                string checksumArchivo = await Task.Run(() => CalcularChecksum(archivoCompleto));
                                if (checksumArchivo == archivo.Value.Item4)
                                {
                                    checksumsrevisar.Items.Add($"{nombreArchivo}: Correcto");
                                    archivosCorrectos.Add(archivoCompleto);
                                    archivosVerificadosCorrectamente++;
                                }
                                else
                                {
                                    checksumsrevisar.Items.Add($"{nombreArchivo}: Incorrecto");
                                }
                            }
                            else
                            {
                                checksumsrevisar.Items.Add($"{nombreArchivo}: No encontrado");
                            }

                            archivosverificados.Text = $"Archivos: {archivosVerificadosCorrectamente}/{totalArchivos}";
                        }

                        // Verificar los archivos comunes
                        if (ArchivosManagerYK2.ArchivosPorPlataforma.TryGetValue("comunes", out var archivosComunes))
                        {
                            totalArchivos += archivosComunes.Count;

                            foreach (var archivo in archivosComunes)
                            {
                                string nombreArchivo = archivo.Key;
                                string rutaInterna = archivo.Value.Item2;

                                string archivoCompleto = Path.Combine(SelectedPath, rutaInterna);

                                if (File.Exists(archivoCompleto))
                                {
                                    string checksumArchivo = await Task.Run(() => CalcularChecksum(archivoCompleto));
                                    if (checksumArchivo == archivo.Value.Item4)
                                    {
                                        checksumsrevisar.Items.Add($"{nombreArchivo}: Correcto");
                                        archivosCorrectos.Add(archivoCompleto);
                                        archivosVerificadosCorrectamente++;
                                    }
                                    else
                                    {
                                        checksumsrevisar.Items.Add($"{nombreArchivo}: Incorrecto");
                                    }
                                }
                                else
                                {
                                    checksumsrevisar.Items.Add($"{nombreArchivo}: No encontrado");
                                }

                                archivosverificados.Text = $"Archivos: {archivosVerificadosCorrectamente}/{totalArchivos}";
                            }
                        }

                        // Verificar los archivos adicionales
                        Dictionary<string, (string, string, string, string)> archivosAdicionales;
                        if (Plataforma.ToLower() == "steam" || Plataforma.ToLower() == "gog")
                        {
                            archivosAdicionales = ArchivosManagerYK2.ArchivosPorPlataforma["adicionales steam y gog"];
                        }
                        else if (Plataforma.ToLower() == "microsoft store")
                        {
                            archivosAdicionales = ArchivosManagerYK2.ArchivosPorPlataforma["adicionales microsoft store"];
                        }
                        else
                        {
                            archivosAdicionales = new Dictionary<string, (string, string, string, string)>();
                        }

                        totalArchivos += archivosAdicionales.Count;

                        foreach (var archivo in archivosAdicionales)
                        {
                            string nombreArchivo = archivo.Key;
                            string rutaInterna = archivo.Value.Item2;

                            string archivoCompleto = Path.Combine(SelectedPath, rutaInterna);

                            if (File.Exists(archivoCompleto))
                            {
                                string checksumArchivo = await Task.Run(() => CalcularChecksum(archivoCompleto));
                                if (checksumArchivo == archivo.Value.Item4)
                                {
                                    checksumsrevisar.Items.Add($"{nombreArchivo}: Correcto");
                                    archivosCorrectos.Add(archivoCompleto);
                                    archivosVerificadosCorrectamente++;
                                }
                                else
                                {
                                    checksumsrevisar.Items.Add($"{nombreArchivo}: Incorrecto");
                                }
                            }
                            else
                            {
                                checksumsrevisar.Items.Add($"{nombreArchivo}: No encontrado");
                            }

                            archivosverificados.Text = $"Archivos: {archivosVerificadosCorrectamente}/{totalArchivos}";
                        }

                        if (archivosVerificadosCorrectamente < totalArchivos)
                        {
                            int archivosNoVerificados = totalArchivos - archivosVerificadosCorrectamente;
                            DialogResult result = MessageBox.Show($"No se verificaron {archivosNoVerificados} archivos. ¿Desea continuar? Tenga en cuenta que podría haber elementos sin traducir o el juego pueda fallar.", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                aceptarbtn.Enabled = true;
                            }
                            else
                            {
                                this.DialogResult = DialogResult.Cancel;
                                this.Close();
                            }
                        }
                        else
                        {
                            aceptarbtn.Enabled = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"No se encontraron archivos asociados para la versión detectada: {version}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string CalcularChecksum(string archivo)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                using (var stream = File.OpenRead(archivo))
                {
                    byte[] buffer = new byte[8192]; // Tamaño del bloque
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        sha256.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    }
                    sha256.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                    return BitConverter.ToString(sha256.Hash).Replace("-", "").ToLower();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el checksum del archivo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Método para identificar la versión según el checksum
        private string IdentificarVersion(string checksum)
        {
            foreach (var entry in Checksums)
            {
                if (entry.Value.Equals(checksum, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Key;
                }
            }
            return null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void aceptarbtn_Click(object sender, EventArgs e)
        {
            // Verifica si se detectó una versión
            if (versionLabel.Text.StartsWith("Versión detectada: "))
            {
                // Configurar el estado del checkbox
                ParcheoAdicional = checkBox1.Checked;

                // Configurar el DialogResult como OK y cerrar el formulario
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Primero detecta una versión antes de proceder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para acceder a los archivos correctos desde otro formulario
        public List<string> ObtenerArchivosCorrectos()
        {
            return archivosCorrectos;
        }
    }
}
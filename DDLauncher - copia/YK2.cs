using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using System.Security.Claims;

namespace Dragones_de_Dojima
{
    public partial class YK2 : Form
    {
        private FirebaseClient firebaseClient;
        private int totalDescargas;

        public YK2()
        {
            InitializeComponent();
            firebaseClient = new FirebaseClient("https://launcher-a44bc-default-rtdb.firebaseio.com/");
        }

        // Método para calcular el checksum de un archivo
        private string CalcularChecksum(string filePath)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = sha256.ComputeHash(fileStream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el checksum del archivo {filePath}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Función para extraer un recurso incrustado
        private async Task ExtractResourceAsync(string resourceName, string outputFilePath)
        {
            try
            {
                // Obtener el ensamblado actual
                Assembly assembly = Assembly.GetExecutingAssembly();

                // Intentar obtener el flujo del recurso incrustado
                using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        // Mostrar el recurso que no se encuentra y la ruta
                        string[] availableResources = assembly.GetManifestResourceNames();
                        MessageBox.Show($"El recurso {resourceName} no se encontró.\nRecursos disponibles:\n{string.Join("\n", availableResources)}\nRuta actual: {outputFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string directory = Path.GetDirectoryName(outputFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Copiar el recurso al archivo de salida
                    using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await resourceStream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar el error en caso de excepción
                MessageBox.Show($"Error al extraer el archivo {resourceName}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Función para aplicar el parche
        private async Task<bool> AplicarParcheAsync(string archivoOriginal, string archivoParche)
        {
            string tempExePath = Path.Combine(Path.GetTempPath(), "xdelta3.exe");
            string archivoOriginalTemp = archivoOriginal.Replace(Path.GetExtension(archivoOriginal), "_temp" + Path.GetExtension(archivoOriginal));

            try
            {
                // Siempre extraer xdelta3.exe para asegurarse de que esté actualizado y funcione
                await ExtractResourceAsync("Dragones_de_Dojima.Resources.xdelta3.exe", tempExePath);

                // Validar que xdelta3.exe fue correctamente extraído
                if (!File.Exists(tempExePath))
                {
                    MessageBox.Show("xdelta3.exe no fue extraído correctamente. No se puede continuar.", "Error crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Validar que la extensión del archivo sea compatible
                string[] extensionesPermitidas = { ".par", ".exe", ".bin", ".usm", ".dds" };
                if (!Array.Exists(extensionesPermitidas, ext => archivoOriginal.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"El archivo {archivoOriginal} no es compatible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Renombrar el archivo original a nombre temporal
                File.Move(archivoOriginal, archivoOriginalTemp);

                // Ejecutar xdelta3
                string argumentos = $"-d -s \"{archivoOriginalTemp}\" \"{archivoParche}\" \"{archivoOriginal}\"";
                System.Diagnostics.Process process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = tempExePath,
                        Arguments = argumentos,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return true;
                }
                else
                {
                    // Reintento una vez más en caso de fallo
                    MessageBox.Show($"Primer intento fallido.\n\nError: {error}", "Reintentando", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    process.Start();
                    output = await process.StandardOutput.ReadToEndAsync();
                    error = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        // Restaurar el archivo original en caso de error
                        File.Move(archivoOriginalTemp, archivoOriginal);
                        MessageBox.Show($"Fallo al aplicar parche (2 intentos).\n\nSalida:\n{output}\n\nError:\n{error}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Restaurar el archivo original si ocurre una excepción
                if (File.Exists(archivoOriginalTemp))
                {
                    File.Move(archivoOriginalTemp, archivoOriginal, true);
                }
                MessageBox.Show($"Excepción al aplicar el parche: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Eliminar archivos temporales
                if (File.Exists(tempExePath)) File.Delete(tempExePath);
                if (File.Exists(archivoParche)) File.Delete(archivoParche);
                if (File.Exists(archivoOriginalTemp)) File.Delete(archivoOriginalTemp);
            }
        }


        // Método para registrar la descarga en Firebase
        // Método para registrar la descarga en Firebase
        private async Task RegistrarDescargaAsync()
        {
            try
            {
                // Mantener estas líneas para registrar el total de descargas
                var currentDownloads = await firebaseClient
                    .Child("estadisticas")
                    .Child("descargas_totales_yakuzakiwami2")
                    .OnceSingleAsync<int>();

                if (currentDownloads != null)
                {
                    totalDescargas = currentDownloads;
                }

                totalDescargas += 1; // Incrementa en 1 ya que todas las descargas cuentan como una única descarga

                await firebaseClient
                    .Child("estadisticas")
                    .Child("descargas_totales_yakuzakiwami2")
                    .PutAsync(totalDescargas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar la descarga: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método asociado al botón "Descargar" y para iniciar el proceso de parcheo
        private async void descargar_Click(object sender, EventArgs e)
        {
            // Mostrar el formulario de selección y detección de versión
            VersionForm versionForm = new VersionForm();
            if (versionForm.ShowDialog() == DialogResult.OK)
            {
                // Obtener la lista de archivos correctos del formulario
                Dictionary<string, (string, string, string, string)> archivos = ArchivosManagerYK2.ArchivosPorPlataforma[versionForm.Plataforma];
                string selectedPath = versionForm.SelectedPath; // Obtener la ruta seleccionada por el usuario
                List<string> archivosCorrectos = versionForm.ObtenerArchivosCorrectos(); // Obtener la lista de archivos verificados correctamente

                // Verificar si el checkbox está activado
                bool parcheoAdicional = versionForm.ParcheoAdicional;

                // Archivos adicionales para parcheo
                Dictionary<string, (string, string, string, string)> archivosAdicionales = new Dictionary<string, (string, string, string, string)>();
                if (versionForm.Plataforma.ToLower() == "steam" || versionForm.Plataforma.ToLower() == "gog")
                {
                    archivosAdicionales = ArchivosManagerYK2.ArchivosPorPlataforma["adicionales steam y gog"];
                }
                else if (versionForm.Plataforma.ToLower() == "microsoft store")
                {
                    archivosAdicionales = ArchivosManagerYK2.ArchivosPorPlataforma["adicionales microsoft store"];
                }

                int totalArchivos = archivosCorrectos.Count + (parcheoAdicional ? archivosAdicionales.Count : 0);
                int archivosParcheadosCorrectamente = 0;
                int archivosConErrores = 0;

                progressBar1.Minimum = 0;
                progressBar1.Maximum = totalArchivos;
                progressBar1.Value = 0;
                progressBar1.Visible = true; // Hacer visible la progressBar1

                foreach (var archivo in archivos)
                {
                    string archivoOriginalRelativo = archivo.Value.Item2; // Ruta relativa del archivo original a parchear
                    string archivoOriginal = Path.Combine(selectedPath, archivoOriginalRelativo); // Ruta completa del archivo original

                    // Verificar si el archivo está en la lista de archivos correctos
                    if (archivosCorrectos.Contains(archivoOriginal))
                    {
                        // Extraer el parche xdelta correspondiente
                        string archivoParche = archivo.Value.Item3; // Ruta del parche xdelta en los recursos

                        // Extraer el parche
                        string outputParchePath = Path.Combine(Path.GetDirectoryName(archivoOriginal), archivo.Key);
                        await ExtractResourceAsync(archivoParche, outputParchePath);

                        // Aplicar el parche al archivo
                        bool parcheAplicado = await AplicarParcheAsync(archivoOriginal, outputParchePath);
                        if (parcheAplicado)
                        {
                            archivosParcheadosCorrectamente++;
                        }
                        else
                        {
                            archivosConErrores++;
                        }

                        // Actualizar la progressBar1
                        progressBar1.Value++;
                    }
                }

                // Parcheo adicional si el checkbox está activado
                if (parcheoAdicional)
                {
                    foreach (var archivo in archivosAdicionales)
                    {
                        string archivoOriginalRelativo = archivo.Value.Item2; // Ruta relativa del archivo original a parchear
                        string archivoOriginal = Path.Combine(selectedPath, archivoOriginalRelativo); // Ruta completa del archivo original

                        // Verificar si el archivo está en la lista de archivos correctos
                        if (archivosCorrectos.Contains(archivoOriginal))
                        {
                            // Extraer el parche xdelta correspondiente
                            string archivoParche = archivo.Value.Item3; // Ruta del parche xdelta en los recursos

                            // Extraer el parche
                            string outputParchePath = Path.Combine(Path.GetDirectoryName(archivoOriginal), archivo.Key);
                            await ExtractResourceAsync(archivoParche, outputParchePath);

                            // Aplicar el parche al archivo
                            bool parcheAplicado = await AplicarParcheAsync(archivoOriginal, outputParchePath);
                            if (parcheAplicado)
                            {
                                archivosParcheadosCorrectamente++;
                            }
                            else
                            {
                                archivosConErrores++;
                            }

                            // Actualizar la progressBar1
                            progressBar1.Value++;
                        }
                    }
                }

                // Ocultar la progressBar1 al finalizar
                progressBar1.Visible = false;

                // Registrar la descarga en Firebase
                await RegistrarDescargaAsync();

                // Mostrar mensaje al final del proceso de parcheo
                if (archivosConErrores == 0)
                {
                    MessageBox.Show("Parche aplicado correctamente a todos los archivos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                else
                {
                    MessageBox.Show($"Parche aplicado con errores. Archivos parcheados correctamente: {archivosParcheadosCorrectamente}. Archivos con errores: {archivosConErrores}.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


        }
    }
}
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dragones_de_Dojima
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();

            // Centrar el PictureBox en el formulario
            CenterPictureBox();

            // Configurar el temporizador para cerrar el splash después de 3 segundos
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 3000; // 3 segundos
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                this.Close();  // Cierra el splash screen después de 3 segundos
            };
            timer.Start();

            // Iniciar la carga de Form1 en segundo plano, pero no mostrarla hasta que el splash termine
            LoadMainFormAsync();
        }

        private void CenterPictureBox()
        {
            if (panel1 != null)
            {
                // Calcular las coordenadas para centrar el PictureBox
                int x = (this.ClientSize.Width - panel1.Width) / 2;
                int y = (this.ClientSize.Height - panel1.Height) / 2;

                // Ajustar la posición del PictureBox
                panel1.Location = new Point(x, y);

                // Asegurarse de que el PictureBox se mantenga centrado al redimensionar el formulario
                this.Resize += (sender, e) =>
                {
                    int resizeX = (this.ClientSize.Width - panel1.Width) / 2;
                    int resizeY = (this.ClientSize.Height - panel1.Height) / 2;
                    panel1.Location = new Point(resizeX, resizeY);
                };
            }
        }

        // Método para cargar Form1 en segundo plano sin mostrarlo hasta el momento adecuado
        private async void LoadMainFormAsync()
        {
            // Esperar a que la ventana principal esté completamente cargada
            await Task.Delay(100);  // Retrasar ligeramente para asegurar la inicialización
            this.Invoke(new Action(() =>
            {
                // Tu código aquí
            }));
        }
    }
}

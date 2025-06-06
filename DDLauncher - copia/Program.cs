namespace Dragones_de_Dojima
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Mostrar la pantalla de bienvenida
            using (var splashScreen = new SplashScreen())
            {
                splashScreen.ShowDialog();
            }

            // Iniciar la aplicación principal
            Application.Run(new Form1());
        }
    }
}

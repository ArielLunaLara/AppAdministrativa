using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace AppAdministrativa
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Permite iniciar sesión con Enter desde el campo contraseña
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Iniciar();
        }

        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
            => Iniciar();

        private void Iniciar()
        {
            TxtError.Visibility = Visibility.Collapsed;

            string usuario = TxtUsuario.Text.Trim();
            string contrasena = TxtPassword.Password;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                MostrarError("Completa usuario y contraseña.");
                return;
            }

            // Si la BD no tiene usuarios aún → modo super de emergencia
            if (!DatabaseService.Instance.IsAvailable ||
                DatabaseService.Instance.ContarAdministradores() == 0)
            {
                SesionActual.Usuario = "admin";
                SesionActual.Role = "super";
                NavegarAlMenu();
                return;
            }

            string? rol = DatabaseService.Instance.ValidarLogin(usuario, contrasena);

            if (rol == null)
            {
                MostrarError("Usuario o contraseña incorrectos.");
                return;
            }

            SesionActual.Usuario = usuario;
            SesionActual.Role = rol;
            NavegarAlMenu();
        }

        private void NavegarAlMenu()
        {
            Frame contenedor = new Frame();
            contenedor.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            contenedor.Navigate(new MenuPrincipal());
            this.Content = contenedor;
        }

        private void MostrarError(string mensaje)
        {
            TxtError.Text = mensaje;
            TxtError.Visibility = Visibility.Visible;
        }
    }
}
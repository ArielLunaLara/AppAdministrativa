using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
    public partial class Usuarios : Page
    {
        ObservableCollection<UsuarioItem> datosUsuarios = new();

        public Usuarios()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosUsuarios = new ObservableCollection<UsuarioItem>(
                DatabaseService.Instance.GetUsuarios());
            TablaUsuarios.ItemsSource = datosUsuarios;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarUsuarioWindow ventana = new AgregarUsuarioWindow();
            if (ventana.ShowDialog() == true)
            {
                DatabaseService.Instance.AgregarUsuario(ventana.NuevoUsuario);
                datosUsuarios.Add(ventana.NuevoUsuario);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaUsuarios.SelectedItem is UsuarioItem seleccionado)
            {
                AgregarUsuarioWindow ventana = new AgregarUsuarioWindow(seleccionado);
                if (ventana.ShowDialog() == true)
                {
                    DatabaseService.Instance.EditarUsuario(seleccionado);
                    TablaUsuarios.Items.Refresh();
                }
            }
            else
                MessageBox.Show("Selecciona un usuario para editar.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaUsuarios.SelectedItem is UsuarioItem seleccionado)
            {
                // No permitir eliminar el propio usuario activo
                if (seleccionado.NombreUsuario == SesionActual.Usuario)
                {
                    MessageBox.Show("No puedes eliminar tu propia cuenta activa.", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"¿Eliminar al usuario '{seleccionado.NombreUsuario}'?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DatabaseService.Instance.EliminarUsuario(seleccionado.ID);
                    datosUsuarios.Remove(seleccionado);
                }
            }
            else
                MessageBox.Show("Selecciona un usuario para eliminar.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class UsuarioItem
    {
        public string ID { get; set; } = "";   // Solo BD, no se muestra en tabla
        public string NombreUsuario { get; set; } = "";
        public string Password { get; set; } = "";   // Texto real para operaciones BD
        public string Role { get; set; } = "normal";

        // Lo que se muestra en la columna Contraseña de la tabla
        public string PasswordMask => string.IsNullOrEmpty(Password) ? "" : "••••••";
    }
}
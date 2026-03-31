using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
    public partial class AgregarUsuarioWindow : Window
    {
        public UsuarioItem NuevoUsuario { get; private set; } = new();
        private UsuarioItem? _usuarioAEditar;

        // Constructor agregar
        public AgregarUsuarioWindow()
        {
            InitializeComponent();
        }

        // Constructor editar — carga datos existentes
        public AgregarUsuarioWindow(UsuarioItem usuario)
        {
            InitializeComponent();
            _usuarioAEditar = usuario;
            TxtTitulo.Text = "Editar Usuario";
            TxtBoton.Text = "Guardar Cambios";
            this.Title = "Editar Usuario";

            txtNombre.Text = usuario.NombreUsuario;
            txtPassword.Password = usuario.Password;

            // Seleccionar el rol actual en el ComboBox
            CmbRol.SelectedIndex = usuario.Role == "super" ? 1 : 0;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string pass = txtPassword.Password;
            string rol = (CmbRol.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "normal";

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Completa usuario y contraseña.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_usuarioAEditar != null)
            {
                // Modo edición: mutar el objeto existente
                _usuarioAEditar.NombreUsuario = nombre;
                _usuarioAEditar.Password = pass;
                _usuarioAEditar.Role = rol;
            }
            else
            {
                // Modo agregar: crear nuevo
                NuevoUsuario = new UsuarioItem
                {
                    NombreUsuario = nombre,
                    Password = pass,
                    Role = rol
                };
            }

            DialogResult = true;
            Close();
        }
    }
}
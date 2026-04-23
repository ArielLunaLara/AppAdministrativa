using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
	public partial class AgregarUsuarioWindow : Window
	{
		public UsuarioItem NuevoUsuario { get; private set; } = new();
		private UsuarioItem? _usuarioAEditar;

		public AgregarUsuarioWindow()
		{
			InitializeComponent();
		}

		public AgregarUsuarioWindow(UsuarioItem usuario)
		{
			InitializeComponent();
			_usuarioAEditar = usuario;
			TxtTitulo.Text = "Editar Usuario";
			TxtBoton.Text = "Guardar Cambios";
			this.Title = "Editar Usuario";

			txtNombre.Text = usuario.NombreUsuario;
			txtPassword.Password = usuario.Password;
			CmbRol.SelectedIndex = usuario.Role == "super" ? 1 : 0;
		}

		private void ChkMostrar_Checked(object sender, RoutedEventArgs e)
		{
			txtPasswordVisible.Text = txtPassword.Password;
			txtPasswordVisible.Visibility = System.Windows.Visibility.Visible;
			txtPassword.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void ChkMostrar_Unchecked(object sender, RoutedEventArgs e)
		{
			txtPassword.Password = txtPasswordVisible.Text;
			txtPassword.Visibility = System.Windows.Visibility.Visible;
			txtPasswordVisible.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			string nombre = txtNombre.Text.Trim();
			// Leer contraseña del campo activo
			string pass = txtPassword.Visibility == System.Windows.Visibility.Visible
						  ? txtPassword.Password
						  : txtPasswordVisible.Text;
			string rol = (CmbRol.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "normal";

			if (string.IsNullOrEmpty(nombre))
			{
				MessageBox.Show("El usuario es obligatorio.", "Aviso",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			if (string.IsNullOrEmpty(pass))
			{
				MessageBox.Show("La contraseña es obligatoria.", "Aviso",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (_usuarioAEditar != null)
			{
				_usuarioAEditar.NombreUsuario = nombre;
				_usuarioAEditar.Password = pass;
				_usuarioAEditar.Role = rol;
			}
			else
			{
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
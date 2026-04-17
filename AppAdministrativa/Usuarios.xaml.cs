using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
	public partial class Usuarios : Page
	{
		ObservableCollection<UsuarioItem> datosUsuarios = new();
		ObservableCollection<UsuarioItem> datosFiltrados = new();

		public Usuarios()
		{
			InitializeComponent();
			CargarDesdeBD();
		}

		private void CargarDesdeBD()
		{
			var listaBD = DatabaseService.Instance.GetUsuarios();
			datosUsuarios = new ObservableCollection<UsuarioItem>(listaBD);

			datosFiltrados.Clear();
			foreach (var u in datosUsuarios) datosFiltrados.Add(u);

			TablaUsuarios.ItemsSource = datosFiltrados;
		}

		private void ActualizarTablaVisual()
		{
			txtBuscar.Text = "Buscar...";
			txtBuscar.Foreground = Brushes.Gray;

			datosFiltrados.Clear();
			foreach (var u in datosUsuarios) datosFiltrados.Add(u);

			TablaUsuarios.Items.Refresh();
		}

		private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar...") return;

			string filtro = txtBuscar.Text.ToLower();
			var resultado = datosUsuarios
				.Where(u => u.NombreUsuario.ToLower().Contains(filtro) ||
							u.Role.ToLower().Contains(filtro))
				.ToList();

			datosFiltrados.Clear();
			foreach (var item in resultado) datosFiltrados.Add(item);
		}

		private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar...")
			{
				txtBuscar.Text = "";
				txtBuscar.Foreground = Brushes.Black;
			}
		}

		private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtBuscar.Text))
			{
				txtBuscar.Text = "Buscar...";
				txtBuscar.Foreground = Brushes.Gray;
			}
		}

		private void BtnAgregar_Click(object sender, RoutedEventArgs e)
		{
			AgregarUsuarioWindow ventana = new AgregarUsuarioWindow();
			if (ventana.ShowDialog() == true)
			{
				DatabaseService.Instance.AgregarUsuario(ventana.NuevoUsuario);
				datosUsuarios.Add(ventana.NuevoUsuario);
				ActualizarTablaVisual();
			}
		}

		// ESTE ES EL MÉTODO QUE FALTABA
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
			else MessageBox.Show("Selecciona un usuario para editar.", "Aviso");
		}

		private void BtnEliminar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaUsuarios.SelectedItem is UsuarioItem seleccionado)
			{
				if (seleccionado.NombreUsuario == SesionActual.Usuario)
				{
					MessageBox.Show("No puedes eliminar tu propia cuenta activa.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (MessageBox.Show($"¿Eliminar al usuario '{seleccionado.NombreUsuario}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					DatabaseService.Instance.EliminarUsuario(seleccionado.ID);
					datosUsuarios.Remove(seleccionado);
					ActualizarTablaVisual();
				}
			}
			else MessageBox.Show("Selecciona un usuario para eliminar.", "Aviso");
		}
	}

	public class UsuarioItem
	{
		public string ID { get; set; } = "";
		public string NombreUsuario { get; set; } = "";
		public string Password { get; set; } = "";
		public string Role { get; set; } = "normal";
		public string PasswordMask => string.IsNullOrEmpty(Password) ? "" : "••••••";
	}
}
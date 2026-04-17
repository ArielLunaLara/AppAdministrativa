using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
	public partial class Multimedia : Page
	{
		ObservableCollection<MultimediaItem> datosMultimedia = new();
		ObservableCollection<MultimediaItem> datosFiltrados = new();

		public Multimedia()
		{
			InitializeComponent();
			CargarDesdeBD();
		}

		private void CargarDesdeBD()
		{
			var listaBD = DatabaseService.Instance.GetMultimedia();
			datosMultimedia = new ObservableCollection<MultimediaItem>(listaBD);

			datosFiltrados.Clear();
			foreach (var item in datosMultimedia) datosFiltrados.Add(item);

			TablaMultimedia.ItemsSource = datosFiltrados;
		}

		private void ActualizarTablaVisual()
		{
			txtBuscar.Text = "Buscar multimedia...";
			txtBuscar.Foreground = Brushes.Gray;

			datosFiltrados.Clear();
			foreach (var item in datosMultimedia) datosFiltrados.Add(item);

			TablaMultimedia.Items.Refresh();
		}

		private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar multimedia...") return;

			string filtro = txtBuscar.Text.ToLower();
			var resultado = datosMultimedia
				.Where(m => m.NombreProyecto.ToLower().Contains(filtro) ||
							m.Ruta.ToLower().Contains(filtro))
				.ToList();

			datosFiltrados.Clear();
			foreach (var item in resultado) datosFiltrados.Add(item);
		}

		private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar multimedia...")
			{
				txtBuscar.Text = "";
				txtBuscar.Foreground = Brushes.Black;
			}
		}

		private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtBuscar.Text))
			{
				txtBuscar.Text = "Buscar multimedia...";
				txtBuscar.Foreground = Brushes.Gray;
			}
		}

		private void BtnAgregar_Click(object sender, RoutedEventArgs e)
		{
			AgregarMultimediaWindow ventana = new AgregarMultimediaWindow();
			if (ventana.ShowDialog() == true)
			{
				DatabaseService.Instance.AgregarMultimedia(ventana.NuevoItem);
				datosMultimedia.Add(ventana.NuevoItem);
				ActualizarTablaVisual();
			}
		}

		private void BtnEditar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaMultimedia.SelectedItem is MultimediaItem seleccionado)
			{
				string rutaOriginal = seleccionado.Ruta;
				AgregarMultimediaWindow ventana = new AgregarMultimediaWindow(seleccionado);
				if (ventana.ShowDialog() == true)
				{
					DatabaseService.Instance.EditarMultimedia(seleccionado, rutaOriginal);
					TablaMultimedia.Items.Refresh();
				}
			}
			else MessageBox.Show("Selecciona un registro para editar.", "Aviso");
		}

		private void BtnEliminar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaMultimedia.SelectedItem is MultimediaItem seleccionado)
			{
				if (MessageBox.Show($"¿Eliminar '{seleccionado.Ruta}'?", "Confirmar",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					DatabaseService.Instance.EliminarMultimedia(seleccionado.NombreProyecto, seleccionado.Ruta);
					datosMultimedia.Remove(seleccionado);
					ActualizarTablaVisual();
				}
			}
			else MessageBox.Show("Selecciona un registro para eliminar.", "Aviso");
		}
	}

	public class MultimediaItem
	{
		public string NombreProyecto { get; set; } = "";
		public string Ruta { get; set; } = "";
		public string Descripcion { get; set; } = "";
	}
}
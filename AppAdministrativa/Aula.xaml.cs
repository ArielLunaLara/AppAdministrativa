using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
	public partial class Aula : Page
	{
		ObservableCollection<FilaAula> datosAulas = new();
		ObservableCollection<FilaAula> datosFiltrados = new();

		public Aula()
		{
			InitializeComponent();
			CargarDesdeBD();
		}

		private void CargarDesdeBD()
		{
			var salones = DatabaseService.Instance.GetSalones();
			datosAulas = new ObservableCollection<FilaAula>(salones);
			datosFiltrados.Clear();
			foreach (var a in datosAulas) datosFiltrados.Add(a);
			TablaAulas.ItemsSource = datosFiltrados;
		}

		private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar Aula...") return;
			string filtro = txtBuscar.Text.ToLower();
			var resultado = datosAulas.Where(v => v.Nombre.ToLower().Contains(filtro) || v.Piso.ToLower().Contains(filtro)).ToList();
			datosFiltrados.Clear();
			foreach (var item in resultado) datosFiltrados.Add(item);
		}

		private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar Aula...")
			{
				txtBuscar.Text = "";
				txtBuscar.Foreground = Brushes.Black;
			}
		}

		private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtBuscar.Text))
			{
				txtBuscar.Text = "Buscar Aula...";
				txtBuscar.Foreground = Brushes.Gray;
			}
		}

		private void BtnAgregar_Click(object sender, RoutedEventArgs e)
		{
			AgregarAulaWindow ventana = new AgregarAulaWindow();
			if (ventana.ShowDialog() == true)
			{
				DatabaseService.Instance.AgregarSalon(ventana.NuevaAula);

				// Agregamos a la lista maestra
				datosAulas.Add(ventana.NuevaAula);

				// Actualizamos lo que el usuario ve
				ActualizarTablaVisual();
			}
		}

		private void BtnEditar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaAulas.SelectedItem is FilaAula aulaSeleccionada)
			{
				string nombreOriginal = aulaSeleccionada.Nombre;

				// 1. Buscamos la posición exacta en la lista maestra
				int indexMaestro = datosAulas.IndexOf(aulaSeleccionada);

				AgregarAulaWindow ventana = new AgregarAulaWindow(aulaSeleccionada);

				if (ventana.ShowDialog() == true)
				{
					// 2. Ejecutamos la edición en BD
					DatabaseService.Instance.EditarSalon(aulaSeleccionada, nombreOriginal);

					// 3. REEMPLAZO FORZADO:
					// Al asignar el objeto de nuevo en la misma posición, la ObservableCollection 
					// lanza un evento de "Replace" y la tabla se actualiza al instante.
					if (indexMaestro != -1)
					{
						datosAulas[indexMaestro] = null; // "Limpiamos" momentáneamente
						datosAulas[indexMaestro] = aulaSeleccionada; // Reasignamos el objeto editado
					}

					// 4. Sincronizamos la vista filtrada
					ActualizarTablaVisual();
				}
			}
			else MessageBox.Show("Selecciona un aula para editar.", "Aviso");
		}

		private void BtnEliminar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaAulas.SelectedItem is FilaAula aulaSeleccionada)
			{
				if (MessageBox.Show($"¿Eliminar '{aulaSeleccionada.Nombre}'?", "Confirmar",
					MessageBoxButton.YesNo) == MessageBoxResult.Yes)
				{
					bool eliminado = DatabaseService.Instance.EliminarSalon(aulaSeleccionada.Nombre);
					if (eliminado)
					{
						datosAulas.Remove(aulaSeleccionada);
						ActualizarTablaVisual();
					}
					else
						MessageBox.Show("No se pudo eliminar el aula.",
							"Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			else MessageBox.Show("Selecciona un aula para eliminar.", "Aviso");
		}

		// Método de actualización optimizado
		private void ActualizarTablaVisual()
		{
			// 1. Desvinculamos la tabla por completo para que no escuche cambios mientras limpiamos
			TablaAulas.ItemsSource = null;

			string filtro = txtBuscar.Text.ToLower();
			datosFiltrados.Clear();

			// 2. Determinamos la fuente de datos
			var fuente = (string.IsNullOrWhiteSpace(filtro) || filtro == "buscar aula...")
						 ? datosAulas.ToList() // Usamos .ToList() para evitar problemas de enumeración
						 : datosAulas.Where(v => v != null &&
							(v.Nombre.ToLower().Contains(filtro) || v.Piso.ToLower().Contains(filtro))).ToList();

			// 3. Llenamos la colección filtrada
			foreach (var a in fuente)
			{
				if (a != null) datosFiltrados.Add(a);
			}

			// 4. Volvemos a vincular la tabla. 
			// Al asignar ItemsSource de nuevo, WPF reconstruye la vista de una sola vez
			TablaAulas.ItemsSource = datosFiltrados;
		}
	}

	public class FilaAula
	{
		public string Nombre { get; set; } = "";
		public string Piso { get; set; } = "";
	}
}
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
    public partial class Proyectos : Page
    {
        ObservableCollection<ProyectoItem> datosProyectos = new();
        ObservableCollection<ProyectoItem> datosFiltrados = new();

        public Proyectos()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosProyectos = new ObservableCollection<ProyectoItem>(
                DatabaseService.Instance.GetProyectos());
            datosFiltrados = new ObservableCollection<ProyectoItem>(datosProyectos);
            TablaProyectos.ItemsSource = datosFiltrados;

        }
        //  BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar Aula...")
                return;

            string filtro = txtBuscar.Text.ToLower();

            var resultado = datosProyectos
                .Where(v => (v.NombreProyecto != null && v.NombreProyecto.ToLower().Contains(filtro)) ||
                            (v.NombreMateria != null && v.NombreMateria.ToLower().Contains(filtro)))
                .ToList();

            datosFiltrados.Clear();

            foreach (var item in resultado)
                datosFiltrados.Add(item);
        }

		// Placeholder comportamiento
		private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar Proyecto...")
			{
				txtBuscar.Text = "";
				txtBuscar.Foreground = Brushes.Black;
			}
		}

		private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtBuscar.Text))
			{
				txtBuscar.Text = "Buscar Proyecto...";
				txtBuscar.Foreground = Brushes.Gray;
			}
		}

		private void BtnAgregar_Click(object sender, RoutedEventArgs e)
		{
			AgregarProyectoWindow ventana = new AgregarProyectoWindow();
			if (ventana.ShowDialog() == true)
			{
				DatabaseService.Instance.AgregarProyecto(ventana.NuevoProyecto);

				// Agregamos a la lista maestra y a la que ve la UI
				datosProyectos.Add(ventana.NuevoProyecto);
				datosFiltrados.Add(ventana.NuevoProyecto);
			}
		}

		private void BtnEditar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaProyectos.SelectedItem is ProyectoItem seleccionado)
			{
				string nombreOriginal = seleccionado.NombreProyecto;
				AgregarProyectoWindow ventana = new AgregarProyectoWindow(seleccionado);

				if (ventana.ShowDialog() == true)
				{
					DatabaseService.Instance.EditarProyecto(seleccionado, nombreOriginal);

					// Forzamos a que la lista filtrada se reconstruya con los nuevos datos
					// pasando el texto actual del buscador
					txtBuscar_TextChanged(null, null);
				}
			}
			else MessageBox.Show("Selecciona un proyecto para editar.", "Aviso");
		}

		private void BtnEliminar_Click(object sender, RoutedEventArgs e)
		{
			if (TablaProyectos.SelectedItem is ProyectoItem seleccionado)
			{
				if (MessageBox.Show($"¿Eliminar '{seleccionado.NombreProyecto}'?", "Confirmar",
					MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
				{
					DatabaseService.Instance.EliminarProyecto(seleccionado.NombreProyecto);

					// Eliminamos de ambas listas
					datosProyectos.Remove(seleccionado);
					datosFiltrados.Remove(seleccionado);
				}
			}
			else MessageBox.Show("Selecciona un proyecto para eliminar.", "Aviso");
		}
	}

    public class ProyectoItem
    {
        public string NombreProyecto { get; set; } = "";   // PK
        public string NombreMateria { get; set; } = "";   // visible, resuelve id_course internamente
    }
}
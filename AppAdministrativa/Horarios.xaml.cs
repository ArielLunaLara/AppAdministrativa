using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
    public partial class Horarios : Page
    {
        ObservableCollection<FilaHorario> datosHorarios = new();
        ObservableCollection<FilaHorario> datosFiltrados = new();

        public Horarios()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosHorarios = new ObservableCollection<FilaHorario>(
                DatabaseService.Instance.GetHorarios());
            datosFiltrados = new ObservableCollection<FilaHorario>(datosHorarios);
            TablaHorarios.ItemsSource = datosFiltrados;
        }
        //  BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar Aula...")
                return;

            string filtro = txtBuscar.Text.ToLower();

            var resultado = datosHorarios
                .Where(v => (v.IDClase != null && v.IDClase.ToLower().Contains(filtro)) ||
                            (v.Tipo != null && v.Tipo.ToLower().Contains(filtro)) ||
                            (v.HoraInicio != null && v.HoraInicio.ToLower().Contains(filtro)) ||
                            (v.HoraFin != null && v.HoraFin.ToLower().Contains(filtro)) ||
                            (v.IDProfesor != null && v.IDProfesor.ToLower().Contains(filtro)) ||
                            (v.IDAula != null && v.IDAula.ToLower().Contains(filtro)))
                .ToList();

            datosFiltrados.Clear();

            foreach (var item in resultado)
                datosFiltrados.Add(item);
        }



		// Placeholder comportamiento
		private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
		{
			if (txtBuscar.Text == "Buscar Horario...")
			{
				txtBuscar.Text = "";
				txtBuscar.Foreground = Brushes.Black;
			}
		}

		private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtBuscar.Text))
			{
				txtBuscar.Text = "Buscar Horario...";
				txtBuscar.Foreground = Brushes.Gray;
			}
		}

		private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarHorarioWindow ventana = new AgregarHorarioWindow();
            if (ventana.ShowDialog() == true)
            {
                datosHorarios.Add(ventana.NuevoHorario);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaHorarios.SelectedItem is FilaHorario seleccionado)
            {
                AgregarHorarioWindow ventana = new AgregarHorarioWindow(seleccionado);
                if (ventana.ShowDialog() == true)
                    TablaHorarios.Items.Refresh();
            }
            else MessageBox.Show("Selecciona un horario para editar.");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaHorarios.SelectedItem is FilaHorario seleccionado)
            {
                if (MessageBox.Show("¿Eliminar este horario?", "Confirmar",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Elimina directamente por id_class (TEXT PRIMARY KEY)
                    DatabaseService.Instance.EliminarHorario(seleccionado.IDClase);
                    datosHorarios.Remove(seleccionado);
                }
            }
            else MessageBox.Show("Selecciona un horario para eliminar.");
        }

        
    }

    public class FilaHorario
    {
        public string IDClase { get; set; } = "";   // "20501", "20501L", etc.
        public string Tipo { get; set; } = "";   // "LM-JV", "LAB", etc.
        public string HoraInicio { get; set; } = "";   // "08:00"
        public string HoraFin { get; set; } = "";   // "09:00"
        public string IDProfesor { get; set; } = "";
        public string IDAula { get; set; } = "";
    }
}
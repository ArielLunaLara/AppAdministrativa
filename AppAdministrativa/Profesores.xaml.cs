using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
    public partial class Profesores : Page
    {
        ObservableCollection<Profesor> datosProfesores = new();
        ObservableCollection<Profesor> datosFiltrados = new();

        public Profesores()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosProfesores = new ObservableCollection<Profesor>(
                DatabaseService.Instance.GetProfesores());
            datosFiltrados = new ObservableCollection<Profesor>(datosProfesores);
            TablaProfesores.ItemsSource = datosFiltrados;
        }

        //  BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar Aula...")
                return;

            string filtro = txtBuscar.Text.ToLower();

            var resultado = datosProfesores
                .Where(v => (v.Nombre != null && v.Nombre.ToLower().Contains(filtro)) ||
                            (v.Estudios != null && v.Estudios.ToLower().Contains(filtro)) ||
                            (v.Investigaciones != null && v.Investigaciones.ToLower().Contains(filtro)) ||
                            (v.Semblanza != null && v.Semblanza.ToLower().Contains(filtro)))
                .ToList();

            datosFiltrados.Clear();

            foreach (var item in resultado)
                datosFiltrados.Add(item);
        }

        // Placeholder comportamiento
        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar Profesor...")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = Brushes.Black;
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar Profesor...";
                txtBuscar.Foreground = Brushes.Gray;
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarProfesorWindow ventana = new AgregarProfesorWindow(datosProfesores);
            if (ventana.ShowDialog() == true)
            {
                DatabaseService.Instance.AgregarProfesor(ventana.NuevoProfesor);
                datosProfesores.Add(ventana.NuevoProfesor);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaProfesores.SelectedItem is Profesor seleccionado)
            {
                AgregarProfesorWindow ventana = new AgregarProfesorWindow(seleccionado, datosProfesores);
                if (ventana.ShowDialog() == true)
                {
                    DatabaseService.Instance.EditarProfesor(seleccionado);
                    TablaProfesores.Items.Refresh();
                }
            }
            else MessageBox.Show("Selecciona un profesor para editar.", "Aviso");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaProfesores.SelectedItem is Profesor seleccionado)
            {
                if (MessageBox.Show($"¿Eliminar a {seleccionado.Nombre}?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DatabaseService.Instance.EliminarProfesor(seleccionado.Clave);
                    datosProfesores.Remove(seleccionado);
                }
            }
        }

        
    }

    public class Profesor
    {
        public string Clave { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Estudios { get; set; } = "";
        public string Investigaciones { get; set; } = "";
        public string Semblanza { get; set; } = "";
        public string Fotografia { get; set; } = "";
        public string Audio { get; set; } = "";
    }
}
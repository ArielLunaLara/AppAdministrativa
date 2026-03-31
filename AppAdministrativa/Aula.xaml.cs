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
            datosAulas = new ObservableCollection<FilaAula>(
                DatabaseService.Instance.GetSalones());
            datosFiltrados = new ObservableCollection<FilaAula>(datosAulas);
            TablaAulas.ItemsSource = datosFiltrados;
           
        }
        //  BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar Aula...")
                return;

            string filtro = txtBuscar.Text.ToLower();

            var resultado = datosAulas
                .Where(v => (v.Nombre != null && v.Nombre.ToLower().Contains(filtro)) ||
                            (v.Piso != null && v.Piso.ToLower().Contains(filtro)))
                .ToList();

            datosFiltrados.Clear();

            foreach (var item in resultado)
                datosFiltrados.Add(item);
        }

        // Placeholder comportamiento
        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar video...")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = Brushes.Black;
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar video...";
                txtBuscar.Foreground = Brushes.Gray;
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarAulaWindow ventana = new AgregarAulaWindow();
            if (ventana.ShowDialog() == true)
            {
                DatabaseService.Instance.AgregarSalon(ventana.NuevaAula);
                datosAulas.Add(ventana.NuevaAula);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaAulas.SelectedItem is FilaAula aulaSeleccionada)
            {
                string nombreOriginal = aulaSeleccionada.Nombre;
                AgregarAulaWindow ventana = new AgregarAulaWindow(aulaSeleccionada);
                if (ventana.ShowDialog() == true)
                {
                    // Pasamos el nombre original por si cambió (es la PK)
                    DatabaseService.Instance.EditarSalon(aulaSeleccionada, nombreOriginal);
                    TablaAulas.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un aula para editar.",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaAulas.SelectedItem is FilaAula aulaSeleccionada)
            {
                var respuesta = MessageBox.Show(
                    $"¿Eliminar el aula '{aulaSeleccionada.Nombre}'?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (respuesta == MessageBoxResult.Yes)
                {
                    DatabaseService.Instance.EliminarSalon(aulaSeleccionada.Nombre);
                    datosAulas.Remove(aulaSeleccionada);
                }
            }
            else
            {
                MessageBox.Show("Selecciona un aula para eliminar.",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void Filtro_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBlock item)
                item.Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter
                    .ConvertFromString("#678EC2"));
        }

        private void Filtro_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBlock item)
                item.Background = System.Windows.Media.Brushes.Transparent;
        }
    }

    public class FilaAula
    {
        public string Nombre { get; set; } = "";   // "I-01", "I-04" — es la PK
        public string Piso { get; set; } = "";
    }
}
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
    public partial class Videos360 : Page
    {
        ObservableCollection<Video360Item> datosVideos = new();
        ObservableCollection<Video360Item> datosFiltrados = new();

        public Videos360()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosVideos = new ObservableCollection<Video360Item>(
                DatabaseService.Instance.GetVideos360());

            datosFiltrados = new ObservableCollection<Video360Item>(datosVideos);

            TablaVideos.ItemsSource = datosFiltrados;
        }

        //  BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar video...")
                return;

            string filtro = txtBuscar.Text.ToLower();

            var resultado = datosVideos
                .Where(v => (v.RutaVideo != null && v.RutaVideo.ToLower().Contains(filtro)) ||
                            (v.IDAula != null && v.IDAula.ToLower().Contains(filtro)))
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

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaVideos.SelectedItem is Video360Item seleccionado)
            {
                AgregarVideo360Window ventana = new AgregarVideo360Window(seleccionado);

                if (ventana.ShowDialog() == true)
                {
                    DatabaseService.Instance.ActualizarRutaVideo360(
                        seleccionado.IDAula,
                        seleccionado.RutaVideo);

                    TablaVideos.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un salón para editar su ruta de video.",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaVideos.SelectedItem is Video360Item seleccionado)
            {
                if (MessageBox.Show($"¿Limpiar la ruta del salón {seleccionado.IDAula}?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    seleccionado.RutaVideo = "";
                    DatabaseService.Instance.ActualizarRutaVideo360(
                        seleccionado.IDAula, null);

                    TablaVideos.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un salón para limpiar su ruta.",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class Video360Item
    {
        public string IDAula { get; set; } = "";
        public string RutaVideo { get; set; } = "";
    }
}
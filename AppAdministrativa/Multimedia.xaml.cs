using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
    public partial class Multimedia : Page
    {
        ObservableCollection<MultimediaItem> datosMultimedia = new();

        public Multimedia()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosMultimedia = new ObservableCollection<MultimediaItem>(
                DatabaseService.Instance.GetMultimedia());
            TablaMultimedia.ItemsSource = datosMultimedia;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            AgregarMultimediaWindow ventana = new AgregarMultimediaWindow();
            if (ventana.ShowDialog() == true)
            {
                DatabaseService.Instance.AgregarMultimedia(ventana.NuevoItem);
                datosMultimedia.Add(ventana.NuevoItem);
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
                }
            }
            else MessageBox.Show("Selecciona un registro para eliminar.", "Aviso");
        }
    }

    public class MultimediaItem
    {
        public string NombreProyecto { get; set; } = "";   // FK a Project, visible como referencia
        public string Ruta { get; set; } = "";   // parte de la PK
        public string Descripcion { get; set; } = "";
    }
}
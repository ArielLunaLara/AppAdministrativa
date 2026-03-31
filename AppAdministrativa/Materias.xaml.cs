using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
    public partial class Materias : Page
    {
        ObservableCollection<Materia> datosMaterias = new();
        ObservableCollection<Materia> datosFiltrados = new();

        public Materias()
        {
            InitializeComponent();
            CargarDesdeBD();
        }

        private void CargarDesdeBD()
        {
            datosMaterias = new ObservableCollection<Materia>(
                DatabaseService.Instance.GetMaterias());
            datosFiltrados = new ObservableCollection<Materia>(datosMaterias);
            TablaMaterias.ItemsSource = datosFiltrados;
        }
        //  BUSCADOR
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBuscar.Text == "Buscar Aula...")
                return;

            string filtro = txtBuscar.Text.ToLower();

            var resultado = datosMaterias
                .Where(v => (v.Nombre != null && v.Nombre.ToLower().Contains(filtro)) ||
                            (v.Companias != null && v.Companias.ToLower().Contains(filtro)))
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
            AgregarMateriaWindow ventana = new AgregarMateriaWindow();
            if (ventana.ShowDialog() == true)
            {
                DatabaseService.Instance.AgregarMateria(ventana.NuevaMateria);
                datosMaterias.Add(ventana.NuevaMateria);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaMaterias.SelectedItem is Materia seleccionada)
            {
                AgregarMateriaWindow ventana = new AgregarMateriaWindow(seleccionada);
                if (ventana.ShowDialog() == true)
                {
                    DatabaseService.Instance.EditarMateria(seleccionada);
                    TablaMaterias.Items.Refresh();
                }
            }
            else MessageBox.Show("Selecciona una materia para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (TablaMaterias.SelectedItem is Materia seleccionada)
            {
                if (MessageBox.Show("¿Eliminar esta materia?", "Confirmar",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DatabaseService.Instance.EliminarMateria(seleccionada.ID);
                    datosMaterias.Remove(seleccionada);
                }
            }
            else MessageBox.Show("Selecciona una materia para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        
        private void Filtro_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) { if (sender is TextBlock i) i.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#678EC2")); }
        private void Filtro_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) { if (sender is TextBlock i) i.Background = System.Windows.Media.Brushes.Transparent; }
    }

    public class Materia
    {
        public string ID { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Companias { get; set; } = "";
        public string RutaTemario { get; set; } = "";
    }
}
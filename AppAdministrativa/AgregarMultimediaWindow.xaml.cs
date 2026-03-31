using Microsoft.Win32;
using System.Windows;

namespace AppAdministrativa
{
    public partial class AgregarMultimediaWindow : Window
    {
        public MultimediaItem NuevoItem { get; private set; }
        private MultimediaItem? itemAEditar;

        // Constructor para AGREGAR
        public AgregarMultimediaWindow()
        {
            InitializeComponent();
            CargarProyectos();
        }

        // Constructor para EDITAR
        public AgregarMultimediaWindow(MultimediaItem item)
        {
            InitializeComponent();
            itemAEditar = item;
            TxtTitulo.Text = "Editar Registro de Multimedia";
            this.Title = "Editar Multimedia";
            TxtBoton.Text = "Guardar Cambios";

            CargarProyectos();
            cmbProyecto.Text = item.NombreProyecto;
            cmbProyecto.IsEnabled = false;  // el proyecto no cambia al editar (parte de PK)
            txtRuta.Text = item.Ruta;
            txtDescripcion.Text = item.Descripcion;
        }

        private void CargarProyectos()
        {
            var proyectos = DatabaseService.Instance.GetNombresProyectos();
            cmbProyecto.ItemsSource = proyectos;
        }

        private void txtRuta_GotFocus(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Archivos Multimedia|*.mp4;*.mp3;*.png;*.jpg;*.jpeg;*.avi;*.wav|Todos los archivos|*.*"
            };
            if (dlg.ShowDialog() == true)
                txtRuta.Text = dlg.FileName;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbProyecto.Text) ||
                string.IsNullOrWhiteSpace(txtRuta.Text) ||
                txtRuta.Text == "Seleccionar archivo...")
            {
                MessageBox.Show("Selecciona un proyecto y un archivo.", "Campos requeridos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (itemAEditar != null)
            {
                // El proyecto no cambia (es parte de la PK y está deshabilitado)
                itemAEditar.Ruta = txtRuta.Text;
                itemAEditar.Descripcion = txtDescripcion.Text.Trim();
            }
            else
            {
                NuevoItem = new MultimediaItem
                {
                    NombreProyecto = cmbProyecto.Text.Trim(),
                    Ruta = txtRuta.Text,
                    Descripcion = txtDescripcion.Text.Trim()
                };
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
using System.Windows;

namespace AppAdministrativa
{
    public partial class AgregarProyectoWindow : Window
    {
        public ProyectoItem NuevoProyecto { get; private set; }
        private ProyectoItem? proyectoAEditar;

        // Constructor para AGREGAR
        public AgregarProyectoWindow()
        {
            InitializeComponent();
            CargarMaterias();
        }

        // Constructor para EDITAR
        public AgregarProyectoWindow(ProyectoItem proyecto)
        {
            InitializeComponent();
            proyectoAEditar = proyecto;
            TxtTitulo.Text = "Editar Registro de Proyectos";
            this.Title = "Editar Proyecto";
            TxtBoton.Text = "Guardar Cambios";

            CargarMaterias();
            cmbMateria.Text = proyecto.NombreMateria;
            txtNombre.Text = proyecto.NombreProyecto;
        }

        private void CargarMaterias()
        {
            var materias = DatabaseService.Instance.GetNombresMaterias();
            cmbMateria.ItemsSource = materias;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cmbMateria.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Completa todos los campos.", "Campos requeridos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (proyectoAEditar != null)
            {
                proyectoAEditar.NombreMateria = cmbMateria.Text.Trim();
                proyectoAEditar.NombreProyecto = txtNombre.Text.Trim();
            }
            else
            {
                NuevoProyecto = new ProyectoItem
                {
                    NombreMateria = cmbMateria.Text.Trim(),
                    NombreProyecto = txtNombre.Text.Trim()
                };
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
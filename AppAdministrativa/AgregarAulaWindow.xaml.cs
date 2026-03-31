using System.Windows;

namespace AppAdministrativa
{
    public partial class AgregarAulaWindow : Window
    {
        public FilaAula NuevaAula { get; private set; }
        private FilaAula? aulaAEditar;

        // Constructor para AGREGAR
        public AgregarAulaWindow()
        {
            InitializeComponent();
        }

        // Constructor para EDITAR
        public AgregarAulaWindow(FilaAula aula)
        {
            InitializeComponent();
            aulaAEditar = aula;
            TxtTitulo.Text = "Editar Registro de Aula";
            this.Title = "Editar Aula";

            txtNombre.Text = aula.Nombre;
            txtNombre.IsEnabled = false; // el nombre es la PK, no se debería cambiar directamente
            cmbPiso.Text = aula.Piso;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El ID del aula es obligatorio.", "Campo requerido",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (aulaAEditar != null)
            {
                // MODO EDICIÓN — solo piso puede cambiar (nombre deshabilitado)
                aulaAEditar.Nombre = txtNombre.Text.Trim();
                aulaAEditar.Piso = cmbPiso.Text;
            }
            else
            {
                // MODO AGREGAR
                NuevaAula = new FilaAula
                {
                    Nombre = txtNombre.Text.Trim(),
                    Piso = cmbPiso.Text
                };
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
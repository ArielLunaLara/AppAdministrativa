using System.Windows;

namespace AppAdministrativa
{
    public partial class AgregarMateriaWindow : Window
    {
        public Materia NuevaMateria { get; private set; }
        private Materia materiaAEditar;

        // CONSTRUCTOR 1: MODO AGREGAR
        public AgregarMateriaWindow()
        {
            InitializeComponent();
        }

        // CONSTRUCTOR 2: MODO EDITAR
        public AgregarMateriaWindow(Materia materia)
        {
            InitializeComponent();
            materiaAEditar = materia;

            // Cambiamos los textos para que parezca ventana de edición
            TxtTitulo.Text = "Editar Registro de Materia";
            this.Title = "Editar Materia";

            // Llenamos las cajas de texto
            txtID.Text = materia.ID;
            txtNombre.Text = materia.Nombre;
            txtCompanias.Text = materia.Companias;
            txtRutaTemario.Text = materia.RutaTemario;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (materiaAEditar != null)
            {
                // ACTUALIZAR EXISTENTE
                materiaAEditar.ID = txtID.Text;
                materiaAEditar.Nombre = txtNombre.Text;
                materiaAEditar.Companias = txtCompanias.Text;
                materiaAEditar.RutaTemario = txtRutaTemario.Text;
            }
            else
            {
                // CREAR NUEVO
                NuevaMateria = new Materia
                {
                    ID = txtID.Text,
                    Nombre = txtNombre.Text,
                    Companias = txtCompanias.Text,
                    RutaTemario = txtRutaTemario.Text
                };
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
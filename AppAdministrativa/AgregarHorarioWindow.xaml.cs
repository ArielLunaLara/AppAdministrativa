using System.Windows;

namespace AppAdministrativa
{
    public partial class AgregarHorarioWindow : Window
    {
        public FilaHorario NuevoHorario { get; private set; }
        private FilaHorario? horarioAEditar;

        // Constructor para AGREGAR
        public AgregarHorarioWindow()
        {
            InitializeComponent();
        }

        // Constructor para EDITAR
        public AgregarHorarioWindow(FilaHorario horario)
        {
            InitializeComponent();
            horarioAEditar = horario;
            TxtTitulo.Text = "Editar Registro de Clase";

            txtIDClase.Text = horario.IDClase;
            txtIDClase.IsEnabled = false; // el ID no se puede cambiar al editar

            cmbTipo.Text = horario.Tipo;
            cmbHoraInicio.Text = horario.HoraInicio;
            cmbHoraFin.Text = horario.HoraFin;
            txtIDProfesor.Text = horario.IDProfesor;
            txtIDAula.Text = horario.IDAula;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(txtIDClase.Text) ||
                string.IsNullOrWhiteSpace(txtIDProfesor.Text) ||
                string.IsNullOrWhiteSpace(txtIDAula.Text))
            {
                MessageBox.Show("Por favor completa todos los campos.", "Campos requeridos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string idClase = txtIDClase.Text.Trim().Replace(" ", "");
            string tipo = cmbTipo.Text;
            string horaInicio = cmbHoraInicio.Text;
            string horaFin = cmbHoraFin.Text;
            string idProfesor = txtIDProfesor.Text.Trim();
            string idAula = txtIDAula.Text.Trim();

            if (horarioAEditar != null)
            {
                // EDITAR: actualizar propiedades del objeto existente
                horarioAEditar.Tipo = tipo;
                horarioAEditar.HoraInicio = horaInicio;
                horarioAEditar.HoraFin = horaFin;
                horarioAEditar.IDProfesor = idProfesor;
                horarioAEditar.IDAula = idAula;

                // Persistir en BD
                DatabaseService.Instance.EditarHorario(horarioAEditar, idAula);
            }
            else
            {
                // AGREGAR: crear nuevo objeto y guardarlo en BD
                NuevoHorario = new FilaHorario
                {
                    IDClase = idClase,
                    Tipo = tipo,
                    HoraInicio = horaInicio,
                    HoraFin = horaFin,
                    IDProfesor = idProfesor,
                    IDAula = idAula
                };

                DatabaseService.Instance.AgregarHorario(NuevoHorario, idAula);
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
using System.Windows;

namespace AppAdministrativa
{
	public partial class AgregarMateriaWindow : Window
	{
		public Materia NuevaMateria { get; private set; }
		private Materia materiaAEditar;
		// Agregamos esta variable para no perder la llave primaria original
		private string idOriginal;

		public AgregarMateriaWindow()
		{
			InitializeComponent();
		}

		public AgregarMateriaWindow(Materia materia)
		{
			InitializeComponent();
			materiaAEditar = materia;
			idOriginal = materia.ID; // Guardamos el ID antes de que se edite

			TxtTitulo.Text = "Editar Registro de Materia";
			this.Title = "Editar Materia";

			txtID.Text = materia.ID;
			txtNombre.Text = materia.Nombre;
			txtCompanias.Text = materia.Companias;
			txtRutaTemario.Text = materia.RutaTemario;
		}

		private void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			if (materiaAEditar != null)
			{
				// NOTA: Si tu DatabaseService usa el objeto materiaAEditar, 
				// asegúrate de que el método EditarMateria reciba el ID Original 
				// o que no se cambie el ID si es la llave primaria.

				materiaAEditar.ID = txtID.Text;
				materiaAEditar.Nombre = txtNombre.Text;
				materiaAEditar.Companias = txtCompanias.Text;
				materiaAEditar.RutaTemario = txtRutaTemario.Text;
			}
			else
			{
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
	public partial class AgregarMateriaWindow : Window
	{
		public Materia NuevaMateria { get; private set; }
		private Materia materiaAEditar;
		private string idOriginal;

		public AgregarMateriaWindow()
		{
			InitializeComponent();
		}

		public AgregarMateriaWindow(Materia materia)
		{
			InitializeComponent();
			materiaAEditar = materia;
			idOriginal = materia.ID;

			TxtTitulo.Text = "Editar Registro de Materia";
			this.Title = "Editar Materia";

			// Al editar, llenamos con datos reales y ponemos color negro
			txtID.Text = materia.ID;
			txtNombre.Text = materia.Nombre;
			txtCompanias.Text = materia.Companias;
			txtRutaTemario.Text = materia.RutaTemario;

			SetTextBlack(txtID, txtNombre, txtCompanias, txtRutaTemario);
		}

		private void SetTextBlack(params TextBox[] boxes)
		{
			foreach (var b in boxes) b.Foreground = Brushes.Black;
		}

		// --- PLACEHOLDERS ---
		private void RemovePlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is not TextBox tb) return;
			if (tb.Name == "txtID" && tb.Text == "#000" ||
				tb.Name == "txtNombre" && tb.Text == "Escribir Nombre de la Materia" ||
				tb.Name == "txtCompanias" && tb.Text == "Escribir Nombre de las Compañías Relacionadas" ||
				tb.Name == "txtRutaTemario" && tb.Text == "Escribir Ruta del Temario")
			{
				tb.Text = "";
				tb.Foreground = Brushes.Black;
			}
		}

		private void AddPlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is not TextBox tb) return;
			if (!string.IsNullOrWhiteSpace(tb.Text)) return;

			tb.Foreground = Brushes.Gray;
			tb.Text = tb.Name switch
			{
				"txtID" => "#000",
				"txtNombre" => "Escribir Nombre de la Materia",
				"txtCompanias" => "Escribir Nombre de las Compañías Relacionadas",
				"txtRutaTemario" => "Escribir Ruta del Temario",
				_ => ""
			};
		}

		private void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			string id = txtID.Text.Trim();
			string nombre = txtNombre.Text.Trim();

			// Validar campos obligatorios
			if (string.IsNullOrWhiteSpace(id) || id == "#000")
			{
				MessageBox.Show("El ID es obligatorio.", "Campo requerido",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (string.IsNullOrWhiteSpace(nombre) || nombre == "Escribir Nombre de la Materia")
			{
				MessageBox.Show("El Nombre es obligatorio.", "Campo requerido",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (materiaAEditar != null)
			{
				materiaAEditar.ID = id;
				materiaAEditar.Nombre = nombre;
				materiaAEditar.Companias = txtCompanias.Text == "Escribir Nombre de las Compañías Relacionadas" ? "" : txtCompanias.Text;
				materiaAEditar.RutaTemario = txtRutaTemario.Text == "Escribir Ruta del Temario" ? "" : txtRutaTemario.Text;
			}
			else
			{
				NuevaMateria = new Materia
				{
					ID = id,
					Nombre = nombre,
					Companias = txtCompanias.Text == "Escribir Nombre de las Compañías Relacionadas" ? "" : txtCompanias.Text,
					RutaTemario = txtRutaTemario.Text == "Escribir Ruta del Temario" ? "" : txtRutaTemario.Text
				};
			}

			this.DialogResult = true;
			this.Close();
		}
	}
}
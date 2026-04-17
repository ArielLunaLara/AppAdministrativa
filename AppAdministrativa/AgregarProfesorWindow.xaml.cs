using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AppAdministrativa
{
	public partial class AgregarProfesorWindow : Window
	{
		public Profesor NuevoProfesor { get; private set; }
		private Profesor profesorAEditar;
		private IEnumerable<Profesor> listaExistente; // Referencia para validar PK duplicada

		// CONSTRUCTOR PARA AGREGAR
		public AgregarProfesorWindow(IEnumerable<Profesor> lista)
		{
			InitializeComponent();
			this.listaExistente = lista;
		}

		// CONSTRUCTOR PARA EDITAR
		public AgregarProfesorWindow(Profesor profesor, IEnumerable<Profesor> lista)
		{
			InitializeComponent();
			this.profesorAEditar = profesor;
			this.listaExistente = lista;

			TxtTituloVentana.Text = "Editar Registro de Profesor";
			TxtTextoBoton.Text = "Guardar Cambios";

			// Llenar campos y quitar color gris de placeholder
			txtClave.Text = profesor.Clave;
			txtNombre.Text = profesor.Nombre;
			txtEstudios.Text = profesor.Estudios;
			txtInvestigaciones.Text = profesor.Investigaciones;
			txtSemblanza.Text = profesor.Semblanza;
			txtFotografia.Text = profesor.Fotografia;
			txtAudio.Text = profesor.Audio;

			SetTextBlack(txtClave, txtNombre, txtEstudios, txtInvestigaciones, txtSemblanza, txtFotografia, txtAudio);
		}

		private void SetTextBlack(params TextBox[] boxes)
		{
			foreach (var b in boxes) b.Foreground = Brushes.Black;
		}

		// --- LÓGICA DE PLACEHOLDERS ---
		private void RemovePlaceholder(object sender, RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (tb.Text.Contains("Escribir") || tb.Text == "#000" || tb.Text.Contains("Seleccionar"))
			{
				tb.Text = "";
				tb.Foreground = Brushes.Black;
			}
		}

		private void AddPlaceholder(object sender, RoutedEventArgs e)
		{
			TextBox tb = sender as TextBox;
			if (string.IsNullOrWhiteSpace(tb.Text))
			{
				tb.Foreground = Brushes.Gray;
				if (tb.Name == "txtClave") tb.Text = "#000";
				else if (tb.Name == "txtNombre") tb.Text = "Escribir el Nombre del Profesor";
				else if (tb.Name == "txtEstudios") tb.Text = "Escribir Universidad de Estudio del Profesor";
				else if (tb.Name == "txtInvestigaciones") tb.Text = "Escribir Investigaciones del Profesor...";
				else if (tb.Name == "txtSemblanza") tb.Text = "Escribir Semblanza del Profesor...";
				else if (tb.Name == "txtFotografia") tb.Text = "Seleccionar Ruta de Fotografía...";
				else if (tb.Name == "txtAudio") tb.Text = "Seleccionar Ruta de Audio...";
			}
		}

		// --- SELECTORES DE ARCHIVOS ---
		private void txtFotografia_GotFocus(object sender, RoutedEventArgs e)
		{
			RemovePlaceholder(sender, e);
			OpenFileDialog ofd = new OpenFileDialog { Filter = "Imágenes|*.jpg;*.png" };
			if (ofd.ShowDialog() == true) txtFotografia.Text = ofd.FileName;
			txtNombre.Focus();
		}

		private void txtAudio_GotFocus(object sender, RoutedEventArgs e)
		{
			RemovePlaceholder(sender, e);
			OpenFileDialog ofd = new OpenFileDialog { Filter = "Audio|*.mp3;*.mp4" };
			if (ofd.ShowDialog() == true) txtAudio.Text = ofd.FileName;
			txtNombre.Focus();
		}

		// --- GUARDAR ---
		private void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			string clave = txtClave.Text.Trim();
			string nombre = txtNombre.Text.Trim();

			// VALIDACIÓN DE CAMPOS OBLIGATORIOS
			if (string.IsNullOrWhiteSpace(clave) || clave == "#000")
			{
				MessageBox.Show("La Clave es obligatoria.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
				return;
			}

			if (string.IsNullOrWhiteSpace(nombre) || nombre == "Escribir el Nombre del Profesor")
			{
				MessageBox.Show("El Nombre es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
				return;
			}

			// Validar Clave Duplicada (PK)
			if (listaExistente.Any(p => p.Clave == clave && p != profesorAEditar))
			{
				MessageBox.Show($"La clave '{clave}' ya existe.", "PK Duplicada", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (profesorAEditar != null)
			{
				AsignarCampos(profesorAEditar);
			}
			else
			{
				NuevoProfesor = new Profesor();
				AsignarCampos(NuevoProfesor);
			}

			this.DialogResult = true;
			this.Close();
		}

		private void AsignarCampos(Profesor p)
		{
			p.Clave = txtClave.Text; p.Nombre = txtNombre.Text; p.Estudios = txtEstudios.Text;
			p.Investigaciones = txtInvestigaciones.Text; p.Semblanza = txtSemblanza.Text;
			p.Fotografia = txtFotografia.Text; p.Audio = txtAudio.Text;
		}
	}


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
	public partial class AgregarHorarioWindow : Window
	{
		public FilaHorario NuevoHorario { get; private set; }
		private FilaHorario? horarioAEditar;
		private string? _idClaseOriginal; // Guardamos la PK original para el DELETE

		private List<(string Id, string Nombre)> _profesores = new();
		private List<(string Id, string Nombre)> _materias = new();

		// Constructor AGREGAR
		public AgregarHorarioWindow()
		{
			InitializeComponent();
			CargarDesplegables();
		}

		// Constructor EDITAR
		public AgregarHorarioWindow(FilaHorario horario)
		{
			InitializeComponent();
			horarioAEditar = horario;
			_idClaseOriginal = horario.IDClase; // Guardamos PK antes de cualquier cambio
			TxtTitulo.Text = "Editar Registro de Clase";

			CargarDesplegables();

			// Preseleccionar materia — buscar por los primeros 4 dígitos del IDClase
			string idCurso = horario.IDClase.Length >= 4 ? horario.IDClase[..4] : horario.IDClase;
			SeleccionarEnCombo(cmbIDClase, idCurso);

			// Preseleccionar tipo, horas
			SeleccionarEnComboByContent(cmbTipo, horario.Tipo);
			SeleccionarEnComboByContent(cmbHoraInicio, horario.HoraInicio);
			SeleccionarEnComboByContent(cmbHoraFin, horario.HoraFin);

			// Preseleccionar profesor y aula
			SeleccionarEnCombo(cmbIDProfesor, horario.IDProfesor);
			SeleccionarEnComboByContent(cmbIDAula, horario.IDAula);
		}

		private void CargarDesplegables()
		{
			// Materias
			_materias = DatabaseService.Instance.GetMateriasParaCombo();
			foreach (var m in _materias)
				cmbIDClase.Items.Add(new ComboBoxItem
				{
					Content = $"{m.Id} — {m.Nombre}",
					Tag = m.Id
				});

			// Profesores
			_profesores = DatabaseService.Instance.GetProfesoresParaCombo();
			foreach (var p in _profesores)
				cmbIDProfesor.Items.Add(new ComboBoxItem
				{
					Content = $"{p.Id} — {p.Nombre}",
					Tag = p.Id
				});

			// Aulas
			var aulas = DatabaseService.Instance.GetNombresAulas();
			foreach (var a in aulas)
				cmbIDAula.Items.Add(new ComboBoxItem { Content = a, Tag = a });
		}

		// Busca en el ComboBox por Tag (ID)
		private void SeleccionarEnCombo(ComboBox cmb, string id)
		{
			foreach (ComboBoxItem item in cmb.Items)
				if (item.Tag?.ToString() == id)
				{
					cmb.SelectedItem = item;
					return;
				}
		}

		// Busca en el ComboBox por Content (texto visible)
		private void SeleccionarEnComboByContent(ComboBox cmb, string texto)
		{
			foreach (ComboBoxItem item in cmb.Items)
				if (item.Content?.ToString() == texto)
				{
					cmb.SelectedItem = item;
					return;
				}
		}

		private void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			string? idMateria = (cmbIDClase.SelectedItem as ComboBoxItem)?.Tag?.ToString();
			string? idProfesor = (cmbIDProfesor.SelectedItem as ComboBoxItem)?.Tag?.ToString();
			string? idAula = (cmbIDAula.SelectedItem as ComboBoxItem)?.Tag?.ToString();
			string tipo = (cmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
			string horaInicio = (cmbHoraInicio.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
			string horaFin = (cmbHoraFin.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

			if (string.IsNullOrEmpty(idMateria) ||
				string.IsNullOrEmpty(idProfesor) ||
				string.IsNullOrEmpty(idAula))
			{
				MessageBox.Show("Por favor selecciona todos los campos.", "Campos requeridos",
					MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				if (horarioAEditar != null)
				{
					// Actualizar el objeto con los nuevos valores
					horarioAEditar.IDClase = idMateria;
					horarioAEditar.Tipo = tipo;
					horarioAEditar.HoraInicio = horaInicio;
					horarioAEditar.HoraFin = horaFin;
					horarioAEditar.IDProfesor = idProfesor;
					horarioAEditar.IDAula = idAula;

					// DELETE + INSERT con la PK original
					DatabaseService.Instance.EditarHorario(horarioAEditar, _idClaseOriginal!, idAula);
				}
				else
				{
					NuevoHorario = new FilaHorario
					{
						IDClase = idMateria,
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
			catch (Exception ex)
			{
				MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
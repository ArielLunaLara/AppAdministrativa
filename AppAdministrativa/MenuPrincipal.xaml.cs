using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppAdministrativa
{
	public partial class MenuPrincipal : Page
	{
		public MenuPrincipal()
		{
			InitializeComponent();

			if (!SesionActual.EsSuper)
			{
				BtnMenuUsuarios.Visibility = Visibility.Collapsed;
				TxtAdministrar.Visibility = Visibility.Collapsed;
			}

			// Carga Profesores por defecto y establece el título inicial
			FrameSecundario.Navigate(new Profesores());
			TxtTituloPagina.Text = "Administración de Profesores";
		}

		private void ResetearColoresMenu()
		{
			BtnMenuProfesores.Background = Brushes.Transparent;
			BtnMenuAulas.Background = Brushes.Transparent;
			BtnMenuHorarios.Background = Brushes.Transparent;
			BtnMenuMaterias.Background = Brushes.Transparent;
			BtnMenuUsuarios.Background = Brushes.Transparent;
			BtnMenuProyectos.Background = Brushes.Transparent;
			BtnMenuMultimedia.Background = Brushes.Transparent;
			BtnMenuCamaras.Background = Brushes.Transparent;
			BtnMenuVideos360.Background = Brushes.Transparent;
			BtnImpExp.Background = Brushes.Transparent; 
		}

		private void Menu_Profesores_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Profesores());
			TxtTituloPagina.Text = "Administración de Profesores";
			ActualizarSeleccion(BtnMenuProfesores);
		}

		private void Menu_Aulas_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Aula());
			TxtTituloPagina.Text = "Gestión de Aulas / Salones";
			ActualizarSeleccion(BtnMenuAulas);
		}

		private void Menu_Materias_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Materias());
			TxtTituloPagina.Text = "Catálogo de Materias";
			ActualizarSeleccion(BtnMenuMaterias);
		}

		private void Menu_Horarios_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Horarios());
			TxtTituloPagina.Text = "Horarios de Clases";
			ActualizarSeleccion(BtnMenuHorarios);
		}

		private void Menu_Usuarios_Click(object sender, MouseButtonEventArgs e)
		{
			if (!SesionActual.EsSuper) return;
			FrameSecundario.Navigate(new Usuarios());
			TxtTituloPagina.Text = "Administración de Usuarios";
			ActualizarSeleccion(BtnMenuUsuarios);
		}

		private void Menu_Proyectos_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Proyectos());
			TxtTituloPagina.Text = "Gestión de Proyectos";
			ActualizarSeleccion(BtnMenuProyectos);
		}

		private void Menu_Multimedia_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Multimedia());
			TxtTituloPagina.Text = "Galería Multimedia";
			ActualizarSeleccion(BtnMenuMultimedia);
		}

		private void Menu_Videos360_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Videos360());
			TxtTituloPagina.Text = "Recorridos Videos 360";
			ActualizarSeleccion(BtnMenuVideos360);
		}

		private void Menu_Camaras_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new Camaras());
			TxtTituloPagina.Text = "Monitoreo de Cámaras";
			ActualizarSeleccion(BtnMenuCamaras);
		}

		private void ImpExp_Click(object sender, MouseButtonEventArgs e)
		{
			FrameSecundario.Navigate(new ImpExp());
			TxtTituloPagina.Text = "Importar / Exportar Datos";
			ActualizarSeleccion(BtnImpExp);
		}

		private void ActualizarSeleccion(Border botonSeleccionado)
		{
			ResetearColoresMenu();
			botonSeleccionado.Background = new SolidColorBrush(
				(Color)ColorConverter.ConvertFromString("#678EC2"));
			BarraLateral.Width = 60;
			TxtAdministrar.Visibility = Visibility.Collapsed;
		}

		private void Menu_MouseEnter(object sender, MouseEventArgs e)
		{
			BarraLateral.Width = 200;
			if (SesionActual.EsSuper)
				TxtAdministrar.Visibility = Visibility.Visible;
		}

		private void Menu_MouseLeave(object sender, MouseEventArgs e)
		{
			BarraLateral.Width = 60;
			TxtAdministrar.Visibility = Visibility.Collapsed;
		}

		private void Menu_Salir_Click(object sender, MouseButtonEventArgs e)
		{
			if (MessageBox.Show("¿Deseas cerrar sesión?", "Salir",
				MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				SesionActual.Usuario = "";
				SesionActual.Role = "normal";
				Application.Current.Shutdown();
			}
		}
	}
}
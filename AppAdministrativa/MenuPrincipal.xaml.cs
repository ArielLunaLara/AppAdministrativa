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

            // Ocultar la sección de Usuarios si el admin no es super
            if (!SesionActual.EsSuper)
            {
                BtnMenuUsuarios.Visibility = Visibility.Collapsed;
                TxtAdministrar.Visibility = Visibility.Collapsed;
            }

            // Carga Profesores por defecto al iniciar
            FrameSecundario.Navigate(new Profesores());
        }

        // --- LÓGICA DE NAVEGACIÓN Y COLORES ---

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
        }

        private void Menu_Profesores_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Profesores());
            ActualizarSeleccion(BtnMenuProfesores);
        }

        private void Menu_Aulas_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Aula());
            ActualizarSeleccion(BtnMenuAulas);
        }

        private void Menu_Materias_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Materias());
            ActualizarSeleccion(BtnMenuMaterias);
        }

        private void Menu_Horarios_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Horarios());
            ActualizarSeleccion(BtnMenuHorarios);
        }

        private void Menu_Usuarios_Click(object sender, MouseButtonEventArgs e)
        {
            // Doble check por si acaso (el botón ya debería estar oculto para no-super)
            if (!SesionActual.EsSuper) return;
            FrameSecundario.Navigate(new Usuarios());
            ActualizarSeleccion(BtnMenuUsuarios);
        }

        private void Menu_Proyectos_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Proyectos());
            ActualizarSeleccion(BtnMenuProyectos);
        }

        private void ImpExp_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new ImpExp());
            ResetearColoresMenu();
            ActualizarSeleccion(BtnImpExp);
        }

        private void Menu_Multimedia_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Multimedia());
            ResetearColoresMenu();
            ActualizarSeleccion(BtnMenuMultimedia);
        }

        private void Menu_Videos360_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Videos360());
            ResetearColoresMenu();
            ActualizarSeleccion(BtnMenuVideos360);
        }

        private void Menu_Camaras_Click(object sender, MouseButtonEventArgs e)
        {
            FrameSecundario.Navigate(new Camaras());
            ResetearColoresMenu();
            ActualizarSeleccion(BtnMenuCamaras);
        }

        private void ActualizarSeleccion(Border botonSeleccionado)
        {
            ResetearColoresMenu();
            botonSeleccionado.Background = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString("#678EC2"));
            BarraLateral.Width = 60;
            TxtAdministrar.Visibility = Visibility.Collapsed;
        }

        // --- LÓGICA DE EXPANSIÓN DEL MENÚ ---

        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            BarraLateral.Width = 200;
            // Solo mostrar la etiqueta "Administrar" si el usuario es super
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
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
    public partial class AgregarVideo360Window : Window
    {
        private readonly Video360Item _video;

        // Solo se usa para editar — siempre recibe el item existente
        public AgregarVideo360Window(Video360Item video)
        {
            InitializeComponent();
            _video = video;
            txtIDSalon.Text = video.IDAula;
            txtRutaVideo.Text = string.IsNullOrEmpty(video.RutaVideo)
                                ? "Seleccionar video..."
                                : video.RutaVideo;
        }

        private void SeleccionarArchivo_GotFocus(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Archivos de Video|*.mp4;*.avi;*.mkv;*.mov|Todos los archivos|*.*"
            };

            if (dialog.ShowDialog() == true)
                txtRutaVideo.Text = dialog.FileName;

            // Quitar foco para evitar que se reabra al volver al TextBox
            Focus();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Actualizar el item en memoria; el Code-Behind de Videos360 persiste en BD
            _video.RutaVideo = txtRutaVideo.Text == "Seleccionar video..."
                               ? ""
                               : txtRutaVideo.Text;

            DialogResult = true;
            Close();
        }
    }
}
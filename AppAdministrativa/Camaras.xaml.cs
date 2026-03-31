using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
    public partial class Camaras : Page
    {
        ObservableCollection<Camara> datosCamaras = new();
        ObservableCollection<Camara> datosFiltrados = new();

        public Camaras()
        {
            InitializeComponent();
            InicializarCamaras();
        }

        private void InicializarCamaras()
        {
            datosCamaras = new ObservableCollection<Camara>(
                DatabaseService.Instance.GetCamaras());
            datosFiltrados = new ObservableCollection<Camara>(datosCamaras);
            ListaCamaras.ItemsSource = datosFiltrados;
        }
        

        // Guarda IP y Ruta de la cámara correspondiente a la tarjeta
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Camara camara)
            {
                DatabaseService.Instance.ActualizarCamara(camara);
                MessageBox.Show($"Cámara del piso {camara.Piso} actualizada correctamente.",
                    "Configuración guardada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Alterna el estado Conectada / Desconectada y lo persiste en BD
        private void BtnToggleConexion_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Camara camara)
            {
                camara.EstaConectada = !camara.EstaConectada;
                DatabaseService.Instance.ActualizarCamara(camara);
            }
        }
    }

    // Modelo de cámara con notificación de cambios para que el binding refresque el botón
    public class Camara : INotifyPropertyChanged
    {
        public int Piso { get; set; }
        public string Titulo => $"Cámara del piso {Piso}";
        public string IP { get; set; } = "";
        public string Ruta { get; set; } = "";

        private bool _estaConectada;
        public bool EstaConectada
        {
            get => _estaConectada;
            set
            {
                _estaConectada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextoBotonConexion));
            }
        }

        public string TextoBotonConexion => EstaConectada ? "Conectada" : "Desconectada";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
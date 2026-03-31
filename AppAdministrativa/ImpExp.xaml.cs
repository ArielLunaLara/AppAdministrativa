using AppAdministrativa.Services;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace AppAdministrativa
{
    public partial class ImpExp : Page
    {
        private DatabaseService db = new DatabaseService();
        private ExcelService excel = new ExcelService();

        public ImpExp()
        {
            InitializeComponent();
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    excel.ExportarTodo(db, dialog.FileName);
                    MessageBox.Show("Exportación exitosa");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnImportar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Excel (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    excel.ImportarTodo(db, dialog.FileName);
                    MessageBox.Show("Importación exitosa");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
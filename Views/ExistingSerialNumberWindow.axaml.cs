using Avalonia.Controls;
using Avalonia.Interactivity;
namespace MyAvaloniaApp.Views
{
    public partial class ExistingSerialNumberWindow : Window
    {
        public bool? UserResponse { get; private set; }

        public ExistingSerialNumberWindow(string seriNo, MainWindow mainWindow)
        {
            InitializeComponent();
            UserResponse = null; // Başlangıçta yanıt yok
        }

        private void OnYesButtonClick(object sender, RoutedEventArgs e)
        {
            UserResponse = true;
            Close(); // Pencereyi kapat
        }

        private void OnNoButtonClick(object sender, RoutedEventArgs e)
        {
            UserResponse = false;
            Close(); // Pencereyi kapat
        }
    }
}

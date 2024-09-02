using Avalonia.Controls;
using Avalonia.Threading; // Needed for DispatcherTimer
using System;

namespace MyAvaloniaApp.Views
{
    public partial class SuccessUpdateWindow : Window
    {
        private DispatcherTimer _timer;

        public SuccessUpdateWindow()
        {
            InitializeComponent(); // Initialize UI components
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Set the timer interval to 3 seconds
            };
            _timer.Tick += OnTimerTick!;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop(); // Stop the timer
            Close(); // Close the window
        }
    }
}

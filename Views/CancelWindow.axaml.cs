using Avalonia.Controls;
using Avalonia.Threading; // Needed for DispatcherTimer
using System;

namespace MyAvaloniaApp.Views
{
    public partial class CancelWindow : Window
    {
        private DispatcherTimer timer;

        public CancelWindow()
        {
            InitializeComponent(); // Initialize UI components
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3) // Set the timer interval to 3 seconds
            };
            timer.Tick += OnTimerTick!;
            timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            timer.Stop(); // Stop the timer
            Close(); // Close the window
        }
    }
}

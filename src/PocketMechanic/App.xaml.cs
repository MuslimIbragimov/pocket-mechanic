using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace PocketMechanic
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new AppShell());
        }
    }
}
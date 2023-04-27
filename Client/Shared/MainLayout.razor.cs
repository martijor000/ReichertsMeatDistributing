using Microsoft.JSInterop;
using System.Drawing;

namespace ReichertsMeatDistributingWebProject.Shared
{
    partial class MainLayout
    {
        private bool IsMenuOpen { get; set; }

        private void ToggleMenu()
        {
            IsMenuOpen = !IsMenuOpen;
        }

    }
}

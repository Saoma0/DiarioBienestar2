using CommunityToolkit.Maui.Views;

namespace DiarioBienestar2
{
    public partial class AlertPopUp : Popup
    {
        public AlertPopUp(string mensaje)
        {
            InitializeComponent();
            popupMessage.Text = mensaje; // Establecer mensaje en el popup
            
        }

        

       
    }
}

namespace DiarioBienestar2;

using CommunityToolkit.Maui.Views;
using Microsoft.Win32;
using MySqlConnector;
using System.Diagnostics;

public partial class MainPage : ContentPage
{
    string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";


    public MainPage()
    {
        InitializeComponent();
        bienvenida_usuario();

    }

    //Mensaje de bienvenida
    public void bienvenida_usuario()
    {
        bienvenidaUsuario.Text = "Bienvenido " + NombreUsuario.nombreUsuario + " a tu diario de bienestar";
    }


    //Navega a la pag de registro
    private async void registro_usuario(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Registro());
    }


    //navega a pag de inicio sesion y si ya has iniciao sesion muestra alerta
    private async void inicio_sesion(object sender, EventArgs e)
    {
        if (Sesion.IsLoggedIn)
        {
            Debug.WriteLine("Debug PopUp: Ya has inciado sesion");
            ShowrPopUpAlert("Ya has iniciado sesion, cierra sesion para continuar");
            Application.Current.MainPage = new AppShell();


        }
        else
        {
            Application.Current.MainPage = new NavigationPage(new InicioSesion());
        }
    }

    //Muestra PopUp
    public async void ShowrPopUpAlert(string messageAlert)
    {
        Debug.WriteLine("PopUp Debug");
        var popup = new AlertPopUp(messageAlert);
        await this.ShowPopupAsync(popup);
    }

}
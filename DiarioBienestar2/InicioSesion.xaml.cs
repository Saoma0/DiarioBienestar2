namespace DiarioBienestar2;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using MySqlConnector;
using System.Diagnostics;

public partial class InicioSesion : ContentPage
{
    string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";


    public InicioSesion()
    {
        InitializeComponent();
       


    }

    private async void iniciar_sesion(object sender, EventArgs e)
    {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT ID, nombre_usuario, contrasena FROM Registro WHERE nombre_usuario = @nombre_usuario";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre_usuario", entryInicioUsuario.Text);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader.GetString("contrasena");
                                int nuevoID = reader.GetInt32("ID");
                                string nuevoNombreUsuario = reader.GetString("nombre_usuario");

                                // Verificar contraseña
                                bool isValidPassword = BCrypt.Net.BCrypt.Verify(entryInicioContraseña.Text, storedPassword);

                                if (isValidPassword)
                                {
                                    // Se establecen los datos de un nuevo inicio de sesion
                                    Identificador.ID = nuevoID;
                                    NombreUsuario.nombreUsuario = nuevoNombreUsuario;
                                    Sesion.IsLoggedIn = true;

                                    ShowrPopUpAlert("Inicio de sesión correcto");
                                    Debug.WriteLine("Debug InicioCorrecto");
                                    Application.Current.MainPage = new AppShell();
                                }
                                else
                                {
                                    ShowrPopUpAlert("Contraseña incorrecta");
                                }
                            }
                            else
                            {
                                ShowrPopUpAlert("Usuario incorrecto");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowrPopUpAlert("Error al iniciar sesion");
                Debug.WriteLine("Error inicio sesion");
            }
        

        
    }

   


    
    public async void ShowrPopUpAlert(string messageAlert)
    {
        Debug.WriteLine("PopUp Debug");
        var popup = new AlertPopUp(messageAlert);
        await this.ShowPopupAsync(popup);
    }
    

}

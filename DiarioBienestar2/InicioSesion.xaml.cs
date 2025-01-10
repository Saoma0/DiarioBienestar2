namespace DiarioBienestar2;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using MySqlConnector;
using System.Diagnostics;

public partial class InicioSesion : ContentPage
{
    //Declara una cadena con la informacion para la conexion con la DB
    string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";


    public InicioSesion()
    {
        InitializeComponent();
       


    }

    //Metodo que inicia sesión
    private async void iniciar_sesion(object sender, EventArgs e)
    {
            try
            {
                //Se conecta con la base de datos
                using (var connection = new MySqlConnection(connectionString))
                {
                    //Abre la conexión
                    await connection.OpenAsync();

                    //Query SQL para obtener id, el usuario y la contraseña introducida por el usuario en la DB
                    string query = "SELECT ID, nombre_usuario, contrasena FROM Registro WHERE nombre_usuario = @nombre_usuario";
                    //crea el comando sql con la conexion   
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre_usuario", entryInicioUsuario.Text);
                    //ejecuta la consulta y obtiene un lector que lee los resultados de la consulta
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                       //Verifica si hay resultados
                            if (reader.Read())
                            {
                            //Obtiene los valores de las columnas que devueve la consulta
                                string storedPassword = reader.GetString("contrasena");
                                int nuevoID = reader.GetInt32("ID");
                                string nuevoNombreUsuario = reader.GetString("nombre_usuario");

                                // Verifica contraseña almacenada en la DB
                                bool isValidPassword = BCrypt.Net.BCrypt.Verify(entryInicioContraseña.Text, storedPassword);

                                //Si la password es valida
                                if (isValidPassword)
                                {
                                    // Se establecen los datos de un nuevo inicio de sesion
                                    Identificador.ID = nuevoID;
                                    NombreUsuario.nombreUsuario = nuevoNombreUsuario;
                                    Sesion.IsLoggedIn = true;

                                    //Muestra una alerta
                                    ShowrPopUpAlert("Inicio de sesión correcto");
                                    Debug.WriteLine("Debug InicioCorrecto");
                                    //Cambia la pagina principal
                                    Application.Current.MainPage = new AppShell();
                                }
                                else
                                {
                                //Alerta password incorrecta
                                    ShowrPopUpAlert("Contraseña incorrecta");
                                }
                            }
                            else
                            {
                            //Alerta usuario incorrecto
                                ShowrPopUpAlert("Usuario incorrecto");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            //Alerta error en inicio de sesion
                ShowrPopUpAlert("Error al iniciar sesion");
                Debug.WriteLine("Error inicio sesion");
            }
        

        
    }

   


    //Muestra el PopUp 
    public async void ShowrPopUpAlert(string messageAlert)
    {
        Debug.WriteLine("PopUp Debug");
        //Crea el popup
        var popup = new AlertPopUp(messageAlert);
        //Muestra el popup
        await this.ShowPopupAsync(popup);
    }
    

}

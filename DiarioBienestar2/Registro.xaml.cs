using CommunityToolkit.Maui.Views;
using MySqlConnector;
using System.Diagnostics;
using System.Security.Cryptography;

namespace DiarioBienestar2;

public partial class Registro : ContentPage
{
    string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";


    public Registro()
    {
        InitializeComponent();
    }

    private async void guardarRegistro(object sender, EventArgs e)
    {

        //Comprueba que los campos no estan vacios
        if (string.IsNullOrWhiteSpace(entryUsuario.Text) || string.IsNullOrWhiteSpace(entryContrase�a.Text))
        {
            ShowrPopUpAlert("Por favor, complete todos los campos.");
            return;
        }

        //Hashea la password
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(entryContrase�a.Text);
       

        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                //Comprueba si el nombre de usuario ya existe en la base de datos
                string checkQuery = "SELECT COUNT(*) FROM Registro WHERE nombre_usuario = @nombre_usuario";
                using (var checkCommand = new MySqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@nombre_usuario", entryUsuario.Text);
                    int count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                    //Si existe muestra alerta
                    if (count > 0)
                    {
                        ShowrPopUpAlert("El nombre ya esta siendo utilizado");
                        return; 
                    }
                }

                // Si el nombre de usuario no existe registra el usuario
                string query = "INSERT INTO Registro (nombre_usuario, contrasena) VALUES (@nombre_usuario, @contrasena);";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nombre_usuario", entryUsuario.Text);
                    command.Parameters.AddWithValue("@contrasena", hashedPassword);

                    await command.ExecuteNonQueryAsync(); 
                }

                ShowrPopUpAlert("Usuario Registrado");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error al guardar");
        }
    }
    public async void ShowrPopUpAlert(string messageAlert)
    {
        Debug.WriteLine("PopUp Debug");
        var popup = new AlertPopUp(messageAlert);
        await this.ShowPopupAsync(popup);
    }

}
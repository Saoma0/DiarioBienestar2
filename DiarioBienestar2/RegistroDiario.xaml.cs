using MySqlConnector;
using System.Diagnostics;

namespace DiarioBienestar2
{
    public partial class RegistroDiario : ContentPage
    {
        string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";

        public RegistroDiario()
        {
            InitializeComponent();
            if (!Sesion.IsLoggedIn)
            {
                // Si no ha iniciado sesión, redirigir a la página de inicio de sesión
                Application.Current.MainPage = new NavigationPage(new InicioSesion());
            }
        }

        private async void guardar_DatosUsuario(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Consulta SQL para verificar si ya existe un registro para el usuario y la fecha
                    string checkQuery = "SELECT COUNT(*) FROM Registro_Diario WHERE id_usuario = @id_usuario AND fecha = @fecha";

                    using (var checkCommand = new MySqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@id_usuario", Identificador.ID);
                        checkCommand.Parameters.AddWithValue("@fecha", fechaRegistro.Date);

                        var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                        if (count > 0)
                        {
                            // Si ya existe un registro para el usuario en esa fecha, mostrar mensaje y no guardar
                            await DisplayAlert("Error", "Ya has guardado un registro para este día", "Cerrar");
                            return; // Salir del método sin guardar el nuevo registro
                        }
                    }

                    // Si no existe un registro para ese día, guardar el nuevo
                    string query = "INSERT INTO Registro_Diario (id_usuario, registro, intensidad, energia, fecha) VALUES (@id_usuario,@registro_usuario,@intensidad,@energia, @fecha);";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Añadir los parámetros
                        command.Parameters.AddWithValue("@id_usuario", Identificador.ID);
                        command.Parameters.AddWithValue("@registro_usuario", editorUsuario.Text);
                        command.Parameters.AddWithValue("@intensidad", valorIntensidad.Value);
                        command.Parameters.AddWithValue("@energia", valorEnergia.Value);
                        command.Parameters.AddWithValue("@fecha", fechaRegistro.Date);

                        await command.ExecuteNonQueryAsync(); // Ejecutar la consulta

                        await DisplayAlert("Exito", "Los datos han sido guardados", "Cerrar");

                        // Actualizamos los datos después de guardar el registro
                        var listaRegistrosPage = Navigation.NavigationStack.FirstOrDefault(p => p is ListaRegistros) as ListaRegistros;
                        if (listaRegistrosPage != null)
                        {
                             listaRegistrosPage.cargar_datos(); // Llamamos a cargar_datos para actualizar la lista
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al guardar: {ex.Message}");
            }
        }
    }
}

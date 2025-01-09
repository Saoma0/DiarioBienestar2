using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace DiarioBienestar2;

public partial class ListaRegistros : ContentPage
{
    string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";
    public ObservableCollection<RegistroLista> registros { get; set; } = new ObservableCollection<RegistroLista>();

    public ListaRegistros()
    {
        InitializeComponent();
        if (!Sesion.IsLoggedIn)
        {
            // Si no ha iniciado sesión, redirigir a la página de inicio de sesión
            Application.Current.MainPage = new NavigationPage(new InicioSesion());
        }
        viewRegistros.ItemsSource = registros;
        cargar_datos(); // Cargar los datos al iniciar la página
    }

    public async void cargar_datos()
    {
        try
        {
            if (Identificador.ID == 0) // MODIFICADO: Validar que el usuario esté identificado
            {
                Debug.WriteLine("El identificador del usuario no está establecido.");
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Consulta SQL para cargar solo los registros del usuario actual
                string query = "SELECT id, registro, intensidad, energia, fecha FROM Registro_Diario WHERE id_usuario = @idUsuario;"; // MODIFICADO
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idUsuario", Identificador.ID); // MODIFICADO
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        registros.Clear(); // Limpiamos la colección antes de cargar los datos
                        while (await reader.ReadAsync())
                        {
                            registros.Add(new RegistroLista
                            {
                                ID = reader.GetInt32("id"),
                                Detalles = reader.IsDBNull("registro") ? "" : reader.GetString("registro"),
                                Intensidad = reader.GetInt32("intensidad"),
                                Energia = reader.GetInt32("energia"),
                                Fecha = reader.GetDateTime("fecha")
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al cargar los datos: {ex.Message}");
        }
    }

    private async void eliminarRegistro(int id)
    {
        try
        {
            if (Identificador.ID == 0) // MODIFICADO: Validar que el usuario esté identificado
            {
                Debug.WriteLine("El identificador del usuario no está establecido.");
                return;
            }

            bool isConfirmed = await DisplayAlert("Confirmar eliminación", "¿Estás seguro de que deseas eliminar este registro?", "Sí", "No");

            if (isConfirmed)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Eliminar solo registros asociados al usuario actual
                    string query = "DELETE FROM Registro_Diario WHERE id = @id AND id_usuario = @idUsuario;"; // MODIFICADO
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@idUsuario", Identificador.ID); // MODIFICADO
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (var registro in registros)
                    {
                        if (registro.ID == id)
                        {
                            registros.Remove(registro);
                            break;
                        }
                    }

                    await DisplayAlert("Éxito", "El registro ha sido eliminado correctamente.", "Cerrar");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al eliminar el registro: {ex.Message}");
            await DisplayAlert("Error", "Hubo un problema al eliminar el registro. Por favor, inténtalo de nuevo.", "Cerrar");
        }
    }

    private void OnEliminarClicked(object sender, EventArgs e)
    {
        var imagebutton = sender as ImageButton;
        var registro = imagebutton?.BindingContext as RegistroLista;
        if (registro != null)
        {
            eliminarRegistro(registro.ID);
        }
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        cargar_datos();
        await DisplayAlert("Actualización", "La lista de registros se ha actualizado correctamente.", "Cerrar");
    }
}

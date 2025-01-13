using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace DiarioBienestar2;

public partial class ListaRegistros : ContentPage
{
    //Declara una cadena con la informacion para la conexion con la DB
    string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";

    //Coleccion para almacenar y notificar los cambios en la lista
    public ObservableCollection<RegistroLista> registros { get; set; } = new ObservableCollection<RegistroLista>();

    public ListaRegistros()
    {
        InitializeComponent();
        //Comprueba si ha iniciado sesion
        if (!Sesion.IsLoggedIn)
        {
            // Si no ha iniciado sesión, redirige a la página de inicio de sesión
            Application.Current.MainPage = new NavigationPage(new InicioSesion());
        }
        viewRegistros.ItemsSource = registros;
        cargar_datos(); // Carga los datos al inicializar la página
    }

    //Metodo que carga los datos de los registros 
    public async void cargar_datos()
    {
        try
        {
            //Comprueba ID
            if (Identificador.ID == 0) 
            {
                Debug.WriteLine("El identificador del usuario no está establecido.");
                return;
            }
            //Se conecta con la base de datos
            using (var connection = new MySqlConnection(connectionString))
            {
                //Abre la conexión
                await connection.OpenAsync();


                //Query SQL para obtener id, registroo, intensidad, energia y fecha introducida por el usuario en la DB
                string query = "SELECT id, registro, intensidad, energia, fecha FROM Registro_Diario WHERE id_usuario = @idUsuario;"; 
                using (var command = new MySqlCommand(query, connection))//crea comando sql
                {
                    command.Parameters.AddWithValue("@idUsuario", Identificador.ID);
                    //ejecuta la consulta y obtiene un lector que lee los resultados de la consulta
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        registros.Clear(); // Limpia la colección antes de cargar los datos
                        while (await reader.ReadAsync()) //Lee los resultaados de cada fila
                        {
                            registros.Add(new RegistroLista
                            {
                                ID = reader.GetInt32("id"),
                                Detalles = reader.IsDBNull("registro") ? "" : reader.GetString("registro"), //si el registro tiene valor null, devuelve una cadena vacia, sino devuelve el registro.
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
            Debug.WriteLine("Error al cargar los datos");
        }
    }

    //Metodo para eliminar registro
    private async void eliminarRegistro(int id)
    {
        try
        {
            if (Identificador.ID == 0) // Valida que el usuario esté identificado
            {
                Debug.WriteLine("El identificador del usuario no está establecido.");
                return;
            }

            //Muestra alerta de confirmacion
            bool isConfirmed = await DisplayAlert("Confirmación", "¿Deseas eliminar este registro?", "Sí", "No");

            //Si confirma 
            if (isConfirmed)
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Elimina solo registros asociados al usuario actual
                    string query = "DELETE FROM Registro_Diario WHERE id = @id AND id_usuario = @idUsuario;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@idUsuario", Identificador.ID);
                        await command.ExecuteNonQueryAsync();
                    }

                    //Elimina registro de la coleccion
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
            Debug.WriteLine("Error al eliminar el registro");
        }
    }

    //Evento quw al hacer clic el boton de eliminar
    private void OnEliminarClicked(object sender, EventArgs e)
    {
        //Obtiene el boton que ha activado ele evento
        var imagebutton = sender as ImageButton;
        //Obtiene el registro asociado al boton
        var registro = imagebutton?.BindingContext as RegistroLista; 
        if (registro != null)
        {
            eliminarRegistro(registro.ID);//llama metodo que elimina registro
        }
    }

    //Evento que actualiza los registros de la lista
    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        cargar_datos();
        await DisplayAlert("Actualización", "La lista de registros se ha actualizado correctamente.", "Cerrar");
    }
}

using MySqlConnector;
using System.Diagnostics;

namespace DiarioBienestar2
{
    public partial class Progreso : ContentPage
    {
        string connectionString = "Server=127.0.0.1;Port=3306;Database=DiarioBienestar;User=saoma;Password=0000;";

        public Progreso()
        {
            InitializeComponent();
            // Si no ha iniciado sesión redirigir a la página de inicio de sesión
            if (!Sesion.IsLoggedIn)
            {
                
                Application.Current.MainPage = new NavigationPage(new InicioSesion());
            }
        }

        //obtiene los promedios y lo muestra en progresbarr
        public async void ObtenerPromediosUltimaSemana()
        {
            double totalActividad = 0;
            double totalEnergia = 0;
            int contador = 0;

            try
            {
                if (Identificador.ID == 0) 
                {
                    Debug.WriteLine("El identificador del usuario no está establecido.");
                    return;
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // 

                    // Query SQL para obtener los registros de la última semana filtrados por usuario
                    string query = "SELECT intensidad, energia FROM Registro_Diario WHERE fecha >= CURDATE() - INTERVAL 7 DAY AND id_usuario = @idUsuario;"; 
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idUsuario", Identificador.ID); 
                        using (MySqlDataReader reader = await command.ExecuteReaderAsync()) // Usar ExecuteReaderAsync
                        {
                            while (await reader.ReadAsync()) // Usar ReadAsync
                            {
                                totalActividad += reader.GetInt32("intensidad");
                                totalEnergia += reader.GetInt32("energia");
                                contador++;
                            }
                        }
                    }
                }

                // Calcula promedios
                double actividadPromedio = contador > 0 ? totalActividad / contador : 0;
                double energiaPromedio = contador > 0 ? totalEnergia / contador : 0;

                // Asigna los valores de progreso
                double valorActividad = Math.Min(1, actividadPromedio / 10);
                double valorEnergia = Math.Min(1, energiaPromedio / 5);

                progressBarActividad.Progress = valorActividad;
                progressBarEnergia.Progress = valorEnergia;

                // Ajusta los colores segun los valores de progreso
                ObtenerColorDeProgresoActividad(valorActividad);
                ObtenerColorDeProgresoEnergia(valorEnergia);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al obtener promedio");
                progressBarActividad.Progress = 0;
                progressBarEnergia.Progress = 0;
                ObtenerColorDeProgresoActividad(0); // Establece color a rojo por defecto
                ObtenerColorDeProgresoEnergia(0); // Establece color a rojo por defecto
            }
        }

        //metodo obtiene colores de la barra de actividad
        private void ObtenerColorDeProgresoActividad(double value)
        {
            if (value < 0.4) // Valor bajo 
            {
                progressBarActividad.ProgressColor = Colors.Red;
            }
            else if (value >= 0.4 && value < 0.7) // Valor medio
            {
                progressBarActividad.ProgressColor = Colors.Orange;
            }
            else // Valor alto
            {
                progressBarActividad.ProgressColor = Colors.Green;
            }
        }

        //metodo obtiene colores de la barra de energia
        private void ObtenerColorDeProgresoEnergia(double value)
        {
            if (value < 0.4) // Valor bajo 
            {
                progressBarEnergia.ProgressColor = Colors.Red;
            }
            else if (value >= 0.4 && value < 0.7) // Valor medio
            {
                progressBarEnergia.ProgressColor = Colors.Orange;
            }
            else // Valor alto 
            {
                progressBarEnergia.ProgressColor = Colors.Green;
            }
        }

        // Metodo que actualiza los datos
        private void actualizar_datos(object sender, EventArgs e)
        {
            ObtenerPromediosUltimaSemana();
        }
    }
}

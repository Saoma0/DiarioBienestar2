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
            if (!Sesion.IsLoggedIn)
            {
                // Si no ha iniciado sesi�n, redirigir a la p�gina de inicio de sesi�n
                Application.Current.MainPage = new NavigationPage(new InicioSesion());
            }
        }

        // M�todo para obtener los promedios de actividad f�sica y energ�a de la �ltima semana
        public async void ObtenerPromediosUltimaSemana()
        {
            double totalActividad = 0;
            double totalEnergia = 0;
            int contador = 0;

            try
            {
                if (Identificador.ID == 0) // MODIFICADO: Validar que el usuario est� identificado
                {
                    Debug.WriteLine("El identificador del usuario no est� establecido.");
                    return;
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync(); // Usar OpenAsync para abrir la conexi�n de manera asincr�nica

                    // Consulta SQL para obtener los registros de la �ltima semana filtrados por usuario
                    string query = "SELECT intensidad, energia FROM Registro_Diario WHERE fecha >= CURDATE() - INTERVAL 7 DAY AND id_usuario = @idUsuario;"; // MODIFICADO
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idUsuario", Identificador.ID); // MODIFICADO
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

                // Calcular promedios
                double actividadPromedio = contador > 0 ? totalActividad / contador : 0;
                double energiaPromedio = contador > 0 ? totalEnergia / contador : 0;

                // Asignar los valores de progreso
                double valorActividad = Math.Min(1, actividadPromedio / 10);
                double valorEnergia = Math.Min(1, energiaPromedio / 5);

                progressBarActividad.Progress = valorActividad;
                progressBarEnergia.Progress = valorEnergia;

                // Ajustar los colores seg�n los valores de progreso
                ObtenerColorDeProgresoActividad(valorActividad);
                ObtenerColorDeProgresoEnergia(valorEnergia);

                // Verifica que los valores sean correctos
                Debug.WriteLine($"Actividad Promedio: {actividadPromedio}");
                Debug.WriteLine($"Energ�a Promedio: {energiaPromedio}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al obtener promedio");
                // En caso de error, asegurarse de poner las barras en 0
                progressBarActividad.Progress = 0;
                progressBarEnergia.Progress = 0;
                ObtenerColorDeProgresoActividad(0); // Establecer color a rojo por defecto
                ObtenerColorDeProgresoEnergia(0); // Establecer color a rojo por defecto
            }
        }

        // M�todo para obtener el color de la barra de progreso de actividad
        private void ObtenerColorDeProgresoActividad(double value)
        {
            if (value < 0.4) // Valor bajo (menos del 40%)
            {
                progressBarActividad.ProgressColor = Colors.Red;
            }
            else if (value >= 0.4 && value < 0.7) // Valor intermedio (40% - 69%)
            {
                progressBarActividad.ProgressColor = Colors.Orange;
            }
            else // Valor bueno (70% - 100%)
            {
                progressBarActividad.ProgressColor = Colors.Green;
            }
        }

        // M�todo para obtener el color de la barra de progreso de energ�a
        private void ObtenerColorDeProgresoEnergia(double value)
        {
            if (value < 0.4) // Valor bajo (menos del 40%)
            {
                progressBarEnergia.ProgressColor = Colors.Red;
            }
            else if (value >= 0.4 && value < 0.7) // Valor intermedio (40% - 69%)
            {
                progressBarEnergia.ProgressColor = Colors.Orange;
            }
            else // Valor bueno (70% - 100%)
            {
                progressBarEnergia.ProgressColor = Colors.Green;
            }
        }

        // M�todo para actualizar los datos al hacer clic en el bot�n
        private void actualizar_datos(object sender, EventArgs e)
        {
            ObtenerPromediosUltimaSemana();
        }
    }
}

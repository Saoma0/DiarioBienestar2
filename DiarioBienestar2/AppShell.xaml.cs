namespace DiarioBienestar2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
        }

        private async void CerrarSesionClicked(object sender, EventArgs e)
        {
            if (!Sesion.IsLoggedIn) // Verifica si el usuario no ha iniciado sesión
            {
                // Si no está logueado, muestra una alerta indicando que debe iniciar sesión primero
                await DisplayAlert("Advertencia", "Primero debes iniciar sesión.", "OK");
                return; // Sale del método sin hacer nada
            }
            bool respuesta = await DisplayAlert("Cierre de sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");

            if (respuesta) // Si el usuario elige "Sí"
            {
                // Limpiar los datos de sesión
                LimpiarDatosSesion();

                // Redirigir a la página de login
                Application.Current.MainPage = new NavigationPage(new InicioSesion());
            }

        }

        private void LimpiarDatosSesion()
        {
            Identificador.ID = 0;
            NombreUsuario.nombreUsuario = string.Empty;
            Sesion.IsLoggedIn = false;
        }
    }
}

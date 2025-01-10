namespace DiarioBienestar2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
        }

        //Metodo que cierra la sesion del usuario
        private async void CerrarSesionClicked(object sender, EventArgs e)
        {
            /** 
             Si la sesion no esta iniciada, saldra una alerta para que inicies sesion
             Si la sesion esta iniciada te muestra una alerta preguntando si quieres cerrar sesion
             Si quieres cerrar sesion te envia automaticamente al inicio de sesion
             */
            if (!Sesion.IsLoggedIn) 
            {
               
                await DisplayAlert("Advertencia", "Primero debes iniciar sesión.", "OK");
                return; //sale del metodo
            }
            bool respuesta = await DisplayAlert("Cierre de sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");

            if (respuesta)
            {
                // Limpia los datos de sesión
                LimpiarDatosSesion();
                Application.Current.MainPage = new NavigationPage(new InicioSesion());
            }

        }

        //Metodo para lipiar los datos de la sesion
        private void LimpiarDatosSesion()
        {
            Identificador.ID = 0;
            NombreUsuario.nombreUsuario = string.Empty;
            Sesion.IsLoggedIn = false;
        }
    }
}

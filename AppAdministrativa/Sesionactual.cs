namespace AppAdministrativa
{
    /// <summary>
    /// Guarda el estado de la sesión activa en toda la aplicación.
    /// Se inicializa en MainWindow al validar el login.
    /// </summary>
    public static class SesionActual
    {
        public static string Usuario { get; set; } = "";
        public static string Role { get; set; } = "normal";

        public static bool EsSuper => Role == "super";
    }
}
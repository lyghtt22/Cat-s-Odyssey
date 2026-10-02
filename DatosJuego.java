public class DatosJuego
{
    // ================================================
    // PERSONAJE ELEGIDO
    // ================================================

    private static String personajeSeleccionado = "Aurora";


    // ================================================
    // NIVEL ACTUAL
    // ================================================

    /*
     * Aunque ya no vamos a usar varios niveles,
     * dejamos esta variable para no romper
     * las clases antiguas que todavía la consultan.
     */
    private static int nivelActual = 1;


    // ================================================
    // SELECCIONAR PERSONAJE
    // ================================================

    public static void seleccionarPersonaje(String nombre)
    {
        personajeSeleccionado = nombre;
    }


    // ================================================
    // OBTENER PERSONAJE
    // ================================================

    public static String getPersonajeSeleccionado()
    {
        return personajeSeleccionado;
    }


    // ================================================
    // COMPROBAR PERSONAJE
    // ================================================

    public static boolean esAurora()
    {
        return personajeSeleccionado.equals("Aurora");
    }


    public static boolean esSelene()
    {
        return personajeSeleccionado.equals("Selene");
    }


    // ================================================
    // COMPATIBILIDAD CON CÓDIGO ANTERIOR
    // ================================================

    public static int getNivelActual()
    {
        return nivelActual;
    }


    public static void setNivelActual(int nivel)
    {
        nivelActual = nivel;
    }


    public static void iniciarPartida()
    {
        nivelActual = 1;
    }


    public static void siguienteNivel()
    {
        nivelActual++;
    }


    public static boolean esUltimoNivel()
    {
        return nivelActual >= 3;
    }


    // ================================================
    // REINICIAR DATOS
    // ================================================

    public static void reiniciar()
    {
        personajeSeleccionado = "Aurora";
        nivelActual = 1;
    }
}
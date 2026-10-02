import greenfoot.*;

public class AccionCombate
{
    // ================================================
    // DATOS DE LA ACCIÓN
    // ================================================

    private String nombre;
    private int danio;
    private int probabilidadAcierto;


    // ================================================
    // CONSTRUCTOR
    // ================================================

    public AccionCombate(
        String nombre,
        int danio,
        int probabilidadAcierto
    )
    {
        this.nombre = nombre;
        this.danio = danio;
        this.probabilidadAcierto = probabilidadAcierto;
    }


    // ================================================
    // GETTERS
    // ================================================

    public String getNombre()
    {
        return nombre;
    }


    public int getDanio()
    {
        return danio;
    }


    public int getProbabilidadAcierto()
    {
        return probabilidadAcierto;
    }


    // ================================================
    // COMPROBAR SI EL ATAQUE ACIERTA
    // ================================================

    public boolean acierta()
    {
        int numero =
            Greenfoot.getRandomNumber(100);

        return numero < probabilidadAcierto;
    }
}
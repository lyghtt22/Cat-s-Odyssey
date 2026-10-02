import greenfoot.*;

public class InfanteriaEnemiga extends Unidad
{
    // =================================================
    // IDLE
    // =================================================

    private int frameIdle = 1;

    private int contadorAnimacion = 0;

    private final int CANTIDAD_FRAMES_IDLE = 8;


    // =================================================
    // DAÑO
    // =================================================

    private boolean recibiendoDanio = false;

    private int contadorDanio = 0;

    private final int DURACION_DANIO = 30;


    // =================================================
    // CONSTRUCTOR
    // =================================================

    public InfanteriaEnemiga()
    {
        super(
            "Reina Malvada",
            40,
            10
        );


        actualizarSpriteIdle();
    }


    // =================================================
    // ACT
    // =================================================

    public void act()
    {
        // =============================================
        // DERROTADA
        // =============================================

        if (!estaViva())
        {
            return;
        }


        // =============================================
        // RECIBIENDO DAÑO
        // =============================================

        if (recibiendoDanio)
        {
            animarDanio();

            return;
        }


        // =============================================
        // IDLE NORMAL
        // =============================================

        animarIdle();
    }


    // =================================================
    // IDLE
    // =================================================

    private void animarIdle()
    {
        contadorAnimacion++;


        if (contadorAnimacion >= 12)
        {
            frameIdle++;


            if (frameIdle > CANTIDAD_FRAMES_IDLE)
            {
                frameIdle = 1;
            }


            actualizarSpriteIdle();


            contadorAnimacion = 0;
        }
    }


    // =================================================
    // ACTUALIZAR SPRITE IDLE
    // =================================================

    private void actualizarSpriteIdle()
    {
        GreenfootImage imagen =
            FabricaImagenes.madrastraIdle(
                frameIdle
            );


        imagen.scale(
            300,
            300
        );


        setImage(
            imagen
        );
    }


    // =================================================
    // AL RECIBIR DAÑO
    // =================================================

    protected void alRecibirDanio()
    {
        // =============================================
        // SI LA REINA MUERE
        // =============================================

        if (!estaViva())
        {
            recibiendoDanio = false;


            GreenfootImage imagen =
                FabricaImagenes.reinaDerrota();


            imagen.scale(
                300,
                300
            );


            setImage(
                imagen
            );


            return;
        }


        // =============================================
        // SI TODAVÍA TIENE VIDA
        // =============================================

        recibiendoDanio = true;


        contadorDanio =
            DURACION_DANIO;


        GreenfootImage imagen =
            FabricaImagenes.reinaDanio();


        imagen.scale(
            300,
            300
        );


        setImage(
            imagen
        );
    }


    // =================================================
    // ANIMACIÓN DE DAÑO
    // =================================================

    private void animarDanio()
    {
        contadorDanio--;


        if (contadorDanio <= 0)
        {
            recibiendoDanio = false;


            frameIdle = 1;

            contadorAnimacion = 0;


            actualizarSpriteIdle();
        }
    }


    // =================================================
    // ATAQUE BASE
    // =================================================

    public int obtenerDanioBase()
    {
        return getAtaque();
    }
}
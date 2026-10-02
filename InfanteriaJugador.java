import greenfoot.*;

public class InfanteriaJugador extends Unidad
{
    // =================================================
    // ESTADOS
    // =================================================

    private static final int ESTADO_IDLE = 0;
    private static final int ESTADO_RAPIDO = 1;
    private static final int ESTADO_NORMAL = 2;
    private static final int ESTADO_FUERTE = 3;

    private int estadoActual = ESTADO_IDLE;


    // =================================================
    // FRAMES
    // =================================================

    private int frameActual = 1;
    private int contadorAnimacion = 0;


    // =================================================
    // FRAMES AURORA
    // =================================================

    private final int AURORA_IDLE = 6;
    private final int AURORA_RAPIDO = 6;
    private final int AURORA_NORMAL = 8;
    private final int AURORA_FUERTE = 8;


    // =================================================
    // FRAMES SELENE
    // =================================================

    private final int SELENE_RAPIDO = 6;
    private final int SELENE_NORMAL = 8;
    private final int SELENE_FUERTE = 6;


    // =================================================
    // VELOCIDADES
    // =================================================

    private final int VELOCIDAD_IDLE = 12;
    private final int VELOCIDAD_RAPIDO = 3;
    private final int VELOCIDAD_NORMAL = 6;
    private final int VELOCIDAD_FUERTE = 8;


    // =================================================
    // RECIBIR DAÑO
    // =================================================

    private boolean recibiendoDanio = false;

    private int contadorDanio = 0;

    private final int DURACION_DANIO = 30;


    // =================================================
    // CONSTRUCTOR
    // =================================================

    public InfanteriaJugador()
    {
        super(
            DatosJuego.getPersonajeSeleccionado(),
            40,
            12
        );

        mostrarIdle();
    }


    // =================================================
    // ACT
    // =================================================

    public void act()
    {
        // =============================================
        // DERROTADO
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
        // ESTADOS NORMALES
        // =============================================

        if (estadoActual == ESTADO_IDLE)
        {
            animarIdle();
        }

        else if (estadoActual == ESTADO_RAPIDO)
        {
            animarAtaqueRapido();
        }

        else if (estadoActual == ESTADO_NORMAL)
        {
            animarAtaqueNormal();
        }

        else if (estadoActual == ESTADO_FUERTE)
        {
            animarAtaqueFuerte();
        }
    }


    // =================================================
    // IDLE
    // =================================================

    private void animarIdle()
    {
        // Aurora tiene animación Idle.
        if (DatosJuego.esAurora())
        {
            contadorAnimacion++;


            if (contadorAnimacion >= VELOCIDAD_IDLE)
            {
                frameActual++;


                if (frameActual > AURORA_IDLE)
                {
                    frameActual = 1;
                }


                mostrarIdle();

                contadorAnimacion = 0;
            }
        }

        // Selene por ahora utiliza su imagen base.
        else
        {
            mostrarIdle();
        }
    }


    // =================================================
    // MOSTRAR IDLE
    // =================================================

    private void mostrarIdle()
    {
        GreenfootImage imagen;


        if (DatosJuego.esAurora())
        {
            imagen =
                FabricaImagenes.auroraIdle(
                    frameActual
                );
        }

        else
        {
            imagen =
                FabricaImagenes.seleneBase();
        }


        imagen.scale(
            280,
            280
        );


        setImage(
            imagen
        );
    }


    // =================================================
    // ATAQUE RÁPIDO
    // =================================================

    public void atacarRapido()
    {
        if (estadoActual != ESTADO_IDLE)
        {
            return;
        }


        if (recibiendoDanio)
        {
            return;
        }


        if (!estaViva())
        {
            return;
        }


        estadoActual = ESTADO_RAPIDO;

        frameActual = 1;

        contadorAnimacion = 0;
    }


    private void animarAtaqueRapido()
    {
        contadorAnimacion++;


        if (contadorAnimacion < VELOCIDAD_RAPIDO)
        {
            return;
        }


        contadorAnimacion = 0;


        GreenfootImage imagen;

        int cantidadFrames;


        // =============================================
        // AURORA
        // =============================================

        if (DatosJuego.esAurora())
        {
            imagen =
                FabricaImagenes.auroraAtaqueRapido(
                    frameActual
                );


            cantidadFrames =
                AURORA_RAPIDO;
        }


        // =============================================
        // SELENE
        // =============================================

        else
        {
            imagen =
                FabricaImagenes.seleneAtaqueRapido(
                    frameActual
                );


            cantidadFrames =
                SELENE_RAPIDO;
        }


        imagen.scale(
            280,
            280
        );


        setImage(
            imagen
        );


        frameActual++;


        if (frameActual > cantidadFrames)
        {
            volverIdle();
        }
    }


    // =================================================
    // ATAQUE NORMAL
    // =================================================

    public void atacarNormal()
    {
        if (estadoActual != ESTADO_IDLE)
        {
            return;
        }


        if (recibiendoDanio)
        {
            return;
        }


        if (!estaViva())
        {
            return;
        }


        estadoActual = ESTADO_NORMAL;

        frameActual = 1;

        contadorAnimacion = 0;
    }


    private void animarAtaqueNormal()
    {
        contadorAnimacion++;


        if (contadorAnimacion < VELOCIDAD_NORMAL)
        {
            return;
        }


        contadorAnimacion = 0;


        GreenfootImage imagen;

        int cantidadFrames;


        // =============================================
        // AURORA
        // =============================================

        if (DatosJuego.esAurora())
        {
            imagen =
                FabricaImagenes.auroraAtaqueNormal(
                    frameActual
                );


            cantidadFrames =
                AURORA_NORMAL;
        }


        // =============================================
        // SELENE
        // =============================================

        else
        {
            imagen =
                FabricaImagenes.seleneAtaqueNormal(
                    frameActual
                );


            cantidadFrames =
                SELENE_NORMAL;
        }


        imagen.scale(
            280,
            280
        );


        setImage(
            imagen
        );


        frameActual++;


        if (frameActual > cantidadFrames)
        {
            volverIdle();
        }
    }


    // =================================================
    // ATAQUE FUERTE
    // =================================================

    public void atacarFuerte()
    {
        if (estadoActual != ESTADO_IDLE)
        {
            return;
        }


        if (recibiendoDanio)
        {
            return;
        }


        if (!estaViva())
        {
            return;
        }


        estadoActual = ESTADO_FUERTE;

        frameActual = 1;

        contadorAnimacion = 0;
    }


    private void animarAtaqueFuerte()
    {
        contadorAnimacion++;


        if (contadorAnimacion < VELOCIDAD_FUERTE)
        {
            return;
        }


        contadorAnimacion = 0;


        GreenfootImage imagen;

        int cantidadFrames;


        // =============================================
        // AURORA
        // =============================================

        if (DatosJuego.esAurora())
        {
            imagen =
                FabricaImagenes.auroraAtaqueFuerte(
                    frameActual
                );


            cantidadFrames =
                AURORA_FUERTE;
        }


        // =============================================
        // SELENE
        // =============================================

        else
        {
            imagen =
                FabricaImagenes.seleneAtaqueFuerte(
                    frameActual
                );


            cantidadFrames =
                SELENE_FUERTE;
        }


        imagen.scale(
            280,
            280
        );


        setImage(
            imagen
        );


        frameActual++;


        if (frameActual > cantidadFrames)
        {
            volverIdle();
        }
    }


    // =================================================
    // AL RECIBIR DAÑO
    // =================================================

    protected void alRecibirDanio()
    {
        // =============================================
        // SI LA VIDA LLEGÓ A 0
        // =============================================

        if (!estaViva())
        {
            recibiendoDanio = false;

            estadoActual = ESTADO_IDLE;


            GreenfootImage imagen;


            if (DatosJuego.esAurora())
            {
                imagen =
                    FabricaImagenes.auroraDerrota();
            }

            else
            {
                imagen =
                    FabricaImagenes.seleneDerrota();
            }


            imagen.scale(
                280,
                280
            );


            setImage(
                imagen
            );


            return;
        }


        // =============================================
        // SI TODAVÍA ESTÁ VIVO
        // =============================================

        recibiendoDanio = true;

        contadorDanio =
            DURACION_DANIO;


        GreenfootImage imagen;


        if (DatosJuego.esAurora())
        {
            imagen =
                FabricaImagenes.auroraDanio();
        }

        else
        {
            imagen =
                FabricaImagenes.seleneDanio();
        }


        imagen.scale(
            280,
            280
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

            volverIdle();
        }
    }


    // =================================================
    // VOLVER A IDLE
    // =================================================

    private void volverIdle()
    {
        // Si murió, NO volver al Idle.
        if (!estaViva())
        {
            return;
        }


        estadoActual =
            ESTADO_IDLE;


        frameActual = 1;

        contadorAnimacion = 0;


        mostrarIdle();
    }


    // =================================================
    // SABER SI ESTÁ ATACANDO
    // =================================================

    public boolean estaAtacando()
    {
        return estadoActual != ESTADO_IDLE;
    }


    // =================================================
    // ATAQUE BASE
    // =================================================

    public int obtenerDanioBase()
    {
        return getAtaque();
    }
}
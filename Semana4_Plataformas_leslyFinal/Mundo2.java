import greenfoot.*;

public class Mundo2 extends NivelBase
{
    // =========================================
    // OBJETIVO
    // =========================================

    private int tesoros = 0;

    private final int OBJETIVO_TESOROS = 4;


    // =========================================
    // JEFE
    // =========================================

    private boolean jefeCreado = false;

    private boolean jefeDerrotado = false;


    // =========================================
    // POWER UP
    // =========================================

    private boolean powerUpActivado = false;


    // =========================================
    // META
    // =========================================

    private boolean metaCreada = false;


    // =========================================
    // ZONAS
    // =========================================

    private boolean zona2 = false;
    private boolean zona3 = false;
    private boolean zona4 = false;


    // =========================================
    // MENSAJES
    // =========================================

    private int contadorInicio = 120;

    private int contadorMensaje = 0;


    // =========================================
    // VICTORIA
    // =========================================

    private boolean completandoNivel = false;

    private int contadorVictoria = 0;


    // =========================================
    // CONSTRUCTOR
    // =========================================

    public Mundo2()
    {
        super(
            2,
            "Mundo 2.png",
            4
        );


        posicionarJugadorInicio();

        prepararZonaInicial();

        mostrarHUD();


        showText(
            "LEVEL 2",
            400,
            190
        );


        showText(
            "ENCUENTRA LOS 4 TESOROS",
            400,
            225
        );
    }


    // =========================================
    // POSICIÓN DE CHOPPER
    // =========================================

    private void posicionarJugadorInicio()
    {
        Jugador jugador =
            getJugador();


        if (jugador != null)
        {
            jugador.setLocation(
                100,
                350
            );
        }
    }


    // =========================================
    // ACT
    // =========================================

    public void act()
    {
        gestionarMensajes();


        if (completandoNivel)
        {
            contadorVictoria--;


            if (contadorVictoria <= 0)
            {
                siguienteNivel();
            }


            return;
        }


        generarElementos();

        revisarAparicionJefe();

        mostrarHUD();
    }


    // =========================================
    // INICIO
    // =========================================

    private void prepararZonaInicial()
    {
        // TESORO 1
        addObject(
            new Tesoro(),
            520,
            330
        );


        // TRAMPA
        addObject(
            new Trampa(),
            700,
            405
        );
    }


    // =========================================
    // GENERACIÓN DEL NIVEL
    // =========================================

    private void generarElementos()
    {
        int avance =
            getDesplazamiento();


        // =====================================
        // ZONA 2
        // =====================================

        if (
            avance >= 500
            &&
            !zona2
        )
        {
            addObject(
                new Villano(),
                680,
                350
            );


            // TESORO 2
            addObject(
                new Tesoro(),
                745,
                300
            );


            zona2 = true;
        }


        // =====================================
        // ZONA 3
        // =====================================

        if (
            avance >= 1050
            &&
            !zona3
        )
        {
            addObject(
                new Trampa(),
                620,
                405
            );


            addObject(
                new Villano(),
                745,
                350
            );


            // TESORO 3
            addObject(
                new Tesoro(),
                700,
                320
            );


            zona3 = true;
        }


        // =====================================
        // ZONA 4
        // =====================================

        if (
            avance >= 1650
            &&
            !zona4
        )
        {
            addObject(
                new Trampa(),
                740,
                405
            );


            // TESORO 4
            addObject(
                new Tesoro(),
                680,
                285
            );


            zona4 = true;
        }
    }


    // =========================================
    // TESOROS
    // =========================================

    public void recogerTesoro(
        Tesoro tesoro
    )
    {
        if (
            tesoro == null
            ||
            tesoro.getWorld() == null
        )
        {
            return;
        }


        removeObject(
            tesoro
        );


        tesoros++;


        mostrarHUD();


        // =====================================
        // RECOLECTÓ LOS CUATRO
        // =====================================

        if (
            tesoros >= OBJETIVO_TESOROS
        )
        {
            showText(
                "4/4 TESOROS!",
                400,
                100
            );


            showText(
                "PREPARATE...",
                400,
                130
            );


            contadorMensaje = 100;
        }
    }


    // =========================================
    // APARECER JEFE
    // =========================================

    private void revisarAparicionJefe()
    {
        /*
         * El jefe aparece solo cuando:
         *
         * 1. consiguió los 4 tesoros
         * 2. Chopper llegó suficientemente
         *    lejos en el nivel
         */

        if (
            tesoros >= OBJETIVO_TESOROS
            &&
            getDesplazamiento() >= 1800
            &&
            !jefeCreado
        )
        {
            jefeCreado = true;


            addObject(
                new JefeNivel2(),
                700,
                350
            );


            showText(
                "JEFE!",
                400,
                100
            );


            showText(
                "SALTA SOBRE SU CABEZA!",
                400,
                130
            );


            contadorMensaje = 150;
        }
    }


    // =========================================
    // DERROTAR JEFE
    // =========================================

    public void derrotarJefe(
        JefeNivel2 jefe
    )
    {
        if (jefeDerrotado)
        {
            return;
        }


        jefeDerrotado = true;


        if (
            jefe != null
            &&
            jefe.getWorld() != null
        )
        {
            removeObject(
                jefe
            );
        }


        // =====================================
        // POWER UP
        // =====================================

        powerUpActivado = true;


        Jugador jugador =
            getJugador();


        if (jugador != null)
        {
            jugador
                .activarPowerUpNivel2();
        }


        showText(
            "JEFE DERROTADO!",
            400,
            90
        );


        showText(
            "POWER UP: SUPER SALTO!",
            400,
            120
        );


        showText(
            "META DESBLOQUEADA!",
            400,
            150
        );


        contadorMensaje = 170;


        crearMeta();
    }


    // =========================================
    // CREAR META
    // =========================================

    private void crearMeta()
    {
        if (metaCreada)
        {
            return;
        }


        metaCreada = true;


        /*
         * Meta alta.
         *
         * El SUPER SALTO sirve
         * para alcanzarla.
         */

        addObject(
            new Meta2(),
            730,
            225
        );
    }


    // =========================================
    // TOCAR META
    // =========================================

    public void tocarMeta()
    {
        if (completandoNivel)
        {
            return;
        }


        if (
            jefeDerrotado
            &&
            powerUpActivado
        )
        {
            completandoNivel = true;


            contadorVictoria = 150;


            Jugador jugador =
                getJugador();


            if (jugador != null)
            {
                jugador
                    .bailarVictoria();
            }


            showText(
                "NIVEL 2 COMPLETADO!",
                400,
                190
            );


            showText(
                "VAMOS AL NIVEL 3!",
                400,
                225
            );
        }
    }


    // =========================================
    // MENSAJES
    // =========================================

    private void gestionarMensajes()
    {
        // LEVEL 2
        if (contadorInicio > 0)
        {
            contadorInicio--;


            if (contadorInicio == 0)
            {
                showText(
                    "",
                    400,
                    190
                );


                showText(
                    "",
                    400,
                    225
                );
            }
        }


        // MENSAJES GENERALES
        if (contadorMensaje > 0)
        {
            contadorMensaje--;


            if (contadorMensaje == 0)
            {
                showText(
                    "",
                    400,
                    90
                );

                showText(
                    "",
                    400,
                    100
                );

                showText(
                    "",
                    400,
                    120
                );

                showText(
                    "",
                    400,
                    130
                );

                showText(
                    "",
                    400,
                    150
                );
            }
        }
    }


    // =========================================
    // HUD
    // =========================================

    private void mostrarHUD()
    {
        // NIVEL
        showText(
            "NIVEL 2",
            60,
            25
        );


        // TESOROS
        showText(
            "TESOROS "
            +
            tesoros
            +
            "/"
            +
            OBJETIVO_TESOROS,
            195,
            25
        );


        // VIDAS
        Jugador jugador =
            getJugador();


        if (jugador != null)
        {
            showText(
                "VIDAS "
                +
                jugador.getVidas(),
                340,
                25
            );
        }


        // PROGRESO
        int progreso =
            (int)(
                (
                    getDesplazamiento()
                    /
                    2400.0
                )
                *
                100
            );


        if (progreso < 0)
        {
            progreso = 0;
        }


        if (progreso > 100)
        {
            progreso = 100;
        }


        showText(
            "PROGRESO "
            +
            progreso
            +
            "%",
            505,
            25
        );


        // ESTADO
        if (jefeDerrotado)
        {
            showText(
                "SUPER SALTO ACTIVO",
                400,
                55
            );
        }

        else if (jefeCreado)
        {
            showText(
                "DERROTA AL JEFE",
                400,
                55
            );
        }

        else if (
            tesoros >=
            OBJETIVO_TESOROS
        )
        {
            showText(
                "JEFE PROXIMO",
                400,
                55
            );
        }

        else
        {
            showText(
                "BUSCA LOS TESOROS",
                400,
                55
            );
        }
    }


    // =========================================
    // JUGADOR
    // =========================================

    private Jugador getJugador()
    {
        java.util.List<Jugador> jugadores =
            getObjects(
                Jugador.class
            );


        if (jugadores.isEmpty())
        {
            return null;
        }


        return jugadores.get(0);
    }


    // =========================================
    // NIVEL 3
    // =========================================

    public void siguienteNivel()
    {
        Greenfoot.setWorld(
            new Mundo3()
        );
    }
}
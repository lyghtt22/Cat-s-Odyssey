import greenfoot.*;

public class Mundo1 extends NivelBase
{
    // =========================================
    // OBJETIVO
    // =========================================

    private int algodones = 0;

    private final int OBJETIVO_ALGODONES =
        5;

    private boolean powerUpActivado =
        false;


    // =========================================
    // ZONAS
    // =========================================

    private boolean zona2 =
        false;

    private boolean zona3 =
        false;

    private boolean zona4 =
        false;

    private boolean zona5 =
        false;

    private boolean metaCreada =
        false;


    // =========================================
    // MENSAJES
    // =========================================

    private int contadorInicio =
        120;

    private int contadorPowerUp =
        0;

    private int contadorMensajeMeta =
        0;


    // =========================================
    // VICTORIA
    // =========================================

    private boolean completandoNivel =
        false;

    private int contadorVictoria =
        0;


    // =========================================
    // CONSTRUCTOR
    // =========================================

    public Mundo1()
    {
        super(
            1,
            "Mundo 1.png",
            3
        );


        prepararZonaInicial();


        mostrarHUD();


        showText(
            "LEVEL 1",
            400,
            210
        );


        showText(
            "RECOLECTA LOS 5 ALGODONES",
            400,
            245
        );
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


            if (
                contadorVictoria <= 0
            )
            {
                siguienteNivel();
            }


            return;
        }


        generarElementosSegunAvance();


        mostrarHUD();
    }


    // =========================================
    // INICIO
    // =========================================

    private void prepararZonaInicial()
    {
        addObject(
            new Algodon(),
            520,
            345
        );


        /*
         * Pinchos más abajo
         * para que queden sobre el suelo.
         */

        addObject(
            new Trampa(),
            700,
            455
        );
    }


    // =========================================
    // GENERACIÓN
    // =========================================

    private void generarElementosSegunAvance()
    {
        int avance =
            getDesplazamiento();


        // ZONA 2
        if (
            avance >= 300
            &&
            !zona2
        )
        {
            addObject(
                new Algodon(),
                730,
                315
            );


            zona2 =
                true;
        }


        // ZONA 3
        if (
            avance >= 600
            &&
            !zona3
        )
        {
            addObject(
                new Trampa(),
                650,
                455
            );


            addObject(
                new Algodon(),
                750,
                345
            );


            /*
             * Villano un poco elevado
             * y lejos del algodón.
             */

            addObject(
                new Villano(),
                735,
                390
            );


            zona3 =
                true;
        }


        // ZONA 4
        if (
            avance >= 950
            &&
            !zona4
        )
        {
            addObject(
                new Algodon(),
                710,
                285
            );


            addObject(
                new Trampa(),
                765,
                455
            );


            zona4 =
                true;
        }


        // ZONA 5
        if (
            avance >= 1250
            &&
            !zona5
        )
        {
            addObject(
                new Algodon(),
                720,
                330
            );


            addObject(
                new Villano(),
                680,
                390
            );


            addObject(
                new Trampa(),
                770,
                455
            );


            zona5 =
                true;
        }


        // =====================================
        // BANDERA
        // =====================================

        if (
            avance >= 1450
            &&
            !metaCreada
        )
        {
            /*
             * Meta elevada.
             *
             * El salto normal tiene una
             * altura aproximada de 72 px.
             *
             * Con doble salto podrá llegar
             * mucho más arriba.
             */

            addObject(
                new Meta(),
                735,
                225
            );


            metaCreada =
                true;
        }
    }


    // =========================================
    // ALGODÓN
    // =========================================

    public void recogerAlgodon(
        Algodon algodon
    )
    {
        if (
            algodon != null
            &&
            algodon.getWorld()
            !=
            null
        )
        {
            removeObject(
                algodon
            );


            algodones++;


            mostrarHUD();


            if (
                algodones
                >=
                OBJETIVO_ALGODONES

                &&

                !powerUpActivado
            )
            {
                powerUpActivado =
                    true;


                Jugador jugador =
                    getJugador();


                if (
                    jugador != null
                )
                {
                    jugador
                        .activarPowerUp();
                }


                showText(
                    "POWER UP!",
                    400,
                    100
                );


                showText(
                    "DOBLE SALTO DESBLOQUEADO!",
                    400,
                    130
                );


                contadorPowerUp =
                    150;
            }
        }
    }


    // =========================================
    // META
    // =========================================

    public void tocarMeta()
    {
        if (
            completandoNivel
        )
        {
            return;
        }


        // META DESBLOQUEADA
        if (
            powerUpActivado
        )
        {
            completandoNivel =
                true;


            contadorVictoria =
                150;


            Jugador jugador =
                getJugador();


            if (
                jugador != null
            )
            {
                jugador
                    .bailarVictoria();
            }


            showText(
                "NIVEL COMPLETADO!",
                400,
                180
            );


            showText(
                "CHOPPER ESTA FELIZ!",
                400,
                215
            );
        }


        // META BLOQUEADA
        else
        {
            showText(
                "META BLOQUEADA",
                400,
                100
            );


            showText(
                "FALTAN ALGODONES",
                400,
                130
            );


            contadorMensajeMeta =
                110;
        }
    }


    // =========================================
    // MENSAJES
    // =========================================

    private void gestionarMensajes()
    {
        // LEVEL 1
        if (
            contadorInicio > 0
        )
        {
            contadorInicio--;


            if (
                contadorInicio == 0
            )
            {
                showText(
                    "",
                    400,
                    210
                );


                showText(
                    "",
                    400,
                    245
                );
            }
        }


        // POWER UP
        if (
            contadorPowerUp > 0
        )
        {
            contadorPowerUp--;


            if (
                contadorPowerUp == 0
            )
            {
                showText(
                    "",
                    400,
                    100
                );


                showText(
                    "",
                    400,
                    130
                );
            }
        }


        // META
        if (
            contadorMensajeMeta > 0
        )
        {
            contadorMensajeMeta--;


            if (
                contadorMensajeMeta == 0
            )
            {
                showText(
                    "",
                    400,
                    100
                );


                showText(
                    "",
                    400,
                    130
                );
            }
        }
    }


    // =========================================
    // HUD
    // =========================================

    private void mostrarHUD()
    {
        showText(
            "NIVEL 1",
            60,
            25
        );


        showText(
            "ALGODONES "
            +
            algodones
            +
            "/"
            +
            OBJETIVO_ALGODONES,
            210,
            25
        );


        Jugador jugador =
            getJugador();


        if (
            jugador != null
        )
        {
            showText(
                "VIDAS "
                +
                jugador
                    .getVidas(),
                365,
                25
            );
        }


        int progreso =
            (int)(
                (
                    getDesplazamiento()
                    /
                    1600.0
                )
                *
                100
            );


        if (
            progreso > 100
        )
        {
            progreso =
                100;
        }


        showText(
            "PROGRESO "
            +
            progreso
            +
            "%",
            535,
            25
        );


        if (
            powerUpActivado
        )
        {
            showText(
                "META ACTIVA  |  DOBLE SALTO",
                400,
                55
            );
        }

        else
        {
            showText(
                "META BLOQUEADA",
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


        if (
            jugadores.isEmpty()
        )
        {
            return null;
        }


        return
            jugadores.get(0);
    }


    // =========================================
    // NIVEL 2
    // =========================================

    public void siguienteNivel()
    {
        Greenfoot.setWorld(
            new Mundo2()
        );
    }
}
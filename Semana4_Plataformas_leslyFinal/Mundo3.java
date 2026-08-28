import greenfoot.*;

public class Mundo3 extends NivelBase
{
    // =========================================
    // OBJETIVO
    // =========================================

    private int cristales = 0;

    private final int OBJETIVO_CRISTALES = 3;

    private boolean dobleSaltoActivo = false;


    // =========================================
    // ZONAS
    // =========================================

    private boolean zona2 = false;
    private boolean zona3 = false;
    private boolean zona4 = false;
    private boolean zona5 = false;


    // =========================================
    // JEFE FINAL
    // =========================================

    private boolean jefeCreado = false;
    private boolean jefeDerrotado = false;


    // =========================================
    // META
    // =========================================

    private boolean metaCreada = false;


    // =========================================
    // MENSAJES
    // =========================================

    private int contadorInicio = 140;
    private int contadorMensaje = 0;


    // =========================================
    // FINAL
    // =========================================

    private boolean juegoTerminado = false;

    private int contadorFinal = 0;


    // =========================================
    // CONSTRUCTOR
    // =========================================

    public Mundo3()
    {
        super(
            3,
            "Mundo 3.png",
            5
        );


        posicionarJugadorInicio();

        prepararZonaInicial();

        mostrarHUD();


        showText(
            "LEVEL 3",
            400,
            190
        );


        showText(
            "NIVEL FINAL",
            400,
            225
        );


        showText(
            "RECOLECTA LOS 3 CRISTALES",
            400,
            260
        );
    }


    // =========================================
    // POSICIÓN INICIAL
    // =========================================

    private void posicionarJugadorInicio()
    {
        Jugador jugador =
            getJugador();


        if (jugador != null)
        {
            jugador.setLocation(
                100,
                375
            );
        }
    }


    // =========================================
    // ACT
    // =========================================

    public void act()
    {
        gestionarMensajes();


        if (juegoTerminado)
        {
            contadorFinal--;


            if (contadorFinal <= 0)
            {
                Greenfoot.stop();
            }


            return;
        }


        generarElementos();

        revisarAparicionJefe();

        mostrarHUD();
    }


    // =========================================
    // ZONA INICIAL
    // =========================================

    private void prepararZonaInicial()
    {
        // CRISTAL 1
        addObject(
            new Cristal(),
            520,
            320
        );


        // TRAMPA
        addObject(
            new Trampa(),
            700,
            425
        );
    }


    // =========================================
    // GENERAR ESCENARIO
    // =========================================

    private void generarElementos()
    {
        int avance =
            getDesplazamiento();


        // =====================================
        // ZONA 2
        // =====================================

        if (
            avance >= 550
            &&
            !zona2
        )
        {
            /*
             * Villano apoyado sobre
             * el suelo.
             */
            addObject(
                new Villano(),
                690,
                405
            );


            addObject(
                new Trampa(),
                760,
                425
            );


            zona2 = true;
        }


        // =====================================
        // ZONA 3
        // =====================================

        if (
            avance >= 1100
            &&
            !zona3
        )
        {
            // CRISTAL 2
            addObject(
                new Cristal(),
                720,
                285
            );


            /*
             * Villano corregido.
             */
            addObject(
                new Villano(),
                620,
                405
            );


            zona3 = true;
        }


        // =====================================
            // ZONA 4
        // =====================================

        if (
            avance >= 1750
            &&
            !zona4
        )
        {   
            addObject(
                new Trampa(),
                630,
                425
            );


            /*
             * Este estaba en y = 30.
             * Por eso aparecía en el cielo.
             */
            addObject(
                new Villano(),
                740,
                405
            );


            // CRISTAL 3
            addObject(
                new Cristal(),
                690,
                290
            );


            zona4 = true;
        }


        // =====================================
        // ZONA 5
        // =====================================

        if (
            avance >= 2300
            &&
            !zona5
        )
        {
            addObject(
                new Trampa(),
                650,
                425
            );


            addObject(
                new Trampa(),
                760,
                425
            );


            zona5 = true;
        }
    }

    // =========================================
    // RECOGER CRISTAL
    // =========================================

    public void recogerCristal(
        Cristal cristal
    )
    {
        if (
            cristal == null
            ||
            cristal.getWorld() == null
        )
        {
            return;
        }


        removeObject(
            cristal
        );


        cristales++;


        mostrarHUD();


        // =====================================
        // DESBLOQUEAR DOBLE SALTO
        // =====================================

        if (
            cristales >= OBJETIVO_CRISTALES
            &&
            !dobleSaltoActivo
        )
        {
            dobleSaltoActivo = true;


            Jugador jugador =
                getJugador();


            if (jugador != null)
            {
                /*
                 * Este método ya existe
                 * desde Nivel 1.
                 */
                jugador.activarPowerUp();
            }


            showText(
                "3/3 CRISTALES!",
                400,
                95
            );


            showText(
                "DOBLE SALTO DESBLOQUEADO!",
                400,
                125
            );


            showText(
                "PREPARATE PARA EL JEFE FINAL",
                400,
                155
            );


            contadorMensaje = 170;
        }
    }


    // =========================================
    // APARECER JEFE FINAL
    // =========================================

    private void revisarAparicionJefe()
    {
        if (
            cristales >= OBJETIVO_CRISTALES
            &&
            getDesplazamiento() >= 2600
            &&
            !jefeCreado
        )
        {
            jefeCreado = true;


            addObject(
                new JefeFinal(),
                700,
                375
            );


            showText(
                "JEFE FINAL!",
                400,
                100
            );


            showText(
                "SALTA SOBRE EL 2 VECES",
                400,
                140
            );


            contadorMensaje = 170;
        }
    }


    // =========================================
    // JEFE DERROTADO
    // =========================================

    public void derrotarJefe(
        JefeFinal jefe
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


        showText(
            "JEFE FINAL DERROTADO!",
            400,
            95
        );


        showText(
            "META DESBLOQUEADA!",
            400,
            125
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
         * Bandera alta.
         * Usa el doble salto.
         */
        addObject(
            new Meta3(),
            730,
            225
        );
    }


    // =========================================
    // TOCAR META
    // =========================================

    public void tocarMeta()
    {
        if (juegoTerminado)
        {
            return;
        }


        if (!jefeDerrotado)
        {
            return;
        }


        juegoTerminado = true;

        contadorFinal = 220;


        Jugador jugador =
            getJugador();


        if (jugador != null)
        {
            jugador.bailarVictoria();
        }


        showText(
            "YOU WIN!",
            400,
            180
        );


        showText(
            "CHOPPER COMPLETO SU AVENTURA!",
            400,
            220
        );


        showText(
            "GRACIAS POR JUGAR",
            400,
            255
        );
    }


    // =========================================
    // MENSAJES
    // =========================================

    private void gestionarMensajes()
    {
        // LEVEL 3
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


                showText(
                    "",
                    400,
                    260
                );
            }
        }


        // MENSAJES DEL NIVEL
        if (contadorMensaje > 0)
        {
            contadorMensaje--;


            if (contadorMensaje == 0)
            {
                showText(
                    "",
                    400,
                    95
                );

                showText(
                    "",
                    400,
                    100
                );

                showText(
                    "",
                    400,
                    125
                );

                showText(
                    "",
                    400,
                    130
                );

                showText(
                    "",
                    400,
                    155
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
            "NIVEL 3",
            60,
            25
        );


        showText(
            "CRISTALES "
            + cristales
            + "/"
            + OBJETIVO_CRISTALES,
            205,
            25
        );


        Jugador jugador =
            getJugador();


        if (jugador != null)
        {
            showText(
                "VIDAS "
                + jugador.getVidas(),
                355,
                25
            );
        }


        /*
         * Mundo 3:
         *
         * 5 x 800 = 4000
         *
         * desplazamiento máximo:
         *
         * 4000 - 800 = 3200
         */

        int progreso =
            (int)(
                (
                    getDesplazamiento()
                    /
                    3200.0
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
            + progreso
            + "%",
            520,
            25
        );


        // =====================================
        // OBJETIVO ACTUAL
        // =====================================

        if (jefeDerrotado)
        {
            showText(
                "META DESBLOQUEADA",
                400,
                55
            );
        }

        else if (jefeCreado)
        {
            showText(
                "JEFE FINAL - SALTA SOBRE EL",
                400,
                55
            );
        }

        else if (dobleSaltoActivo)
        {
            showText(
                "DOBLE SALTO ACTIVO - BUSCA AL JEFE",
                400,
                55
            );
        }

        else
        {
            showText(
                "RECOLECTA LOS CRISTALES",
                400,
                55
            );
        }
    }


    // =========================================
    // BUSCAR JUGADOR
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
    // FINAL
    // =========================================

    public void siguienteNivel()
    {
        /*
         * Mundo 3 es el último.
         */
        Greenfoot.stop();
    }
}
import greenfoot.*;

public class GestorCombate
{
    // ================================================
    // UNIDADES
    // ================================================

    private InfanteriaJugador jugador;
    private InfanteriaEnemiga enemigo;


    // ================================================
    // ACCIONES DEL JUGADOR
    // ================================================

    private AccionCombate ataqueRapido;
    private AccionCombate ataqueNormal;
    private AccionCombate ataqueFuerte;


    // ================================================
    // ACCIÓN PENDIENTE
    // ================================================

    private AccionCombate accionPendiente;


    // ================================================
    // ESTADO DEL COMBATE
    // ================================================

    private boolean turnoJugador = true;
    private boolean animando = false;
    private boolean combateTerminado = false;

    private int contadorEspera = 0;


    /*
     * FASES:
     *
     * 0 = esperando acción del jugador
     *
     * 1 = esperando que termine
     *     la animación del jugador
     *
     * 2 = esperando antes del
     *     turno de la Reina
     *
     * 3 = esperando después del
     *     ataque de la Reina
     *
     * 4 = combate terminado
     */
    private int fase = 0;


    // ================================================
    // MENSAJE DEL ÚLTIMO EVENTO
    // ================================================

    private String ultimoMensaje = "";


    // ================================================
    // CONSTRUCTOR
    // ================================================

    public GestorCombate(
        InfanteriaJugador jugador,
        InfanteriaEnemiga enemigo
    )
    {
        this.jugador = jugador;
        this.enemigo = enemigo;

        crearAcciones();
    }


    // ================================================
    // CREAR ATAQUES
    // ================================================

    private void crearAcciones()
    {
        ataqueRapido =
            new AccionCombate(
                "Ataque rapido",
                8,
                90
            );


        ataqueNormal =
            new AccionCombate(
                "Ataque normal",
                12,
                75
            );


        ataqueFuerte =
            new AccionCombate(
                "Ataque fuerte",
                20,
                50
            );
    }


    // ================================================
    // ACTUALIZAR COMBATE
    // ================================================

    public void actualizar()
    {
        if (combateTerminado)
        {
            return;
        }


        if (!animando)
        {
            return;
        }


        // ============================================
        // FASE 1
        // ESPERAR ANIMACIÓN DEL JUGADOR
        // ============================================

        if (fase == 1)
        {
            /*
             * Mientras el personaje siga atacando,
             * esperamos.
             */
            if (jugador.estaAtacando())
            {
                return;
            }


            /*
             * Cuando termina la animación,
             * recién resolvemos daño o fallo.
             */
            resolverAtaqueJugador();

            return;
        }


        // ============================================
        // PAUSAS
        // ============================================

        contadorEspera--;


        if (contadorEspera > 0)
        {
            return;
        }


        // ============================================
        // FASE 2
        // TURNO DE LA REINA
        // ============================================

        if (fase == 2)
        {
            if (!enemigo.estaViva())
            {
                terminarCombate();

                return;
            }


            ejecutarTurnoEnemigo();

            return;
        }


        // ============================================
        // FASE 3
        // VOLVER AL JUGADOR
        // ============================================

        if (fase == 3)
        {
            if (!jugador.estaViva())
            {
                terminarCombate();

                return;
            }


            turnoJugador = true;
            animando = false;
            fase = 0;


            ultimoMensaje =
                "Turno de "
                + jugador.getNombre();
        }
    }


    // ================================================
    // ATAQUE RÁPIDO
    // ================================================

    public void usarAtaqueRapido()
    {
        iniciarAccionJugador(
            ataqueRapido,
            "rapido"
        );
    }


    // ================================================
    // ATAQUE NORMAL
    // ================================================

    public void usarAtaqueNormal()
    {
        iniciarAccionJugador(
            ataqueNormal,
            "normal"
        );
    }


    // ================================================
    // ATAQUE FUERTE
    // ================================================

    public void usarAtaqueFuerte()
    {
        iniciarAccionJugador(
            ataqueFuerte,
            "fuerte"
        );
    }


    // ================================================
    // INICIAR ACCIÓN DEL JUGADOR
    // ================================================

    private void iniciarAccionJugador(
        AccionCombate accion,
        String tipo
    )
    {
        if (combateTerminado)
        {
            return;
        }


        if (!turnoJugador)
        {
            return;
        }


        if (animando)
        {
            return;
        }


        // Guardamos el ataque que se está utilizando.
        accionPendiente = accion;


        animando = true;
        turnoJugador = false;


        // ============================================
        // REPRODUCIR ANIMACIÓN
        // ============================================

        if (tipo.equals("rapido"))
        {
            jugador.atacarRapido();
        }


        else if (tipo.equals("normal"))
        {
            jugador.atacarNormal();
        }


        else if (tipo.equals("fuerte"))
        {
            jugador.atacarFuerte();
        }


        // Esperamos que termine la animación.
        fase = 1;
    }


    // ================================================
    // RESOLVER ATAQUE DEL JUGADOR
    // ================================================

    private void resolverAtaqueJugador()
    {
        if (accionPendiente == null)
        {
            return;
        }


        // ============================================
        // ATAQUE ACIERTA
        // ============================================

        if (accionPendiente.acierta())
        {
            enemigo.recibirDanio(
                accionPendiente.getDanio()
            );


            mostrarTextoFlotante(
                enemigo,
                "-"
                + accionPendiente.getDanio(),
                true
            );


            ultimoMensaje =
                jugador.getNombre()
                + " usa "
                + accionPendiente.getNombre()
                + " y causa "
                + accionPendiente.getDanio()
                + " de daño";
        }


        // ============================================
        // ATAQUE FALLA
        // ============================================

        else
        {
            mostrarTextoFlotante(
                enemigo,
                "FALLO",
                false
            );


            ultimoMensaje =
                jugador.getNombre()
                + " usa "
                + accionPendiente.getNombre()
                + " - FALLO";
        }


        accionPendiente = null;


        // Tiempo para leer el mensaje.
        contadorEspera = 200;

        fase = 2;
    }


    // ================================================
    // TURNO DE LA REINA
    // ================================================

    private void ejecutarTurnoEnemigo()
    {
        if (!enemigo.estaViva())
        {
            terminarCombate();

            return;
        }


        int numero =
            Greenfoot.getRandomNumber(100);


        // ============================================
        // REINA ACIERTA
        // ============================================

        if (numero < 80)
        {
            int danio =
                enemigo.getAtaque();


            jugador.recibirDanio(
                danio
            );


            mostrarTextoFlotante(
                jugador,
                "-"
                + danio,
                true
            );


            ultimoMensaje =
                enemigo.getNombre()
                + " causa "
                + danio
                + " de daño";
        }


        // ============================================
        // REINA FALLA
        // ============================================

        else
        {
            mostrarTextoFlotante(
                jugador,
                "FALLO",
                false
            );


            ultimoMensaje =
                enemigo.getNombre()
                + " - FALLO";
        }


        contadorEspera = 200;

        fase = 3;
    }


    // ================================================
    // MOSTRAR TEXTO FLOTANTE
    // ================================================

    private void mostrarTextoFlotante(
        Unidad objetivo,
        String texto,
        boolean esDanio
    )
    {
        if (objetivo == null)
        {
            return;
        }


        World mundo =
            objetivo.getWorld();


        if (mundo == null)
        {
            return;
        }


        TextoFlotante textoFlotante =
            new TextoFlotante(
                texto,
                esDanio
            );


        mundo.addObject(
            textoFlotante,
            objetivo.getX(),
            objetivo.getY() - 120
        );
    }


    // ================================================
    // TERMINAR COMBATE
    // ================================================

    private void terminarCombate()
    {
        combateTerminado = true;

        animando = false;

        turnoJugador = false;

        fase = 4;


        if (jugador.estaViva())
        {
            ultimoMensaje =
                "¡VICTORIA!";
        }
        else
        {
            ultimoMensaje =
                "DERROTA";
        }
    }


    // ================================================
    // GETTERS DE ESTADO
    // ================================================

    public boolean esTurnoJugador()
    {
        return turnoJugador;
    }


    public boolean estaAnimando()
    {
        return animando;
    }


    public boolean combateTerminado()
    {
        return combateTerminado;
    }


    public String getUltimoMensaje()
    {
        return ultimoMensaje;
    }


    // ================================================
    // GETTERS DE ACCIONES
    // ================================================

    public AccionCombate getAtaqueRapido()
    {
        return ataqueRapido;
    }


    public AccionCombate getAtaqueNormal()
    {
        return ataqueNormal;
    }


    public AccionCombate getAtaqueFuerte()
    {
        return ataqueFuerte;
    }
}
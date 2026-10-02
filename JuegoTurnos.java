import greenfoot.*;

public class JuegoTurnos extends World
{
    // =================================================
    // PERSONAJES
    // =================================================

    private InfanteriaJugador jugador;
    private InfanteriaEnemiga enemiga;


    // =================================================
    // GESTOR DE COMBATE
    // =================================================

    private GestorCombate gestorCombate;


    // =================================================
    // BARRAS DE VIDA
    // =================================================

    private BarraHP barraJugador;
    private BarraHP barraEnemiga;


    // =================================================
    // RETRATOS
    // =================================================

    private Actor retratoJugador;
    private Actor retratoEnemiga;


    // =================================================
    // NOMBRES
    // =================================================

    private Actor nombreJugador;
    private Actor nombreEnemiga;


    // =================================================
    // BOTONES DE ACCIÓN
    // =================================================

    private BotonAccion botonRapido;
    private BotonAccion botonNormal;
    private BotonAccion botonFuerte;


    // =================================================
    // BOTONES DE FIN DE COMBATE
    // =================================================

    private BotonMenu botonReintentar;
    private BotonMenu botonVolverMenu;

    private boolean botonesFinalMostrados = false;


    // =================================================
    // MENSAJE DEL COMBATE
    // =================================================

    private Actor textoCombate;

    private String mensajeAnterior = "";


    // =================================================
    // CONSTRUCTOR
    // =================================================

    public JuegoTurnos()
    {
        super(
            900,
            600,
            1
        );


        prepararFondo();

        prepararPersonajes();

        prepararGestorCombate();

        prepararBarras();

        prepararRetratos();

        prepararNombres();

        prepararTextoCombate();

        prepararBotones();
    }


    // =================================================
    // ACT
    // =================================================

    public void act()
    {
        gestorCombate.actualizar();

        actualizarTextoCombate();

        revisarFinCombate();
    }


    // =================================================
    // FONDO
    // =================================================

    private void prepararFondo()
    {
        GreenfootImage fondo =
            new GreenfootImage(
                "FondoCombate.png"
            );


        fondo.scale(
            getWidth(),
            getHeight()
        );


        setBackground(
            fondo
        );
    }


    // =================================================
    // PERSONAJES
    // =================================================

    private void prepararPersonajes()
    {
        jugador =
            new InfanteriaJugador();


        enemiga =
            new InfanteriaEnemiga();


        addObject(
            jugador,
            280,
            390
        );


        addObject(
            enemiga,
            670,
            390
        );
    }


    // =================================================
    // GESTOR DE COMBATE
    // =================================================

    private void prepararGestorCombate()
    {
        gestorCombate =
            new GestorCombate(
                jugador,
                enemiga
            );
    }


    // =================================================
    // BARRAS DE VIDA
    // =================================================

    private void prepararBarras()
    {
        barraJugador =
            new BarraHP(
                jugador,
                true
            );


        barraEnemiga =
            new BarraHP(
                enemiga,
                false
            );


        int yBarra = 70;


        addObject(
            barraJugador,
            235,
            yBarra
        );


        addObject(
            barraEnemiga,
            665,
            yBarra
        );
    }


    // =================================================
    // RETRATOS
    // =================================================

    private void prepararRetratos()
    {
        String rutaJugador;


        String personaje =
            DatosJuego.getPersonajeSeleccionado();


        if (
            personaje.equalsIgnoreCase(
                "Aurora"
            )
        )
        {
            rutaJugador =
                "retratos/Aurora_retrato.png";
        }
        else
        {
            rutaJugador =
                "retratos/Selene_retrato.png";
        }


        retratoJugador =
            crearRetrato(
                rutaJugador
            );


        retratoEnemiga =
            crearRetrato(
                "retratos/ReinaMalvada_retrato.png"
            );


        addObject(
            retratoJugador,
            70,
            70
        );


        addObject(
            retratoEnemiga,
            830,
            70
        );
    }


    // =================================================
    // CREAR RETRATO
    // =================================================

    private Actor crearRetrato(
        String ruta
    )
    {
        Actor retrato =
            new Actor()
            {
            };


        GreenfootImage imagen =
            new GreenfootImage(
                ruta
            );


        imagen.scale(
            75,
            75
        );


        retrato.setImage(
            imagen
        );


        return retrato;
    }


    // =================================================
    // NOMBRES
    // =================================================

    private void prepararNombres()
    {
        String personaje =
            DatosJuego.getPersonajeSeleccionado();


        nombreJugador =
            crearNombre(
                personaje.toUpperCase()
            );


        nombreEnemiga =
            crearNombre(
                "REINA MALVADA"
            );


        addObject(
            nombreJugador,
            235,
            28
        );


        addObject(
            nombreEnemiga,
            665,
            28
        );
    }


    // =================================================
    // CREAR NOMBRE
    // =================================================

    private Actor crearNombre(
        String texto
    )
    {
        Actor nombre =
            new Actor()
            {
            };


        GreenfootImage sombra =
            new GreenfootImage(
                texto,
                22,
                new Color(
                    70,
                    0,
                    30
                ),
                new Color(
                    0,
                    0,
                    0,
                    0
                )
            );


        GreenfootImage textoPrincipal =
            new GreenfootImage(
                texto,
                22,
                new Color(
                    255,
                    215,
                    235
                ),
                new Color(
                    0,
                    0,
                    0,
                    0
                )
            );


        int ancho =
            Math.max(
                sombra.getWidth(),
                textoPrincipal.getWidth()
            )
            +
            10;


        int alto =
            Math.max(
                sombra.getHeight(),
                textoPrincipal.getHeight()
            )
            +
            10;


        GreenfootImage imagenFinal =
            new GreenfootImage(
                ancho,
                alto
            );


        imagenFinal.setColor(
            new Color(
                0,
                0,
                0,
                0
            )
        );


        imagenFinal.fill();


        imagenFinal.drawImage(
            sombra,
            7,
            7
        );


        imagenFinal.drawImage(
            textoPrincipal,
            5,
            5
        );


        nombre.setImage(
            imagenFinal
        );


        return nombre;
    }


    // =================================================
    // TEXTO DEL COMBATE
    // =================================================

    private void prepararTextoCombate()
    {
        textoCombate =
            new Actor()
            {
            };


        actualizarImagenTexto(
            "Turno de "
            + jugador.getNombre()
        );


        addObject(
            textoCombate,
            450,
            485
        );
    }


    // =================================================
    // ACTUALIZAR TEXTO DEL COMBATE
    // =================================================

    private void actualizarTextoCombate()
    {
        String mensaje =
            gestorCombate.getUltimoMensaje();


        if (
            mensaje == null
            ||
            mensaje.equals("")
        )
        {
            return;
        }


        if (
            mensaje.equals(
                mensajeAnterior
            )
        )
        {
            return;
        }


        mensajeAnterior =
            mensaje;


        actualizarImagenTexto(
            mensaje
        );
    }


    // =================================================
    // CREAR IMAGEN DEL TEXTO
    // =================================================

    private void actualizarImagenTexto(
        String texto
    )
    {
        GreenfootImage imagen =
            new GreenfootImage(
                texto,
                24,
                new Color(
                    255,
                    230,
                    240
                ),
                new Color(
                    80,
                    0,
                    40,
                    180
                )
            );


        textoCombate.setImage(
            imagen
        );
    }


    // =================================================
    // BOTONES DE ATAQUE
    // =================================================

    private void prepararBotones()
    {
        botonRapido =
            new BotonAccion(
                "rapido",
                gestorCombate
            );


        botonNormal =
            new BotonAccion(
                "normal",
                gestorCombate
            );


        botonFuerte =
            new BotonAccion(
                "fuerte",
                gestorCombate
            );


        botonRapido.setImage(
            crearImagenBoton(
                "RAPIDO",
                "8 DMG - 90%"
            )
        );


        botonNormal.setImage(
            crearImagenBoton(
                "NORMAL",
                "12 DMG - 75%"
            )
        );


        botonFuerte.setImage(
            crearImagenBoton(
                "FUERTE",
                "20 DMG - 50%"
            )
        );


        addObject(
            botonRapido,
            220,
            550
        );


        addObject(
            botonNormal,
            450,
            550
        );


        addObject(
            botonFuerte,
            680,
            550
        );
    }


    // =================================================
    // REVISAR FIN DEL COMBATE
    // =================================================

    private void revisarFinCombate()
    {
        if (!gestorCombate.combateTerminado())
        {
            return;
        }


        if (botonesFinalMostrados)
        {
            return;
        }


        botonesFinalMostrados = true;


        // =============================================
        // QUITAR BOTONES DE ATAQUE
        // =============================================

        if (botonRapido.getWorld() != null)
        {
            removeObject(
                botonRapido
            );
        }


        if (botonNormal.getWorld() != null)
        {
            removeObject(
                botonNormal
            );
        }


        if (botonFuerte.getWorld() != null)
        {
            removeObject(
                botonFuerte
            );
        }


        // =============================================
        // BOTÓN JUGAR DE NUEVO
        // =============================================

        botonReintentar =
            new BotonMenu(
                "Boton_JugarDeNuevo.png",
                "REINTENTAR"
            );


        addObject(
            botonReintentar,
            450,
            470
        );


        // =============================================
        // BOTÓN VOLVER AL MENÚ
        // =============================================

        botonVolverMenu =
            new BotonMenu(
                "Boton_VolverMenu.png",
                "MENU"
            );


        addObject(
            botonVolverMenu,
            450,
            540
        );
    }


    // =================================================
    // CREAR IMAGEN BOTÓN ATAQUE
    // =================================================

    private GreenfootImage crearImagenBoton(
        String titulo,
        String descripcion
    )
    {
        int ancho = 190;
        int alto = 55;


        GreenfootImage boton =
            new GreenfootImage(
                ancho,
                alto
            );


        boton.setColor(
            new Color(
                90,
                10,
                50,
                220
            )
        );

        boton.fill();


        boton.setColor(
            new Color(
                255,
                150,
                200
            )
        );

        boton.drawRect(
            0,
            0,
            ancho - 1,
            alto - 1
        );


        GreenfootImage textoTitulo =
            new GreenfootImage(
                titulo,
                20,
                Color.WHITE,
                new Color(
                    0,
                    0,
                    0,
                    0
                )
            );


        GreenfootImage textoDescripcion =
            new GreenfootImage(
                descripcion,
                14,
                new Color(
                    255,
                    190,
                    220
                ),
                new Color(
                    0,
                    0,
                    0,
                    0
                )
            );


        int xTitulo =
            (
                ancho
                -
                textoTitulo.getWidth()
            )
            /
            2;


        int xDescripcion =
            (
                ancho
                -
                textoDescripcion.getWidth()
            )
            /
            2;


        boton.drawImage(
            textoTitulo,
            xTitulo,
            5
        );


        boton.drawImage(
            textoDescripcion,
            xDescripcion,
            31
        );


        return boton;
    }


    // =================================================
    // GET JUGADOR
    // =================================================

    public InfanteriaJugador getJugador()
    {
        return jugador;
    }


    // =================================================
    // GET ENEMIGA
    // =================================================

    public InfanteriaEnemiga getEnemiga()
    {
        return enemiga;
    }


    // =================================================
    // GET GESTOR
    // =================================================

    public GestorCombate getGestorCombate()
    {
        return gestorCombate;
    }
}
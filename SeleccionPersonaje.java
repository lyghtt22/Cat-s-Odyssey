import greenfoot.*;

public class SeleccionPersonaje extends World
{
    // =================================================
    // PERSONAJES
    // =================================================

    private Actor aurora;
    private Actor selene;


    // =================================================
    // NOMBRES
    // =================================================

    private Actor nombreAurora;
    private Actor nombreSelene;


    // =================================================
    // IMÁGENES PERSONAJES
    // =================================================

    private GreenfootImage auroraNormal;
    private GreenfootImage auroraHover;

    private GreenfootImage seleneNormal;
    private GreenfootImage seleneHover;


    // =================================================
    // IMÁGENES NOMBRES
    // =================================================

    private GreenfootImage nombreAuroraNormal;
    private GreenfootImage nombreAuroraHover;

    private GreenfootImage nombreSeleneNormal;
    private GreenfootImage nombreSeleneHover;


    // =================================================
    // MARGEN PARA EL RESPLANDOR
    // =================================================

    private static final int MARGEN_GLOW = 18;


    // =================================================
    // CONSTRUCTOR
    // =================================================

    public SeleccionPersonaje()
    {
        super(
            900,
            600,
            1
        );

        prepararFondo();

        prepararTextos();

        prepararPersonajes();
    }


    // =================================================
    // ACT
    // =================================================

    public void act()
    {
        revisarMouse();

        revisarSeleccion();
    }


    // =================================================
    // FONDO
    // =================================================

    private void prepararFondo()
    {
        GreenfootImage fondo =
            new GreenfootImage(
                "FondoSeleccion.png"
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
    // TEXTOS
    // =================================================

    private void prepararTextos()
    {
        // =============================================
        // TÍTULO
        // =============================================

        agregarImagenUI(
            "menu/Titulo_ElegirPersonaje.png",
            450,
            70,
            450
        );


        // =============================================
        // NOMBRE AURORA
        // =============================================

        nombreAurora =
            new Actor()
            {
            };


        GreenfootImage imagenAurora =
            cargarImagenEscalada(
                "menu/Nombre_Aurora.png",
                190
            );


        nombreAuroraNormal =
            crearImagenNormal(
                imagenAurora
            );


        nombreAuroraHover =
            crearImagenConGlow(
                imagenAurora
            );


        nombreAurora.setImage(
            nombreAuroraNormal
        );


        addObject(
            nombreAurora,
            260,
            145
        );


        // =============================================
        // NOMBRE SELENE
        // =============================================

        nombreSelene =
            new Actor()
            {
            };


        GreenfootImage imagenSelene =
            cargarImagenEscalada(
                "menu/Nombre_Selene.png",
                190
            );


        nombreSeleneNormal =
            crearImagenNormal(
                imagenSelene
            );


        nombreSeleneHover =
            crearImagenConGlow(
                imagenSelene
            );


        nombreSelene.setImage(
            nombreSeleneNormal
        );


        addObject(
            nombreSelene,
            640,
            145
        );


        // =============================================
        // TEXTO INFERIOR
        // =============================================

        agregarImagenUI(
            "menu/Texto_HazClick.png",
            450,
            545,
            260
        );
    }


    // =================================================
    // PERSONAJES
    // =================================================

    private void prepararPersonajes()
    {
        aurora =
            new Actor()
            {
            };


        selene =
            new Actor()
            {
            };


        // =============================================
        // AURORA
        // =============================================

        GreenfootImage imagenAurora =
            new GreenfootImage(
                "sprites/Aurora/Aurora_Idle_1.png"
            );


        imagenAurora.scale(
            260,
            260
        );


        auroraNormal =
            crearImagenNormal(
                imagenAurora
            );


        auroraHover =
            crearImagenConGlow(
                imagenAurora
            );


        aurora.setImage(
            auroraNormal
        );


        // =============================================
        // SELENE
        // =============================================

        GreenfootImage imagenSelene =
            new GreenfootImage(
                "sprites/Selene/Selene_Base_1.png"
            );


        imagenSelene.scale(
            260,
            260
        );


        seleneNormal =
            crearImagenNormal(
                imagenSelene
            );


        seleneHover =
            crearImagenConGlow(
                imagenSelene
            );


        selene.setImage(
            seleneNormal
        );


        // =============================================
        // POSICIONES
        // =============================================

        addObject(
            aurora,
            260,
            335
        );


        addObject(
            selene,
            640,
            335
        );
    }


    // =================================================
    // REVISAR MOUSE
    // =================================================

    private void revisarMouse()
    {
        MouseInfo mouse =
            Greenfoot.getMouseInfo();


        // Si el mouse está fuera
        // del mundo, dejamos todo normal.
        if (mouse == null)
        {
            ponerAuroraNormal();

            ponerSeleneNormal();

            return;
        }


        Actor actorMouse =
            mouse.getActor();


        // =============================================
        // AURORA
        // =============================================

        boolean sobreAurora =
            actorMouse == aurora
            ||
            actorMouse == nombreAurora;


        if (sobreAurora)
        {
            ponerAuroraHover();
        }
        else
        {
            ponerAuroraNormal();
        }


        // =============================================
        // SELENE
        // =============================================

        boolean sobreSelene =
            actorMouse == selene
            ||
            actorMouse == nombreSelene;


        if (sobreSelene)
        {
            ponerSeleneHover();
        }
        else
        {
            ponerSeleneNormal();
        }
    }


    // =================================================
    // AURORA NORMAL
    // =================================================

    private void ponerAuroraNormal()
    {
        aurora.setImage(
            auroraNormal
        );


        nombreAurora.setImage(
            nombreAuroraNormal
        );
    }


    // =================================================
    // AURORA HOVER
    // =================================================

    private void ponerAuroraHover()
    {
        aurora.setImage(
            auroraHover
        );


        nombreAurora.setImage(
            nombreAuroraHover
        );
    }


    // =================================================
    // SELENE NORMAL
    // =================================================

    private void ponerSeleneNormal()
    {
        selene.setImage(
            seleneNormal
        );


        nombreSelene.setImage(
            nombreSeleneNormal
        );
    }


    // =================================================
    // SELENE HOVER
    // =================================================

    private void ponerSeleneHover()
    {
        selene.setImage(
            seleneHover
        );


        nombreSelene.setImage(
            nombreSeleneHover
        );
    }


    // =================================================
    // SELECCIÓN
    // =================================================

    private void revisarSeleccion()
    {
        // =============================================
        // AURORA
        // =============================================

        if (
            Greenfoot.mouseClicked(
                aurora
            )
            ||
            Greenfoot.mouseClicked(
                nombreAurora
            )
        )
        {
            seleccionarAurora();

            return;
        }


        // =============================================
        // SELENE
        // =============================================

        if (
            Greenfoot.mouseClicked(
                selene
            )
            ||
            Greenfoot.mouseClicked(
                nombreSelene
            )
        )
        {
            seleccionarSelene();
        }
    }


    // =================================================
    // SELECCIONAR AURORA
    // =================================================

    private void seleccionarAurora()
    {
        DatosJuego.seleccionarPersonaje(
            "Aurora"
        );


        DatosJuego.iniciarPartida();


        Greenfoot.setWorld(
            new JuegoTurnos()
        );
    }


    // =================================================
    // SELECCIONAR SELENE
    // =================================================

    private void seleccionarSelene()
    {
        DatosJuego.seleccionarPersonaje(
            "Selene"
        );


        DatosJuego.iniciarPartida();


        Greenfoot.setWorld(
            new JuegoTurnos()
        );
    }


    // =================================================
    // CARGAR IMAGEN ESCALADA
    // =================================================

    private GreenfootImage cargarImagenEscalada(
        String ruta,
        int ancho
    )
    {
        GreenfootImage imagen =
            new GreenfootImage(
                ruta
            );


        double proporcion =
            (double) imagen.getHeight()
            /
            imagen.getWidth();


        int alto =
            (int)(
                ancho
                *
                proporcion
            );


        imagen.scale(
            ancho,
            alto
        );


        return imagen;
    }


    // =================================================
    // IMAGEN NORMAL
    // =================================================

    private GreenfootImage crearImagenNormal(
        GreenfootImage original
    )
    {
        int ancho =
            original.getWidth()
            +
            MARGEN_GLOW * 2;


        int alto =
            original.getHeight()
            +
            MARGEN_GLOW * 2;


        GreenfootImage resultado =
            crearLienzoTransparente(
                ancho,
                alto
            );


        resultado.drawImage(
            original,
            MARGEN_GLOW,
            MARGEN_GLOW
        );


        return resultado;
    }


    // =================================================
    // IMAGEN CON GLOW
    // =================================================

    private GreenfootImage crearImagenConGlow(
        GreenfootImage original
    )
    {
        int ancho =
            original.getWidth()
            +
            MARGEN_GLOW * 2;


        int alto =
            original.getHeight()
            +
            MARGEN_GLOW * 2;


        GreenfootImage resultado =
            crearLienzoTransparente(
                ancho,
                alto
            );


        // Copia utilizada como brillo
        GreenfootImage glow =
            new GreenfootImage(
                original
            );


        convertirEnRosa(
            glow
        );


        int x =
            MARGEN_GLOW;


        int y =
            MARGEN_GLOW;


        // =============================================
        // RESPLANDOR EXTERIOR
        // =============================================

        resultado.drawImage(
            glow,
            x - 8,
            y
        );

        resultado.drawImage(
            glow,
            x + 8,
            y
        );

        resultado.drawImage(
            glow,
            x,
            y - 8
        );

        resultado.drawImage(
            glow,
            x,
            y + 8
        );


        // =============================================
        // DIAGONALES
        // =============================================

        resultado.drawImage(
            glow,
            x - 6,
            y - 6
        );

        resultado.drawImage(
            glow,
            x + 6,
            y - 6
        );

        resultado.drawImage(
            glow,
            x - 6,
            y + 6
        );

        resultado.drawImage(
            glow,
            x + 6,
            y + 6
        );


        // =============================================
        // BRILLO CERCANO
        // =============================================

        resultado.drawImage(
            glow,
            x - 3,
            y
        );

        resultado.drawImage(
            glow,
            x + 3,
            y
        );

        resultado.drawImage(
            glow,
            x,
            y - 3
        );

        resultado.drawImage(
            glow,
            x,
            y + 3
        );


        // =============================================
        // ORIGINAL ENCIMA
        // =============================================

        resultado.drawImage(
            original,
            x,
            y
        );


        return resultado;
    }


    // =================================================
    // CONVERTIR A ROSA
    // =================================================

    private void convertirEnRosa(
        GreenfootImage imagen
    )
    {
        for (
            int x = 0;
            x < imagen.getWidth();
            x++
        )
        {
            for (
                int y = 0;
                y < imagen.getHeight();
                y++
            )
            {
                Color original =
                    imagen.getColorAt(
                        x,
                        y
                    );


                if (
                    original.getAlpha()
                    <= 20
                )
                {
                    continue;
                }


                Color rosa =
                    new Color(
                        255,
                        35,
                        135,
                        85
                    );


                imagen.setColorAt(
                    x,
                    y,
                    rosa
                );
            }
        }
    }


    // =================================================
    // LIENZO TRANSPARENTE
    // =================================================

    private GreenfootImage crearLienzoTransparente(
        int ancho,
        int alto
    )
    {
        GreenfootImage imagen =
            new GreenfootImage(
                ancho,
                alto
            );


        imagen.setColor(
            new Color(
                0,
                0,
                0,
                0
            )
        );


        imagen.fill();


        return imagen;
    }


    // =================================================
    // IMAGEN UI DECORATIVA
    // =================================================

    private void agregarImagenUI(
        String ruta,
        int x,
        int y,
        int ancho
    )
    {
        Actor imagenUI =
            new Actor()
            {
            };


        GreenfootImage imagen =
            cargarImagenEscalada(
                ruta,
                ancho
            );


        imagenUI.setImage(
            imagen
        );


        addObject(
            imagenUI,
            x,
            y
        );
    }
}
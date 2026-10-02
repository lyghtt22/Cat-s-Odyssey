import greenfoot.*;

public class BotonMenu extends Actor
{
    private String accion;

    private GreenfootImage imagenNormal;
    private GreenfootImage imagenHover;

    private static final int ANCHO_BOTON = 250;

    // Espacio adicional alrededor del botón
    // para que quepa el resplandor.
    private static final int MARGEN_GLOW = 18;


    public BotonMenu(
        String nombreImagen,
        String accion
    )
    {
        this.accion = accion;

        cargarImagenes(nombreImagen);

        setImage(imagenNormal);
    }


    public void act()
    {
        revisarMouse();
        revisarClick();
    }


    // =================================================
    // CARGAR IMÁGENES
    // =================================================

    private void cargarImagenes(
        String nombreArchivo
    )
    {
        GreenfootImage original =
            new GreenfootImage(
                "menu/" + nombreArchivo
            );


        // Mantener proporción original
        double proporcion =
            (double) original.getHeight()
            /
            original.getWidth();


        int altoBoton =
            (int)(
                ANCHO_BOTON
                *
                proporcion
            );


        // =============================================
        // BOTÓN BASE ESCALADO
        // =============================================

        GreenfootImage botonBase =
            new GreenfootImage(
                original
            );


        botonBase.scale(
            ANCHO_BOTON,
            altoBoton
        );


        // =============================================
        // TAMAÑO DEL LIENZO
        // =============================================

        int anchoLienzo =
            ANCHO_BOTON
            +
            MARGEN_GLOW * 2;


        int altoLienzo =
            altoBoton
            +
            MARGEN_GLOW * 2;


        // =============================================
        // IMAGEN NORMAL
        // =============================================

        imagenNormal =
            crearLienzoTransparente(
                anchoLienzo,
                altoLienzo
            );


        imagenNormal.drawImage(
            botonBase,
            MARGEN_GLOW,
            MARGEN_GLOW
        );


        // =============================================
        // IMAGEN HOVER
        // =============================================

        imagenHover =
            crearLienzoTransparente(
                anchoLienzo,
                altoLienzo
            );


        // Crear una copia rosa brillante
        GreenfootImage resplandor =
            new GreenfootImage(
                botonBase
            );


        convertirEnRosa(
            resplandor
        );


        // =============================================
        // SOMBRA / GLOW ROSADO
        // =============================================

        dibujarGlow(
            imagenHover,
            resplandor
        );


        // Dibujamos el botón real encima
        imagenHover.drawImage(
            botonBase,
            MARGEN_GLOW,
            MARGEN_GLOW
        );
    }


    // =================================================
    // CREAR LIENZO TRANSPARENTE
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
    // CONVERTIR COPIA EN ROSA
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


                // Ignorar píxeles transparentes
                if (
                    original.getAlpha()
                    <= 10
                )
                {
                    continue;
                }


                // Rosa brillante
                Color rosa =
                    new Color(
                        255,
                        40,
                        140,
                        100
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
    // CREAR RESPLANDOR ALREDEDOR
    // =================================================

    private void dibujarGlow(
        GreenfootImage destino,
        GreenfootImage glow
    )
    {
        int centroX =
            MARGEN_GLOW;

        int centroY =
            MARGEN_GLOW;


        // Glow exterior
        destino.drawImage(
            glow,
            centroX - 8,
            centroY
        );

        destino.drawImage(
            glow,
            centroX + 8,
            centroY
        );

        destino.drawImage(
            glow,
            centroX,
            centroY - 8
        );

        destino.drawImage(
            glow,
            centroX,
            centroY + 8
        );


        // Diagonales
        destino.drawImage(
            glow,
            centroX - 6,
            centroY - 6
        );

        destino.drawImage(
            glow,
            centroX + 6,
            centroY - 6
        );

        destino.drawImage(
            glow,
            centroX - 6,
            centroY + 6
        );

        destino.drawImage(
            glow,
            centroX + 6,
            centroY + 6
        );


        // Glow más cercano al botón
        destino.drawImage(
            glow,
            centroX - 3,
            centroY
        );

        destino.drawImage(
            glow,
            centroX + 3,
            centroY
        );

        destino.drawImage(
            glow,
            centroX,
            centroY - 3
        );

        destino.drawImage(
            glow,
            centroX,
            centroY + 3
        );
    }


    // =================================================
    // MOUSE
    // =================================================

    private void revisarMouse()
    {
        // Entró al botón
        if (
            Greenfoot.mouseMoved(this)
        )
        {
            setImage(
                imagenHover
            );
        }


        // Salió del botón
        if (
            Greenfoot.mouseMoved(null)
            &&
            !Greenfoot.mouseMoved(this)
        )
        {
            setImage(
                imagenNormal
            );
        }
    }


    // =================================================
    // CLICK
    // =================================================

    private void revisarClick()
    {
        if (
            !Greenfoot.mouseClicked(this)
        )
        {
            return;
        }


        // =============================================
        // COMENZAR
        // =============================================

        if (
            accion.equals(
                "COMENZAR"
            )
        )
        {
            Greenfoot.setWorld(
                new SeleccionPersonaje()
            );
        }


        // =============================================
        // ELEGIR PERSONAJE
        // =============================================

        else if (
            accion.equals(
                "PERSONAJE"
            )
        )
        {
            Greenfoot.setWorld(
                new SeleccionPersonaje()
            );
        }


        // =============================================
        // CÓMO JUGAR
        // =============================================

        else if (
            accion.equals(
                "COMO_JUGAR"
            )
        )
        {
            Greenfoot.setWorld(
                new ComoJugar()
            );
        }
// =============================================
// REINTENTAR COMBATE
// =============================================

else if (
    accion.equals(
        "REINTENTAR"
    )
)
{
    Greenfoot.setWorld(
        new JuegoTurnos()
    );
}


// =============================================
// VOLVER AL MENÚ PRINCIPAL
// =============================================

else if (
    accion.equals(
        "MENU"
    )
)
{
    Greenfoot.setWorld(
        new MenuPrincipal()
    );
}

        // =============================================
        // SALIR
        // =============================================

        else if (
            accion.equals(
                "SALIR"
            )
        )
        {
            Greenfoot.stop();
        }
    }
}
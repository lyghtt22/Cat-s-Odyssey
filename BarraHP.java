import greenfoot.*;

public class BarraHP extends Actor
{
    private Unidad unidad;
    private boolean jugador;

    private final int ANCHO = 245;
    private final int ALTO = 50;

    private int vidaAnterior = -1;


    // =============================================
    // CONSTRUCTOR
    // =============================================

    public BarraHP(
        Unidad unidad,
        boolean jugador
    )
    {
        this.unidad = unidad;
        this.jugador = jugador;

        actualizarBarra();
    }


    // =============================================
    // ACT
    // =============================================

    public void act()
    {
        actualizarBarra();
    }


    // =============================================
    // ACTUALIZAR BARRA
    // =============================================

    private void actualizarBarra()
    {
        if (unidad == null)
        {
            return;
        }


        if (unidad.getVidaMaxima() <= 0)
        {
            return;
        }


        // Si la vida sigue igual,
        // no necesitamos redibujar.
        if (vidaAnterior == unidad.getVida())
        {
            return;
        }


        vidaAnterior =
            unidad.getVida();


        // =============================================
        // CALCULAR PORCENTAJE REAL
        // =============================================

        double porcentaje =
            (double) unidad.getVida()
            /
            unidad.getVidaMaxima();


        if (porcentaje < 0.0)
        {
            porcentaje = 0.0;
        }


        if (porcentaje > 1.0)
        {
            porcentaje = 1.0;
        }


        // =============================================
        // CARGAR BARRA VACÍA
        // =============================================

        String ruta;


        if (jugador)
        {
            ruta =
                "ui_combate/BarraHP_Jugador_0.png";
        }
        else
        {
            ruta =
                "ui_combate/BarraHP_Enemigo_0.png";
        }


        GreenfootImage imagenFinal =
            new GreenfootImage(
                ruta
            );


        imagenFinal.scale(
            ANCHO,
            ALTO
        );


        // =============================================
        // ZONA INTERIOR DE VIDA
        // =============================================

        /*
         * Estos valores corresponden a la parte
         * interior de tu barra.
         *
         * Si queda unos píxeles corrida,
         * después los afinamos.
         */

        int xVida = 25;
        int yVida = 19;

        int anchoMaximoVida = 195;
        int altoVida = 12;


        // =============================================
        // CALCULAR ANCHO ACTUAL
        // =============================================

        int anchoVida =
            (int)(
                anchoMaximoVida
                *
                porcentaje
            );


        // =============================================
        // DIBUJAR VIDA
        // =============================================

        if (jugador)
        {
            imagenFinal.setColor(
                new Color(
                    255,
                    65,
                    150
                )
            );
        }
        else
        {
            imagenFinal.setColor(
                new Color(
                    220,
                    20,
                    60
                )
            );
        }


        imagenFinal.fillRect(
            xVida,
            yVida,
            anchoVida,
            altoVida
        );


        // =============================================
        // MOSTRAR RESULTADO
        // =============================================

        setImage(
            imagenFinal
        );
    }
}
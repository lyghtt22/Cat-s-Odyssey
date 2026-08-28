import greenfoot.*;

public class Villano extends Actor
{
    private int velocidad = 1;

    private int direccion = -1;


    public Villano()
    {
        crearImagen();
    }


    // =========================================
    // IMAGEN
    // =========================================

    private void crearImagen()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                42,
                42
            );


        // Cuerpo
        imagen.setColor(
            new Color(
                110,
                25,
                130
            )
        );


        imagen.fillOval(
            4,
            4,
            34,
            34
        );


        // Ojos
        imagen.setColor(
            Color.WHITE
        );


        imagen.fillOval(
            11,
            13,
            7,
            9
        );


        imagen.fillOval(
            24,
            13,
            7,
            9
        );


        // Pupilas
        imagen.setColor(
            Color.BLACK
        );


        imagen.fillOval(
            13,
            16,
            3,
            4
        );


        imagen.fillOval(
            26,
            16,
            3,
            4
        );


        setImage(
            imagen
        );
    }


    public void act()
    {
        patrullar();

        revisarColisionPrecisa();
    }


    // =========================================
    // PATRULLA
    // =========================================

    private void patrullar()
    {
        setLocation(
            getX()
            +
            velocidad
            *
            direccion,
            getY()
        );


        if (
            getX() <= 120
        )
        {
            direccion = 1;
        }


        if (
            getX() >= 760
        )
        {
            direccion = -1;
        }
    }


    // =========================================
    // HITBOX
    // =========================================

    private void revisarColisionPrecisa()
    {
        java.util.List<Jugador> jugadores =
            getWorld()
                .getObjects(
                    Jugador.class
                );


        if (
            jugadores.isEmpty()
        )
        {
            return;
        }


        Jugador jugador =
            jugadores.get(0);


        int dx =
            Math.abs(
                jugador.getX()
                -
                getX()
            );


        int dy =
            Math.abs(
                jugador.getY()
                -
                getY()
            );


        /*
         * HITBOX pequeña.
         *
         * Deben estar MUY cerca.
         */

        if (
            dx < 18
            &&
            dy < 24
        )
        {
            jugador
                .recibirDanio();
        }
    }
}
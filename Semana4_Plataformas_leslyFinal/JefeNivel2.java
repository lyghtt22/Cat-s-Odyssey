import greenfoot.*;

public class JefeNivel2 extends Actor
{
    private int velocidad = 2;
    private int direccion = -1;

    private boolean derrotado = false;


    public JefeNivel2()
    {
        crearImagen();
    }


    // =========================================
    // IMAGEN DEL JEFE
    // =========================================

    private void crearImagen()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                65,
                65
            );


        // Cuerpo
        imagen.setColor(
            new Color(
                90,
                20,
                130
            )
        );

        imagen.fillOval(
            5,
            5,
            55,
            55
        );


        // Ojos
        imagen.setColor(
            Color.WHITE
        );

        imagen.fillOval(
            17,
            20,
            10,
            12
        );

        imagen.fillOval(
            38,
            20,
            10,
            12
        );


        // Pupilas
        imagen.setColor(
            Color.BLACK
        );

        imagen.fillOval(
            20,
            23,
            5,
            6
        );

        imagen.fillOval(
            41,
            23,
            5,
            6
        );


        // Boca
        imagen.drawLine(
            22,
            44,
            43,
            44
        );


        setImage(
            imagen
        );
    }


    // =========================================
    // ACT
    // =========================================

    public void act()
    {
        if (derrotado)
        {
            return;
        }


        patrullar();

        revisarJugador();
    }


    // =========================================
    // MOVIMIENTO
    // =========================================

    private void patrullar()
    {
        setLocation(
            getX()
            +
            velocidad * direccion,
            getY()
        );


        if (getX() <= 500)
        {
            direccion = 1;
        }


        if (getX() >= 750)
        {
            direccion = -1;
        }
    }


    // =========================================
    // COLISIÓN
    // =========================================

    private void revisarJugador()
    {
        java.util.List<Jugador> jugadores =
            getWorld()
                .getObjects(
                    Jugador.class
                );


        if (jugadores.isEmpty())
        {
            return;
        }


        Jugador jugador =
            jugadores.get(0);


        int distanciaX =
            Math.abs(
                jugador.getX()
                -
                getX()
            );


        int distanciaY =
            Math.abs(
                jugador.getY()
                -
                getY()
            );


        // No están realmente cerca
        if (
            distanciaX > 35
            ||
            distanciaY > 50
        )
        {
            return;
        }


        /*
         * CHOPPER MATA AL JEFE SI:
         *
         * 1. está saltando
         * 2. está cayendo
         * 3. está por ENCIMA del jefe
         */

        boolean chopperEncima =
            jugador.getY()
            <
            getY() - 15;


        if (
            jugador.estaSaltando()
            &&
            jugador.estaCayendo()
            &&
            chopperEncima
        )
        {
            derrotado = true;


            jugador.rebotarSobreEnemigo();


            World mundo =
                getWorld();


            if (mundo instanceof Mundo2)
            {
                ((Mundo2)mundo)
                    .derrotarJefe(this);
            }
        }

        else
        {
            /*
             * Si Chopper lo toca
             * por el lado:
             *
             * pierde una vida.
             */

            jugador.recibirDanio();
        }
    }
}
import greenfoot.*;

public class JefeFinal extends Actor
{
    private int velocidad = 2;

    private int direccion = -1;

    private int golpes = 0;

    private final int GOLPES_NECESARIOS = 2;

    private int tiempoProteccion = 0;


    public JefeFinal()
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
                80,
                80
            );


        // Cuerpo
        imagen.setColor(
            new Color(
                145,
                20,
                40
            )
        );


        imagen.fillOval(
            5,
            5,
            70,
            70
        );


        // Ojos
        imagen.setColor(
            Color.WHITE
        );


        imagen.fillOval(
            18,
            23,
            15,
            15
        );


        imagen.fillOval(
            47,
            23,
            15,
            15
        );


        // Pupilas
        imagen.setColor(
            Color.BLACK
        );


        imagen.fillOval(
            23,
            28,
            6,
            7
        );


        imagen.fillOval(
            52,
            28,
            6,
            7
        );


        // Boca
        imagen.drawLine(
            25,
            55,
            55,
            55
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
        patrullar();


        if (tiempoProteccion > 0)
        {
            tiempoProteccion--;
        }


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
            velocidad
            *
            direccion,
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


        if (
            distanciaX > 42
            ||
            distanciaY > 55
        )
        {
            return;
        }


        /*
         * CHOPPER DEBE ESTAR:
         *
         * saltando,
         * cayendo,
         * y encima del jefe.
         */

        boolean encima =
            jugador.getY()
            <
            getY() - 18;


        if (
            jugador.estaSaltando()
            &&
            jugador.estaCayendo()
            &&
            encima
        )
        {
            golpearJefe(
                jugador
            );
        }

        else
        {
            jugador.recibirDanio();
        }
    }


    // =========================================
    // GOLPE
    // =========================================

    private void golpearJefe(
        Jugador jugador
    )
    {
        /*
         * Evita contar muchos golpes
         * en el mismo contacto.
         */

        if (tiempoProteccion > 0)
        {
            return;
        }


        golpes++;

        tiempoProteccion = 40;


        jugador.rebotarSobreEnemigo();


        if (
            golpes >= GOLPES_NECESARIOS
        )
        {
            World mundo =
                getWorld();


            if (mundo instanceof Mundo3)
            {
                ((Mundo3)mundo)
                    .derrotarJefe(
                        this
                    );
            }
        }

        else
        {
            getWorld().showText(
                "JEFE FINAL: 1/2 GOLPES",
                400,
                100
            );
        }
    }
}
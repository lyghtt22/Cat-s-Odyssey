import greenfoot.*;

public class Trampa extends Actor
{
    public Trampa()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                "spikes.png"
            );


        imagen.scale(
            75,
            30
        );


        setImage(
            imagen
        );
    }


    public void act()
    {
        revisarColisionPrecisa();
    }


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


        /*
         * Parte inferior del sprite
         * de Chopper.
         */

        int piesJugador =
            jugador.getY()
            +
            jugador
                .getImage()
                .getHeight()
            /
            2;


        /*
         * Parte superior
         * de los pinchos.
         */

        int parteSuperiorTrampa =
            getY()
            -
            getImage()
                .getHeight()
            /
            2;


        int distanciaX =
            Math.abs(
                jugador.getX()
                -
                getX()
            );


        /*
         * Tiene que estar prácticamente
         * encima de los pinchos.
         */

        boolean encimaHorizontalmente =
            distanciaX < 22;


        /*
         * Y los pies tienen que estar
         * realmente cerca de las puntas.
         */

        boolean piesTocando =
            piesJugador
            >=
            parteSuperiorTrampa
            -
            3

            &&

            piesJugador
            <=
            parteSuperiorTrampa
            +
            14;


        if (
            encimaHorizontalmente
            &&
            piesTocando
        )
        {
            jugador
                .recibirDanio();
        }
    }
}
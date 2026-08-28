import greenfoot.*;

public class Cristal extends Actor
{
    public Cristal()
    {
        crearImagen();
    }


    private void crearImagen()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                40,
                50
            );


        // Cristal
        imagen.setColor(
            new Color(
                70,
                220,
                255
            )
        );


        int[] x =
        {
            20,
            34,
            28,
            20,
            12,
            6
        };


        int[] y =
        {
            2,
            16,
            38,
            48,
            38,
            16
        };


        imagen.fillPolygon(
            x,
            y,
            6
        );


        // Brillo
        imagen.setColor(
            Color.WHITE
        );


        imagen.drawLine(
            15,
            12,
            20,
            7
        );


        imagen.drawLine(
            20,
            7,
            25,
            12
        );


        setImage(
            imagen
        );
    }


    public void act()
    {
        Jugador jugador =
            (Jugador)
            getOneIntersectingObject(
                Jugador.class
            );


        if (jugador != null)
        {
            World mundo =
                getWorld();


            if (mundo instanceof Mundo3)
            {
                ((Mundo3)mundo)
                    .recogerCristal(
                        this
                    );
            }
        }
    }
}
import greenfoot.*;

public class Tesoro extends Actor
{
    public Tesoro()
    {
        crearImagen();
    }


    // =========================================
    // CREAR COFRE
    // =========================================

    private void crearImagen()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                45,
                40
            );


        // Parte inferior
        imagen.setColor(
            new Color(
                130,
                75,
                25
            )
        );

        imagen.fillRect(
            4,
            14,
            37,
            22
        );


        // Parte superior
        imagen.setColor(
            new Color(
                170,
                105,
                35
            )
        );

        imagen.fillOval(
            4,
            3,
            37,
            24
        );


        // Franja dorada
        imagen.setColor(
            new Color(
                255,
                210,
                40
            )
        );

        imagen.fillRect(
            19,
            12,
            7,
            24
        );


        // Cerradura
        imagen.setColor(
            Color.BLACK
        );

        imagen.fillOval(
            20,
            20,
            6,
            7
        );


        setImage(
            imagen
        );
    }


    // =========================================
    // RECOGER TESORO
    // =========================================

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


            if (mundo instanceof Mundo2)
            {
                ((Mundo2)mundo)
                    .recogerTesoro(
                        this
                    );
            }
        }
    }
}
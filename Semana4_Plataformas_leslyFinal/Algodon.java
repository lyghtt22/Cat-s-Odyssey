import greenfoot.*;

public class Algodon extends Actor
{
    public Algodon()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                "cotton_candy.png"
            );


        imagen.scale(
            40,
            50
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


        if (
            jugador != null
        )
        {
            World mundo =
                getWorld();


            if (
                mundo
                instanceof
                Mundo1
            )
            {
                ((Mundo1)mundo)
                    .recogerAlgodon(
                        this
                    );
            }
        }
    }
}
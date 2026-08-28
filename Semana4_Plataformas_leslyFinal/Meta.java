import greenfoot.*;

public class Meta extends Actor
{
    public Meta()
    {
        GreenfootImage imagen =
            new GreenfootImage(
                "goal_flag.png"
            );


        imagen.scale(
            60,
            85
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
                    .tocarMeta();
            }
        }
    }
}
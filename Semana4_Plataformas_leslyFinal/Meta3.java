import greenfoot.*;

public class Meta3 extends Actor
{
    public Meta3()
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


        if (jugador != null)
        {
            World mundo =
                getWorld();


            if (mundo instanceof Mundo3)
            {
                ((Mundo3)mundo)
                    .tocarMeta();
            }
        }
    }
}
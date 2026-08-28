import greenfoot.*;

public class Meta2 extends Actor
{
    public Meta2()
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


            if (
                mundo instanceof Mundo2
            )
            {
                ((Mundo2)mundo)
                    .tocarMeta();
            }
        }
    }
}
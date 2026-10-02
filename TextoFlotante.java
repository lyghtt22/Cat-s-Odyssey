import greenfoot.*;

public class TextoFlotante extends Actor
{
    // ================================================
    // DURACIÓN DEL TEXTO
    // ================================================

    private int duracion = 60;


    // ================================================
    // CONSTRUCTOR
    // ================================================

    public TextoFlotante(
        String texto,
        boolean esDanio
    )
    {
        crearImagen(
            texto,
            esDanio
        );
    }


    // ================================================
    // ACT
    // ================================================

    public void act()
    {
        subir();

        duracion--;


        if (duracion <= 0)
        {
            World mundo =
                getWorld();


            if (mundo != null)
            {
                mundo.removeObject(
                    this
                );
            }
        }
    }


    // ================================================
    // SUBIR POCO A POCO
    // ================================================

    private void subir()
    {
        if (getY() > 0)
        {
            setLocation(
                getX(),
                getY() - 1
            );
        }
    }


    // ================================================
    // CREAR IMAGEN
    // ================================================

    private void crearImagen(
        String texto,
        boolean esDanio
    )
    {
        Color colorTexto;


        if (esDanio)
        {
            colorTexto =
                new Color(
                    255,
                    80,
                    110
                );
        }
        else
        {
            colorTexto =
                new Color(
                    255,
                    230,
                    240
                );
        }


        GreenfootImage imagen =
            new GreenfootImage(
                texto,
                28,
                colorTexto,
                new Color(
                    0,
                    0,
                    0,
                    0
                )
            );


        setImage(
            imagen
        );
    }
}
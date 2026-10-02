import greenfoot.*;

public class MenuPrincipal extends World
{
    public MenuPrincipal()
    {
        super(900, 600, 1);

        prepararFondo();
        prepararTitulo();
        prepararBotones();
    }


    // =================================================
    // FONDO
    // =================================================

    private void prepararFondo()
    {
        GreenfootImage fondo =
            new GreenfootImage(
                "menu/FondoMenu_original.png"
            );

        fondo.scale(
            getWidth(),
            getHeight()
        );

        setBackground(fondo);
    }


    // =================================================
    // TITULO
    // =================================================

    private void prepararTitulo()
    {
        Actor titulo = new Actor()
        {
        };

        GreenfootImage imagen =
            new GreenfootImage(
                "menu/TitleGame.png"
            );

        imagen.scale(
            500,
            160
        );

        titulo.setImage(imagen);

        addObject(
            titulo,
            getWidth() / 2,
            120
        );
    }


    // =================================================
    // BOTONES
    // =================================================

    private void prepararBotones()
    {
        BotonMenu comenzar =
            new BotonMenu(
                "ComenzarButton.png",
                "COMENZAR"
            );


        BotonMenu personaje =
            new BotonMenu(
                "PersonajeButton.png",
                "PERSONAJE"
            );


        BotonMenu instrucciones =
            new BotonMenu(
                "InstruccionesButton.png",
                "COMO_JUGAR"
            );


        BotonMenu salir =
            new BotonMenu(
                "ExitButton.png",
                "SALIR"
            );


        addObject(
            comenzar,
            getWidth() / 2,
            300
        );


        addObject(
            personaje,
            getWidth() / 2,
            370
        );


        addObject(
            instrucciones,
            getWidth() / 2,
            440
        );


        addObject(
            salir,
            getWidth() / 2,
            510
        );
    }
}
import greenfoot.*;

public class JuegoWorld extends World
{
    private Jugador jugador;
    private GestorOleadas gestor;
    private HUD hud;

    public JuegoWorld()
    {
        super(900, 600, 1);

        prepararFondo();

        jugador = new Jugador();

        addObject(
            jugador,
            getWidth() / 2,
            getHeight() / 2
        );

        gestor = new GestorOleadas(
            this,
            jugador
        );

        hud = new HUD(
            this,
            jugador,
            gestor
        );

        addObject(
            hud,
            getWidth() / 2,
            30
        );
    }

    public void act()
    {
        gestor.actualizar();
    }

    public void gameOver()
    {
        showText(
            "GAME OVER",
            getWidth() / 2,
            getHeight() / 2
        );

        Greenfoot.stop();
    }


    // =========================================
    // FONDO DEL JUEGO
    // =========================================

    private void prepararFondo()
    {
        GreenfootImage fondo =
            new GreenfootImage(
                getWidth(),
                getHeight()
            );


        // Rosa palo principal
        fondo.setColor(
            new Color(
                244,
                220,
                228
            )
        );

        fondo.fill();


        // =====================================
        // CUADRÍCULA MUY SUAVE
        // =====================================

        fondo.setColor(
            new Color(
                235,
                202,
                214
            )
        );


        for (
            int x = 0;
            x < getWidth();
            x += 50
        )
        {
            fondo.drawLine(
                x,
                0,
                x,
                getHeight()
            );
        }


        for (
            int y = 0;
            y < getHeight();
            y += 50
        )
        {
            fondo.drawLine(
                0,
                y,
                getWidth(),
                y
            );
        }


        // =====================================
        // PUNTITOS DECORATIVOS
        // =====================================

        fondo.setColor(
            new Color(
                255,
                238,
                243
            )
        );


        for (int i = 0; i < 40; i++)
        {
            int x =
                Greenfoot.getRandomNumber(
                    getWidth()
                );

            int y =
                Greenfoot.getRandomNumber(
                    getHeight()
                );


            fondo.fillOval(
                x,
                y,
                5,
                5
            );
        }


        setBackground(
            fondo
        );
    }
}
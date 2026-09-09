import greenfoot.*;

public class SoldadoEnemigo extends Soldado
{
    private Jugador objetivo;

    private int danio;

    private int cooldownAtaque = 0;


    public SoldadoEnemigo(
        Jugador objetivo,
        int vida,
        int velocidad,
        int danio
    )
    {
        super(
            vida,
            velocidad
        );


        this.objetivo =
            objetivo;


        this.danio =
            danio;


        setImage(
            FabricaImagenes.soldadoEnemigo()
        );
    }


    public void act()
    {
        // Si el jugador ya no existe,
        // dejamos de actuar.
        if (
            objetivo == null
            ||
            objetivo.getWorld() == null
        )
        {
            return;
        }


        perseguir();


        if (cooldownAtaque > 0)
        {
            cooldownAtaque--;
        }


        atacar();
    }


    // =========================================
    // PERSEGUIR AL JUGADOR
    // =========================================

    private void perseguir()
    {
        int dx =
            objetivo.getX()
            -
            getX();


        int dy =
            objetivo.getY()
            -
            getY();


        double distancia =
            Math.sqrt(
                dx * dx
                +
                dy * dy
            );


        if (distancia > 0)
        {
            int movX =
                (int)Math.round(
                    velocidad
                    *
                    dx
                    /
                    distancia
                );


            int movY =
                (int)Math.round(
                    velocidad
                    *
                    dy
                    /
                    distancia
                );


            setLocation(
                getX() + movX,
                getY() + movY
            );
        }
    }


    // =========================================
    // ATAQUE
    // =========================================

    private void atacar()
    {
        if (
            isTouching(Jugador.class)
            &&
            cooldownAtaque == 0
        )
        {
            objetivo.recibirDanio(
                danio
            );


            // No puede quitar vida
            // en todos los frames.
            cooldownAtaque = 30;
        }
    }
}
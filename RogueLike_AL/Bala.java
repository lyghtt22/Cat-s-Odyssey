import greenfoot.*;

public class Bala extends Actor
{
    private int dx;
    private int dy;

    private int velocidad = 8;

    // La bala dura como máximo 80 ciclos
    private int vidaUtil = 80;


    public Bala(
        int dirX,
        int dirY
    )
    {
        dx = dirX;
        dy = dirY;


        setImage(
            FabricaImagenes.bala()
        );


        // =====================================
        // ORIENTACIÓN VISUAL
        // =====================================

        if (dx == 1)
        {
            setRotation(0);
        }

        else if (dx == -1)
        {
            setRotation(180);
        }

        else if (dy == 1)
        {
            setRotation(90);
        }

        else if (dy == -1)
        {
            setRotation(270);
        }
    }


    public void act()
    {
        mover();


        // Si sigue existiendo,
        // revisamos el impacto.
        if (getWorld() != null)
        {
            revisarImpacto();
        }


        // Si todavía sigue existiendo,
        // revisamos su vida útil.
        if (getWorld() != null)
        {
            revisarCicloDeVida();
        }
    }


    // =========================================
    // MOVIMIENTO
    // =========================================

    private void mover()
    {
        setLocation(
            getX() + dx * velocidad,
            getY() + dy * velocidad
        );


        vidaUtil--;
    }


    // =========================================
    // IMPACTO CONTRA ENEMIGO
    // =========================================

    private void revisarImpacto()
    {
        SoldadoEnemigo enemigo =
            (SoldadoEnemigo)
            getOneIntersectingObject(
                SoldadoEnemigo.class
            );


        if (enemigo != null)
        {
            // Cada disparo quita 10 de vida
            enemigo.recibirDanio(10);


            World mundo =
                getWorld();


            if (mundo != null)
            {
                mundo.removeObject(this);
            }
        }
    }


    // =========================================
    // CICLO DE VIDA
    // =========================================

    private void revisarCicloDeVida()
    {
        if (
            vidaUtil <= 0
            ||
            fueraDelMundo()
        )
        {
            World mundo =
                getWorld();


            if (mundo != null)
            {
                mundo.removeObject(this);
            }
        }
    }


    // =========================================
    // BORDE DEL MAPA
    // =========================================

    private boolean fueraDelMundo()
    {
        return
            getX() <= 3
            ||
            getY() <= 3
            ||
            getX() >= getWorld().getWidth() - 3
            ||
            getY() >= getWorld().getHeight() - 3;
    }
}
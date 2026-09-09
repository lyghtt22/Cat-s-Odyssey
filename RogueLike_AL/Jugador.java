import greenfoot.*;

public class Jugador extends Soldado
{
    private int cooldownDisparo = 0;

    // Dirección inicial: derecha
    private int dirX = 1;
    private int dirY = 0;


    public Jugador()
    {
        super(100, 4);

        setImage(
            FabricaImagenes.soldadoJugador()
        );
    }


    public void act()
    {
        mover();
        disparar();

        if (cooldownDisparo > 0)
        {
            cooldownDisparo--;
        }
    }


    // =========================================
    // MOVIMIENTO CON FLECHAS
    // =========================================

    private void mover()
    {
        int dx = 0;
        int dy = 0;


        // ↑ ARRIBA
        if (Greenfoot.isKeyDown("up"))
        {
            dy = -velocidad;

            dirX = 0;
            dirY = -1;
        }


        // ↓ ABAJO
        if (Greenfoot.isKeyDown("down"))
        {
            dy = velocidad;

            dirX = 0;
            dirY = 1;
        }


        // ← IZQUIERDA
        if (Greenfoot.isKeyDown("left"))
        {
            dx = -velocidad;

            dirX = -1;
            dirY = 0;
        }


        // → DERECHA
        if (Greenfoot.isKeyDown("right"))
        {
            dx = velocidad;

            dirX = 1;
            dirY = 0;
        }


        // =====================================
        // EVITAR SALIR DEL MAPA
        // =====================================

        int nuevoX =
            Math.max(
                20,
                Math.min(
                    getWorld().getWidth() - 20,
                    getX() + dx
                )
            );


        int nuevoY =
            Math.max(
                70,
                Math.min(
                    getWorld().getHeight() - 20,
                    getY() + dy
                )
            );


        setLocation(
            nuevoX,
            nuevoY
        );
    }


    // =========================================
    // DISPARAR
    // =========================================

    private void disparar()
    {
        if (
            Greenfoot.isKeyDown("space")
            &&
            cooldownDisparo == 0
        )
        {
            int cantidadBalas =
                getWorld()
                .getObjects(Bala.class)
                .size();


            // Máximo 60 balas simultáneas
            if (cantidadBalas < 60)
            {
                Bala bala =
                    new Bala(
                        dirX,
                        dirY
                    );


                getWorld().addObject(
                    bala,
                    getX(),
                    getY()
                );
            }


            // Pequeña espera entre disparos
            cooldownDisparo = 8;
        }
    }


    // =========================================
    // MUERTE DEL JUGADOR
    // =========================================

    protected void morir()
    {
        World mundo =
            getWorld();


        if (mundo instanceof JuegoWorld)
        {
            ((JuegoWorld)mundo)
                .gameOver();
        }


        if (mundo != null)
        {
            mundo.removeObject(this);
        }
    }
}
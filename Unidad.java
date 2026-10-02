import greenfoot.*;

public abstract class Unidad extends Actor
{
    protected String nombre;

    protected int vidaMaxima;
    protected int vida;

    protected int ataque;


    public Unidad(
        String nombre,
        int vidaMaxima,
        int ataque
    )
    {
        this.nombre = nombre;
        this.vidaMaxima = vidaMaxima;
        this.vida = vidaMaxima;
        this.ataque = ataque;
    }


    // ================================================
    // RECIBIR DAÑO
    // ================================================

    public void recibirDanio(int cantidad)
    {
        if (cantidad < 0)
        {
            return;
        }

        vida -= cantidad;

        if (vida < 0)
        {
            vida = 0;
        }

        alRecibirDanio();
    }


    // ================================================
    // EVENTO VISUAL DE DAÑO
    // ================================================

    protected void alRecibirDanio()
    {
        /*
         * Las clases hijas pueden sobrescribir
         * este método para mostrar su sprite
         * de daño.
         */
    }


    // ================================================
    // CURAR
    // ================================================

    public void curar(int cantidad)
    {
        if (cantidad < 0)
        {
            return;
        }

        vida += cantidad;

        if (vida > vidaMaxima)
        {
            vida = vidaMaxima;
        }
    }


    // ================================================
    // ESTADO
    // ================================================

    public boolean estaViva()
    {
        return vida > 0;
    }


    public boolean estaDerrotada()
    {
        return vida <= 0;
    }


    // ================================================
    // PORCENTAJE DE VIDA
    // ================================================

    public int getPorcentajeVida()
    {
        if (vidaMaxima <= 0)
        {
            return 0;
        }

        return (vida * 100) / vidaMaxima;
    }


    // ================================================
    // GETTERS
    // ================================================

    public int getVida()
    {
        return vida;
    }


    public int getVidaMaxima()
    {
        return vidaMaxima;
    }


    public int getAtaque()
    {
        return ataque;
    }


    public String getNombre()
    {
        return nombre;
    }


    // ================================================
    // SET VIDA
    // ================================================

    public void setVida(int nuevaVida)
    {
        vida = nuevaVida;

        if (vida > vidaMaxima)
        {
            vida = vidaMaxima;
        }

        if (vida < 0)
        {
            vida = 0;
        }
    }
}
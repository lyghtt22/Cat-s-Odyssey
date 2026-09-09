import greenfoot.*;

public class GestorOleadas
{
    private JuegoWorld mundo;
    private Jugador jugador;

    private int oleada = 1;

    private int generados = 0;

    private int objetivoOleada = 0;

    private int contadorSpawn = 0;

    private int esperaEntreOleadas = 0;


    public GestorOleadas(
        JuegoWorld mundo,
        Jugador jugador
    )
    {
        this.mundo = mundo;
        this.jugador = jugador;

        prepararOleada();
    }


    public void actualizar()
    {
        if (
            jugador == null
            ||
            jugador.getWorld() == null
        )
        {
            return;
        }


        int activos =
            mundo
                .getObjects(
                    SoldadoEnemigo.class
                )
                .size();


        // =====================================
        // CREAR ENEMIGOS
        // =====================================

        if (
            generados < objetivoOleada
        )
        {
            contadorSpawn--;


            if (
                contadorSpawn <= 0
                &&
                activos < maxSimultaneos()
            )
            {
                crearEnemigo();

                generados++;

                contadorSpawn =
                    intervaloSpawn();
            }
        }


        // =====================================
        // SIGUIENTE OLEADA
        // =====================================

        else if (
            activos == 0
        )
        {
            if (
                esperaEntreOleadas == 0
            )
            {
                esperaEntreOleadas = 100;
            }


            esperaEntreOleadas--;


            if (
                esperaEntreOleadas <= 0
            )
            {
                oleada++;

                prepararOleada();
            }
        }
    }


    // =========================================
    // PREPARAR OLEADA
    // =========================================

    private void prepararOleada()
    {
        generados = 0;

        objetivoOleada =
            Math.min(
                5 + oleada * 3,
                80
            );

        contadorSpawn = 20;

        esperaEntreOleadas = 0;
    }


    // =========================================
    // MÁXIMO SIMULTÁNEO
    // =========================================

    private int maxSimultaneos()
    {
        return Math.min(
            6 + oleada * 2,
            35
        );
    }


    // =========================================
    // VELOCIDAD DE APARICIÓN
    // =========================================

    private int intervaloSpawn()
    {
        return Math.max(
            12,
            50 - oleada * 3
        );
    }


    // =========================================
    // VIDA ENEMIGO
    // =========================================

    private int vidaEnemigo()
    {
        return
            20
            +
            oleada * 4;
    }


    // =========================================
    // VELOCIDAD ENEMIGO
    // =========================================

    private int velocidadEnemigo()
    {
        return Math.min(
            1 + oleada / 4,
            4
        );
    }


    // =========================================
    // DAÑO ENEMIGO
    // =========================================

    private int danioEnemigo()
    {
        return Math.min(
            4 + oleada,
            15
        );
    }


    // =========================================
    // CREAR ENEMIGO
    // =========================================

    private void crearEnemigo()
    {
        SoldadoEnemigo enemigo =
            new SoldadoEnemigo(
                jugador,
                vidaEnemigo(),
                velocidadEnemigo(),
                danioEnemigo()
            );


        int lado =
            Greenfoot.getRandomNumber(4);


        int x = 0;
        int y = 0;


        // IZQUIERDA
        if (lado == 0)
        {
            x = 10;

            y =
                Greenfoot
                    .getRandomNumber(
                        mundo.getHeight()
                    );
        }


        // DERECHA
        else if (lado == 1)
        {
            x =
                mundo.getWidth() - 10;

            y =
                Greenfoot
                    .getRandomNumber(
                        mundo.getHeight()
                    );
        }


        // ARRIBA
        else if (lado == 2)
        {
            x =
                Greenfoot
                    .getRandomNumber(
                        mundo.getWidth()
                    );

            y = 10;
        }


        // ABAJO
        else
        {
            x =
                Greenfoot
                    .getRandomNumber(
                        mundo.getWidth()
                    );

            y =
                mundo.getHeight() - 10;
        }


        mundo.addObject(
            enemigo,
            x,
            y
        );
    }


    // =========================================
    // OBTENER OLEADA
    // =========================================

    public int getOleada()
    {
        return oleada;
    }
}
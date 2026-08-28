import greenfoot.*;
import java.util.List;

public abstract class NivelBase extends World
{
    private GreenfootImage fondoGrande;

    private int desplazamiento = 0;

    private int repeticiones;

    private int anchoNivel;

    private String nombreFondo;

    private int numeroNivel;


    public NivelBase(
        int numeroNivel,
        String nombreFondo,
        int repeticiones
    )
    {
        super(800, 500, 1);

        this.numeroNivel = numeroNivel;
        this.nombreFondo = nombreFondo;
        this.repeticiones = repeticiones;

        anchoNivel =
            getWidth() * repeticiones;

        prepararFondo();

        prepararJugador();
    }


    // =========================================
    // FONDO
    // =========================================

    private void prepararFondo()
    {
        GreenfootImage parte =
            new GreenfootImage(
                nombreFondo
            );


        parte.scale(
            getWidth(),
            getHeight()
        );


        fondoGrande =
            new GreenfootImage(
                anchoNivel,
                getHeight()
            );


        for (
            int i = 0;
            i < repeticiones;
            i++
        )
        {
            fondoGrande.drawImage(
                parte,
                i * getWidth(),
                0
            );
        }


        actualizarFondo();
    }


    private void actualizarFondo()
    {
        GreenfootImage vista =
            new GreenfootImage(
                getWidth(),
                getHeight()
            );


        vista.drawImage(
            fondoGrande,
            -desplazamiento,
            0
        );


        setBackground(
            vista
        );
    }


    // =========================================
    // JUGADOR
    // =========================================

    private void prepararJugador()
    {
        Jugador jugador =
            new Jugador();


        addObject(
            jugador,
            100,
            400
        );
    }


    // =========================================
    // CÁMARA
    // =========================================

    public boolean moverCamara(
        int cantidad
    )
    {
        int nuevoDesplazamiento =
            desplazamiento
            +
            cantidad;


        if (
            nuevoDesplazamiento < 0
        )
        {
            nuevoDesplazamiento = 0;
        }


        int maximo =
            anchoNivel
            -
            getWidth();


        if (
            nuevoDesplazamiento
            >
            maximo
        )
        {
            nuevoDesplazamiento =
                maximo;
        }


        int cambio =
            nuevoDesplazamiento
            -
            desplazamiento;


        if (cambio == 0)
        {
            return false;
        }


        desplazamiento =
            nuevoDesplazamiento;


        actualizarFondo();


        moverElementosEscenario(
            -cambio
        );


        return true;
    }


    // =========================================
    // MOVER OBJETOS DEL ESCENARIO
    // =========================================

    private void moverElementosEscenario(
        int cantidad
    )
    {
        List<Actor> actores =
            getObjects(
                Actor.class
            );


        for (
            Actor actor :
            actores
        )
        {
            // Chopper no se mueve
            // con el fondo.
            if (
                !(actor instanceof Jugador)
            )
            {
                int nuevaX =
                    actor.getX()
                    +
                    cantidad;


                /*
                 * Si el objeto ya quedó atrás,
                 * lo eliminamos.
                 *
                 * Así no queda pegado
                 * al borde izquierdo.
                 */
                if (
                    nuevaX <= 5
                )
                {
                    removeObject(
                        actor
                    );
                }
                else
                {
                    actor.setLocation(
                        nuevaX,
                        actor.getY()
                    );
                }
            }
        }
    }


    // =========================================
    // INFORMACIÓN DEL NIVEL
    // =========================================

    public int getDesplazamiento()
    {
        return desplazamiento;
    }


    public int getNumeroNivel()
    {
        return numeroNivel;
    }


    public boolean camaraAlFinal()
    {
        return
            desplazamiento
            >=
            anchoNivel
            -
            getWidth();
    }


    public abstract void siguienteNivel();
}
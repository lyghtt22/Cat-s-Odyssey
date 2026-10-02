import greenfoot.*;

public class BotonAccion extends Actor
{
    // ================================================
    // DATOS DEL BOTÓN
    // ================================================

    private String tipoAccion;
    private GestorCombate gestorCombate;


    // ================================================
    // CONSTRUCTOR
    // ================================================

    public BotonAccion(
        String tipoAccion,
        GestorCombate gestorCombate
    )
    {
        this.tipoAccion = tipoAccion;
        this.gestorCombate = gestorCombate;
    }


    // ================================================
    // ACT
    // ================================================

    public void act()
    {
        revisarClick();
    }


    // ================================================
    // REVISAR CLICK
    // ================================================

    private void revisarClick()
    {
        if (!Greenfoot.mouseClicked(this))
        {
            return;
        }


        // No permitir acciones
        // si el combate terminó.
        if (gestorCombate.combateTerminado())
        {
            return;
        }


        // No permitir acciones
        // si no es turno del jugador.
        if (!gestorCombate.esTurnoJugador())
        {
            return;
        }


        // No permitir spam de botones
        // mientras se resuelve una acción.
        if (gestorCombate.estaAnimando())
        {
            return;
        }


        // ============================================
        // ATAQUE RÁPIDO
        // ============================================

        if (tipoAccion.equals("rapido"))
        {
            gestorCombate.usarAtaqueRapido();
            return;
        }


        // ============================================
        // ATAQUE NORMAL
        // ============================================

        if (tipoAccion.equals("normal"))
        {
            gestorCombate.usarAtaqueNormal();
            return;
        }


        // ============================================
        // ATAQUE FUERTE
        // ============================================

        if (tipoAccion.equals("fuerte"))
        {
            gestorCombate.usarAtaqueFuerte();
        }
    }
}
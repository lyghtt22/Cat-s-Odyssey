import greenfoot.*;

public class FabricaImagenes
{
    // =================================================
    // CARGAR IMAGEN
    // =================================================

    public static GreenfootImage cargar(String ruta)
    {
        return new GreenfootImage(ruta);
    }


    // =================================================
    // AURORA - IDLE
    // =================================================

    public static GreenfootImage auroraIdle(int frame)
    {
        return cargar(
            "sprites/Aurora/Aurora_Idle_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // AURORA - ATAQUE RÁPIDO
    // =================================================

    public static GreenfootImage auroraAtaqueRapido(int frame)
    {
        return cargar(
            "sprites/Aurora/Aurora_AtaqueRapido_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // AURORA - ATAQUE NORMAL
    // =================================================

    public static GreenfootImage auroraAtaqueNormal(int frame)
    {
        return cargar(
            "sprites/Aurora/Aurora_AtaqueNormal_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // AURORA - ATAQUE FUERTE
    // =================================================

    public static GreenfootImage auroraAtaqueFuerte(int frame)
    {
        return cargar(
            "sprites/Aurora/Aurora_AtaqueFuerte_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // AURORA - DAÑO
    // =================================================

    public static GreenfootImage auroraDanio()
    {
        return cargar(
            "sprites/Aurora/Aurora_Danio.png"
        );
    }


    // =================================================
    // AURORA - DERROTA
    // =================================================

    public static GreenfootImage auroraDerrota()
    {
        return cargar(
            "sprites/Aurora/Aurora_Derrota.png"
        );
    }


    // =================================================
    // SELENE - BASE / IDLE
    // =================================================

    public static GreenfootImage seleneBase()
    {
        return cargar(
            "sprites/Selene/Selene_Base_1.png"
        );
    }


    // =================================================
    // SELENE - ATAQUE RÁPIDO
    // =================================================

    public static GreenfootImage seleneAtaqueRapido(int frame)
    {
        return cargar(
            "sprites/Selene/Selene_Ataque_Rapido_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // SELENE - ATAQUE NORMAL
    // =================================================

    public static GreenfootImage seleneAtaqueNormal(int frame)
    {
        return cargar(
            "sprites/Selene/Selene_Attack_Normal_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // SELENE - ATAQUE FUERTE
    // =================================================

    public static GreenfootImage seleneAtaqueFuerte(int frame)
    {
        return cargar(
            "sprites/Selene/Selene_Attack_Strong_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // SELENE - DAÑO
    // =================================================

    public static GreenfootImage seleneDanio()
    {
        return cargar(
            "sprites/Selene/Selene_Danio.png"
        );
    }


    // =================================================
    // SELENE - DERROTA
    // =================================================

    public static GreenfootImage seleneDerrota()
    {
        return cargar(
            "sprites/Selene/Selene_Derrota.png"
        );
    }


    // =================================================
    // REINA MALVADA - IDLE
    // =================================================

    public static GreenfootImage madrastraIdle(int frame)
    {
        return cargar(
            "sprites/ReinaMadrastra/Madrastra_Idle_"
            + frame
            + ".png"
        );
    }


    // =================================================
    // REINA MALVADA - DAÑO
    // =================================================

    public static GreenfootImage reinaDanio()
    {
        return cargar(
            "sprites/ReinaMadrastra/ReinaMalvada_Danio.png"
        );
    }


    // =================================================
    // REINA MALVADA - DERROTA
    // =================================================

    public static GreenfootImage reinaDerrota()
    {
        return cargar(
            "sprites/ReinaMadrastra/ReinaMalvada_Derrota.png"
        );
    }
}
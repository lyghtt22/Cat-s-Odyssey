import greenfoot.*;

public class FabricaImagenes
{
    // =========================================
    // IMÁGENES REUTILIZADAS
    // =========================================

    private static final GreenfootImage JUGADOR =
        crearJugadorBonito();

    private static final GreenfootImage ENEMIGO =
        crearEnemigoBonito();

    private static final GreenfootImage BALA =
        crearBalaBonita();


    // =========================================
    // MÉTODOS PÚBLICOS
    // =========================================

    public static GreenfootImage soldadoJugador()
    {
        return JUGADOR;
    }


    public static GreenfootImage soldadoEnemigo()
    {
        return ENEMIGO;
    }


    public static GreenfootImage bala()
    {
        return BALA;
    }


    // =========================================
    // JUGADOR BONITO
    // =========================================

    private static GreenfootImage crearJugadorBonito()
    {
        GreenfootImage img =
            new GreenfootImage(48, 50);


        // Sombra
        img.setColor(
            new Color(170, 120, 145, 80)
        );

        img.fillOval(
            10,
            42,
            28,
            6
        );


        // Piernas
        img.setColor(
            new Color(120, 80, 105)
        );

        img.fillRect(
            15,
            35,
            6,
            9
        );

        img.fillRect(
            27,
            35,
            6,
            9
        );


        // Zapatitos
        img.setColor(
            new Color(210, 105, 145)
        );

        img.fillRect(
            14,
            43,
            8,
            4
        );

        img.fillRect(
            26,
            43,
            8,
            4
        );


        // Cuerpo
        img.setColor(
            new Color(230, 135, 165)
        );

        img.fillRect(
            12,
            22,
            25,
            15
        );


        // Hombros
        img.fillOval(
            9,
            21,
            10,
            12
        );

        img.fillOval(
            31,
            21,
            10,
            12
        );


        // Cabeza
        img.setColor(
            new Color(255, 222, 210)
        );

        img.fillOval(
            14,
            5,
            21,
            21
        );


        // Cabello
        img.setColor(
            new Color(105, 70, 80)
        );

        img.fillOval(
            13,
            3,
            23,
            16
        );


        // Cara
        img.setColor(
            new Color(255, 222, 210)
        );

        img.fillOval(
            15,
            10,
            19,
            17
        );


        // Flequillo
        img.setColor(
            new Color(105, 70, 80)
        );

        img.fillOval(
            15,
            7,
            8,
            8
        );

        img.fillOval(
            20,
            6,
            8,
            8
        );


        // Ojos
        img.setColor(
            new Color(70, 50, 60)
        );

        img.fillOval(
            19,
            15,
            3,
            4
        );

        img.fillOval(
            28,
            15,
            3,
            4
        );


        // Brillito ojos
        img.setColor(
            Color.WHITE
        );

        img.fillOval(
            20,
            15,
            1,
            1
        );

        img.fillOval(
            29,
            15,
            1,
            1
        );


        // Mejillas
        img.setColor(
            new Color(245, 150, 175)
        );

        img.fillOval(
            16,
            20,
            5,
            3
        );

        img.fillOval(
            30,
            20,
            5,
            3
        );


        // Sonrisa
        img.setColor(
            new Color(125, 70, 85)
        );

        img.drawLine(
            23,
            23,
            27,
            23
        );


        // Corazón del uniforme
        img.setColor(
            new Color(255, 238, 245)
        );

        img.fillOval(
            21,
            27,
            5,
            5
        );

        img.fillOval(
            25,
            27,
            5,
            5
        );


        int[] corazonX =
        {
            21,
            30,
            25
        };

        int[] corazonY =
        {
            30,
            30,
            36
        };


        img.fillPolygon(
            corazonX,
            corazonY,
            3
        );


        // Arma
        img.setColor(
            new Color(95, 85, 92)
        );

        img.fillRect(
            34,
            25,
            12,
            4
        );


        // Empuñadura
        img.setColor(
            new Color(150, 115, 130)
        );

        img.fillRect(
            35,
            28,
            4,
            6
        );


        return img;
    }


    // =========================================
    // ENEMIGO BONITO
    // =========================================

    private static GreenfootImage crearEnemigoBonito()
    {
        GreenfootImage img =
            new GreenfootImage(48, 50);


        // Sombra
        img.setColor(
            new Color(90, 45, 60, 80)
        );

        img.fillOval(
            10,
            42,
            28,
            6
        );


        // Piernas
        img.setColor(
            new Color(75, 45, 55)
        );

        img.fillRect(
            15,
            35,
            6,
            9
        );

        img.fillRect(
            27,
            35,
            6,
            9
        );


        // Cuerpo
        img.setColor(
            new Color(160, 70, 90)
        );

        img.fillRect(
            12,
            22,
            25,
            15
        );


        // Hombros
        img.fillOval(
            9,
            21,
            10,
            12
        );

        img.fillOval(
            31,
            21,
            10,
            12
        );


        // Cabeza
        img.setColor(
            new Color(235, 195, 190)
        );

        img.fillOval(
            14,
            5,
            21,
            21
        );


        // Casco
        img.setColor(
            new Color(85, 42, 55)
        );

        img.fillOval(
            13,
            3,
            23,
            15
        );

        img.fillRect(
            13,
            11,
            23,
            4
        );


        // Ojos enojados
        img.setColor(
            new Color(50, 35, 40)
        );

        img.drawLine(
            18,
            15,
            22,
            17
        );

        img.drawLine(
            31,
            15,
            27,
            17
        );

        img.fillOval(
            21,
            17,
            2,
            3
        );

        img.fillOval(
            27,
            17,
            2,
            3
        );


        // Boca
        img.drawLine(
            22,
            23,
            28,
            23
        );


        // Arma
        img.setColor(
            new Color(70, 65, 70)
        );

        img.fillRect(
            34,
            25,
            12,
            4
        );


        img.setColor(
            new Color(110, 75, 85)
        );

        img.fillRect(
            35,
            28,
            4,
            6
        );


        return img;
    }


    // =========================================
    // BALA BONITA
    // =========================================

    private static GreenfootImage crearBalaBonita()
    {
        GreenfootImage img =
            new GreenfootImage(18, 10);


        // Resplandor exterior
        img.setColor(
            new Color(
                255,
                180,
                215,
                110
            )
        );

        img.fillOval(
            0,
            0,
            18,
            10
        );


        // Centro rosa
        img.setColor(
            new Color(
                255,
                105,
                170
            )
        );

        img.fillOval(
            3,
            2,
            12,
            6
        );


        // Brillito
        img.setColor(
            Color.WHITE
        );

        img.fillOval(
            6,
            3,
            4,
            3
        );


        return img;
    }
}
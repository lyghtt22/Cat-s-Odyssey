import greenfoot.*;

public class HUD extends Actor
{
    private JuegoWorld mundo;
    private Jugador jugador;
    private GestorOleadas gestor;

    private int contador = 0;


    public HUD(
        JuegoWorld mundo,
        Jugador jugador,
        GestorOleadas gestor
    )
    {
        this.mundo = mundo;
        this.jugador = jugador;
        this.gestor = gestor;

        actualizarImagen();
    }


    public void act()
    {
        contador++;

        // La guía pide no reconstruir
        // el HUD en cada ciclo.
        if (contador >= 10)
        {
            actualizarImagen();
            contador = 0;
        }
    }


    private void actualizarImagen()
    {
        int enemigos =
            mundo.getObjects(
                SoldadoEnemigo.class
            ).size();

        int balas =
            mundo.getObjects(
                Bala.class
            ).size();


        GreenfootImage img =
            new GreenfootImage(
                620,
                64
            );


        // =====================================
        // PANEL GENERAL
        // =====================================

        dibujarRedondeado(
            img,
            0,
            0,
            620,
            64,
            18,
            new Color(210, 154, 178)
        );

        dibujarRedondeado(
            img,
            2,
            2,
            616,
            60,
            17,
            new Color(255, 248, 251)
        );


        // =====================================
        // CAJITAS
        // =====================================

        dibujarCaja(
            img,
            10,
            10,
            135,
            44
        );

        dibujarCaja(
            img,
            155,
            10,
            135,
            44
        );

        dibujarCaja(
            img,
            300,
            10,
            165,
            44
        );

        dibujarCaja(
            img,
            475,
            10,
            135,
            44
        );


        // =====================================
        // TEXTO
        // =====================================

        img.setFont(
            new Font(
                "Arial",
                true,
                false,
                16
            )
        );


        img.setColor(
            new Color(
                100,
                67,
                82
            )
        );


        img.drawString(
            "OLEADA  " + gestor.getOleada(),
            27,
            38
        );


        img.drawString(
            "" + jugador.getVida(),
            210,
            38
        );


        img.drawString(
            "ENEMIGOS  " + enemigos,
            318,
            38
        );


        img.drawString(
            "BALAS  " + balas,
            495,
            38
        );


        // =====================================
        // CORAZÓN
        // =====================================

        dibujarCorazon(
            img,
            173,
            24
        );


        setImage(img);
    }


    // =========================================
    // CAJA ROSADA
    // =========================================

    private void dibujarCaja(
        GreenfootImage img,
        int x,
        int y,
        int ancho,
        int alto
    )
    {
        // Borde
        dibujarRedondeado(
            img,
            x,
            y,
            ancho,
            alto,
            12,
            new Color(226, 181, 199)
        );


        // Interior
        dibujarRedondeado(
            img,
            x + 2,
            y + 2,
            ancho - 4,
            alto - 4,
            10,
            new Color(248, 229, 236)
        );
    }


    // =========================================
    // RECTÁNGULO REDONDEADO MANUAL
    // =========================================

    private void dibujarRedondeado(
        GreenfootImage img,
        int x,
        int y,
        int ancho,
        int alto,
        int radio,
        Color color
    )
    {
        img.setColor(color);


        // Centro horizontal
        img.fillRect(
            x + radio,
            y,
            ancho - radio * 2,
            alto
        );


        // Centro vertical
        img.fillRect(
            x,
            y + radio,
            ancho,
            alto - radio * 2
        );


        // Esquina superior izquierda
        img.fillOval(
            x,
            y,
            radio * 2,
            radio * 2
        );


        // Esquina superior derecha
        img.fillOval(
            x + ancho - radio * 2,
            y,
            radio * 2,
            radio * 2
        );


        // Esquina inferior izquierda
        img.fillOval(
            x,
            y + alto - radio * 2,
            radio * 2,
            radio * 2
        );


        // Esquina inferior derecha
        img.fillOval(
            x + ancho - radio * 2,
            y + alto - radio * 2,
            radio * 2,
            radio * 2
        );
    }


    // =========================================
    // CORAZÓN DE VIDA
    // =========================================

    private void dibujarCorazon(
        GreenfootImage img,
        int x,
        int y
    )
    {
        img.setColor(
            new Color(
                210,
                94,
                133
            )
        );


        img.fillOval(
            x,
            y,
            9,
            9
        );


        img.fillOval(
            x + 7,
            y,
            9,
            9
        );


        int[] puntosX =
        {
            x,
            x + 16,
            x + 8
        };


        int[] puntosY =
        {
            y + 5,
            y + 5,
            y + 17
        };


        img.fillPolygon(
            puntosX,
            puntosY,
            3
        );
    }
}
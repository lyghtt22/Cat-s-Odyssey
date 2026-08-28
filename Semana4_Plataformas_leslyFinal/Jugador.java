import greenfoot.*;

public class Jugador extends Actor
{
    // =========================================
    // ESTADOS
    // =========================================

    private enum Estado
    {
        QUIETO,
        CAMINANDO_DERECHA,
        CAMINANDO_IZQUIERDA,
        SALTANDO
    }


    private Estado estado =
        Estado.QUIETO;


    // =========================================
    // SPRITES
    // =========================================

    private GreenfootImage imagenQuieto;

    private GreenfootImage imagenSalto;

    private GreenfootImage[] caminar;


    // =========================================
    // ANIMACIÓN
    // =========================================

    private int frameActual = 0;

    private int contadorAnimacion = 0;

    private boolean mirandoIzquierda =
        false;


    // =========================================
    // MOVIMIENTO
    // =========================================

    private int velocidad = 4;


    // =========================================
    // SALTO
    // =========================================

    private boolean saltando =
        false;

    private int tiempoSalto = 0;

    private int yInicioParabola;

    private int ySuelo;

    private double velocidadInicial =
        12.0;

    private double gravedad =
        1.0;


    // =========================================
    // DOBLE SALTO
    // =========================================

    private int saltosRealizados = 0;

    private int maxSaltos = 1;

    private boolean teclaSaltoPresionada =
        false;

    private boolean powerUpActivo =
        false;


    // =========================================
    // VIDAS
    // =========================================

    private int vidas = 3;

    private int tiempoInvulnerable = 0;


    // =========================================
    // VICTORIA
    // =========================================

    private boolean bailando =
        false;

    private int contadorBaile = 0;

    private int yBaile;


    // =========================================
    // CONSTRUCTOR
    // =========================================

    public Jugador()
    {
        cargarImagenes();
    }


    // =========================================
    // IMÁGENES
    // =========================================

    private void cargarImagenes()
    {
        imagenQuieto =
            new GreenfootImage(
                "idle.png"
            );


        imagenSalto =
            new GreenfootImage(
                "jump.png"
            );


        caminar =
            new GreenfootImage[4];


        caminar[0] =
            new GreenfootImage(
                "walk1.png"
            );

        caminar[1] =
            new GreenfootImage(
                "walk2.png"
            );

        caminar[2] =
            new GreenfootImage(
                "walk3.png"
            );

        caminar[3] =
            new GreenfootImage(
                "walk4.png"
            );


        setImage(
            imagenQuieto
        );
    }


    // =========================================
    // ACT
    // =========================================

    public void act()
    {
        // Si está celebrando,
        // bloqueamos controles.
        if (bailando)
        {
            actualizarBaile();

            return;
        }


        /*
         * IMPORTANTE:
         *
         * movimiento horizontal y salto
         * se revisan por separado.
         *
         * Así puede hacer:
         *
         * D + W
         * D + SPACE
         * A + W
         * A + SPACE
         */

        controlarMovimientoHorizontal();

        controlarSalto();

        actualizarSalto();

        actualizarAnimacion();

        gestionarInvulnerabilidad();
    }


    // =========================================
    // MOVIMIENTO HORIZONTAL
    // =========================================

    private void controlarMovimientoHorizontal()
    {
        NivelBase mundo =
            (NivelBase)getWorld();


        // =====================================
        // DERECHA - D
        // =====================================

        if (
            Greenfoot.isKeyDown("d")
        )
        {
            mirandoIzquierda =
                false;


            // Primero Chopper avanza.
            if (
                getX() < 350
            )
            {
                setLocation(
                    getX()
                    +
                    velocidad,
                    getY()
                );
            }


            // Después avanza el escenario.
            else
            {
                boolean camaraSeMovio =
                    mundo.moverCamara(
                        velocidad
                    );


                // Final del escenario.
                if (
                    !camaraSeMovio
                )
                {
                    if (
                        getX() < 750
                    )
                    {
                        setLocation(
                            getX()
                            +
                            velocidad,
                            getY()
                        );
                    }
                }
            }


            /*
             * Si está saltando,
             * NO cambiamos SALTANDO.
             */
            if (!saltando)
            {
                estado =
                    Estado
                    .CAMINANDO_DERECHA;
            }
        }


        // =====================================
        // IZQUIERDA - A
        // =====================================

        else if (
            Greenfoot.isKeyDown("a")
        )
        {
            mirandoIzquierda =
                true;


            if (
                getX() > 40
            )
            {
                setLocation(
                    getX()
                    -
                    velocidad,
                    getY()
                );
            }


            if (!saltando)
            {
                estado =
                    Estado
                    .CAMINANDO_IZQUIERDA;
            }
        }


        // =====================================
        // QUIETO
        // =====================================

        else
        {
            if (!saltando)
            {
                estado =
                    Estado.QUIETO;
            }
        }
    }


    // =========================================
    // SALTO
    // =========================================

    private void controlarSalto()
    {
        /*
         * Puede usar:
         *
         * W
         * o
         * SPACE
         *
         * Esto ayuda si tu teclado
         * tiene problemas con W + D.
         */

        boolean saltoAhora =
            Greenfoot.isKeyDown("w")
            ||
            Greenfoot.isKeyDown(
                "space"
            );


        /*
         * Detectamos una pulsación nueva.
         *
         * No queremos gastar los dos
         * saltos por mantener W apretada.
         */

        if (
            saltoAhora
            &&
            !teclaSaltoPresionada
            &&
            saltosRealizados
            <
            maxSaltos
        )
        {
            iniciarSalto();
        }


        teclaSaltoPresionada =
            saltoAhora;
    }


    private void iniciarSalto()
    {
        // Primer salto.
        if (
            saltosRealizados == 0
        )
        {
            ySuelo =
                getY();
        }


        /*
         * El segundo salto empieza
         * desde donde está Chopper
         * actualmente.
         */

        yInicioParabola =
            getY();


        tiempoSalto = 0;

        saltosRealizados++;

        saltando = true;

        estado =
            Estado.SALTANDO;
    }


    // =========================================
    // SALTO PARABÓLICO
    // =========================================

    private void actualizarSalto()
    {
        if (saltando)
        {
            tiempoSalto++;


            /*
             * Fórmula:
             *
             * y(t) =
             * y0
             * - v0*t
             * + 0.5*g*t²
             */

            double y =
                yInicioParabola
                -
                velocidadInicial
                *
                tiempoSalto
                +
                0.5
                *
                gravedad
                *
                tiempoSalto
                *
                tiempoSalto;


            /*
             * Solo aterriza cuando
             * vuelve al suelo original.
             */

            if (
                y >= ySuelo
                &&
                tiempoSalto > 1
            )
            {
                y =
                    ySuelo;


                saltando =
                    false;


                saltosRealizados =
                    0;


                estado =
                    Estado.QUIETO;
            }


            setLocation(
                getX(),
                (int)y
            );
        }
    }


    // =========================================
    // POWER UP
    // =========================================

    public void activarPowerUp()
    {
        if (!powerUpActivo)
        {
            powerUpActivo =
                true;


            /*
             * Los 5 algodones
             * desbloquean DOBLE SALTO.
             */

            maxSaltos = 2;
        }
    }


    // =========================================
    // VIDAS
    // =========================================

    public int getVidas()
    {
        return vidas;
    }


    public void recibirDanio()
    {
    // No recibe daño mientras baila
    if (bailando)
    {
        return;
    }

    // Mientras parpadea es invulnerable
    if (tiempoInvulnerable > 0)
    {
        return;
    }

    vidas--;

    tiempoInvulnerable = 120;

    // =============================
    // TODAVÍA TIENE VIDAS
    // =============================

    if (vidas > 0)
    {
        int nuevaX =
            getX() - 55;

        if (nuevaX < 45)
        {
            nuevaX = 45;
        }

        setLocation(
            nuevaX,
            getY()
        );

        return;
    }


    // =============================
    // GAME OVER
    // =============================

    getImage().setTransparency(255);

    World mundoActual =
        getWorld();

    mundoActual.showText(
        "GAME OVER",
        400,
        220
    );

    mundoActual.showText(
        "REINTENTANDO NIVEL...",
        400,
        255
    );

    Greenfoot.delay(60);


    // =============================
    // REINICIAR NIVEL ACTUAL
    // =============================

    if (mundoActual instanceof Mundo1)
    {
        Greenfoot.setWorld(
            new Mundo1()
        );
    }

    else if (mundoActual instanceof Mundo2)
    {
        Greenfoot.setWorld(
            new Mundo2()
        );
    }

    else if (mundoActual instanceof Mundo3)
    {
        Greenfoot.setWorld(
            new Mundo3()
            );
        }
    }


    // =========================================
    // PARPADEO
    // =========================================

    private void gestionarInvulnerabilidad()
    {
        if (
            tiempoInvulnerable > 0
        )
        {
            tiempoInvulnerable--;


            /*
             * Alterna transparencia.
             */

            if (
                (
                    tiempoInvulnerable
                    /
                    6
                )
                %
                2
                ==
                0
            )
            {
                getImage()
                    .setTransparency(
                        80
                    );
            }

            else
            {
                getImage()
                    .setTransparency(
                        255
                    );
            }
        }

        else
        {
            getImage()
                .setTransparency(
                    255
                );
        }
    }


    // =========================================
    // BAILE
    // =========================================

    public void bailarVictoria()
    {
        bailando =
            true;


        contadorBaile =
            0;


        yBaile =
            getY();


        setImage(
            new GreenfootImage(
                imagenQuieto
            )
        );
    }


    private void actualizarBaile()
    {
        contadorBaile++;


        if (
            contadorBaile
            %
            16
            <
            8
        )
        {
            setRotation(
                -10
            );


            setLocation(
                getX(),
                yBaile - 3
            );
        }

        else
        {
            setRotation(
                10
            );


            setLocation(
                getX(),
                yBaile
            );
        }
    }


    // =========================================
    // ANIMACIÓN
    // =========================================

    private void actualizarAnimacion()
    {
        // QUIETO
        if (
            estado ==
            Estado.QUIETO
        )
        {
            GreenfootImage imagen =
                new GreenfootImage(
                    imagenQuieto
                );


            if (mirandoIzquierda)
            {
                imagen
                    .mirrorHorizontally();
            }


            setImage(
                imagen
            );
        }


        // CAMINANDO DERECHA
        else if (
            estado
            ==
            Estado
            .CAMINANDO_DERECHA
        )
        {
            animarCaminata(
                false
            );
        }


        // CAMINANDO IZQUIERDA
        else if (
            estado
            ==
            Estado
            .CAMINANDO_IZQUIERDA
        )
        {
            animarCaminata(
                true
            );
        }


        // SALTANDO
        else if (
            estado
            ==
            Estado.SALTANDO
        )
        {
            GreenfootImage imagen =
                new GreenfootImage(
                    imagenSalto
                );


            if (mirandoIzquierda)
            {
                imagen
                    .mirrorHorizontally();
            }


            setImage(
                imagen
            );
        }
    }


    // =========================================
    // CAMINATA
    // =========================================

    private void animarCaminata(
        boolean izquierda
    )
    {
        contadorAnimacion++;


        if (
            contadorAnimacion >= 6
        )
        {
            frameActual++;


            if (
                frameActual
                >=
                caminar.length
            )
            {
                frameActual = 0;
            }


            GreenfootImage imagen =
                new GreenfootImage(
                    caminar[
                        frameActual
                    ]
                );


            if (izquierda)
            {
                imagen
                    .mirrorHorizontally();
            }


            setImage(
                imagen
            );


            contadorAnimacion = 0;
        }
    }
    
    // =========================================
    // INFORMACIÓN DEL SALTO
    // PARA EL JEFE DEL NIVEL 2
    // =========================================

    public boolean estaSaltando()
    {
        return saltando;
    }


    public boolean estaCayendo()
    {
        if (!saltando)
        {
        return false;
        }


        /*
         * En una parábola:
         *
         * primero sube,
         * llega al punto más alto
         * y después cae.
         *
         * v0 / gravedad indica
         * aproximadamente cuándo
         * comienza a caer.
         */

        double puntoMasAlto =
            velocidadInicial
            /
            gravedad;


        return
        tiempoSalto
        >
        puntoMasAlto;
    }


    // =========================================
    // REBOTE AL PISAR AL JEFE
    // =========================================

    public void rebotarSobreEnemigo()
    {
        /*
         * Comenzamos una nueva
         * parábola desde la posición
         * actual.
         */

        yInicioParabola =
            getY();


        tiempoSalto = 0;

        saltando = true;

        saltosRealizados = 1;

        estado =
            Estado.SALTANDO;
    }


    // =========================================
    // POWER UP EXCLUSIVO NIVEL 2
    // =========================================

    public void activarPowerUpNivel2()
    {
        /*
         * Super salto.
         *
         * NO es el mismo power-up
         * del Nivel 1.
         */

        velocidadInicial =
        15.0;
    }
}
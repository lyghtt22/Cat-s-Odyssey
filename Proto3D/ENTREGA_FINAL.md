# ENTREGA FINAL — PROTO3D / SUGAR BLOOM

**Asignatura:** Programación de Videojuegos
**Docente:** Ernesto Vivanco
**Integrantes:** Lesly Chuquipoma y Anderson Mejías
**Motor de desarrollo:** Unity 2021.1.2f1
**Lenguaje:** C#
**Plataforma:** PC / Windows
**Fecha de entrega:** 29 de octubre de 2026

---

## 1. Descripción del videojuego

**Proto3D / Sugar Bloom** es un videojuego de carreras arcade en 3D, inspirado en las mecánicas generales de juegos como Mario Kart y en la ambientación de fantasía de mundos de dulces.

El proyecto comenzó como un prototipo de conducción en Unity y evolucionó hacia una experiencia de carreras que incorpora circuitos temáticos, vehículos seleccionables, rivales controlados por inteligencia artificial, objetos especiales, trampas y una interfaz personalizada.

Se buscó desarrollar una identidad visual propia, utilizando colores rosados, decoraciones de chocolate, caramelos y malvaviscos.

Este videojuego corresponde a la actividad de desarrollo en Unity y es independiente del proyecto Cat's Odyssey.

## 2. Objetivo del proyecto

Desarrollar un videojuego de carreras 3D funcional, utilizando Unity y programación en C#, que permita controlar un vehículo, competir contra rivales CPU y completar carreras con diferentes elementos interactivos.

Los principales objetivos de desarrollo fueron:

- Implementar conducción y lógica de carreras.
- Incorporar rivales controlados por CPU.
- Crear un sistema de vueltas, posiciones y resultados.
- Desarrollar una interfaz clara para el jugador.
- Incorporar circuitos y vehículos seleccionables.
- Agregar power-ups, obstáculos y trampas.
- Mejorar la presentación visual y sonora.
- Realizar pruebas de funcionamiento y documentar los resultados.

## 3. Funcionalidades implementadas

### 3.1. Sistema de carreras

Se desarrolló un sistema de carreras de tres vueltas con detección de checkpoints, clasificación de posiciones y finalización al cruzar la meta.

El videojuego contempla cuatro participantes: un jugador y tres vehículos controlados por CPU.

Al finalizar la carrera se muestra la posición obtenida y el tiempo registrado. También se permite reiniciar la competencia.

### 3.2. Vehículos y pilotos

Se incorporó un sistema de selección de vehículos y personajes desde el menú principal.

Los vehículos presentan diferencias visuales y ajustes de comportamiento. Se incluyó además un sistema sencillo de desbloqueo de un vehículo mediante los resultados de carrera.

El menú contiene una vista previa tridimensional del kart seleccionado.

### 3.3. Circuitos

Se incorporaron dos circuitos seleccionables:

- **Sugar Bloom:** escenario de carreras con ambientación de dulces.
- **Cocoa Cloud:** circuito alternativo de fantasía con decoración temática.

Los circuitos utilizan rutas y checkpoints para organizar el recorrido de los competidores.

### 3.4. Rivales controlados por CPU

Se amplió el sistema de rivales hasta contar con tres competidores automáticos.

Los vehículos CPU utilizan rutas de seguimiento y ajustes de trayectoria para recorrer los circuitos. También se incorporaron comportamientos básicos para evitar otros vehículos y utilizar determinadas mecánicas del juego.

### 3.5. Power-ups y objetos especiales

Se incorporaron cajas sorpresa que permiten conseguir diferentes habilidades:

- **Turbo de fresa:** aumenta temporalmente la velocidad.
- **Escudo de caramelo:** permite protegerse de un impacto.
- **Bola de chocolate:** proyectil que puede afectar a otros vehículos.
- **Nube de malvavisco:** objeto especial con efecto sobre la carrera.

También se agregaron avisos visuales para indicar cuándo se obtiene o utiliza un poder.

### 3.6. Trampas y elementos del circuito

Se incorporaron obstáculos, efectos de ralentización y advertencias para el jugador.

Se desarrolló además un carril alternativo señalizado que puede proporcionar una bonificación temporal de velocidad.

### 3.7. Cámara y HUD

El videojuego permite alternar entre tres modos de cámara.

La interfaz muestra información relevante durante la carrera:

- Posición del jugador.
- Número de vuelta.
- Tiempo transcurrido.
- Velocidad del vehículo.
- Minimap del circuito.
- Power-up disponible.
- Estado del turbo.
- Advertencias de peligros y ataques.

### 3.8. Menú y presentación

Se desarrolló un menú principal con selección de vehículo, piloto y circuito.

La presentación incluye una vista previa del vehículo, fondo ilustrado, instrucciones de controles, efectos visuales y botones de interacción.

Se incorporaron también música y efectos de sonido generados mediante el sistema de audio del proyecto.

## 4. Controles

| Control                   | Función                         |
| ------------------------- | ------------------------------- |
| W / Flecha arriba         | Acelerar                        |
| S / Flecha abajo          | Frenar o retroceder             |
| A y D / Flechas laterales | Girar                           |
| Espacio                   | Freno de mano                   |
| F                         | Turbo                           |
| E                         | Utilizar objeto                 |
| C                         | Cambiar cámara                  |
| R                         | Recuperar vehículo              |
| Esc                       | Pausar o reanudar               |
| F2                        | Modo de demostración automática |
| M                         | Activar o desactivar sonido     |
| Enter                     | Reiniciar después de finalizar  |

## 5. Organización técnica del proyecto

El videojuego utiliza scripts en C# con responsabilidades diferenciadas.

| Archivo                | Responsabilidad                                                            |
| ---------------------- | -------------------------------------------------------------------------- |
| `RacePrototypeGame.cs` | Control de carrera, movimiento, vueltas, posiciones, cámaras y resultados. |
| `RaceCandyItems.cs`    | Cajas sorpresa, power-ups, trampas y efectos especiales.                   |
| `RaceFantasyTrack.cs`  | Construcción y configuración del circuito de fantasía.                     |
| `RacePresentation.cs`  | Selección de vehículos, presentación y audio.                              |
| `RaceMenuView.cs`      | Interfaz del menú y vista previa del kart.                                 |
| `RacePowerHud.cs`      | Indicadores de poderes y advertencias.                                     |
| `RaceVerification.cs`  | Herramientas para verificar el funcionamiento del proyecto.                |

Los archivos principales se encuentran en `Proto3D/Assets/RacePrototype/`.

## 6. Evolución y mejoras del videojuego

Durante el desarrollo se realizaron diferentes avances.

**Primera etapa: prototipo inicial**

Se preparó el proyecto Unity, se incorporaron los recursos de pista y vehículos y se desarrolló un sistema básico de conducción y competencia.

**Segunda etapa: ampliación del juego**

Se trabajó en las tres vueltas, clasificación de posiciones, comportamiento de rivales y finalización de carreras.

**Tercera etapa: mejoras visuales y mecánicas**

Se implementaron circuitos temáticos, selección de vehículos, power-ups, trampas, cámaras alternativas y elementos visuales de dulces.

**Cuarta etapa: correcciones y verificación**

Se realizaron ajustes de contacto de los vehículos con el suelo, comportamiento de la cámara, interfaz, presentación y pruebas de finalización de carreras.

## 7. Trabajo realizado por los integrantes

### Lesly Chuquipoma

Participación documentada en:

- Definición y propuesta de la dirección visual del juego de carreras con temática de dulces.
- Propuesta de incorporar power-ups, atajos, trampas y nuevas perspectivas de cámara.
- Coordinación de avances y comunicación con el profesor.
- Organización y publicación de avances mediante GitHub.
- Revisión del videojuego funcionando en su computador.
- Preparación de la presentación y documentación de entrega.

### Anderson Mejías

Participación documentada como integrante del equipo en:

- Coordinación para la revisión del prototipo.
- Comunicación de dificultades encontradas al intentar abrir y ejecutar una copia del proyecto en otro computador.
- Participación en la preparación de la demostración del videojuego.

**Atribución de programación:** los commits de la versión reciente aparecen publicados desde la cuenta del repositorio `lyghtt22`. Esto no permite identificar de forma independiente qué código implementó cada integrante. La distribución detallada de scripts y tareas técnicas debe ser confirmada por ambos integrantes antes de presentar el documento como versión definitiva.

## 8. Pruebas realizadas

El repositorio contiene registros de verificación correspondientes al 25 de octubre de 2026.

### Carreras completas

Se registró una prueba de tres vueltas en cada circuito:

| Circuito    | Tiempo registrado | Posición |
| ----------- | ----------------: | -------: |
| Sugar Bloom |   196,93 segundos |      1.º |
| Cocoa Cloud |   193,73 segundos |      2.º |

Estos resultados corresponden a los registros de prueba automatizada almacenados en el proyecto y no representan necesariamente una partida manual.

### Mecánicas verificadas

Los registros incluyen comprobaciones satisfactorias de:

- Tres vueltas completas.
- Validación de checkpoints.
- Rechazo de cruces incorrectos.
- Reinicio de carrera.
- Pausa y reanudación.
- Tres perspectivas de cámara.
- Turbo y escudo.
- Proyectil de chocolate.
- Avisos de trampas y ataques.
- Menú y selección de personajes.
- Contacto de los vehículos con la pista.

### Compilación

Se registraron compilaciones de Windows completadas correctamente.

El último registro de compilación incluido en `Verification/2026-10-25/powers/build-result.txt` indica:

- Resultado: `Succeeded`
- Errores: `0`
- Advertencias: `0`

Las evidencias se encuentran en:

`Proto3D/Verification/2026-10-25/`

## 9. Dificultades y soluciones

Durante el desarrollo se identificaron dificultades relacionadas con la conducción, el contacto de los vehículos con la pista, el comportamiento de la cámara y la correcta finalización de las carreras.

Se realizaron modificaciones en el código de movimiento y posicionamiento, y se incorporaron herramientas de prueba para verificar que los vehículos completaran el circuito.

También se reportaron dificultades al abrir una copia comprimida del proyecto en otro computador. Este problema debe diferenciarse de los resultados de las pruebas realizadas en el entorno donde se desarrolló el juego.

## 10. Retroalimentación del docente

El 20 de octubre, el profesor Ernesto Vivanco destacó que el videojuego tenía una estética clara y que presentaba gran parte de los elementos esperados para un juego de vehículos.

A partir de esa retroalimentación, se conversó sobre la incorporación de rivales CPU, power-ups, caminos alternativos, trampas y nuevas perspectivas de cámara.

Estas propuestas orientaron la evolución posterior del prototipo.

## 11. Instrucciones para ejecutar el videojuego

1. Abrir el proyecto `Proto3D` desde Unity Hub.
2. Utilizar Unity `2021.1.2f1`.
3. Abrir la escena `Assets/Scenes/RacePrototype.unity`.
4. Esperar la importación y compilación de los recursos.
5. Presionar **Play**.
6. Seleccionar vehículo, piloto y circuito.
7. Iniciar la carrera desde el menú.

Para trasladar el proyecto a otro computador se deben conservar las carpetas `Assets`, `Packages`, `ProjectSettings` y sus archivos de configuración necesarios.

No es necesario distribuir las carpetas temporales generadas por Unity, como `Library`, `Temp` y `Logs`.

## 12. Conclusión

Proto3D / Sugar Bloom evolucionó desde un prototipo de carreras a un videojuego arcade 3D con una identidad visual definida, circuitos temáticos, rivales automáticos, vehículos seleccionables, power-ups y sistemas de interfaz.

El desarrollo permitió aplicar conocimientos de programación orientada a objetos, lógica de videojuegos, manejo de escenas, interfaces, organización de scripts, pruebas y control de versiones.

La entrega incluye el código fuente y las evidencias de pruebas disponibles, manteniendo separado el proyecto Cat's Odyssey.

---

**Repositorio:** [https://github.com/lyghtt22/Cat-s-Odyssey](https://github.com/lyghtt22/Cat-s-Odyssey)

**Rama de entrega:** `Entrega-Final-Proto3D`

**Archivo de documentación:** `Proto3D/ENTREGA_FINAL.md`

**Equipo:** Lesly Chuquipoma y Anderson Mejías


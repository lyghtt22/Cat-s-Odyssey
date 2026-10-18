# Proto3D — Juego de carreras (Unity)

Prototipo de un **juego de carreras estilo Mario Kart**, con estética de dulces **Sugar Bloom — Candy Grand Prix**: tres vueltas en una pista ovalada, un rival controlado por IA y turbo.

> Este avance está en la rama `Avance-Proto3D` del repositorio `lyghtt22/Cat-s-Odyssey`. Las otras actividades se conservan en sus respectivas ramas.

## Integrantes

- Lesly Chuquipoma
- Anderson Mejías

## Qué hay hoy

- Auto manejable con **flechas / WASD**, freno de mano con **Espacio** y cámara de seguimiento.
- Un rival con **IA propia** que sigue la ruta de la pista y reduce la velocidad en curvas.
- Carrera de **3 vueltas**, cuenta regresiva y checkpoints ordenados.
- Interfaz con velocidad, vuelta actual, tiempo de carrera y pantalla de meta con tiempo final.
- **Turbo con Shift izquierdo** o al pasar por la zona de impulso, con indicador de recarga y estela visual.
- Recuperación del auto con **R** y recuperación automática al volcar o caer fuera del circuito.
- **F2** activa o desactiva la demostración con piloto automático. **Enter** reinicia al terminar.

## Pendiente (la idea del juego)

- Obstáculos que afecten la conducción y objetos tipo Mario Kart.
- Más pistas y rivales, menú y selección de dificultad.
- Posiciones en vivo y tiempos individuales por vuelta.
- Mejorar el manejo y el comportamiento de la IA.

## Instalación (paso a paso)

### 1. Requisitos

- Unity Hub y **Unity 2021.1.2f1**, versión registrada en `Proto3D/ProjectSettings/ProjectVersion.txt`.
- Git y **Git LFS**, necesario para descargar algunas texturas de la pista.

### 2. Clonar la rama del avance

```bash
git lfs install
git clone -b Avance-Proto3D --single-branch https://github.com/lyghtt22/Cat-s-Odyssey.git
cd Cat-s-Odyssey
git lfs pull
```

### 3. Abrir en Unity

1. En Unity Hub, selecciona **Add / Agregar proyecto desde disco**.
2. Elige la carpeta **`Cat-s-Odyssey/Proto3D`**.
3. Abre con **Unity 2021.1.2f1** y espera a que termine la importación y compilación.

### 4. Abrir la carrera y jugar

1. En la ventana Project, abre **`Assets/Scenes/RacePrototype.unity`**.
2. Pulsa **Play** y espera la cuenta regresiva.
3. Completa tres vueltas pasando por los checkpoints de la pista.

## Pista y recursos

Esta rama **sí incluye la carpeta `Assets/CartoonTracksPack1/`**. Algunas texturas están guardadas mediante **Git LFS**, según `.gitattributes`; ejecuta `git lfs pull` antes de abrir el proyecto.

Si faltan texturas o aparecen errores al descargar recursos, revisa la salida de Git LFS y confirma que los archivos se hayan descargado completamente.

## Estructura

- `Proto3D/Assets/Scenes/RacePrototype.unity` — escena de la carrera.
- `Proto3D/Assets/RacePrototype/RacePrototypeGame.cs` — conducción, rival IA, vueltas, checkpoints, turbo, recuperación, cámara e interfaz.
- `Proto3D/Assets/RacePrototype/Editor/` — herramientas de preparación, estilo visual y verificación del avance.
- `Proto3D/Assets/CartoonTracksPack1/` — recursos de la pista.
- `Proto3D/Assets/Standard Assets/` — componentes de Unity Standard Assets, incluido `CarController`.
- `Proto3D/Packages/` — dependencias del proyecto.
- `Proto3D/ProjectSettings/` — configuración de Unity.

## Trabajo en equipo

- No subir `Library/`, `Temp/`, `Obj/`, `Logs/` ni `UserSettings/`: están excluidos en `.gitignore`.
- Los archivos **`.meta` sí se suben** junto con sus recursos.
- Mantener `.gitattributes` y usar Git LFS para los archivos que lo requieren.
- Coordinar los cambios de `RacePrototype.unity` antes de editar la escena o ejecutar herramientas que la modifiquen.
- Guardar los avances de este proyecto en **`Avance-Proto3D`**.

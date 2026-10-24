# EcoBot: Misión Reciclaje

Videojuego serio (*serious game*) de plataformas 2D hecho en **Unity 6 (6000.3.x)** para sensibilizar sobre el reciclaje.
Controlas a **EcoBot**, recoges 12 residuos (cada uno muestra un dato sobre reciclaje), esquivas slimes de contaminación
y agua sucia, y llevas todo a la caneca verde del final del nivel.

## Cómo abrir el proyecto
1. Clona o descarga este repositorio.
2. En **Unity Hub → Add**, selecciona la carpeta del proyecto y ábrela con Unity 6 (6000.3.x).
3. La primera vez Unity regenera la carpeta `Library/` (puede tardar unos minutos). Si pide actualizar la versión, acepta.
4. Si no se crean solas las escenas, usa el menú **EcoBot → Preparar proyecto**.
5. Abre la escena **MainMenu** y presiona **Play**.

## Controles
| Acción | Tecla |
|---|---|
| Mover | A / D o flechas |
| Saltar | Espacio / W / ↑ |
| Pausa | Esc / P |
| Eliminar un slime | Saltar sobre él |

> Si el teclado no responde: *Edit → Project Settings → Player → Active Input Handling = Both*.

## Estructura
```
Assets/
├── Scenes/                 MainMenu, GameScene
└── _Project/
    ├── Scripts/
    │   ├── Core/           Juego, nivel, enemigos, coleccionables, cámara, audio, input
    │   ├── Player/         EcoPlayer (movimiento, salto, física 2D)
    │   ├── UI/             Menú principal, HUD, animaciones de UI
    │   └── Editor/         Preparación automática del proyecto
    └── Resources/          Sprites y audio (originales)
Packages/  ProjectSettings/
```

## Características técnicas
- Menú con Canvas (Scale With Screen Size 1920x1080), botones JUGAR / CRÉDITOS / SALIR.
- Nivel con Tilemap + TilemapCollider2D (Composite) + CompositeCollider2D.
- Jugador con Rigidbody2D, animaciones Idle / Walk / salto / caída.
- Cámara con zona muerta y límites.
- Nivel, HUD y menús construidos por código al dar Play.

## Créditos
Todo el arte y el audio son originales, generados para este prototipo (sin recursos de terceros).
Personaliza los créditos en `Assets/_Project/Scripts/UI/EcoMenuBuilder.cs` (constante `CreditsText`).

## Control de versiones
El `.gitignore` excluye `Library/`, `Temp/`, `Logs/`, `UserSettings/` y archivos generados de IDE.
Los archivos `.meta` **sí** se versionan.

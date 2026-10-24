# EcoBot: Misión Reciclaje (Unity 6, 2D) — Serious Game

Plataformas 2D educativo, distinto al del PDF: controlas a EcoBot, recoges 12 residuos
(cada uno muestra un dato sobre reciclaje), esquivas slimes de contaminación y agua sucia,
y llevas todo a la caneca verde del final.

## Cómo abrirlo
1. Unity Hub → Add → selecciona esta carpeta (EcoBot_Unity) y ábrela con tu Unity 6.
   (Si pide actualizar la versión del proyecto, acepta.)
2. Espera a que compile. Se crean solas las carpetas del taller, las escenas
   **MainMenu** (índice 0) y **GameScene** (índice 1) y Build Settings. Si no ocurre:
   menú **EcoBot → Preparar proyecto**.
3. Abre MainMenu y presiona **Play**. (También funciona dándole Play en cualquier escena.)

## Controles
A/D o flechas: mover · Espacio/W/↑: saltar · Esc/P: pausa · Salta SOBRE los slimes para eliminarlos.

## Qué usa del PDF 2
- Menú: Canvas (Scale With Screen Size 1920x1080, Match 0.5), BackgroundImage primero en el Canvas,
  título, 3 botones (JUGAR / CRÉDITOS / SALIR), CreditsPanel oculto + botón VOLVER,
  EventSystem y MainMenuManager (PlayGame, OpenCredits, CloseCredits, QuitGame).
- Tilemap: Grid + Tilemap + TilemapCollider2D (Composite Operation = Merge) + CompositeCollider2D
  + Rigidbody2D Static. Tiles cortados con Grid By Cell Size (16x16), Point (no filter), sin compresión.
- Player: Rigidbody2D (Continuous, Freeze Rotation Z, Gravity 3, Interpolate), BoxCollider2D, flip por escala X.
- Animación Idle / Walk según el parámetro Speed (EcoSpriteAnimator) + frames de salto/caída.
- Cámara: zona muerta (Dead Zone) y límites minX/maxX (EcoCameraFollow2D).

## Notas
- Todo el arte y audio es original, generado para este prototipo (sin recursos de terceros).
- El nivel, el HUD y los menús se construyen por código al darle Play, así que la Hierarchy
  de las escenas está vacía en modo edición; los objetos aparecen al ejecutar.
- Personaliza los créditos (nombres, referencias) en Scripts/UI/EcoMenuBuilder.cs (constante CreditsText).
- Teclado sin respuesta: Edit → Project Settings → Player → Active Input Handling = Both.

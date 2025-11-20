# Juego de Carretera Infinita 2D

Proyecto de videojuego educativo/procedimental hecho en Unity donde conduces un carro por una carretera infinita generada pieza por pieza de forma aleatoria. El objetivo es avanzar todo lo posible sin chocar con obstáculos, mientras la velocidad del coche varía dinámicamente y el jugador debe sortear diferentes tipos de caminos y terrenos.

---

## Características principales

- **Carretera vertical infinita**: se generan piezas o tramos (prefabs) de carretera que se conectan perfectamente entre sí.
- **Sistema de unión inteligente**: cada pieza tiene un tipo de inicio y fin, lo que garantiza que las pistas encajen sin errores visuales ni saltos.
- **Variedad de caminos**: soporta curvas, bifurcaciones, carreteras dobles, zonas de pasto/acera, etc.
- **Obstáculos dinámicos**: aparecen autos y otros obstáculos de manera aleatoria durante el avance.
- **Optimización**: las piezas viejas se destruyen automáticamente cuando salen del rango de la cámara, evitando sobrecarga.
- **Velocidad variable**: la velocidad del coche aumenta con el tiempo y disminuye por colisiones o ciertos terrenos especiales.

---

## Estructura del proyecto
Assets/
├── Resources/
│ └── Calles/ # Prefabs de tramos o piezas (cada uno con script Pieza.cs)
├── Scenes/
│ └── Main.unity
├── Scripts/
│ ├── MotorCarreteras.cs
│ ├── Pieza.cs
│ ├── Coche.cs
│ ├── ControladorCoche.cs
│ ├── CocheObstaculo.cs
│ └── ...

- **Pieza.cs:** Define los campos `inicioTipo` y `finTipo`.
- **MotorCarreteras.cs:** Sistema que selecciona, instancia, posiciona y destruye piezas en tiempo real.
- **Coche.cs / ControladorCoche.cs:** Lógica del jugador, controles y movimiento.
- **CocheObstaculo.cs:** Lógica de los obstáculos interactivos.

---

## Lógica de generación de carretera

1. Se arranca con un contenedor inicial (ejemplo: `Calle`) en la escena.
2. Al avanzar, se instancian nuevas piezas compatibles directo desde Resources/Calles, uniéndolas al final de la última pieza activa.
3. El juego mide dinámicamente la distancia a la cámara para anticipar la generación y evitar huecos.
4. Todo tramo fuera del campo de visión se elimina automáticamente (incluso el objeto fijo del arranque).

---

## Instalación y ejecución

1. Clona o descarga el repositorio.
2. Abre el proyecto en Unity (`2021.x` o superior recomendado).
3. Asegúrate que tus prefabs de carretera (con `Pieza.cs`) estén en `Assets/Resources/Calles/`.
4. Abre la escena principal y ejecuta (`Play`).
5. Modifica, agrega o ajusta las piezas para expandir el sistema procedural.

---

## Personalización

- Puedes añadir nuevas piezas sólo creando prefabs compatibles (`inicioTipo` y `finTipo`).
- Para añadir obstáculos, solo crea más prefabs con `CocheObstaculo.cs` o extiende lógica en `MotorCarreteras.cs`.
- Ajusta el tamaño, velocidad y margen en `MotorCarreteras.cs` para cambiar la dificultad.

---

## Créditos y licencia

- Autor: [Tu Nombre o usuario GitHub]
- Licencia: [MIT / GPL / CC-BY o la que prefieras]

---

¡Contribuciones y sugerencias bienvenidas!



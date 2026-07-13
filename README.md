# 🎮 CYBER DRIFTER

> **Endless runner cyberpunk 3D** — esquiva, salta, desliza y sobrevive en una ciudad futurística neón.

![cyber-drifter](https://img.shields.io/badge/Unity-6000.5.3f1-000?logo=unity)
![platform](https://img.shields.io/badge/Platform-PC%20%7C%20Mobile-cyan)
![license](https://img.shields.io/badge/License-MIT-magenta)

---

## 🌆 Concepto

Eres un **cyber runner** deslizándote a toda velocidad por los callejones de Neo-Mexicali. La ciudad entera es tu pista, los edificios son tu decorado, y los orbes de neón tu única esperanza de score.

**Tematica:** Cyberpunk + Synthwave + Endless Runner

---

## 🎯 Gameplay

| Mecánica | Controles | Efecto |
|---|---|---|
| ⬅️ Izquierda | `A` / `←` / Toque izquierdo | Cambio de carril |
| ➡️ Derecha | `D` / `→` / Toque derecho | Cambio de carril |
| ⬆️ Saltar | `Space` / `W` / `↑` / Toque centro | Salto (esquiva obstáculos bajos) |
| ⬇️ Deslizar | `S` / `↓` / Toque abajo | Slide (esquiva obstáculos altos) |

**Velocidad aumenta progresivamente** de 12 m/s hasta 40 m/s. Los puntos se acumulan automáticamente + 50 por orbe neón recogido.

---

## 🎨 Características

- ✅ **Cámara cinemática** que sigue al jugador con offset dinámico + shake on game over
- ✅ **3 carriles** con cambio suave interpolado
- ✅ **Obstáculos procedurales** en colores neón aleatorios
- ✅ **Orbes flotantes** con animación sinusoidal + rotación
- ✅ **Edificios infinitos** que se generan adelante y se reciclan
- ✅ **Suelo con efecto parallax** + líneas de neón decorativas
- ✅ **Sistema de partículas** tipo trail del jugador
- ✅ **High score persistente** con PlayerPrefs
- ✅ **UI neón** estilo synthwave con sombras y outlines
- ✅ **Fog atmosférico** violeta para sensación cyberpunk
- ✅ **Mobile + Desktop** responsive

---

## 🚀 Cómo correr el juego

### Requisitos
- **Unity 6000.5.3f1** o superior (Unity 6)
- macOS / Windows / Linux

### Pasos
1. **Abrir el proyecto** en Unity Hub: `Add project from disk` → selecciona esta carpeta
2. **Espera** a que importe los assets
3. **Menú:** `Cyber Drifter → Build Scene` (en la barra superior del Editor)
4. Click **Play** ▶️ en el Editor
5. **Opcional:** `File → Build Settings → Build` para generar `.app` / `.exe`

---

## 🎮 Controles de testing

Una vez en Play mode:
- `A/D` o `←/→` — mover entre carriles
- `Space` — saltar
- `S` o `↓` — deslizar
- `R` — reiniciar (cuando estés en game over)

---

## 📦 Estructura

```
juegounity/
├── Assets/
│   ├── Scripts/        # Lógica del juego
│   ├── Editor/         # Script para construir escena
│   ├── Prefabs/        # Obstáculos, orbes, edificios
│   ├── Scenes/         # CyberDrifter.unity
│   └── Materials/      # Materiales neón
├── Packages/
│   └── manifest.json   # Dependencias
├── ProjectSettings/    # Config del proyecto
└── README.md           # ← tú estás aquí
```

---

## 🛠️ Stack técnico

- **Unity 6** (6000.5.3f1)
- **C#** scripts vanilla (sin dependencias externas)
- **Standard Shader** con emission maps para neón
- **UI legacy** (uGUI) con CanvasScaler responsive

---

## 🎯 Roadmap (ideas para extender)

- [ ] Power-ups: shield, slow-mo, magnet
- [ ] Más tipos de obstáculos (móviles, expandibles)
- [ ] Boss cada 1000 puntos
- [ ] Leaderboard online
- [ ] Skin selector para el personaje
- [ ] Sistema de combos con orbes
- [ ] Efectos de post-processing (bloom, chromatic aberration)

---

## 📞 Créditos

**Desarrollado por:** [AGUITECH](https://aguitech.com)
**Repositorio:** github.com/aguitech/juegounity
**Licencia:** MIT

🎮 Hecho con ❤️ y mucho neón
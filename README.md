# Galaga Reborn

**Galaga Reborn** is a 2D/3D arcade-style retro space shooter. Developed using **Unity 6**, the game pays homage to classic arcade shoot-'em-ups while introducing modern mobile mechanics, secure online integration, and an intense scoring system.

## Gameplay
The year is 2026. Deep space is under attack by relentless waves of hostile alien swarms. As the vanguard of the Starfleet, you control the galaxy's ultimate fighter ship. Your mission is simple: blast through enemy formations, dodge deadly kamikaze dives, activate powerful weapon upgrades, and claim the ultimate high score!

## 🚀 Core Features

*   **Mobile-Optimized Touch Controls:** Features a modern touch-drag control scheme. The spaceship perfectly tracks your finger movement along the X-axis, paired with an automatic firing system when the screen is pressed for fluid mobile gameplay.
*   **PlayFab Backend Integration:** Includes a custom secure UI panel for user authentication. Players can seamlessly log in or register accounts via PlayFab, featuring dynamic, localized status text responses for server states.
*   **Elite Hardcore Combo System:** Rewards skilled players with a scaling scoring multiplier. Chaining quick successive alien kills increases your streak, unlocking up to a **x5 Score Multiplier** accompanied by flashing UI notifications.
*   **Double Laser Power-Up:** Dynamically upgrades your weapon configuration into a dual-firing laser stream to shred through heavy enemy grid formations.
*   **On-the-Fly Dynamic Localization:** Supports real-time, runtime language switching between **English** and **Italian**.

## 🛠️ Technical Specifications

*   **Game Engine:** Unity 6
*   **Programming Language:** C#
*   **UI Framework:** TextMeshPro (TMP) for pixel-perfect font rendering and custom UI scaling
*   **Input Pipeline:** Dual-input fallback system supporting touch inputs (`Input.GetTouch`) on mobile devices and native keyboard bindings (`Input.GetAxisRaw` / Spacebar) inside the Unity Editor environment.

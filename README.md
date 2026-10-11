# Castle Mage

![Gameplay](Docs/gameplay.gif)

**Play in the browser:** [Castle Mage on itch.io](https://kon-koutsioumpas.itch.io/castle-mage)

A short arcade game made in Unity 6. You hold a castle wall. Enemies cross the field. The mage fires on his own, but only at targets inside a range and a forward cone. You move him so the cone covers the threat that matters.

The GIF above is `Docs/gameplay.gif`. The play link is a placeholder until the WebGL build is uploaded.

## Controls

- **Left / Right arrows** move the mage along the rampart. He stays inside the camera frame.
- **Shooting is automatic.** If an enemy is inside the cone, he fires. There is no fire button.
- **Mouse** uses the menu, Retry, and Return to Menu buttons.

## Features

- Eight finite waves. Clearing the last one is a victory. If the castle’s health reaches zero, the run ends.
- Three enemies, tuned as data: a fast skeleton, a slow minotaur, and a spitter that stops and shoots the wall.
- Three power-ups: Rapid Fire, Multi-Shot, and Castle Repair. Timed buffs refresh their duration. They do not stack.
- Score on a kill only. Reaching the wall does not score. The high score is kept between sessions.
- Arrows are reused from a pool instead of being created and destroyed on every shot.

## Architecture

Gameplay code lives in the `CastleArcher` namespace. Components talk through serialized references and C# events. There is no game-manager singleton.

**Health** is one component, used by enemies and by the castle. `TakeDamage` and `Heal` clamp the value. `OnHealthChanged` drives the health bar. `OnDied` fires once. The castle dying ends the run. An enemy dying awards score and plays the death sequence. Colliders turn off immediately, so a corpse cannot still hurt the wall.

**ScriptableObjects** hold the tunable data:

- `EnemyDefinition` — health, speed, castle damage, and score
- `PowerupDefinition` — which effect, how long it lasts, and its colour
- `WaveDefinition` — which enemies spawn, and how many

The spawner applies those numbers after it creates the instance. Changing a wave does not require a code change.

**Targeting** is a cone on the ground plane, about 18 units long and 50 degrees wide. The mage’s root keeps facing the field, so the cone does not swing when he strafes. Among enemies inside the cone, the one closest to the castle (the smallest world Z) is the target. The Scene view draws the range and the cone edges when the mage is selected.

![Targeting gizmos](Docs/targeting-gizmos.png)

**Event flow.** A wave starts and spawns enemies from their definitions. Targeting publishes the current target. The shooter pulls an arrow from the pool and fires only while that target exists. The arrow damages `Health`. `OnDied` updates the score and plays death. Castle health reaching zero raises game over. Clearing every wave raises victory. The HUD only listens. It does not decide any of this.

## Credits and third-party assets

The code, data, scenes and prefabs in this repository are my own work.

The 3D models, animations and particle effects come from Unity Asset Store packs. Their licence does not allow redistribution as source files, so they are **not included** here. Cloning the repository gives you the code and scenes, but the mage, enemies, castle, potion and hit effects will show as missing until you import these packs into `Assets/AssetStore`:

- Castle: [Simple Modular Castle Assets](https://assetstore.unity.com/packages/3d/props/simple-modular-castle-assets-309297) (Bublik)
- Mage: [Pack Heros](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/free-hero-pack-273048) (OlyPoly)
- Skeleton: [Stylized Low Poly Skeleton](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/stylized-low-poly-skeleton-306857) (SazenGames)
- Minotaur: [minotaur1](https://assetstore.unity.com/packages/3d/characters/minotaur1-196863) (ninj4ext)
- Spitter: [Fantasy Monster 29](https://marketplace.unity.com/packages/3d/characters/creatures/fantasy-monster-29-rig-animation-pixeliusvita-358468) (PixeliusVita)
- Particles: [Particle Pack](https://assetstore.unity.com/packages/vfx/particles/particle-pack-starter-assets-127325) (Unity Technologies)
- Potion: [Create with Code Course Library](https://assetstore.unity.com/packages/templates/tutorials/create-with-code-complete-course-library-146048) (Unity)

Audio is included. All of it is CC0:

- [Impact Sounds](https://www.kenney.nl/assets/impact-sounds) and [RPG Audio](https://kenney.nl/assets/rpg-audio) by Kenney
- [Medieval: The Old Tower Inn](https://opengameart.org/content/medieval-the-old-tower-inn) by RandomMind
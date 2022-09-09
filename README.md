# GameplayProgrammerTest

- A quick unity test with a bunch of basic features.

project made with Unity 2021.3.5f1

All self made files are in GameplayProgrammerTest folder under Assets, other folders are downloaded and not mine
All scripts in the folder are created from scratch

### Controls
- WASD / left stick - walk/run
- mouse / right stick - camera movement
- Space / X/A - jump (controller configurations might differ - this uses joystick button 0 and 2)
- ESC / start - pause menu

### Features

Ziplines 
- jump onto a zipline to be taken to its destination 
- starting points have visible handles
- can jump off the ziplines

Enemies
- enemies will chase the player when the player is whitin range
- they deal damage to the player by touching the players Hurtbox trigger
- they stop breifly after attacking
- the player has an invincibility period after taking damage
- player spawns at spawn point when dead

Collision Event system
- used for ziplines
- triggers with the TestTrigger script can bind an event to a function and based on tag/reference they will call that function.

Pause menu
- [ESC]
- pauses game
- can navigate around but not functonal



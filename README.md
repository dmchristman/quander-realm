# Quander Realm

# Table of contents
1. [Overview](#overview)
2. [Merging work into develop](#merge)
4. [Firebase](#firebase)


## Overview <a name="overview"></a>
The Quander Realm is a hub world that holds and manages the various minigames in this project.

The project files are split into three main areas under the `Assets` folder.

- Code
	- C\# code files that add behavior to game objects and do background work.
- Levels
	- A "level" in this context is an area of Quander that is loaded independently (i.e., each minigame).
	- Contains prefabs of game objects and scenes found in that level.
- Resources
	- Art, sound files, textures, dialogue, etc.
	- Basically anything else.

Then each of those three main areas are split into the area Quander that they affect.

- Common -- Shared code/assets used by more than one level.
- World -- The "hub" world and main menu.
- BuriedTreasure -- Buried treasure hunt with Wane.
- TanglesLair -- Circuit simplification game.
- TwinTanglement -- Maze solving with Fran and Ken.
- Qupcakery -- Cupcake gate problem solving.
- Queuebits -- Connect four with qubits.

See [Wiki](../../wiki) for more detailed documentation. 


## Merging work into develop <a name="merge"></a>
- Create a pull request to merge a work branch into `develop`. Make sure that the target repo is the `dmchristman` fork.
- If there are conflicts, check out the work branch, merge `develop` into this branch, resolve the conflicts locally, and then push.
- Add Devon as a reviewer to the pull request.

 
### Firebase <a name="firebase"></a>
Do not touch Firebase.

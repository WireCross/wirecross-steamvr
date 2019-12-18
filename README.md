# CrossWire SteamVR Edition

CrossWire is a serious game to explore the effects of synesthesia on the human perception. In essence, it is a simple puzzle game that progressively becomes more and more difficult as time and progression continues.

We (WireCross) have implemented the following functions:
- Glowing effect (through the use of MKGlow)
- 2 means of player input for puzzle solutions
- 2 means of generating the hint/solution the player is supposed to figure out
- Basic high score tracking
- SteamVR support
- Difficulty scaling based on progression
- Unqiue puzzle generator (BoardGenerator.cs)
- Sound-based approach puzzle (VignetteEffect.cs)

We have successfully fufilled the following outcomes:
- Procedurally generated puzzles
- Difficulty scaling based on time and player progression
- Procedurally generated animations by use of the Vignette/Chromatic Abberation/Bloom intensity values and remaining time
- Physics based lighting and physics simulations (glow effects contribute to lighting, which has been properly textured and applied normals; we utilize a physics-based input system where the player must pick up and place objects in the right order)

A precompiled version of the game can be found in the "bin" directory.

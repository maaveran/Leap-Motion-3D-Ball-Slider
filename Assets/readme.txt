Version 1.2
Fixed scoreboard display on bonus ball for tenth frame after rolling a spare.
Upgraded to Unity 4.1.

Version 1.1
Removed standard light flares that caused import warnings.
Added scoreboard script to the scene.
Moved the Ball object as a child to the StateMachine so it responds to
the ResetPosition message.

Version 1.0
Initial version

This is a sample bowling game, using a simplified version of the
controls used in the Fugu Bowl and HyperBowl apps and generic version
of the bowling game logic implemented for the Unity version of
HyperBowl.

The logic is implemented in StateMachine.js - the logic is implemented
as a state machine with the states as coroutines, and the Start
function transitioning among them. The logic and score data structures
actually support multiple players but in this project only one player
is active.

All objects that receive events from the state machine are placed as
children of the StateMachine object, e.g. the ball.

The ball has scripts attached that apply force from the mouse movement
and a message handler for resetting its position, plus a script that
plays the rolling sound based on contact with a specific physics
material.

A sample UnityGUI-based scoreboard is implemented in Scoreboard.js,
displaying the results of each roll. StateMachine.js also has
functions that return the total score and total score up to each
frame, e.g. GetSinglePlayerScore().

The pause menu is a variant of one available on the Unify community wiki.

All audio and graphical assets are taken from Unity standard assets or Unity Technologies demos.

Please direct any questions to the Fugu Games Facebook page,
http://facebook.com/fugugames or the Fugu Games thread in the Unity
forums Asset Store topic.

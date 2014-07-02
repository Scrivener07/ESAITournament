#ESAITournament

The goal here is to develop 3rd-party AIs for Endless Space, sort of like [BWAPI](https://code.google.com/p/bwapi/)

## Current status

At present we have a framework that can inject arbitrary code into the game process, and a proof of concept AI.  The game's internal API is not well-understood, so some reverse-engineering and debugging is necessary to make things work.

## Use

Build and run the project in Xamarin Studio.  It probably only runs on OSX but it can be ported trivially.

Running the project clobbers some files in the game install, so use Steam to get them back.

## Development

Xamarin Studio is capable of decompiling the Assembly-CSharp.dll, which is very handy.  Just double-click on the file under References.

Some notable components

### AITrampoline

This library gets dynamically injected into Endless Space.  This library provides functionality common to all AIs.  AIs should inherit the AITrampoline.TournamentAI class.  The AITrampoline.ESDebug class has some features that may be useful for AI debugging.

### ESStaticInjector

This modifies the Endless Space bytecode.  In particular it modifies the game code to use AITrampoline instead of its internal AI and it changes access modifiers on some methods to allow more extensibility.

### DrunkenWalkAI

This is a "proof of concept" AI that wanders aimlessly about the star system.

## Understanding ES internals

Here's what I know.

### Existing AIs

The AI stuff is all spread throughout the codebase, not confined to any particular subsystem (visible to us, anyway).  Separate AI implementations control at least StarSystems and Empires (and possibly things like fleets, although I haven't gotten that far.)

`AIPlayerController` seems top-level.  Seems to control the notifications a player would get in the righthand menu.  This appears to occur via a bidirectional system called "mail".  AITrampoline guts a lot of this functionality since we want a 

`AI_Master` and `AI_Empire` work together in a way that is not well understood.  `AI_Master` seems to control "the ministers" while `AI_Empire` controls "the layers".

`AI_System` seems to control an individual star system.

`Amas` seems to handle diplomacy.

Lots of these components communicate together via a shared dictionary called "Blackboard".

### Performing actions

Eventually the AI issues "orders" via the various "deparments".  The "departments" seem to control the authoritative game-state.  They may leak certain information that a non-cheating AI wouldn't know.

### Threading model

Internally Unity uses is single-threaded, and uses coroutines to implement some functionality, which are non-standard.  I'm able to hide a lot of this complexity in the AITrampoline.TournamentAI contract, so most of what you want to know you can read there.  However if you are reversing ES sourcode it might be useful to understand more about coroutine internals to make sense of the bytecode.  At the time of this writing some documentation about coroutines exists inside AITrampoline.cs.


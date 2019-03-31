# Manafest: Arena 1.0 Retrospective

Development for the first iteration of Manafest: Arena has concluded and a
release will be made containing this retrospective. This project was successful,
and it outlines room for improvement.

## Background/Motivation
First, some background. As a kid, I dreamed of making a videogame. In college,
I realized that dream was within my reach, and so I started a new project every
time I had an idea. Some time after college I realized that each project was a
check, and I couldn't cash them all at once. This led to tabling the multitude
of half-baked ideas and focusing on one large project instead. That eventually
turned into https://github.com/blukatstudios/FPS_Framework . Said project was
scoped for an insane number of inter-connected features that each are subject
to multiple rounds of refactoring. Worse, these features were largely tightly
coupled, and so making radical changes to one caused a domino effect to the
others that left me untangling spaghetti code for hours without seeing any
return on my investment. I decided that I should break up the larger project
into completely separate code bases so that I could experiment with building out
a small number of features at a time. Manafest: Arena is thus an attempt to
port my existing FPS gameplay from FPS_Framework, build out some RPG elements
to influence that gameplay, and then package that as a standalone videogame that
offered players an experience from start to end.

## What was best

### Time constraint
Having a 3-month deadline motivated me to work on the project even when I was
tired after work. The build 

### Successful delivery
The deliverable made it through build->testing->launch within the project's
timeline.

### Some patterns in the build worked well
Some of the original patterns from FPS_Framework worked well.

**Session**
Static methods and a pseudo-singleton to provide high-level global functionality
provide a degree of separation between individual classes. One instance of this
is Session.Event(event), which allows the Session to either handle that event
on its own, or deligate it to other classes. This then allows classes to emit events
without caring about who will receive them, and without the recipient having to know
what nodes it needs to connect by what signals.

At the same time, having a Session.session object allows other features that could be
singletons to not worry about providing static methods. The Session works as a wrapper
to treat functionality like menus, background music, and the active gamemode as static.


**DB classes**
For each class that needed to be persisted, I created a separate class that
handles saving/loading to abstract that problem away from the class's actual
business logic. This allowed me to have a Career object that was saved across
multiple files, and yet was simple enough to retrieve with CareerDb.LoadCareer().
The implementation is self-contained, and so can be completely rewritten by changing
a single file, provided its interface with other code doesn't change.


## What was good

**Testing**
Myself, my wife, and some internet volunteers played the game and gave feedback. Having
input from people that lacked the context of programmer/designer revealed problems with
navigation and playability that I was unaware existed.

**Music**
For this project I was intimidated at the prospect of creating a few songs that would be pleasing
to the ear within the time constraints. Fortunately, Hally Labs was offering an album for free
use. This project is FOSS with a $0 budget, and this music is higher quality than anything I can
currently produce.

## What was bad

**Time budget**
Since this is all made for fun, I don't have a 9-5 Mon-Fri time slot to budget towards
development, and simply won't work when I have conflicting obligations or when I simply
don't feel up to the task. That throws a ton of variability into a project timeline. 
Firstly, an hour after work is not the same as an hour after just waking up on the weekend.
Secondly, it's a crapshoot to know what days some event is going to magically eat up all my free
time, or what days I'll randomly feel inspired to work way more than normally. It might make
sense to log hours spent on development each day so that I can get a chart and tease out a
pattern. It might also make sense to try to recruit other team members to pad development time.
That might be far easier said than done, as I would offer no compensation.


**Project management**
To me, project management means setting up realistic expectations, adjusting those expectations 
according to reality, and making sure that resources are allocated efficiently to meet those
expectations. As an individual working on a FOSS project, I have no obligation to investors or
salaried employees to get a product to market that will make money. This is a double-edged sword,
as I have absolute creative freedom, but lack any objective indicator of success. It might be

**Unit tests**
I put together a few tests for the stats formulas, but am incredibly intimidated at the prospect
of mocking other classes for testing purposes. I might want to use a legitimate testing framework
so that I can do the following:
- Make sure tests pass before merging PRs
- Get tools to mock other classes
- Introduce automated tests wherever modular functionality exists
- Make code as modular as possible.

**3D graphics**
I don't know how to put textures onto models in a way that work in Godot.
This has to change for development to not look like a prototype.

**Using other C# assemblies**
They build, but they don't export.
I need to determine that this is definitely still the case with 3.1 and properly report it as
a bug if it is. Godot has a built-in JSON library, but I want to use an external one.
Alternately, I could find alternatives that are built into the version of C# I'm using.

## What could have been better

**File structure**
Source code should go in its own src/ directory to keep things relatively clean.
Util, Session should go in the src/ folder and not a util/ folder, as they are used globally.
All asset files should go in an Assets/ folder.

**Music**
The music used was great, but doesn't provide a cohesive atmosphere to immerse the player
in the Manafest universe. Ideally, I should create some music that thematically fits.

**SFX**
Is placeholder sound effects that are not satisfying, nor terribly appropriate for
the sounds they are attempting to mimmick.

**Voice acting**
There's none here. Adding some would make it easier to convey a narrative and atmosphere to the
player without forcing them to read a wall of text.

**Actor model**
The current model is a pill with a floating hand. The minimum expecation in this day and age 
is a rigged humanoid model that aims with their torso, has walking/jumping/item use 
animations, and ragdolls when dead.

## Refactors

**Items**
Item functionality is currently suspended in an inheritance tree. This makes life difficult
when a specific item mixes in multiple behaviors that live on different branches of the tree.
The solution is to make components/modules for item behavior, convert items into an IItem 
interface, store the state of an item as JSON, and provide a factory to convert from JSON
into a particular item. Item.Factory(string json) should delegate to factories in order to 
prevent a single giant factory like the one that currently exists. That should allow items to 
have arbitrary data footprints, functionality, and still allow re-use of code by virtue of
compisition over inheritance.

**Menus**
Most menus implementing IMenu have copy/pasted code. A class to contian these common elements 
would go a long way to reducing a menu class to some config settings and then menu-specific 
logic. Ideally, I'd also like to add an Up() Down() Left() Right() Select() Back() Exit() to
IMenu to allow joypad input for menus.(The implementation might rely on Godot's built-in)

**Session**
The session should ideally not know anything about the active game mode, and should attempt to
interact with it through interfaces. Other classes that are curious about the current gamemode
can then also use interfaces to interact with it.

**Input**
Currently the abstracions are like so:
Device -> InputEvent -> InputHandler
The abstraction is incomplete, resulting in the InputHandler caring about what Device is in use.
The mapping from Device -> InputEvent also hardcodes all controls and makes changing them a huge
undertaking.

To resolve this, I could restructure my abstractions like so:
DeviceState -> InputMapping -> InputEvent -> InputHandler

DeviceState - Provided a list of relevant buttons(per device), keys and axes, maintains the
state of each to be read.

InputMapping - Determines whether a particular InputEvent should be created, and what
information should be provided if it is. Some examples of configurable input mapping:
- pressed (jump, weak melee attack)
- held (charging an attack, continue sprinting)
- dual-axis (looking/moving)
- long press(powerful melee attack)
- long held (display context menu)
- super press (start sprinting, navigate context menu, weapon wheel)

**Data storage**
Currently I'm using csv files for everything. That's pretty gross.
When an object of one class goes into one file, I should use binary serialization.
For flexible or human-readable storage, I should use JSON wherever possible.
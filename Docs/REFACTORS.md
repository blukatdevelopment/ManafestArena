# Refactors

**JSON**
There's currently no JSON for the other refactored parts. A dynamic JSON object 
would go a long way to get various forms of data saved interchangeably as well
as providing an easy way to interpret results from a server.

**Menus**
Most menus implementing IMenu have copy/pasted code. A class to contian these common elements 
would go a long way to reducing a menu class to some config settings and then menu-specific 
logic. Ideally, I'd also like to add an Up() Down() Left() Right() Select() Back() Exit() to
IMenu to allow joypad input for menus.(The implementation might rely on Godot's built-in)

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
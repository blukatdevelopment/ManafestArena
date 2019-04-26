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

**AI**
The AI must be rewritten as an IInputSource that controls its actor by creating
MappedInputEvents and refers to IItems instead of Items.

**Actor**
In addition to some general cleaning, the Actor should replace the abstraction of
Brain with IInputHandler.

**Data storage**
Currently I'm using csv files for everything. That's pretty gross.
When an object of one class goes into one file, I should use binary serialization.
For flexible or human-readable storage, I should use JSON wherever possible.

**Textures**
Models should be equipped with textures, as albido colors along will not provide
enough graphic fidelity for contemporary demographics.

**Career**
The abstractions should be restructured like this
Career: A tree of Encounters the player must move up
Encounter: Should be an interface or base class so as to loosely couple with Career.

**Career Menu**
The career map should use icons to portray what each encounter is,
and the assets should make the career menu look like a map with encounters
as stops on a journey. Each 
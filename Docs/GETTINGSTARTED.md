# Getting Started

## File Structure

- /             Godot and C# project files
/Properties/    C# project files
/Screenshots/   Images referenced by README.md

**/Assets/**
- /Assets/Audio/
- /Assets/Materials/
- /Assets/Models
- /Assets/Scenes/Maps/          map scenes to be edited in-engine
- /Assets/Scenaes/SpawnPoints/  Drag and drop into maps
- /Assets/Terrain/              All files used to make the terrain meshlib
- /Assets/Textures/

**/Docs/** Documentation

**/Saves/** Save data. Might not want to version control this

**/src/** Source code
- /src/Actor/               An Actor is a character controlled by a player or AI
- /src/Ai/                  Controls an Actor
- /src/Arena/               Hosts Arena matches
- /src/Career/              Career gamemode
- /src/Db/                  Separate classes to abstract away file I/O
- /src/Inventory/           Classes to store items

**/src/Items/**               Code for itemms
- /src/Items/Archetypes/    Item classes
- /src/Items/Components/    Components used inside item classes
- /src/Items/Interfaces/    Interfaces for item use

- /src/Menus/               Code for UI
- /src/PressEvents/         Data files for career's unused PressEvent feature.
- /src/Util/                Misc and global scope of use classes


## How-to

### How to add a new kind of item

1. Create a class (ie Shovel) that implements IItem to /src/Items/Archetypes
    a. Inherit from Item unless you have a reason not to
    b. Use or create classes in /src/Items/Components/ wherever possible
2. Update ItemFactory.GetDelegateFactories to use your class
3. Create specific variants of new item (eg WoodShovel, StoneShove, IronShovel)
    a. Add these to ItemFactory.Items enum
    b. Return these from your IItem.GetSupportedItems()
    c. Update your IItem.Factory() to construct each variant
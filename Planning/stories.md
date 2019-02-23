# Stories

This should be short and sweet.
Each story should identify a thing to worry about (and not forget), and should only
have essential info in it.

**Template**
**name**:
**Description**:

## In Progress
Work is in progress on these

**name**: Generate career map
**Description**:
As a player, I should progress a career map containing random nodes so that I have better replayability.

**name**: Create Press events
**Description**: 
As a player, I should be confronted with difficult choices to make that will impact my playthrough.
- Create Press event trees providing compelling prompts
- Allow outcomes to effect the current playthrough

**name**: Item Models
**Description**:
As a player, I would like to see distinct item models to know what I'm looking at.
- Spear
- Claw
- Crossbow
- Flintlock Pistol
- Magical Staff
- Knife

**name**: Create character art
**Description**:
As a player, I should see a visual representation of each of the three characters I can choose.

**name**: Skin the UI
**Description**:
As a player, I should see a visual theme in the menus that identifies I am playing ManafestArena.

**name**: Finalize rest site menu
**Description**:
As a player, I should be able to pick between restoring health and a character upgrade.
- Player can heal
- Player can choose between up to three upgrades provided by Career instance.

**name**: Character upgrades
**Description**:
As a player with one playthrough, I should be compelled to return to unlock upgrades I missed on the first pass.

**name**: Character abilities
**Description**:
As a player with multiple playthroughs, I should feel each character has a distinct playstyle characterized by their abilities and items.

**name**: Skeletal Arena maps
**Description**:
As a player, I should be challenged to fight in maps containing different topography conducive to different tactics.
- Create roughly 5 different arena maps

**name**: Arena Map visual polish
**Description**:
As a player, I should be able to visually differentiate maps through a combination of colors and set pieces.
- Create and place some set pieces
- Find some way to introduce color into maps
- Mitigate roughness and ugliness where possible

**name**: Playtesting Round 1
**Description**: 
As a developer, I should gain some insight into my game's quality by collecting feedback from playtesters.

**name**: Defect and change Round 1
**Description**: 
As a playtester, I should see an effort made to correct major problems within the game.
- Fix some cited bugs
- Make some tweaks to functionality according to feedback

**name**: Music
**Description**:
As a player, I should be treated to music to set the mood in menus and encounters.
- Menu music
- Arena music

**name**: Sound effects
**Description**:
As a player, I should hear sound effects for various actions in the game so that they feel satisfying.
- Walking
- Item/ability uses
- UI Interactions


## Completed
These stories have had a first pass. Their acceptance criteria is either completely met, will be scrapped, or will be pushed into new stories.

**name**: Career map
**Description**: 
Player traverses random tree from leaf to root.
PUSHED - Generate tree of random nodes
DONE - Save/load nodes from .csv file(s)
DONE - leave career map to execute nodes
DONE - career map menu

**name**: normal arena match (career map node)
**Description**: 
SCRAPPED - Load random new arena from normal arena list
DONE - Clear out current save on loss
DONE - return to career map on win
SCRAPPED - anouncer dialogue/events

**name**: press event (career map node)
**Description**: 
DONE - press event menu
PUSHED - pick random event to load into menu
DONE- return to destination after event complete (arena, career map)

**name**: Stat manager
**Description**:
DONE - ICEPAWS attributes
DONE - additional stats (health, agility)
DONE - stat checks (roll X to win)
DONE - receive damage (filter for applying bonuses)
DONE - int GetStat(string stat)
SCRAPPED - void HandleEvent(SessionEvent event)

**name**: Melee weapons
**Description**:
PUSHED - models
SCRAPPED - textures
SCRAPPED - scenes/code
SCRAPPED - any custom logic

**name**: rest site (career map node)
**Description**: 
DONE - rest site menu
PUSHED - heal/upgrade menu

**name**: thrown weapons
**Description**:
PUSHED - models
SCRAPPED - textures
SCRAPPED - scenes/code
DONE - throw effect

**name**: projectile weapons
**Description**:
- models
- textures
- scenes/code
- any custom logic

**name**: character selection
**Description**: 
DONE - create character archetypes
PUSHED - create art for each
DONE - character select menu
PUSHED - create upgrades for each archetype

**name**: Save game data
**Description**:
- DONE save profile
- DONE save inventory/abilities
- DONE method call to clear this
- SCRAPPED save wins/losses?

**name**: arena maps
**Description**:
PUSHED - Create arena map layouts
PUSHED - create custom assets (models, textures, scripts?)

**name**: abilities effects
**Description**:
DONE - Fireball
PUSHED - Healing
SCRAPPED - Levitation
SCRAPPED - Speed
SCRAPPED - Force push
SCRAPPED - Force wall

**name**: abilities
**Description**:
DONE (See: SpellCaster)- bind one-shot item use to key press
PUSHED - manage this binding separately from items

**name**: healing/special items
**Description**:
PUSHED - models
SCRAPPED - textures
DONE - scenes/code
DONE - any custom logic

## Scrapped stories
After re-assessing, the following stories have been scrapped for the April 4th release.
They may be pulled back, but only if polishing and fleshing out the other areas is
finished sufficiently early.

**name**: commentators
**Description**:
- audio files
- trigger on events

**name**: final boss fight (career map node)
**Description**: 
- root node should be this
- arena map especially for this
- win/credits menu on win
- anouncer dialogue/events

**name**: boss arena match (career map node)
**Description**: 
- Load random new arena from normal arena list
- Clear out current save on loss
- return to career map on win
- anouncer dialogue/events

**name**: shop (career map node)
**Description**: 
- items menu
- load random items for sale
# Stories

This should be short and sweet.
Each story should identify a thing to worry about (and not forget), and should only
have essential info in it.

**Template**
**name**:
**Description**:

**name**: Career map
**Description**: 
Player traverses random tree from leaf to root.
- Generate tree of random nodes
- Save/load nodes from .csv file(s)
- leave career map to execute nodes
- career map menu

**name**: rest site (career map node)
**Description**: 
- rest site menu
- heal/upgrade menu

**name**: normal arena match (career map node)
**Description**: 
- Load random new arena from normal arena list
- Clear out current save on loss
- return to career map on win
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

**name**: press event (career map node)
**Description**: 
- press event menu
- pick random event to load into menu
- return to destination after event complete (arena, career map)

**name**: final boss fight (career map node)
**Description**: 
- root node should be this
- arena map especially for this
- win/credits menu on win
- anouncer dialogue/events

**name**: Stat manager
**Description**:
- ICEPAWS attributes
- additional stats (health, agility)
- stat checks (roll X to win)
- receive damage (filter for applying bonuses)
- int GetStat(string stat)
- void HandleEvent(SessionEvent event)

**name**: Melee weapons
**Description**:
- models
- textures
- scenes/code
- any custom logic

**name**: projectile weapons
**Description**:
- models
- textures
- scenes/code
- any custom logic

**name**: thrown weapons
**Description**:
- models
- textures
- scenes/code
- throw effect

**name**: healing/special items
**Description**:
- models
- textures
- scenes/code
- any custom logic

**name**: abilities
**Description**:
- bind one-shot item use to key press
- manage this binding separately from items

**name**: abilities effects
**Description**:
- Fireball
- Healing
- Levitation
- Speed
- Force push
- Force wall

**name**: character selection
**Description**: 
- create character archetypes
- create art for each
- character select menu
- create upgrades for each archetype

**name**: arena maps
**Description**:
- Create arena map layouts
- create custom assets (models, textures, scripts?)

**name**: commentators
**Description**:
- audio files
- trigger on events

**name**: Save game data
**Description**:
- save profile
- save inventory/abilities
- method call to clear this
- save wins/losses?
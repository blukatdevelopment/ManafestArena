# Planning
A delivery will be released March 4th, 2019, finished or not.

Many priorities and goals will be laid out. The strategy will be to approach each of them with roughly 50% effort in order to 
not get stuck on one for too long.

## Non-negotiable requirements

Some requirements cannot be compromised, and must be provided as early in 
development as possible

### Build

An executable must be built to provide a playable game.
[READY]Godot's build process should be proven.

### Win state

A game is a simulation that contains both win and lose states associated with user input.
[READY]win state
[READY]lose state

## Negotiable requirements

Some requirements can be scaled back, or cut entirely if need be.

### Random career tree of encounters

The fighter's career consists of a tree of nodes where the leaves are
starting points, and the root is the final fight for the championship.
If the player dies at any point during their career, they lose.

#### Public match

The fighter competes in an arena against enemies. 
Winning grants sponsorship Carthage credits and career points.

#### Title match

The fighter fights to complete a title-holder's arena, which may involve
platforming, traps, and enemies.
Reward includes carthage credits, career points, and special items.

#### Rest spot

The fighter can rest to restore HP, or can upgrade an ability.

#### Press event

The fighter makes an appearance in carthage media.
This can result in a random reward, punishment, or decision.

#### Shop

The fighter can spend Carthage credits to buy items.

### FPS gameplay

Arena matches should take place with FPS combat taking place against AI
enemies on 3D maps.

#### Items

Items should provide situational advantages and disadvantages.
Balancing will be an ongoing task.

**Melee Weapons**

- Knife
- Spear
- Sword
- Club
- Fist

**Ranged weapons**

- Bow
- Spear
- Rock
- Musket
- Magic Pistol
- Magic rifle

**Splash damage**
- Thrown potion
- Bomb
- rocket

**Armors**
- leather armor (half, full)
- bronze armor (half, full)
- iron (half, full)
- steel (half, full)

**relic**
A relic can provide a positive effect, a negative one, a perk, or
some combination of the three.


#### Abilities

Magic-users should be able to equip magical abilities to
ability slots (1-0 keys) and use them in combat.

- Fireball
- Healing
- Levitation
- Speed
- Force push
- Force wall

### Character customization

The player should be able to select one of N starting archetypes,
each with different backgrounds, stats, starting equipment, and perks.

#### ICEPAWS
A compromise between SPECIAL, GURPS, and DND attributes.

**Intelligence**
Scaleable: 
- Bonus on attribute points earned ( int + 1)/2 per earn
3: Magic use (also requires willpower 3)
5: Examine enemy by aiming( also requires 5 perc)
6: Stronger magic spells
10: Double strength magic
**Charisma**
Scaleable:
- Earn bonus Career points base + (base * (chr -1) /10)
3: unlock taunts that earn Career points after kills 
7: tier 3 swearwords in taunts for extra career points
10: Set up crossteaming before matches
**Endurance**
Scaleable:
- health points
- natural regen 1 health per 30 - (1.5 * end)
3: base damage resistance 
6: improved healing from items
10: greater base damage resistance 
**Perception**
Scaleable:
3: Aim a projectile weapon
5: Examine enemy by aiming (also needs 5 int)
6: reduced cone of fire
10: HUD indicator for enemies
**Agility**
Scaleable:
- movement speed
- jump height
3: normal jump
6: doublejump
7: wall hang
8: wall jump
10: cheetah speed
**Willpower**
Scaleable:
- max mana
- mana regen
3: can use magic (also requires int 3)
6: + 50% base max mana
10: half spell cost 
**Strength**
Scaleable:
- melee damage
- carry weight
- grapple defense bonus
3: fists do more than 1 damage
5: Grapple button (on success, steal their item)
10: Throw any item as weapon

#### Character roster

TBD after skills/abilities implemented.


#### Corporate sponsors

Press events should give the player a chance to gain sponsors, which
pays extra carthage credits for winning in the arena. The player can
invest additional career points to improve sponsor relations. Once maxed
out, the player gains a sponsor perk.

**The royal kingdom**
- Random perk per match

**Nutri-corp**
- free nutriloaf

**Opulence Network**
- double cash earned

**Security, Inc**
- Free high tech armor

**Thrift Central**
- free low quality items

**Fig**
- Free Advanced HUD

**InfoCo**
- % chance of reavealing rare items in shops

**Identity**
- Tax break: rebates on sales

**Duet**
- Finds you a companion for battle

**Life LLC**
- Rest sites gives bonus healing


### Assets

#### Voice

**announcers**
- introduce each arena
- comment on deaths/kills
- comment on win

#### Music
Free open-license music should be used only.

- background menu music
- win screen music
- lose screen music

#### Models
3D models should be minimal, only providing enough detail to get the
idea across to the player.

- player
- Knife
- Spear
- Sword
- Club
- Fist
- Bow
- Spear
- Rock
- Musket
- Magic Pistol
- Magic rifle
- potion
- Bomb
- rocket

#### Textures
Textures of some sort should be applied to items and terrain to prevent
player from bleeding out of the eyes.

- Every model mentioned above
- Every model in the gridmap tileset

### Arenas

Different arenas should exist that can be randomly chosen.

### Menus

#### Main menu

#### Career map

#### Character customization/loadout

#### Shop

#### Rest site menu

#### Character selection menu

#### Press event menu
# Roadmap for Manafest: Arena 2

This document is intended to provide context and a detailed description of goals for the development of the sequel to Manafest: Arena.

## Recap of Manafest: Arena

After a few months of development on the FPS_Framework repo, I decided it was time to make a video game using what I had done already as a starting point.
With a three month timeline, Manafest: Arena was developed.

### Initial build

The goal was relatively simple: I wanted to produce an FPS rogue-like with elements similar to Slay The Spire https://store.steampowered.com/app/646570/Slay_the_Spire/ .
The player choose one of three unique starting characters to begin their adventure. They are then presented a map consisting of a tree of encounters, and are given the choice of which encounter to start with on one of the leaves of this tree. Encounters included random Arena matches as well as rest sites where the player can choose between an upgrade for their character, or healing.

A couple features were scrapped due to lack of time. One feature was a shop encounter that would allow the player to acquire new items for combat. Another feature was a press event encounter in which the player would respond to a multiple choice prompt and reward or punish the player based on their choice and random chance. Lastly, there was a sponsorship feature which would grant abilities or bonuses to the player when earned. Before work was started, all netcode was pulled from the code base.

### Refactors

After the release, I returned to clean up the code base. said features included a new file structure as well as new menu, input, Actor, and Item systems. JSON was
added and used for saved files and dumping object data when debugging.

## Manafest: Arena 2

The sequel shares the premise of the previous game. The player selects a character with unique abilities. Given a tree of encounters, they choose their own path up the tree and advance to the end of the game by completing encounters.

### Menus and flow overview

The following is a rundown of features and menus the player will interact with.

#### Main menu

The player is presented with the game's logo, background music, and buttons to navigate to other menus.
- Continue: Present only if a game has been started
- New: Go to new game menu
- Settings: Go to settings menu
- Credits: Go to credits menu
- Quit: Exit the game

#### Settings Menu

This menu provides some settings

- Master Volume: Volume for all sound 
- Sound Effects Volume: Volume for sound effects
- Music Volume: Volume for in-game music
- Mouse sensitivity X: Sensitivity for turning left and right
- Mouse Sensitivity y: Sensitivity for looking up and down
- Name: Player's name

There are also buttons on this menu:
Main Menu: Return to main menu
Revert: reload saved settings
Save: Save current settings
Controls: Navigate to controls menu

#### Controls menu

This menu provides various actions and the inputs they are mapped to.
The user can click an action, then a new key to edit the mapping.

- There are then some buttons on this menu:
- Back: returns to options menu
- Revert: load saved settings
- Save: save current settings

#### Credits menu

A list of credits identifies all contributors and any special thanks.

There is additionally a single button to return to the main menu.

#### New game menu

This menu shows the selection of the three starting characters. The player should be able to select each character to see information about them.

There are buttons on this page:
- Character selection buttons: When pressed highlight character and display information.
- Main menu: Return to main menu
- Start game: Appears when a character is selected. Creates a new save and navigates to the Career map.

#### Career Menu

This menu displays the player's progression through their career in the arena as an inverted tree of encounters.
The tree's root node is at the top level of the menu, and its leaves are at the bottom of the menu.
Each encounter is represented by a button that is disabled until active.
To start with, only the leaf nodes of this tree on the bottom level are active. After playing through one encounter
on a level, the parent nodes of that encounter become active. This is functionally identical to the map in Slay The Spire.
The entire tree is randomly generated with different nodes upon the start of a new game.

If the map has more levels than will fit on the screen, the player can scroll vertically by moving their mouse to the top or
bottom center of the screen.

There is also a button to return to the main menu.

#### Rest Site Encounter Menu

This menu initially displays two buttons
- Rest button: Restores some health and completes encounter
- Upgrade button: Display new set of buttons

Once an upgrade is selected, the following buttons are displayed:
- Buttons for up to three random upgrades
- Confirmation button: Appears once an upgrade is selected. If clicked, upgrade is applied and encounter is completed

#### Press Event Encounter Menu

This menu displays a random prompt and up to four options for responses. 
Some of these options have side effects(such as gaining money or upgrades or losing health)

#### Shop Encounter Menu

This menu displays a random selection of items, abilities, and upgrades available for purchase.
Hovering over the items should display additional information as a tooltip or (the equivalent of a ) modal.

#### Arena match Encounter

An arena encounter places the player's character in a random arena with enemies.
Victory condition is to be decided. In the last game, the player had to defeat 5 enemies to win.

#### Arena Match Victory Menu

After the player has won an Arena Match, this menu should display the rewards they earned from the match.
Each reward is displayed as a button that can be clicked to redeem the reward.


### Project context

#### Stakeholders and team

- Blukat 
- Adwaith

#### Goals

1. Produce a game that can be proudly displayed in a portfolio.
2. Create software that can be reused by both Godot and Linux communities on future projects.
3. Build a small team of developers for future projects.
4. Gain business and game development experience without the financial risk that comes with creating a company.
5. Have fun.

#### Audience

The primary audience for this game are English-speaking(Localization is not currently implemented) players interested in casual FPS games of the age range 18-35.

#### Supported Systems

This game should be compiled for Linux and Windows desktop computers, both in 32-bit and 64-bit versions.

Recommended specifications machine
CPU: AMD FX 8350
GPU: Radeon HD 6950
RAM: 16 GB

Minimum specifications machine
CPU: AMD Phenom II X4
GPU: 
RAM: 8 GB

### Development Methodologies

Below are a list of methodologies to 

**Story creation**
Before the build begins, we should create [user stories](https://en.wikipedia.org/wiki/User_story) to describe each task.
Each story should contain a description that fits one of the common templates. It should then include specific acceptance criteria, 
items that can be verified to confirm a feature is finished.

**Implementation**
Each feature (or small group of features) should be built out on a feature branch off of develop. A pull request should be created
in order to provide an opportunity for peer review. If significant changes have been made during review, devs should manually test
the feature again before merging in order to reduce the accumulation of bugs that could block the development of other features.

**Internal playtesting**
Testing is currently 100% manual. We should perform playtesting occasionally in order to discover and document bugs
so that they can be assigned priority and fixed. These tests should ideally reveal and resolve obvious bugs such as the game
crashing, freezing, or nor behaving as intended.

**External playtesting**
After bugs revealed by one or more rounds of internal testing have been fixed, we should have this game tested by external parties
that don't have the context on the project that we do. By the time the game reaches our external testers, obvious bugs should
be addressed. These external testers should give us insight into features that are either confusing or not fun. The feedback from
external testing should be considered carefully, solutions discussed before implementing. We should confirm that our players are
familiar with FPS gameplay before they test. 

**Asset Creation**
Much of the work for this game will revolve around the creation of game assets such as textures, meshes, animations, music, sounds, voice-overs,
maps, and JSON files containing dialogue. These files should all be released under permissive licenses such as CC-BY so that this project remains
FOSS. Most of these files are binary, and thus do not benefit fully from version control. Additionally, they will potentially be large. 

**Sprints**
Not all features of agile software development are applicable to this team or project(especially since this is not a full-time job for either of us), but I feel managing 
our work in [sprints](https://www.atlassian.com/agile/scrum/sprints) would be helpful. Each sprint should last two weeks. It should start by identifying a set of tasks
that we reasonably believe can be completed within the sprint. After the sprint is done, we can have a short retrospective to discuss what worked, what didn't, and what 
could be changed to make development more fun and productive.

**Points**
[Story Points](https://www.visual-paradigm.com/scrum/what-is-story-point-in-agile/) will be used to estimate the complexity of a given task. The understanding of what 
these points mean should improve with multiple sprints.

**Commercial Release**
After we deem this project complete, we should make it available for sale. I, Blukat, am currently not in the position to accept money from this project. Revenue from 
sales of Manafest: Arena 2 will be split between the other members of the team. My first and foremost goal is to produce something something valuable 
that can be expanded upon at a later time. If we produce a successful product with even a small fan base, then the very next project can be built using 

**Security of Product**
This project should be free and open source, all creative assets released under permissive licenses. Any person is theoretically capable of legally selling any version of 
this project. We can reduce the risk of an external party selling our game by keeping creative assets available only to the development team in a during the 
build and sales period. After either a year has passed, or sales have dropped off to nothing, we can release the assets publicly so that this project can serve as a 
starting point for the linux and Godot communities.

**Advertising and Public Relations**
In order to drive sales, we will want some form of advertising and public relations. This will involve posting promotional messaging to social media. The development of 
promotional materials such as posters, art, and trailers will be critical to this game's marketability. I am not making any commitment to provide a marketing budget, but 
it might very well be the case that I will chip in a couple hundred dollars in order to get some ads on websites once our product is polished.

### Epics

#### Characters

We will need rigged models, textures, animations, and animation controllers, and voice-overs for all characters in this game.
This should cover all three playable characters as well as a number of enemy characters.

Each character should have a unique play style or niche in combat. This can partly be provided by a specific set of ICEPAWS stats, but should be reinforced with a set of 
abilities that can be acquired and upgraded to modify this play style or niche.

#### Items/abilities/upgrades

We will need a set of items and abilities that can be used with all humanoid characters. If we have non-humanoid characters, we'll need abilities(ie biting, flying) that 
may be unique to them.

Each item or ability should have at least one possible upgrade that changes how it functions. These can come in the form of simply damage increases, or can introduce 
entirely new functionality. (ie a crossbow that does less damage, but knocks the enemy back very far).

#### Menus

There are many menus outlined above in this document. We will need to build out and polish these menus where applicable, in order to deliver an intuitive and visually 
appealing experience for the user.

#### Music

We will need a soundtrack that fits the game's mood and tone. We can use any music released CC-BY, or develop it outselves.
This music will chiefly be broken into the following categories:
- Opening menu music
- Combat music
- Victory music
- Defeat music

#### Sound effects

We will need sound effects for just about every in-game action as well as audio cues when a player navigates through menus or
earns points/experience/currency.

#### Level design

We will need to design levels for the arena encounters. All scenery and terrain will need to be modeled and textured.
Each level should contain spawn points for players, enemies, and powerups. Interactive elements such as doors or destructable
windows will also need to be developed in order to give

#### User experience/balancing

It will be critical to focus on delivering a good experience to our end users. To do this, we will need to allow external playtesting and respond
to feedback provided. If a feature does not positively contribute to the user's experience, it should be modified or removed.

One common point of contention is balancing the rewards for different play styles so that they all feel valid to the player. This is highly subjective and will require much consideration.

#### Writing

Any dialogue, copy, or decisions regarding lore/story will need to be made as cohesively and consistently as possible. The game should have a consistent set of themes that 
it adheres to.

#### 2D Art

Many UI elements will require us to make images.

#### Marketing/PR

We will need to create promotional materials for the game and likely give periodic press releases or dev log entries for the progress of the game.

### Timeline

“A delayed game is eventually good, a bad game is bad forever.” 
― Shigeru Miyamoto

The intent of this timeline is to identify how many sprints the team can commit to for this project, and to make the development of the game fit that window of time.
We can periodically stop to reconsider the timeline if we either decide we need to release sooner, or extend the timeline to release later. 
[Scope creep](https://en.wikipedia.org/wiki/Scope_creep) is something we will need to avoid when possible.

The schedule for epics is tentative, and will very likely change as we begin work and realize the need for more or less stories. The epics are listed in the order that shey should be started. Many of the sprints are blank, as they will be used to continue work on epics from previous sprints.

#### Sprint 1 preparation August 15, 2019
- Marketing/PR

#### Sprint 1 August 19, 2019
- Menus
- Characters

#### Sprint 2 September 2, 2019

#### Sprint 3 September 16, 2019
- Items/abilities/upgrades

#### Sprint 4 September 30, 2019

#### Sprint 5 October 14, 2019
- Level design

#### Sprint 6 October 28, 2019

#### Sprint 7 November 11, 2019
- User experience/balancing
- Writing
- 2D Art
- Sound effects

#### Sprint 8 November 25, 2019

#### Sprint 9 December 9, 2019
- Music

#### Sprint 10 December 23, 2019

#### Sprint 11 January 6, 2020

#### Sprint 12 January 20, 2020

#### Sprint 13 February 3, 2020

#### Release day February 15, 2020
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

### Game Overview

The following is a rundown of features and menus the player can interact with.

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
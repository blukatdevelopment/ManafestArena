# Refactors
This is a backlog of changes to change how the game's existing scope is structured or implemented. Architecture, optimizations, and renaming of methods/classes/files goes here.

## Data storage
Currently there are csv files as far as the eye can see. This is gross, and should be replaced. When files are supposed to be human-editable and small, JSON would be ideal. When files are supposed to be giant blobs of data (such as terrain), binary formatting sounds like the ticket.

## Career persistence
A career object should contain all player data. It should be able to either save itself in binary format, or as JSON. The player's Actor should not be the source of truth for career info, except at the end of an encounter in deciding how much health the player has left.

## Boxcast
Implement box-cast to look for sibling actor nodes that have a line-of-sight to the seeking actor. This should be a candidate to replace gridcasts.

## HUD condition bars
The player should be able to see their health/stamina/mana as bars that grow to full size when each condition is full. Stats with a max of 0 should not 
get bars.
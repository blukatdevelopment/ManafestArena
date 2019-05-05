# Refactors

## Get the game running again
As a result of the other refactors ripping wires out willy-nilly like, the game's in a broken/unplayable state where the main features cannot be manually tested. The game should be put into a playable state so that the menus and arena gameplay are accessible.

## Data storage
Currently there are csv files as far as the eye can see. This is gross, and should be replaced. When files are supposed to be human-editable and small, JSON would be ideal. When files are supposed to be giant blobs of data (such as terrain), binary formatting sounds like the ticket.
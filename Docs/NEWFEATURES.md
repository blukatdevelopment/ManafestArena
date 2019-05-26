# New Features
This backlog contains changes that expand the scope of the game, inherently increasing the surface area of the game that a user can experience.

## PressEvent encounters
A dynamic JSOn object should be read from hand-written files to determine what dialogue nodes to display. Options should be able to contain checks executed beforehand using Career data (and random numbers) that decide whether they are displayed or not. Options should likewise be able to contain effects that change the state of the Career.

## InputHandlers
Input handlers should have an AcceptEvent(MappedInputEvent evt) method so that the actor can be interacted with by external forces such as recoil from a projectile weapon.

## HUD item info
Item info should be cached by the hud and an InfoChanged() method should decide whether the HUD should update or not. Items should provide dynamic JSON blobs from their GetInfo() methods, and the HUD can then decide how to rebuild the item display when switching items. For example, a double-crossbow should show
two white crossbow bolts representing total capacity that turn dark when ammo
is used, and turn white again after reloading. Above these, displaying the reserve ammo should give the player an idea of how many reloads they have left.

## HUD hit indicator
When an actor receives damage from a player, a hit indicator should be created temporarily that displays on the player's HUD through obstacles.

## HUD reticle
A reticle should display where the player is aiming and should change when the user is crouched.

## HumanoidBody
A humanoidBody class should provide a new rigged humanoid body that can handle
leg and torso angle/position/animations separately.

## Item damage transfer
Items should be able to receive damage. In doing so, they should by default transfer that damage to the actor. Said can be changed later based on game design decisions.

## RagdollBody
Once killed, or temporarily knocked out, a HumanoidBody should replace itself
with a ragdoll body that flops around in the typical 2006 AAA game fashion by
utilizing the physics engine.

## Animations
If one doesn't already exist, an animation state machine should be made to manage animations performed on an actor. The IBody should have a GetAnimator() method so that Items can trigger or set animation variables. These animators
should probably implement a new IAnimator class so that they can be used interchangeably in the event that there's more than one option (ie a turret VS a humanoid) Items, themselves, should have IAnimators when appropriate.
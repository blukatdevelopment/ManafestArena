/*
  An encounter is a single interaction or challenge for the player on their
  path to the end of the game.
*/
using Godot;
using System;
using System.Collections.Generic;

public interface IEncounter {
  string GetDisplayName();
  void StartEncounter();
  IEncounter GetRandomEncounter();
}
/*
  In-game character
*/

using Godot;
using System;
using System.Collections.Generic;

public class Actor {
  public IInputHandler inputHandler;
  public IStats stats;
  public IBody body;
  public IInventory inventory;
  public HotBar hotbar;
  public PaperDoll paperdoll;
}
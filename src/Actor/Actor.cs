/*
  In-game character whose functionality is broken out between its components.
*/

using Godot;
using System;
using System.Collections.Generic;

public class Actor : IHasInputHandler, IHasStats, IHasBody, IHasInventory {
  public IInputHandler inputHandler; // 
  public IStats stats;
  public IBody body;
  public IInventory inventory;
  public HotBar hotbar;
  public PaperDoll paperdoll;

  public IInputHandler GetInputHandler(){
    return inputHandler;
  }

  public IStats GetStats(){
    return stats;
  }

  public IBody GetBody(){
    return body;
  }

  public IInventory GetInventory(){
    return inventory;
  }
}
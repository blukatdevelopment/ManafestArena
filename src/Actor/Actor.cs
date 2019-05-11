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

  public void Update(float delta){
    if(inputHandler != null){
      inputHandler.Update(delta);
    }
    if(stats != null){
      stats.Update(delta);
    }
    if(body != null){
      body.Update(delta);
    }
  }

  public static Actor GetActorFromNode(Node node){
    IBody actorBody = node as IBody;
    if(actorBody != null){
      return actorBody.GetActor();
    }
    return null;
  }
}
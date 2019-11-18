/*
  In-game character whose functionality is broken out between its components.
*/

using Godot;
using System;
using System.Collections.Generic;

public class Actor : IHasInputHandler, IHasStats, IHasBody {
  public ActorFactory.Bodies bodyType;
  public IInputHandler inputHandler;
  public Stats stats;
  public IBody body;
  public HotBar hotbar;
  public int camId = -1;

  public Actor(){
    hotbar = new HotBar(10, this);
  }

  public Node GetNode(){
    if(body != null){
      return body.GetNode();
    }
    return null;
  }

  public IInputHandler GetInputHandler(){
    return inputHandler;
  }

  public Stats GetStats(){
    return stats;
  }

  public IBody GetBody(){
    return body;
  }

  public void SetBodyType(ActorFactory.Bodies bodyType){
    this.bodyType = bodyType;
  }

  public void InitCam(int id){
    camId = id;
  }

  public void Update(float delta){
    if(body != null && body.IsDead()){
      return;
    }

    if(inputHandler != null){
      inputHandler.Update(delta);
    }
    if(stats != null){
      stats.Update(delta);
    }
    if(body != null){
      body.Update(delta);
    }

    if(hotbar.GetActiveSlot() != null){
      hotbar.GetActiveSlot().Update(delta);
    }
  }

  public void QueueFree(){
     Node actorNode = body as Node;
      if(actorNode != null){
        hotbar.UnequipActive();
        actorNode.QueueFree();
      }
      body=null;
  }

  public static Actor GetActorFromNode(Node node){
    if(node == null){
      return null;
    }
    
    IBody actorBody = node as IBody;
    if(actorBody != null){
      return actorBody.GetActor();
    }

    IItem item = node as IItem;
    if(item != null){
      actorBody = item.GetWielder() as IBody;
      if(actorBody != null){
        GD.Print("Found actor by its item");
        return actorBody.GetActor();
      }
    }

    return null;
  }
}
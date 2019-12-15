using Godot;
using System;
using System.Collections.Generic;

public class ActorFactory {
  public enum InputSources {
    None,
    Keyboard,
    AI
  };

  public enum Bodies {
    None,
    HumanoidBody,
    WolfBody,
    BatBody
  };


  public enum Characters {
    None,
    DebugPlayer,
    Target,
    DebugEnemy
  };

  public static Actor FromComponentTypes(
    InputSources inputSource,
    Bodies body
  ){
    Actor actor = new Actor();
    InitInputHandler(inputSource, actor);
    actor.SetBodyType(body);

    return actor;
  }

  public static void InitActor(Actor actor){
    InitBody(actor);
    int playerOneCam = 0;
    if(actor.camId == playerOneCam){
      actor.body.InitCam(playerOneCam);
    }
    actor.hotbar.EquipActive();
  }

  public static void InitInputHandler(InputSources inputSource, Actor actor){
    FPSInputHandler fps;
    MappedInputSource mapped;
    StateAi ai;
    switch(inputSource){
      case InputSources.Keyboard:
        mapped = new MappedInputSource(Session.GetDevice(0), FPSInputHandler.GetMappings());
        fps = new FPSInputHandler(actor);
        fps.RegisterInputSource(mapped as IInputSource);
        actor.inputHandler = fps as IInputHandler;
      break;
      case InputSources.AI:
        ai = new StateAi(actor);
        fps = new FPSInputHandler(actor);
        fps.RegisterInputSource(ai as IInputSource);
        actor.inputHandler = fps as IInputHandler;
      break;
    }
  }

  public static void InitBody(Actor actor){
    switch(actor.bodyType){
      case Bodies.HumanoidBody:
        actor.body = new HumanoidBody(actor);
      break;
      case Bodies.WolfBody:
        actor.body = new WolfBody(actor);
      break;
      case Bodies.BatBody:
        actor.body = new BatBody(actor);
      break;
    }
  }

  public static Actor FromName(string name){
      GD.Print("Making character: " + name);
      return FromCharacter(Characters.DebugPlayer);
  }

  public static Actor FromCharacter(Characters character){
    Actor ret = null;
    switch(character){
      case Characters.DebugPlayer:
        ret = DebugPlayerCharacter();
      break;
      case Characters.Target:
        ret = TargetCharacter();
        break;
      case Characters.DebugEnemy:
        ret = DebugEnemyCharacter();
        break;
    }
    return ret;
  }

  public static Actor DebugPlayerCharacter(){
    Actor actor = FromComponentTypes(InputSources.Keyboard, Bodies.HumanoidBody);
  
    actor.stats = new Stats(Stats.EmptyStats(5));
    actor.hotbar.AddItem(0, ItemFactory.Factory(ItemFactory.Items.Knife));
    actor.InitCam(0);
    return actor;
  }

  public static Actor TargetCharacter(){
    Actor actor = FromComponentTypes(InputSources.None, Bodies.HumanoidBody);

    actor.stats = new Stats(Stats.EmptyStats(5));
    actor.hotbar = new HotBar(10, actor);
    actor.hotbar.AddItem(0, ItemFactory.Factory(ItemFactory.Items.Knife));
    return actor;
  }

  public static Actor DebugEnemyCharacter(){
    Actor actor = FromComponentTypes(InputSources.AI, Bodies.WolfBody);

    actor.stats = new Stats(Stats.EmptyStats(5));
    actor.hotbar = new HotBar(10, actor);
    return actor;
  }
}
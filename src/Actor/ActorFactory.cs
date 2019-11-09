using Godot;
using System;
using System.Collections.Generic;

public class ActorFactory {
  public enum InputSources {
    None,
    Keyboard,
    AI
  };

  public enum StatsHandlers {
    None,
    Icepaws
  };

  public enum InventoryHandlers {
    None,
    Simple
  };

  public enum Bodies {
    None,
    PillBody,
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
    StatsHandlers statsHandler,
    Bodies body,
    InventoryHandlers inventoryHandler
  ){
    Actor actor = new Actor();
    InitInputHandler(inputSource, actor);
    InitStats(statsHandler, actor);
    actor.SetBodyType(body);

    InitInventory(inventoryHandler, actor);
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

  public static void InitStats(StatsHandlers statsHandler, Actor actor){
    switch(statsHandler){
      case StatsHandlers.Icepaws:
        actor.stats = new IcepawsStats();
      break;
    }
  }

  public static void InitBody(Actor actor){
    switch(actor.bodyType){
      case Bodies.PillBody:
        actor.body = new PillBody(actor);
      break;
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

  public static void InitInventory(InventoryHandlers inventoryHandler, Actor actor){
    switch(inventoryHandler){
      case InventoryHandlers.Simple:
        // Set up a simple inventory
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
    Actor actor = FromComponentTypes(InputSources.Keyboard, StatsHandlers.Icepaws, Bodies.HumanoidBody, InventoryHandlers.None);
    actor.stats.SetStat("intelligence", 5);
    actor.stats.SetStat("charisma", 5);
    actor.stats.SetStat("endurance", 5);
    actor.stats.SetStat("perception", 5);
    actor.stats.SetStat("agility", 5);
    actor.stats.SetStat("willpower", 5);
    actor.stats.SetStat("strength", 5);
    actor.stats.RestoreCondition();
    actor.hotbar.AddItem(0, ItemFactory.Factory(ItemFactory.Items.Knife));
    actor.InitCam(0);
    return actor;
  }

  public static Actor TargetCharacter(){
    Actor actor = FromComponentTypes(InputSources.None, StatsHandlers.Icepaws, Bodies.HumanoidBody, InventoryHandlers.None);
    actor.stats.SetStat("intelligence", 5);
    actor.stats.SetStat("charisma", 5);
    actor.stats.SetStat("endurance", 5);
    actor.stats.SetStat("perception", 5);
    actor.stats.SetStat("agility", 5);
    actor.stats.SetStat("willpower", 5);
    actor.stats.SetStat("strength", 5);
    actor.stats.RestoreCondition();
    actor.hotbar = new HotBar(10, actor);
    actor.hotbar.AddItem(0, ItemFactory.Factory(ItemFactory.Items.Knife));
    return actor;
  }

  public static Actor DebugEnemyCharacter(){
    Actor actor = FromComponentTypes(InputSources.AI, StatsHandlers.Icepaws, Bodies.HumanoidBody, InventoryHandlers.None);
    actor.stats.SetStat("intelligence", 5);
    actor.stats.SetStat("charisma", 5);
    actor.stats.SetStat("endurance", 5);
    actor.stats.SetStat("perception", 5);
    actor.stats.SetStat("agility", 5);
    actor.stats.SetStat("willpower", 5);
    actor.stats.SetStat("strength", 5);
    actor.stats.RestoreCondition();
    actor.hotbar = new HotBar(10, actor);
    actor.hotbar.AddItem(0, ItemFactory.Factory(ItemFactory.Items.Knife));
    return actor;
  }
}
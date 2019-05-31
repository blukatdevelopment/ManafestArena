using Godot;
using System;
using System.Collections.Generic;

public class ActorFactory {
  public enum InputSources {
    None,
    Player1, // Keyboard input
    Remote,
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
    HumanoidBody
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
    InitBody(body, actor);
    InitInventory(inventoryHandler, actor);
    return actor;
  }

  public static void InitInputHandler(InputSources inputSource, Actor actor){
    FPSInputHandler fps;
    MappedInputSource mapped;
    StateAi ai;
    switch(inputSource){
      case InputSources.Player1:
        mapped = new MappedInputSource(Session.GetDevice(0), FPSInputHandler.GetMappings());
        fps = new FPSInputHandler(actor);
        fps.RegisterInputSource(mapped as IInputSource);
        actor.inputHandler = fps as IInputHandler;
      break;
      case InputSources.Remote:
        // Set up net source
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

  public static void InitBody(Bodies body, Actor actor){
    switch(body){
      case Bodies.PillBody:
        actor.body = new PillBody(actor);
      break;
      case Bodies.HumanoidBody:
        actor.body = new HumanoidBody(actor);
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
      case Characters.DebugPlayer: // Test player1
        ret = DebugPlayerCharacter();
      break;
      case Characters.Target: // For target practice
        ret = TargetCharacter();
        break;
      case Characters.DebugEnemy: // For live fire practice
        ret = DebugEnemyCharacter();
        break;
    }
    return ret;
  }

  public static Actor DebugPlayerCharacter(){
    Actor actor = FromComponentTypes(InputSources.Player1, StatsHandlers.Icepaws, Bodies.HumanoidBody, InventoryHandlers.None);
    actor.stats.SetStat("intelligence", 5);
    actor.stats.SetStat("charisma", 5);
    actor.stats.SetStat("endurance", 5);
    actor.stats.SetStat("perception", 5);
    actor.stats.SetStat("agility", 5);
    actor.stats.SetStat("willpower", 5);
    actor.stats.SetStat("strength", 5);
    actor.stats.RestoreCondition();
    actor.hotbar.AddItem(0, ItemFactory.Factory(ItemFactory.Items.Knife));
    actor.hotbar.AddItem(1, ItemFactory.Factory(ItemFactory.Items.Crossbow));
    actor.body.InitCam(0);
    return actor;
  }

  public static Actor TargetCharacter(){
    Actor actor = FromComponentTypes(InputSources.None, StatsHandlers.Icepaws, Bodies.PillBody, InventoryHandlers.None);
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
    actor.hotbar.AddItem(1, ItemFactory.Factory(ItemFactory.Items.Crossbow));
    return actor;
  }
}
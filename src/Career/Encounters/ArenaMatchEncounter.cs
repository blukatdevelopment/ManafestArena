using Godot;
using System;
using System.Collections.Generic;

public class ArenaMatchEncounter : IEncounter {
  string mapName;

  public ArenaMatchEncounter(){}

  public ArenaMatchEncounter(string mapName){
    this.mapName = mapName;
  }

  public string GetDisplayName(){
    return "Arena Match";
  }  

  public void StartEncounter(){
    Career career = Career.GetActiveCareer();
    if(career == null){
      return;
    }

    Arena arena = new Arena();
    arena.killQuota = 5;
    arena.player = career.GetPlayer();
    for(int i = 0; i < 5; i++){
      arena.enemies.Add(ActorFactory.FromCharacter(ActorFactory.Characters.DebugEnemy));
    }

    Session.AddGamemode(arena as Node);
    arena.Init(new string[]{ mapName });
    Session.ChangeMenu("HUDMenu");
  }
  
  public IEncounter GetRandomEncounter(){
    return new ArenaMatchEncounter(RandomArenaMap()) as IEncounter;
  }

  private string RandomArenaMap(){
    return "res://Assets/Scenes/Maps/Small.tscn"; // TODO: Remove
    List<string> arenaMaps = new List<string>{
      "res://Assets/Scenes/Maps/Levels.tscn",
      "res://Assets/Scenes/Maps/Maze.tscn",
      "res://Assets/Scenes/Maps/Open.tscn",
      "res://Assets/Scenes/Maps/Pillars.tscn",
      "res://Assets/Scenes/Maps/Urban.tscn",
      "res://Assets/Scenes/Maps/Colleseum.tscn",
      "res://Assets/Scenes/Maps/MazeII.tscn",
      "res://Assets/Scenes/Maps/Rural.tscn",
      "res://Assets/Scenes/Maps/Town.tscn"
    };
    int choice = Util.RandInt(0, arenaMaps.Count);
    return arenaMaps[choice];
  }

}
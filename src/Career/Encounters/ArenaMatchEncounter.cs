using Godot;
using System;
using System.Collections.Generic;

public class ArenaMatchEncounter : IEncounter {
  string info;

  public string GetDisplayName(){
    return "";
  }  

  public void StartEncounter(){
    ArenaSettings settings = new ArenaSettings();
    settings.useKits = false;
    settings.usePowerups = false;
    //settings.enemies = EnemiesForMap(info);
    //settings.player = playerData;
    Arena.arenaSettings = settings;

    Arena.LocalArena(info);
  }
  
  public IEncounter GetRandomEncounter(){
    return null;
  }

  private string RandomArenaMap(){
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
    int choice = Util.RandInt(0, arenaMaps.Count-1);
    return arenaMaps[choice];
  }

}
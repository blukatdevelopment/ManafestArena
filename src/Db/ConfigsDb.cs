/*
  This class automatically loads data from configuration files into a static context.
*/
using Godot;
using System;
using System.Collections.Generic;

public class ConfigsDb {
  public const string CardsFile = "Configs/cards.json";
  public const string SoundsFile = "Configs/sounds.json";
  public const string SongsFile = "Configs/songs.json";
  public const string LootFile = "Configs/loot.json";
  public const string ItemsFile = "Configs/items.json";
  public const string ActorsFile = "Configs/actors.json";

  public static List<CardRecord> cards;
  public static List<SoundRecord> sounds;
  public static List<SongRecord> songs;
  public static List<LootActionRecord> loots;
  public static List<ItemRecord> items;
  public static List<ActorRecord> actors;

  public static void Init(){
    cards = RecordsFromJsonFile<CardRecord>(CardsFile);
    sounds = RecordsFromJsonFile<SoundRecord>(SoundsFile);
    songs = RecordsFromJsonFile<SongRecord>(SongsFile);
    loots = RecordsFromJsonFile<LootActionRecord>(LootFile);
    items = RecordsFromJsonFile<ItemRecord>(ItemsFile);
    actors = RecordsFromJsonFile<ActorRecord>(ActorsFile);
  }

  public static List<T> RecordsFromJsonFile<T>(string filePath){
    string json = System.IO.File.ReadAllText(filePath);
    List<T> list = Util.FromJson<List<T>>(json);

    GD.Print("Read " + list.Count + " records from " + filePath);
    return list;
  }
}
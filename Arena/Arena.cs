/*
  The Arena is a self-contained game mode.
  Actors score points by killing other actors
  over the course of a round's duration.
*/

using Godot;
using System;
using System.Collections.Generic;

public class Arena : Spatial {
  public bool local;
  public List<Actor> actors;
  public Spatial terrain;
  public List<Vector3> actorSpawnPoints, itemSpawnPoints;
  public int nextId = -2147483648;
  public bool gameStarted = false;
  public ArenaSettings settings;
  const float ScoreDuration = 5f;
  public float roundTimeRemaining, secondCounter;
  public bool roundTimerActive = false;
  public bool scorePresented = false;
  public System.Collections.Generic.Dictionary<int, int> scores;
  public int playerWorldId = -1;

  int playersReady = 0;

  public void Init(bool local){
    settings = Session.session.arenaSettings;
    
    if(settings == null){
      GD.Print("Using default arena settings.");
      settings = new ArenaSettings();
    }
    
    this.local = local;
    actors = new List<Actor>();
    scores = new System.Collections.Generic.Dictionary<int, int>();

    InitTerrain();
    InitSpawnPoints();
    LocalInit();
  }

  public override void _Process(float delta){
    if(roundTimerActive){
      Timer(delta);
    }
  }

  public void Timer(float delta){
    secondCounter += delta;

    if(secondCounter >= 1.0f){
      roundTimeRemaining -= secondCounter;
      secondCounter = 0f;
    }
  }
  
  public bool PlayerWon(){
    int max = playerWorldId;
    
    foreach(KeyValuePair<int, int> key in scores){
      int score = key.Value;
      
      if(score > scores[max]){
        max = score;
      }
    }
    
    if(max == playerWorldId){
      return true;
    }
    
    return false;
  }

  public string GetObjectiveText(){
    if(playerWorldId == -1){
      return "Player not initialized.";
    }
    
    if(scorePresented){
      return PlayerWon() ? "Victory!" : "Defeat!";
    }

    string ret = "Arena\n";
    
    string timeText = TimeFormat( (int)roundTimeRemaining);
    ret += "Time: " + timeText + "\n";
    ret += "Score: " + scores[playerWorldId];
    
    return ret;
  }
  
  public string TimeFormat(int timeSeconds){
    int minutes = timeSeconds / 60;
    int seconds = timeSeconds % 60;
    string minutesText = "" + minutes;
    
    if(minutes < 1){
      minutesText = "00";
    }
    
    string secondsText = "" + seconds;
    
    if(seconds < 1){
      secondsText = "00";
    }
    
    return minutesText + ":" + secondsText;
  }

  public int NextId(){
    int ret = nextId;
    nextId++;
    return ret;
  }

  public string NextBotName(){
    string name = "Bot_" + nextId;
    nextId++;
    return name;
  }

  public string NextItemName(){
    string name = "Item_" + nextId;
    nextId++;
    return name;
  }

  public void LocalInit(){
    if(settings.usePowerups){
      for(int i = 0; i < 1; i++){
        SpawnItem(Item.Types.AmmoPack, 10);
        SpawnItem(Item.Types.HealthPack);  
      }
    }

    InitActor(Actor.Brains.Player1, NextId());
    for(int i = 0; i < settings.bots; i++){
      InitActor(Actor.Brains.Ai, NextId());
    }

    roundTimeRemaining = settings.duration * 60;

    roundTimerActive = true;
  }

  public Actor InitActor(Actor.Brains brain, int id){
    scores.Add(id, 0);

    SpawnActor(brain, id);
    
    if(brain == Actor.Brains.Player1){
      playerWorldId = id;
    }
    
    return null;
  }

  public void InitTerrain(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Prefabs/Terrain.tscn");
    Node instance = ps.Instance();
    AddChild(instance);
    terrain = (Spatial)instance;
  }
  
  public void HandleEvent(SessionEvent sessionEvent){
    if(sessionEvent.type == SessionEvent.Types.ActorDied ){
      HandleActorDead(sessionEvent);
    }
    else if(sessionEvent.type == SessionEvent.Types.Pause){
      TogglePause();
    }
  }

  public void HandleActorDead(SessionEvent sessionEvent){
    string[] actors = sessionEvent.args;  
    
    if(actors == null || actors.Length == 0 || actors[0] == ""){
      return;
    }

    Node actorNode = GetNode(new NodePath(actors[0]));
    Actor actor = actorNode as Actor;
    Actor.Brains brain = actor.brainType;
    
    int id = actor.id;

    actorNode.Name = "Deadplayer" + id;
    actor.QueueFree();
    actor = SpawnActor(brain, id);
    
    if(actor.brainType == Actor.Brains.Player1){
      Session.ChangeMenu(Menu.Menus.HUD);
    }
    
    if(actors.Length < 2 || actors[1] == ""){
     return; 
    }

    Node killerNode = GetNode(new NodePath(actors[1]));
    Actor killer = killerNode as Actor;

    if(killer != null){
      scores[killer.id]++;
    }
  }

  public void SetPause(bool val){
    foreach(Actor actor in actors){
      if(actor.IsPaused() != val){
        actor.TogglePause();
      }
    }
  }
  
  public void TogglePause(){
    foreach(Actor actor in actors){
      actor.TogglePause();
    }
  }
  
  public void InitSpawnPoints(){
    SceneTree st = GetTree();
    Godot.Array actorSpawns = st.GetNodesInGroup("ActorSpawnPoint");
    
    this.actorSpawnPoints = new List<Vector3>();
    
    for(int i = 0; i < actorSpawns.Count; i++){
      Spatial spawnPoint = actorSpawns[i] as Spatial;
      
      if(spawnPoint != null){
        this.actorSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
      }
    }
    
    Godot.Array itemSpawns = st.GetNodesInGroup("ItemSpawnPoint");
    this.itemSpawnPoints = new List<Vector3>();
    
    for(int i = 0; i < itemSpawns.Count; i++){
      Spatial spawnPoint = itemSpawns[i] as Spatial;
      
      if(spawnPoint != null){
        this.itemSpawnPoints.Add(spawnPoint.GetGlobalTransform().origin);
      }
    }
  }
  
  public void SpawnItem(Item.Types type, int quantity = 1){
    Vector3 pos = RandomItemSpawn();
    Item item = Item.Factory(type);
    item.Translation = pos;
    AddChild(item);
  }
  
  public Vector3 RandomItemSpawn(){
    System.Random rand = Session.GetRandom();
    int randInt = rand.Next(itemSpawnPoints.Count);
    return itemSpawnPoints[randInt];
  }
  
  public Actor SpawnActor(Actor.Brains brain = Actor.Brains.Player1, int id = 0){
    Vector3 pos = RandomActorSpawn();
    

    ActorData dat = new ActorData();

    dat.id = id;
    dat.pos = pos;
    dat.health = dat.healthMax = 100;
    if(settings.useKits){
      dat.inventory.ReceiveItem(Item.Factory(Item.Types.Rifle));
      List<Item> kitItems = Item.BulkFactory(Item.Types.Ammo, 100);

      dat.inventory.ReceiveItem(kitItems[0]);
    }

    Actor actor = Actor.Factory(brain, dat);
    actor.NameHand(actor.Name + "(Hand)");  
    
    actors.Add(actor);
    AddChild(actor);

    if(settings.useKits){
      EquipActor(actor, Item.Types.Rifle, "Rifle");
    }

    return actor;
  }
  
  public Vector3 RandomActorSpawn(){
    System.Random rand = Session.GetRandom();
    int randInt = rand.Next(actorSpawnPoints.Count);
    return actorSpawnPoints[randInt];
  }
  
  /* A factory to do all that node stuff in lieu of a constructor */ 
  public static Arena ArenaFactory(){
    PackedScene ps = (PackedScene)GD.Load("res://Scenes/Arena.tscn");
    Node instance = ps.Instance();
    return (Arena)instance;
  }

  public void PlayerReady(){}

  public void EquipActor(Actor actor, Item.Types itemType, string itemName){
    int index = actor.IndexOf(itemType, itemName);
    if(index == -1){
      GD.Print("Actor doesn't have this weapon.");
    }
    else{
      GD.Print("Equipping Actor with " + itemType.ToString());
      actor.EquipItem(index);
    }
  }
}
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
  public List<Vector3> enemySpawnPoints, playerSpawnPoints, itemSpawnPoints;
  public List<Vector3> usedEnemySpawnPoints;
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
  public int totalEnemies;

  public void Init(bool local, string terrainFile){
    settings = Session.session.arenaSettings;
    
    if(settings == null){
      GD.Print("Using default arena settings.");
      settings = new ArenaSettings();
    }
    
    this.local = local;
    actors = new List<Actor>();
    scores = new System.Collections.Generic.Dictionary<int, int>();

    InitTerrain(terrainFile);
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
    if(actors.Count > 1){
      return false;
    }

    if(actors[0].brainType == Actor.Brains.Player1){
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
    
    ret += "Score: " + scores[playerWorldId];
    

    if(totalEnemies == 1){
      ret += "\n" + totalEnemies + " enemies left.";  
    }
    else{
      ret += "\n" + totalEnemies + " enemy left.";
    }
    
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

    Session.session.player = InitActor(settings.player, NextId());
    playerWorldId = Session.session.player.id;
    //GD.Print("Session.session.Player set to " + Session.session.player.ToString());

    foreach(ActorData enemy in settings.enemies){
      Actor enemyActor = InitActor(enemy, NextId());
      totalEnemies++;
    }

    roundTimerActive = false;
  }

  public Actor InitActor(ActorData dat, int id){
    scores.Add(id, 0);
    dat.id = id;
    Actor ret = SpawnActor(dat);

    return ret;
  }

  public void InitTerrain(string terrainFile){
    PackedScene ps = (PackedScene)GD.Load(terrainFile);
    Node instance = ps.Instance();
    AddChild(instance);
    terrain = (Spatial)instance;

    // Everything below this line is a dirty hack to work around a bug.
    GridMap gm = Util.GetChildByName(terrain, "Map") as GridMap;
    
    MeshLibrary theme = gm.Theme;
    List<Vector3> colors = new List<Vector3>{
      new Vector3(1,0,0),
      new Vector3(0,1,0),
      new Vector3(0,0,1),
      new Vector3(1,1,0),
      new Vector3(0,1,1),
      new Vector3(1,0,1),
      new Vector3(1,1,1),
      new Vector3(0,0,0)
    };

    for(int i = 0; i < 8; i++){
      ArrayMesh arrMesh = theme.GetItemMesh(i) as ArrayMesh;
      SpatialMaterial material = GFX.GetColorSpatialMaterial(colors[i]) as SpatialMaterial;
      
      material.EmissionEnabled = true;
      material.Emission = GFX.Color(colors[i]);
      material.EmissionEnergy = 0.5f;
      
      material.TransmissionEnabled = true;
      material.Transmission = GFX.Color(colors[i]);

      material.RefractionEnabled = false;
      
      material.Metallic = 1f;
      
      material.Roughness = 0.5f;

      arrMesh.SurfaceSetMaterial(0, material);

    }
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
    string[] actorPaths = sessionEvent.args;  
    
    if(actorPaths == null || actorPaths.Length == 0 || actorPaths[0] == ""){
      return;
    }

    Node actorNode = GetNode(new NodePath(actorPaths[0]));
    Actor actor = actorNode as Actor;
    Actor.Brains brain = actor.brainType;
    ClearActor(actor);

    if(actor.brainType == Actor.Brains.Player1){
      Session.session.career.FailEncounter();
      return;
    }
    totalEnemies--;
    
    AwardPoints(actorPaths);
    if(PlayerWon()){
      Session.session.career.CompleteEncounter();
    }
  }

  public void ClearActor(Actor actor){
    if(actor == null){
      return;
    }
    int id = actor.id;

    actor.QueueFree();
    actors.Remove(actor);
  }

  public void AwardPoints(string[] actorPaths){
    if(actorPaths.Length < 2 || actorPaths[1] == ""){
     GD.Print("No killer specified");
     return; 
    }

    Node killerNode = GetNode(new NodePath(actorPaths[1]));
    Actor killer = killerNode as Actor;

    if(killer != null && killer.id != -1){
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
    
    playerSpawnPoints = GetSpawnPoints("PlayerSpawnPoint");
    enemySpawnPoints = GetSpawnPoints("EnemySpawnPoint");
    itemSpawnPoints = GetSpawnPoints("ItemSpawnPoint");

    usedEnemySpawnPoints = new List<Vector3>();
  }
  

  List<Vector3> GetSpawnPoints(string name){
    List<Vector3> ret = new List<Vector3>();

    SceneTree st = GetTree();
    List<System.Object> objs = Util.ArrayToList(st.GetNodesInGroup(name));

    foreach(System.Object obj in objs){
      Spatial spat = obj as Spatial;
      if(spat != null){
        ret.Add(spat.GetGlobalTransform().origin);
      }
    }

    return ret;
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
  
  public Actor SpawnActor(ActorData dat){
    Actor.Brains brain = dat.GetBrain();

    Vector3 pos;
    if(brain == Actor.Brains.Player1){
      pos = RandomSpawn(playerSpawnPoints);
    }
    else{
      pos = RandomSpawn(enemySpawnPoints, usedEnemySpawnPoints);
      usedEnemySpawnPoints.Add(pos);
    }

    dat.pos = pos;

    Actor actor = Actor.Factory(brain, dat);
    actor.NameHand(actor.Name + "(Hand)");  
    
    actors.Add(actor);
    AddChild(actor);

    return actor;
  }
  
  public Vector3 RandomSpawn(List<Vector3> spawnList){
    System.Random rand = Session.GetRandom();
    int randInt = rand.Next(spawnList.Count);
    
    return spawnList[randInt];
  }

  // Don't place spawnpoints on top of each other.
  public Vector3 RandomSpawn(List<Vector3> spawnList, List<Vector3> usedList){
    if(spawnList.Count == usedList.Count){
      usedList = new List<Vector3>();
    }
    if(spawnList.Count == 0){
      GD.Print("RandomSpawn: spawnList empty");
    }

    System.Random rand = Session.GetRandom();

    Vector3 ret = new Vector3();
    bool finished = false;
    const int MaxIterations = 500;
    int i = 0;

    while(!finished && i < MaxIterations){
      i++;
      int randInt = rand.Next(spawnList.Count);
      ret = spawnList[randInt];
      
      if(usedList.IndexOf(ret) == -1){
        finished = true;
      } 
    }

    return ret;
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
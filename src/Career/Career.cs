/*
    Store's players progress through a game.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Career : Node , IGamemode{
  public List<CareerNode> careerNodes;
  public int lastNode;
  public bool encounterInProgress;
  public const int CareerLevels = 15;
  public const int MaxCareerNodesPerLevel = 3;
  public Actor player;
  public CareerData careerData;
  public LootTable lootTable;

  public Career(){
    careerNodes = new List<CareerNode>();
    lootTable = new LootTable();
  }

  new public string ToString(){
    string text = "Career:\n";

    return text;
  }

  public void BeginEncounter(int id){
    CareerNode node = CareerNode.GetById(careerNodes, id);
    if(node != null){
      encounterInProgress = true;
      lastNode = id;
      node.encounter.StartEncounter();
    }
    else{
      GD.Print("BeginEncounter: id " + id + " doesn't exist.");
    }
  }

  public void Update(float delta){}

  public void CompleteEncounter(){
    GD.Print("CompleteEncounter");
    encounterInProgress = false;
    Session.ChangeMenu("CareerMenu");

    // if(Session.GetPlayer() != null){
    //   //playerData = Session.GetPlayer().GetData();
    //   if(playerData != null && playerData.stats != null){
    //     int health = playerData.stats.GetBaseStat(StatsManager.Stats.Health);
    //     stats.SetBaseStat(StatsManager.Stats.Health, health);
    //     playerData.stats = stats;
    //   }
    // }

    // Session.ClearGame();
    // int id = stats.GetStat(StatsManager.Stats.CurrentNode);
    // CareerNode node = CareerNode.GetNode(id, careerNodes);
    // int nodeLevel = CareerNode.GetLevel(node, careerNodes);
    // int nextLevel = nodeLevel -1;

    // stats.SetBaseStat(StatsManager.Stats.CurrentLevel, nextLevel);
    // stats.SetBaseStat(StatsManager.Stats.LastNode, id);
    // stats.SetBaseStat(StatsManager.Stats.CurrentNode, 0);
    // stats.SetBaseStat(StatsManager.Stats.NodeInProgress, 0);
    
    // CareerDb.SaveCareer(this);

    // if(nextLevel == -1){
    //   CompleteGame();
    // }
    // else{
    //   Session.ChangeMenu(Menu.Menus.Career);
    // }
  }

  public Actor GetPlayer(){
    return player;
  }

  public string GetObjectiveText(){
    return "";
  }

  public void HandleEvent(SessionEvent evt){
  }

  public void Init(string[] args){

  }

  public bool GameOver(){
    return false;
  }

  public bool Victory(){
    return false;
  }

  public void CompleteGame(){
    GD.Print("CompleteGame");
    Session.ClearGame();
    Session.ChangeMenu("EndGameMenu");
  }

  public void FailEncounter(){
    Session.ClearGame();
    GD.Print("FailEncounter");
    Session.ChangeMenu("EndGameMenu");
  }

  public static IEncounter RandomEncounter(){
    List<IEncounter> encounterTypes = new List<IEncounter>{
      new ArenaMatchEncounter() as IEncounter//,
      //new PressEventEncounter() as IEncounter,
      //new RestSiteEncounter() as IEncounter,
      //new ShopEncounter() as IEncounter
    };
    int choice = Util.RandInt(0, encounterTypes.Count);
    return encounterTypes[choice].GetRandomEncounter();
  }

  public static void StartNewCareer(string championName = ""){
      Career career = CareerTreeFactory.Factory(championName);
      career.careerData = new CareerData();

      Session.AddGamemode("career", career as Node);
      Session.ChangeMenu("CareerMenu");
      CareerDb.SaveCareerData(career.careerData);
  }

  public static void ContinueCareer(){
    Career career = new Career();
    career.careerData = CareerDb.LoadCareerData();
    Session.AddGamemode("career", career as Node);
    Session.ChangeMenu("CareerMenu");
  }

  public static void Save(){
    Career career = Career.GetActiveCareer();
    if(career != null){
      CareerDb.SaveCareerData(career.careerData);
    }
    else{
      GD.Print("Cannot save null career");
    }
  }

  public static Career GetActiveCareer(){
    if(Session.session.activeGamemodes.ContainsKey("career")){
      return Session.session.activeGamemodes["career"] as Career;
    }
    return null;
  }

}

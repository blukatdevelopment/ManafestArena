/*
    Store's players progress through a game.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Career : Node {
  public List<CareerNode> careerNodes;
  public int lastNode;
  public bool encounterInProgress;
  public const int CareerLevels = 15;
  public const int MaxCareerNodesPerLevel = 3;

  public Career(string championName = ""){
    careerNodes = new List<CareerNode>();
  }

  public string ToString(){
    string text = "Career:\n";

    return text;
  }

  public void BeginEncounter(int id){
    CareerNode node = CareerNode.GetById(careerNodes, id);
    if(node != null){
      node.encounter.StartEncounter();
      encounterInProgress = true;
      lastNode = id;
    }
    else{
      GD.Print("BeginEncounter: id " + id + " doesn't exist.");
    }
  }

  public void CompleteEncounter(){
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

  public bool GameOver(){
    return false;
  }

  public bool Victory(){
    return false;
  }

  public void CompleteGame(){
    GD.Print("CompleteGame");
    Session.ClearGame();
    //stats.SetBaseStat(StatsManager.Stats.Victory, 1);
    Session.ChangeMenu("EndGameMenu");
  }

  public void FailEncounter(){
    Session.ClearGame();
    GD.Print("FailEncounter");
    Session.ChangeMenu("EndGameMenu");
  }

  public static Career Factory(string championName){
    Career ret = new Career(championName);
    
    CareerTreeFactory factory = new CareerTreeFactory();
    factory.Generate();
    ret.careerNodes = factory.careerNodes;
    ret.lastNode = -1;

    return ret;
  }

  public static IEncounter RandomEncounter(){
    List<IEncounter> encounterTypes = new List<IEncounter>{
      new ArenaMatchEncounter() as IEncounter,
      new PressEventEncounter() as IEncounter,
      new RestSiteEncounter() as IEncounter,
      new ShopEncounter() as IEncounter
    };
    int choice = Util.RandInt(0, encounterTypes.Count);
    return encounterTypes[choice].GetRandomEncounter();
  }

  public static void StartNewCareer(string championName = ""){
      Career career = Factory(championName);
      Session.AddGamemode(career as Node);
      Session.ChangeMenu("CareerMenu");
      CareerDb.SaveCareer(career);
  }

  public static void ContinueCareer(){
    Career career = CareerDb.LoadCareer();
    Session.AddGamemode(career as Node);
    Session.ChangeMenu("CareerMenu");
  }

  public static void Save(){
    Career career = Career.GetActiveCareer();
    if(career != null){
      CareerDb.SaveCareer(career);
    }
    else{
      GD.Print("Cannot save null career");
    }
  }

  public static Career GetActiveCareer(){
    foreach(Node gamemodeNode in Session.session.activeGamemodes){
      Career career = gamemodeNode as Career;
      if(career != null){
        return career;
      }
    }
    return null;
  }

}

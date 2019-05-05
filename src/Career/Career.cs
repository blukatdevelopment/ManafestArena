/*
    Store's players progress through a game.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Career : Node {
  public List<CareerNode> careerNodes, leaves;
  public CareerNode root;

  public Career(string championName = ""){
    careerNodes = new List<CareerNode>();
    root = null;
    leaves = new List<CareerNode>();
    if(championName != ""){
      //stats = new StatsManager(championName);
      //playerData = new ActorData(stats);
    }
  }

  public string ToString(){
    string text = "Career:\n";

    text += "Root: " + root.ToString() + "\n";

    text += "\nLeaves:\n";

    foreach(CareerNode leaf in leaves){
      text += "\t" + leaf.ToString() + "\n";
    }

    text += "\nNodes:\n";
    foreach(CareerNode node in careerNodes){
      text += "\t" + node.ToString() + "\n";
    }

    text += "Levels:\n";

    System.Collections.Generic.Dictionary<int, CareerNode[]> levels;
    levels = CareerNode.GetLevels(careerNodes);

    foreach(int key in levels.Keys){
        text += "\n level[" + key + "]:"; 
        CareerNode[] level = levels[key];
        for(int i = 0; i < level.Length; i++){
          text += level[i].ToString();
        }
        text += "\n";
    }

    return text;
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
    ret.careerNodes = GenerateCareerTree(championName);
    ret.root = CareerNode.Root(ret.careerNodes);
    ret.leaves = CareerNode.Leaves(ret.careerNodes);
    return ret;
  }

  public static List<CareerNode> GenerateCareerTree(string championName){
    List<CareerNode> ret = new List<CareerNode>();
    ret.Add(RandomCareerNode(1, 2, -1, 3));
    ret.Add(RandomCareerNode(2, 4, -1, -1));
    ret.Add(RandomCareerNode(3, 4, -1, -1));
    ret.Add(RandomCareerNode(4, 5, -1, 6));
    ret.Add(RandomCareerNode(5, 7, -1, -1));
    ret.Add(RandomCareerNode(6, 8, 9, -1));
    ret.Add(RandomCareerNode(7, 10, -1, -1));
    ret.Add(RandomCareerNode(8, 11, -1, -1));
    ret.Add(RandomCareerNode(9, 12, -1, -1));
    ret.Add(RandomCareerNode(10, 13, -1, 14));
    ret.Add(RandomCareerNode(11, 15, -1, -1));
    ret.Add(RandomCareerNode(12, 15, -1, -1));
    ret.Add(RandomCareerNode(13, 16, -1, -1));
    ret.Add(RandomCareerNode(14, 16, -1, -1));
    ret.Add(RandomCareerNode(15, 17, -1, -1));
    ret.Add(RandomCareerNode(16, 18, -1, -1));
    ret.Add(RandomCareerNode(17, 18, -1, -1));
    ret.Add(RandomCareerNode(18, 19, -1, 20));
    ret.Add(RandomCareerNode(19, -1, -1, -1));
    ret.Add(RandomCareerNode(20, -1, -1, -1));
    return ret;
  }

  public static CareerNode RandomCareerNode(int id, int child1, int child2, int child3){
    CareerNode node = new CareerNode();
    node.nodeId = id;
    node.child1 = child1;
    node.child2 = child2;
    node.child3 = child3;

    node.encounter = RandomEncounter();
    return node;
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

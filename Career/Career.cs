/*
    Store's players progress through a game.
*/
using Godot;
using System;
using System.Collections.Generic;

public class Career {
    public List<CareerNode> careerNodes, leaves;
    public CareerNode root;
    public StatsManager stats;
    public PressEvent pressEvent;
    public ActorData playerData;

    public Career(string championName = ""){
      careerNodes = new List<CareerNode>();
      root = null;
      leaves = new List<CareerNode>();
      if(championName != ""){
        stats = new StatsManager(championName);
        playerData = new ActorData(stats);
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

    public void ExecuteNode(int id){
      CareerNode node = CareerNode.GetNode(id, careerNodes);
      if(node == null){
        GD.Print("Can't execute null node id " + id);
        return;
      }

      stats.SetBaseStat(StatsManager.Stats.LastNode, stats.GetStat(StatsManager.Stats.CurrentNode));
      stats.SetBaseStat(StatsManager.Stats.CurrentNode, id);
      stats.SetBaseStat(StatsManager.Stats.NodeInProgress, 1);
      CareerDb.SaveCareer(this);

      switch(node.nodeType){
        case CareerNode.NodeTypes.None:
          GD.Print("NullEncounter");
          CompleteEncounter();
          break;
        case CareerNode.NodeTypes.ArenaMatch:
          ArenaEncounter(node.extraInfo);
          break;
        case CareerNode.NodeTypes.BossMatch:
          ArenaEncounter(node.extraInfo);
          break;
        case CareerNode.NodeTypes.FinalBoss:
          ArenaEncounter(node.extraInfo);
          break;
        case CareerNode.NodeTypes.Shop:
          ShopEncounter(node.extraInfo);
          break;
        case CareerNode.NodeTypes.RestSite:
          RestSiteEncounter(node.extraInfo);
          break;
        case CareerNode.NodeTypes.PressEvent:
          PressEventEncounter(node.extraInfo);
          break;
      }
    }

    public void ArenaEncounter(string info = ""){
      GD.Print("ArenaMatchEncounter: " + info);
      ArenaSettings settings = new ArenaSettings();
      settings.useKits = false;
      settings.usePowerups = false;
      settings.enemies = EnemiesForMap(info);
      settings.player = playerData;
      Session.session.arenaSettings = settings;

      Session.LocalArena(info);
    }

    public void ShopEncounter(string info = ""){
      GD.Print("ShopEncounter");
      Session.ChangeMenu(Menu.Menus.Shop);
    }

    public void RestSiteEncounter(string info = ""){
      GD.Print("RestSiteEncounter");
      Session.ChangeMenu(Menu.Menus.RestSite);
    }

    public void PressEventEncounter(string info = ""){
      GD.Print("PressEventEncounter -info " + info);
      pressEvent = CareerDb.LoadPressEvent(info);
      
      Session.ChangeMenu(Menu.Menus.PressEvent);
    }

    public void CompleteEncounter(){
      if(Session.session.player != null){
        playerData = Session.session.player.GetData();
        playerData.SaveStats(stats);
      }

      Session.ClearGame();
      int id = stats.GetStat(StatsManager.Stats.CurrentNode);
      CareerNode node = CareerNode.GetNode(id, careerNodes);
      int nodeLevel = CareerNode.GetLevel(node, careerNodes);
      int nextLevel = nodeLevel -1;

      stats.SetBaseStat(StatsManager.Stats.CurrentLevel, nextLevel);
      stats.SetBaseStat(StatsManager.Stats.LastNode, id);
      stats.SetBaseStat(StatsManager.Stats.CurrentNode, 0);
      stats.SetBaseStat(StatsManager.Stats.NodeInProgress, 0);
      
      CareerDb.SaveCareer(this);

      if(nextLevel == -1){
        CompleteGame();
      }
      else{
        Session.ChangeMenu(Menu.Menus.Career);
      }
    }

    public void CompleteGame(){
      GD.Print("CompleteGame");
      Session.ClearGame();
      stats.SetBaseStat(StatsManager.Stats.Victory, 1);
      Session.ChangeMenu(Menu.Menus.EndGame);
    }

    public void FailEncounter(){
      Session.ClearGame();
      GD.Print("FailEncounter");
      Session.ChangeMenu(Menu.Menus.EndGame);
    }

    public static Career Factory(string championName){
      Career ret = new Career(championName);
      ret.careerNodes = GenerateCareerTree(championName);
      ret.root = CareerNode.Root(ret.careerNodes);
      ret.leaves = CareerNode.Leaves(ret.careerNodes);
      return ret;
    }

    public static Career Factory(List<CareerNode> careerNodes, StatsManager stats){
      Career ret = new Career();
      ret.careerNodes = careerNodes;
      ret.root = CareerNode.Root(ret.careerNodes);
      ret.leaves = CareerNode.Leaves(ret.careerNodes);
      ret.stats = stats;
      ret.playerData = new ActorData(ret.stats);
      return ret;
    }

    // TODO: Remove hardcoding and randomize this
    public static List<CareerNode> GenerateCareerTree(string championName){
      List<CareerNode> ret = new List<CareerNode>();

      ret.Add(CareerNode.FromRow(new string[] {"1", "3", "2", "-1", "3", "res://Scenes/Maps/Test.tscn"}));
      ret.Add(CareerNode.FromRow(new string[] {"2", "2", "4", "-1", "-1", "res://Scenes/Maps/Test.tscn"}));
      ret.Add(CareerNode.FromRow(new string[] {"3", "1", "5", "-1", "-1", "res://Scenes/Maps/Test.tscn"}));
      
      ret.Add(CareerNode.FromRow(new string[] {"4", "5", "6", "-1", "-1", ""}));
      ret.Add(CareerNode.FromRow(new string[] {"5", "4", "6", "-1", "-1", ""}));
      ret.Add(CareerNode.FromRow(new string[] {"6", "6", "-1", "-1", "-1", "PressEvents/test.csv"}));

      return ret;
    }

    public static void StartNewCareer(string championName = ""){
        Career career = Factory(championName);
        Session.session.career = career;
        Session.ChangeMenu(Menu.Menus.Career);
        CareerDb.SaveCareer(career);
    }

    public static void Save(){
      CareerDb.SaveCareer(Session.session.career);
    }


    public static List<ItemData> ShopItems(){
      List<string> names = RandomShopItemNames();
      List<ItemData> ret = new List<ItemData>();
      
      foreach(string name in names){
        ret.Add(ItemData.Factory(name));
      }
      
      return ret;
    }

    public static List<string> RandomShopItemNames(){
      return new List<string>{
        "sword",
        "magic_rifle",
        "magic_beans",
        "old_fish",
        "magic_talisman",
        "nutriloaf",
        "fire_tome",
        "bow_and_arrows"
      };
    }

    public static List<string> RestSiteUpgrades(){
      return new List<string>{
        "one",
        "two",
        "three"
      };
    }

    public static string UpgradeDescription(string upgradeName){
      switch(upgradeName){
        case "one":
          return "First is the worst.";
          break;
        case "two":
          return "Second is the best.";
          break;
        case "three":
          return "Third is the bird in a polka-dot dress.";
          break;
      }
      return "";
    }


  public static List<ActorData> EnemiesForMap(string mapName){
    List<string> enemyNames = EnemyNamesForMap(mapName);
    List<ActorData> ret = new List<ActorData>();

    foreach(string name in enemyNames){
      StatsManager sm = new StatsManager(name);
      ret.Add(new ActorData(sm));
    }

    return ret;
  }

  public static List<string> EnemyNamesForMap(string mapName){
    switch(mapName){
      case "res://Scenes/Maps/Test.tscn":
        return new List<string>{
          "old man rivers",
          "old man jenkins"
        };
        break;
    }

    GD.Print("Invalid map name " + mapName);
    return new List<string>();
  }

}
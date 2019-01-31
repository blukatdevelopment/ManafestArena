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

    public Career(){
      careerNodes = new List<CareerNode>();
      root = null;
      leaves = new List<CareerNode>();
      stats = new StatsManager();
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
          ArenaMatchEncounter(node.extraInfo);
          break;
        case CareerNode.NodeTypes.BossMatch:
          BossMatchEncounter();
          break;
        case CareerNode.NodeTypes.FinalBoss:
          FinalBossEncounter();
          break;
        case CareerNode.NodeTypes.Shop:
          ShopEncounter();
          break;
        case CareerNode.NodeTypes.RestSite:
          RestSiteEncounter();
          break;
        case CareerNode.NodeTypes.PressEvent:
          PressEventEncounter(node.extraInfo);
          break;
      }
    }

    public void ArenaMatchEncounter(string info = ""){
      GD.Print("ArenaMatchEncounter: " + info);
      ArenaSettings settings = new ArenaSettings();
      settings.useKits = false;
      settings.usePowerups = false;
      settings.duration = 100;
      settings.bots = 1;

      Session.session.arenaSettings = settings;

      Session.LocalArena();
      //CompleteEncounter();
    }

    public void BossMatchEncounter(string info = ""){
      GD.Print("BossMatchEncounter");
      CompleteEncounter();
    }

    public void FinalBossEncounter(string info = ""){
      GD.Print("FinalBossEncounter");
      CompleteEncounter();
    }

    public void ShopEncounter(string info = ""){
      GD.Print("ShopEncounter");
      CompleteEncounter();
    }

    public void RestSiteEncounter(string info = ""){
      GD.Print("RestSiteEncounter");
      CompleteEncounter();
    }

    public void PressEventEncounter(string info = ""){
      GD.Print("PressEventEncounter -info " + info);
      if(Session.session.activeMenu == null){
        GD.Print("active menu is null");
      }
      pressEvent = CareerDb.LoadPressEvent(info);
      
      Session.ChangeMenu(Menu.Menus.PressEvent);
    }

    public void CompleteEncounter(){
      int id = stats.GetStat(StatsManager.Stats.CurrentNode);
      CareerNode node = CareerNode.GetNode(id, careerNodes);
      int nodeLevel = CareerNode.GetLevel(node, careerNodes);
      int nextLevel = nodeLevel -1;
      stats.SetBaseStat(StatsManager.Stats.CurrentLevel, nextLevel);
      stats.SetBaseStat(StatsManager.Stats.LastNode, id);
      stats.SetBaseStat(StatsManager.Stats.CurrentNode, 0);
      stats.SetBaseStat(StatsManager.Stats.NodeInProgress, 0);
      
      CareerDb.SaveCareer(this);

      Session.ChangeMenu(Menu.Menus.Career);
    }

    public void FailEncounter(){
      Session.ClearGame();
      
      Session.ChangeMenu(Menu.Menus.Main);
    }


    public static Career Factory(StatsManager.Archetypes archetype){
      Career ret = new Career();
      ret.careerNodes = GenerateCareerTree();
      ret.root = CareerNode.Root(ret.careerNodes);
      ret.leaves = CareerNode.Leaves(ret.careerNodes);
      ret.stats.Init(archetype);
      return ret;
    }

    public static Career Factory(List<CareerNode> careerNodes, StatsManager stats){
      Career ret = new Career();
      ret.careerNodes = careerNodes;
      ret.root = CareerNode.Root(ret.careerNodes);
      ret.leaves = CareerNode.Leaves(ret.careerNodes);
      ret.stats = stats;
      return ret;
    }

    // TODO: Remove hardcoding and randomize this
    public static List<CareerNode> GenerateCareerTree(){
      List<CareerNode> ret = new List<CareerNode>();

      ret.Add(CareerNode.FromRow(new string[] {"1", "1", "2", "-1", "3", "first"}));
      ret.Add(CareerNode.FromRow(new string[] {"2", "1", "4", "-1", "-1", ""}));
      ret.Add(CareerNode.FromRow(new string[] {"3", "1", "5", "-1", "-1", ""}));
      
      ret.Add(CareerNode.FromRow(new string[] {"4", "1", "6", "-1", "-1", ""}));
      ret.Add(CareerNode.FromRow(new string[] {"5", "1", "6", "-1", "-1", ""}));
      ret.Add(CareerNode.FromRow(new string[] {"6", "6", "-1", "-1", "-1", "PressEvents/test.csv"}));

      return ret;
    }

    public static void StartNewCareer(StatsManager.Archetypes archetype = StatsManager.Archetypes.None){
        Career career = Factory(archetype);
        Session.session.career = career;
        Session.ChangeMenu(Menu.Menus.Career);
        CareerDb.SaveCareer(career);
    }

    public static void Save(){
      CareerDb.SaveCareer(Session.session.career);
    }

}
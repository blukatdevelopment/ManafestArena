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
        if(playerData != null && playerData.stats != null){
          int health = playerData.stats.GetBaseStat(StatsManager.Stats.Health);
          stats.SetBaseStat(StatsManager.Stats.Health, health);
          playerData.stats = stats;
        }
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


      Session.session.player = null;
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

    public static StatsManager GetPlayerStats(){
      return Session.session.career.playerData.stats;
    }

    // Primarily for rest sites
    public static void HealPlayer(int health){
      Damage dmg = new Damage();
      dmg.health = -health;

      Session.session.career.playerData.stats.ReceiveDamage(dmg);

      int playerHealth = Session.session.career.playerData.stats.GetStat(StatsManager.Stats.Health);

      if(playerHealth <= 0){
        GD.Print("Career.HealPlayer just killed the player.");
        Session.session.career.FailEncounter();
      }
      else{
        GD.Print("Player was healed " + health + " health and now has " + playerHealth + " health.");
      }
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

      ret.Add(CareerNode.FromRow(new string[] {"1", "1", "2", "-1", "3", "res://Scenes/Maps/Open.tscn"}));
      ret.Add(CareerNode.FromRow(new string[] {"2", "1", "4", "-1", "-1", "res://Scenes/Maps/Levels.tscn"}));
      ret.Add(CareerNode.FromRow(new string[] {"3", "1", "5", "-1", "-1", "res://Scenes/Maps/Pillars.tscn"}));
      
      ret.Add(CareerNode.FromRow(new string[] {"4", "5", "6", "-1", "-1", ""}));
      ret.Add(CareerNode.FromRow(new string[] {"5", "1", "6", "-1", "-1", "res://Scenes/Maps/Urban.tscn"}));
      ret.Add(CareerNode.FromRow(new string[] {"6", "1", "-1", "-1", "-1", "res://Scenes/Maps/Urban.tscn"}));

      return ret;
    }

    public static void StartNewCareer(string championName = ""){
        Session.session.player = null;
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

    public List<Item.Types> GetAvailableSpells(){
      return new List<Item.Types>{
        Item.Types.FireballSpell,
        Item.Types.HealSpell,
        Item.Types.StaminaSpell,
        Item.Types.ManaSpell
      };
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
      string archetype = GetPlayerStats().GetFact(StatsManager.Facts.Archetype);
      GD.Print("Archetype " + archetype);
      List<string> ret = new List<string>();
      switch(archetype){
        case "beast": ret = GetBeastUpgrades(); break;
        case "mage": ret = GetMageUpgrades(); break;
        case "soldier": ret = GetSoldierUpgrades(); break;
      }

      // Fill in the remaining three.
      int needed = 3 - ret.Count;
      List<int> used = new List<int>();
      List<string> generic = GetGenericUpgrades();

      for(int i = 0; i < needed; i++){
        int choice = 0;
        while(used.IndexOf(choice) != -1){
          choice = Util.RandInt(0, generic.Count);
        }
        used.Add(choice);
        ret.Add(generic[choice]);
      }
      return ret;
    }

    public static void SelectRestSiteUpgrade(string selection){
      GD.Print("Selected " + selection);
      StatsManager stats = GetPlayerStats();
      
      int intelligenceBuff = stats.GetStatBuff(StatsManager.Stats.Intelligence);
      int charismaBuff = stats.GetStatBuff(StatsManager.Stats.Charisma);
      int enduranceBuff = stats.GetStatBuff(StatsManager.Stats.Endurance);
      int perceptionBuff = stats.GetStatBuff(StatsManager.Stats.Perception);
      int agilityBuff = stats.GetStatBuff(StatsManager.Stats.Agility);
      int willpowerBuff = stats.GetStatBuff(StatsManager.Stats.Willpower);
      int strengthBuff = stats.GetStatBuff(StatsManager.Stats.Strength);
      int currentHealth = stats.GetStat(StatsManager.Stats.Health);
      int healthBuff = stats.GetStatBuff(StatsManager.Stats.HealthMax);


      switch(selection){
        case "Extra endurance":
          stats.SetStatBuff(StatsManager.Stats.Endurance, enduranceBuff + 2);
        break;
        case "Extra agility":
          stats.SetStatBuff(StatsManager.Stats.Agility, agilityBuff + 2);
        break;
        case "+50 max health":
          stats.SetStatBuff(StatsManager.Stats.HealthMax, healthBuff + 50);
          stats.SetBaseStat(StatsManager.Stats.Health, currentHealth + 50);
        break;
      }
    }

    public static List<string> GetBeastUpgrades(){
      StatsManager stats = GetPlayerStats();

      List<string> ret = new List<string>();

      if(stats.GetFact(StatsManager.Facts.Slot2) != "spear"){
        ret.Add("Second spear");
      }
      return ret;
    }

    public static List<string> GetMageUpgrades(){
      List<string> ret = new List<string>();


      return ret;
    }

    public static List<string> GetSoldierUpgrades(){
      List<string> ret = new List<string>();
      
      return ret;
    }

    public static List<string> GetGenericUpgrades(){
      return new List<string>{
        "Extra endurance",
        "Extra agility",
        "+50 max health"
      };
    }

    public static string UpgradeDescription(string upgradeName){
      System.Collections.Generic.Dictionary<string, string> descriptionMap;
      descriptionMap = new System.Collections.Generic.Dictionary<string, string>{
        {"Extra endurance","Improved health, take less damage."},
        {"+50 max health", "Take more damage"},
        {"Extra agility", "Run faster"},
        {"Second spear", "Two spears are better than one."}

      };

      return descriptionMap[upgradeName];
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
        return Util.ListOfDupes("goon1", 10);
        break;
        case "res://Scenes/Maps/Pillars.tscn":
        return Util.ListOfDupes("goon1", 10);
        break;
        case "res://Scenes/Maps/Urban.tscn":
        return Util.ListOfDupes("goon1", 10);
        break;
        case "res://Scenes/Maps/Maze.tscn":
        return Util.ListOfDupes("goon1", 10);
        break;
        case "res://Scenes/Maps/Levels.tscn":
        return Util.ListOfDupes("goon1", 10);
        break;
        case "res://Scenes/Maps/Open.tscn":
        return Util.ListOfDupes("goon1", 10);
        break;
    }

    GD.Print("Invalid map name " + mapName);
    return new List<string>();
  }



}
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
      string[] args = new string[6];
      args[0] = "" + id;
      args[2] = "" + child1;
      args[3] = "" + child2;
      args[4] = "" + child3;
      args[5] = "";

      int nodeType = 1;
      
      if(Util.RandInt(0,100) > 80){
        nodeType = 5; // Weighted distribution of rest sites
      }

      args[1] = "" + nodeType;

      if(nodeType == 1){
        args[5] = RandomArenaMap();
      }

      return CareerNode.FromRow(args);
    }

    public static string RandomArenaMap(){
      List<string> arenaMaps = new List<string>{
        "res://Scenes/Maps/Levels.tscn",
        "res://Scenes/Maps/Maze.tscn",
        "res://Scenes/Maps/Open.tscn",
        "res://Scenes/Maps/Pillars.tscn",
        "res://Scenes/Maps/Urban.tscn",
	    "res://Scenes/Maps/Colleseum.tscn",
	    "res://Scenes/Maps/MazeII.tscn",
	    "res://Scenes/Maps/Rural.tscn",
	    "res://Scenes/Maps/Town.tscn"
      };
      int choice = Util.RandInt(0, arenaMaps.Count-1);
      return arenaMaps[choice];
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
      StatsManager stats = GetPlayerStats();
      List<Item.Types> ret = new List<Item.Types>();

      List<StatsManager.Facts> spellSlots = new List<StatsManager.Facts>{
      StatsManager.Facts.SpellSlot1,
      StatsManager.Facts.SpellSlot2,
      StatsManager.Facts.SpellSlot3,
      StatsManager.Facts.SpellSlot4,
      StatsManager.Facts.SpellSlot5,
      StatsManager.Facts.SpellSlot6,
      StatsManager.Facts.SpellSlot7,
      StatsManager.Facts.SpellSlot8,
      StatsManager.Facts.SpellSlot9,
      StatsManager.Facts.SpellSlot10
      };

      int slotsMax = stats.GetStat(StatsManager.Stats.SlotsMax);
      
      for(int i = 0; i < slotsMax; i++){
        string slotContents = stats.GetFact(spellSlots[i]);
        Item.Types spell = Item.GetTypeFromString(slotContents);
        if(spell != Item.Types.None){
          ret.Add(spell);
        }
      }

      return ret;
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
      case "Second spear":
        GD.Print("Awarding spear");
        stats.SetFact(StatsManager.Facts.Slot2, "spear");
        stats.SetFact(StatsManager.Facts.Slot3, "claws");
      break;
      case "Double crossbow":
        stats.SetFact(StatsManager.Facts.Slot1, "doublecrossbow");
      break;
      case "Rapid crossbow":
        stats.SetFact(StatsManager.Facts.Slot1, "rapidcrossbow");
      break;
      case "Fireball II":
        stats.SetFact(StatsManager.Facts.SpellSlot1, "FireballIISpell");
      break;
      case "Fireball III":
        stats.SetFact(StatsManager.Facts.SpellSlot1, "FireballIIISpell");
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

    StatsManager stats = GetPlayerStats();

    if(stats.GetFact(StatsManager.Facts.SpellSlot1) == "FireballSpell"){
      ret.Add("Fireball II");
    }
    if(stats.GetFact(StatsManager.Facts.SpellSlot1) == "FireballIISpell"){
      ret.Add("Fireball III");
    }

    return ret;
  }

  public static List<string> GetSoldierUpgrades(){
    List<string> ret = new List<string>();
    StatsManager stats = GetPlayerStats();

    if(stats.GetFact(StatsManager.Facts.Slot1) == "Crossbow"){
      ret.Add("Double crossbow");
      ret.Add("Rapid crossbow");
    }


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
      {"Second spear", "Two spears are better than one."},
      {"Double crossbow", "Fire twice before reloading"},
      {"Rapid crossbow", "Reload rapidly."},
      {"Fireball II", "Stronger projectile attack."},
      {"Fireball III", "Strongest projectile attack."}
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
    return Util.ListOfDupes("goon1", 10);
  }

}

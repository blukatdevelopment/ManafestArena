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

      ret.Add(CareerNode.FromRow(new string[] {"1", "1", "2", "-1", "3"}));
      ret.Add(CareerNode.FromRow(new string[] {"2", "1", "4", "-1", "-1"}));
      ret.Add(CareerNode.FromRow(new string[] {"3", "1", "5", "-1", "-1"}));
      
      ret.Add(CareerNode.FromRow(new string[] {"4", "1", "6", "-1", "-1"}));
      ret.Add(CareerNode.FromRow(new string[] {"5", "1", "6", "-1", "-1"}));
      ret.Add(CareerNode.FromRow(new string[] {"6", "1", "-1", "-1", "-1"}));

      return ret;
    }

    public static void StartNewCareer(StatsManager.Archetypes archetype = StatsManager.Archetypes.None){
        Career career = Factory(archetype);
        Session.session.career = career;
        Session.ChangeMenu(Menu.Menus.Career);
        CareerDb.SaveCareer(career);
    }

}
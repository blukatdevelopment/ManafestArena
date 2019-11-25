/*
    Manages player's progress through a game.
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

  public string ToString(){
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
    encounterInProgress = false;
    Session.ChangeMenu("CareerMenu");
    Session.RemoveGamemode("arena");
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
    Session.ChangeMenu("CreditsMenu");
    Session.ClearGame();
  }

  public void FailEncounter(){
    GD.Print("FailEncounter");
    Session.ChangeMenu("CreditsMenu");
    Session.ClearGame();
  }

  public static IEncounter RandomEncounter(){
    List<IEncounter> encounterTypes = new List<IEncounter>{
      new ArenaMatchEncounter() as IEncounter//,
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
    return Session.GetGamemode("career") as Career;
  }

}

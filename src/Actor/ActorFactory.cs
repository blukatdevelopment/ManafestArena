public class ActorFactory {
  public enum InputSources {
    None,
    Player1, // Keyboard input
    Remote,
    AI
  };

  public enum StatsHandlers {
    None,
    Icepaws
  };

  public enum InventoryHandlers {
    None,
    Simple
  };

  public enum Bodies {
    None,
    PillBody
  };


  public enum Characters {
    None,
    Debug
  };

  public static Actor FromComponentTypes(
    InputSources inputSource,
    StatsHandlers statsHandler,
    Bodies body,
    InventoryHandlers inventoryHandler
  ){
    Actor actor = new Actor();
    InitInputSource(inputSource, actor);
    InitStats(statHandler, actor);
    InitBody(body, actor);
    InitInventory(inventoryHandler, actor);
  }

  public static void InitInputSourceHandler(InputSources inputSource, Actor actor){
    switch(inputSource){
      case InputSources.Player1:
        // Set up devicestate[0]
      break;
      case InputSources.Remote:
        // Set up net source
      break;
      case InputSources.AI:
        // Set up AI
      break;
    }
  }

  public static void InitStats(StatsHandlers statsHandler, Actor actor){
    switch(statsHandler){
      case StatsHandlers.Icepaws:
        // Set up an ICEPAWS based IStats
      break;
    }
  }

  public static void InitBody(Bodies body, Actor actor){
    switch(){
      case Bodies.PillBody:
        // Init pill body
      break;
    }
  }

  public static void InitInventory(InventoryHandlers inventoryHandler, Actor actor){
    switch(inventoryHandler){
      case InventoryHandlers.Simple:
        // Set up a simple inventory
      break;
    }
  }

  public static void InitHotbar {
    // TODO
  }

  public static void InitPaperDoll {
    // TODO
  }

  public static Actor FromCharacter(Characters character){
    switch(character){
      case Characters.Debug:
        return DebugCharacter();
      break;
    }
    return null;
  }

  public static Actor DebugCharacter(){
    Actor actor = FromComponentTypes(InputSources.Player1, StatsHandlers.None, Bodies.PillBody, InventoryHandlers.None);
  }
}
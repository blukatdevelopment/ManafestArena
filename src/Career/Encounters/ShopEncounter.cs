using Godot;
using System;
using System.Collections.Generic;

public class ShopEncounter : IEncounter {
  public string GetDisplayName(){
    return "shop";
  }


  public void StartEncounter(){
    Session.ChangeMenu(Menu.Menus.Shop);
    // TODO Add items to shop menu here
  }
  
  public IEncounter GetRandomEncounter(){
    return null;
  }

  private List<ItemData> ShopItems(){
    List<string> names = RandomShopItemNames();
    List<ItemData> ret = new List<ItemData>();
    
    foreach(string name in names){
      //ret.Add(ItemData.Factory(name));
    }
    
    return ret;
  }

  private List<string> RandomShopItemNames(){
    // TODO: do some random stuff here
    return new List<string>{
      "sword",
      "magic_rifle",
      "magic_beans",
      "old_fish",
      "magic_talisman",
      "nutriloaf",
      "fire_tome",
      "bow_and_arrows",
      "a very large boot"
    };
  }
}
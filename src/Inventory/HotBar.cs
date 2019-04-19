/*
    An array of items with some convenience methods attached.
*/

using System.Collections.Generic;
using Godot;

public class HotBar : IHasItem {
  public Item[] items;
  public int equippedSlot;


  public HotBar(int slots){
    items = new Item[slots];
    equippedSlot = 0;
  }

  public HotBar(StatsManager stats){
    if(stats == null){
      return;
    }
    List<StatsManager.Facts> slots = new List<StatsManager.Facts>{
      StatsManager.Facts.Slot1,
      StatsManager.Facts.Slot2,
      StatsManager.Facts.Slot3,
      StatsManager.Facts.Slot4,
      StatsManager.Facts.Slot5,
      StatsManager.Facts.Slot6,
      StatsManager.Facts.Slot7,
      StatsManager.Facts.Slot8,
      StatsManager.Facts.Slot9,
      StatsManager.Facts.Slot10
    };

    int slotsMax = stats.GetStat(StatsManager.Stats.SlotsMax);
    items = new Item[slotsMax];
    for(int i = 0; i < slotsMax; i++){
      string slotContents = stats.GetFact(slots[i]);
      Item item = ItemFactory.Factory(slotContents, "") as Item; 
      if(item != null){
        items[i] = item;
      }
    }
    equippedSlot = 0;

    GD.Print(this.ToString());
  }

  public void SetItemSlot(int slot, Item item){
    if(slot < 0 || slot >= items.Length){
      GD.Print("HotBar.SetItemSlot: Slot " + slot + " is invalid.");
      return;
    }

    items[slot] = item;
  }

  public int FirstEmptySlot(){
    for(int i = 0; i < items.Length; i++){
      if(items[i] == null){
        return i;
      }
    }

    return -1;
  }

  public int EquippedSlot(){
    return equippedSlot;
  }

  public Item EquippedItem(){
    return items[equippedSlot];
  }

  public void DropEquippedItem(){
    items[equippedSlot] = null;
  }

  public void SetEquippedSlot(int slot){
    if(slot < 0 || slot >= items.Length){
      GD.Print("HotBar.EquipItem: Out of range");
      return;
    }

    equippedSlot = slot;
  }

  public Item EquipItem(int slot){
    if(slot < 0 || slot >= items.Length){
      GD.Print("HotBar.EquipItem: Out of range");
      return null;
    }

    if(items[slot] == null){
      GD.Print("Can't equip an empty slot");
      return null;
    }

    equippedSlot = slot;
    return items[slot];
  }

  public Item EquipNextItem(){
    equippedSlot++;
    
    if(equippedSlot >= items.Length){
      equippedSlot = 0;
    }

    if(ActiveSlotsCount() > 0 && items[equippedSlot] == null){
      equippedSlot = GetNextActiveSlot();
    }
    
    return items[equippedSlot];
  }

  public int GetNextActiveSlot(){
    // Check after equppedSlot
    for(int i = equippedSlot + 1; i < items.Length; i++){
      if(items[i] != null){
        return i;
      }
    }

    //Check before equippedSlot
    for(int i = 0; i < equippedSlot; i++){
      if(items[i] != null){
        return i;
      }
    }

    return -1;
  }

  public Item EquipPreviousItem(){
    equippedSlot--;
    
    if(equippedSlot < 0){
      equippedSlot = items.Length -1;
    }

    return items[equippedSlot];
  }


  public int GetSlotsCount(){
    return items.Length;
  }

  public int ActiveSlotsCount(){
    int ret = 0;

    for(int i = 0; i < items.Length; i++){
      if(items[i] != null){
        ret++;
      }
    }

    return ret;
  }

  public string ToString(){
    string ret = "HotBar: [" + items.Length + "]\n";
    for(int i = 0; i < items.Length; i++){
      if(items[i] == null){
        ret += "\tNULL\n";
      }
      else{
        ret += "\t" + items[i].ToString() + "\n";
      }
    }
    return ret;
  }


  public bool HasItem(string itemName){ return false; }
  public string ItemInfo(){ return ""; }
  public bool ReceiveItem(Item item){ return true; }
  public Item PrimaryItem(){ return items[equippedSlot]; }
  public int ItemCount(){ return GetSlotsCount(); }
  public List<ItemData> GetAllItems(){ return null; }
  
  public List<Item> GetEveryItem(){
    List<Item> ret = new List<Item>();

    for(int i = 0; i < items.Length; i++){
      if(items[i] != null){
        ret.Add(items[i]);
      }
    }

    return ret;
  }

}
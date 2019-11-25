/*
    This provides functionality for cycling through a list
    of N item slots. If it helps, you can think of them 
    as holsters and pockets, highly-accessible storage.

    A hotbar assumes an actor has a non-null body with a non-null node from IBody.GetNode(). 
    If you want to violate this assumption, you'll need to rewrite stuff.
*/

using System.Collections.Generic;
using Godot;

public class HotBar : IHasInfo {
  public Actor actor;
  public IItem[] itemSlots;
  public int equippedSlot;

  public HotBar(int slots, Actor actor){
    itemSlots = new IItem[slots];
    equippedSlot = 0;
    this.actor = actor;
  }

public IItem[] GetItemSlots(){
  return itemSlots;
}
  public List<int> GetEmptySlots(){
    List<int> ret = new List<int>();

    for(int i = 0; i < itemSlots.Length; i++){
      if(itemSlots[i] == null){
        ret.Add(i);
      }
    }
    return ret;
  }

  public IItem GetActiveSlot(){
    return itemSlots[equippedSlot];
  }

  public string GetInfo(){
    string ret = "";
    IHasInfo itemInfo = itemSlots[equippedSlot] as IHasInfo;
    if(itemInfo == null){
      ret = "";
    }
    else{
      ret = itemInfo.GetInfo();
    }
    return ret;
  }
  
  public string GetMoreInfo(){
    string ret = "";

    return ret;
  }

  public bool ValidSlot(int i){
    if(i < 0 || i >= itemSlots.Length){
      return false;
    }
    return true;
  }

  public IItem GetSlot(int i){
    return ValidSlot(i) ? itemSlots[i] : null;
  }

  /* How many times do you need to hit NextItem to get from the
     the start to finish.
  */
  public int PressesToFinishSlot(int start, int finish){
    if(start < finish){
      return finish - start;
    }
    if(start > finish){
      int ret = itemSlots.Length;
      ret -= start;
      ret += finish;
      return ret;
    }
    return 0;

  }

  public int GetEquippedSlot(){
    return equippedSlot;
  }

  public int GetSlotByItem(IItem item){
    for(int i = 0; i < itemSlots.Length; i++){
      if(itemSlots[i] == item){
        return i;
      }
    }
    return -1;
  }

  public List<IItem> Getitems(){
    List<IItem> ret = new List<IItem>();
    for(int i = 0; i < itemSlots.Length; i++){
      if(itemSlots[i] != null){
        ret.Add(itemSlots[i]);
      }
    }
    return ret;
  }

  public IItem RemoveItem(int i){
    IItem ret = GetSlot(i);
    if(ret != null){
      itemSlots[i] = null;
      ret.Unequip();
    }
    return ret;
  }

  public void AddItem(int i, IItem item){
    if(!ValidSlot(i)){
      GD.Print(i + " is not a valid slot. Aborting");
      return;
    }

    itemSlots[i] = item;

    if(i == equippedSlot){
      item.Equip(actor.body);
    }
  }

  public void AddItemToNextSlot(IItem item){
    if(GetEmptySlots().Count == 0){
      GD.Print("No slots to add to");
      return;
    }
    bool added = false;
    int slot = NextSlot();
    while(!added){
      if(itemSlots[slot] == null){
        AddItem(slot, item);
        added = true;
      }
    }
  }

  public void EquipSlot(int i){
    if(!ValidSlot(i)){
      GD.Print(i + " is an invalid hotbar slot");
      return;
    }
    if(itemSlots[equippedSlot] != null){
      itemSlots[equippedSlot].Unequip();
    }
  }

  public void UnequipActive(){
    if(itemSlots[equippedSlot] == null){
      return;
    }
    itemSlots[equippedSlot].Unequip();
  }

  public void EquipActive(){
    if(itemSlots[equippedSlot] == null){
      GD.Print("Active slot is null. Not equipping.");
      return;
    }
    itemSlots[equippedSlot].Equip(actor.body);
  }

  public void EquipNext(){
    UnequipActive();
    equippedSlot = NextNonEmptySlot(equippedSlot);
    EquipActive();
  }

  public void EquipPrevious(){
    UnequipActive();
    equippedSlot = PrevNonEmptySlot(equippedSlot);
    EquipActive();
  }

  private int NextSlot(int i = -1){
    if(i == -1){
      i = equippedSlot;
    }
    int ret = i + 1;
    if(!ValidSlot(ret)){
      ret = 0;
    }
    return ret;
  }

  private int NextNonEmptySlot(int i = -1){

    if(i == -1){
      i = equippedSlot;
    }
    int ret = NextSlot(i);
    while(itemSlots[ret]==null){
      ret = NextSlot(ret);
      if(ret==i){
        return i;
      }
    }
    return ret;
  }

  private int PrevSlot(int i = -1){
    if(i == -1){
      i = equippedSlot;
    }
    int ret = i -1;
    if(!ValidSlot(ret)){
      ret = itemSlots.Length -1;
    }
    return ret;
  }

  private int PrevNonEmptySlot(int i = -1){

    if(i == -1){
      i = equippedSlot;
    }
    int ret = PrevSlot(i);
    while(itemSlots[ret]==null){
      ret = PrevSlot(ret);
      if(ret==i){
        return i;
      }
    }
    return ret;
  }

  public void UseEquippedItem(MappedInputEvent inputEvent){
    if(itemSlots[equippedSlot] == null){
      return;
    }
    itemSlots[equippedSlot].Use(inputEvent);
  }

}
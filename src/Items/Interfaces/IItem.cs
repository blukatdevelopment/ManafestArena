using Godot;
using System;
using System.Collections.Generic;

public interface IItem {
    
    void Use(Item.ItemInputs input);
    void Equip(object wielder);
    void Unequip();
    
    // Simple Getters
    object GetWielder();
    int GetId();
    Node GetNode();

    // Setters
    void SetCollision(bool val);

    // For serialization
    ItemFactory.Items GetItemEnum();
    void LoadJson(string json);
    string GetJson();

    // provide a delegate factory
    List<ItemFactory.Items> GetSupportedItems();
    IItem Factory(ItemFactory.Items item);
}
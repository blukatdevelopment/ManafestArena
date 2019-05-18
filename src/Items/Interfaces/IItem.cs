using Godot;
using System;
using System.Collections.Generic;

public interface IItem {
    
    void Use(MappedInputEvent inputEvent);
    void Equip(object wielder);
    void Unequip();
    void Update(float delta);
    
    // Simple Getters
    object GetWielder();
    int GetId();
    Node GetNode();

    // Setters
    void SetCollision(bool val);
    void SetId(int id);

    // For serialization
    ItemFactory.Items GetItemEnum();
    void LoadJson(string json);
    string GetJson();

    // provide a delegate factory
    List<ItemFactory.Items> GetSupportedItems();
    IItem Factory(ItemFactory.Items item);
}
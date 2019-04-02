/*
    Fundamentally the spellcaster is a hotbar of individual spells that can be cycled through.
    Ideally, it would either have a single spell (like a spell ring), or it would be a staff with a selection of spells.
    To cast spells, mana is consumed from the wielder.
*/

using Godot;
using System;
using System.Collections.Generic;

public class SpellCaster : Item, IHasStats {
    HotBar hotbar; 

    public SpellCaster(){
        hotbar = new HotBar(0);
    }

    public SpellCaster(List<Item.Types> spells){
        LoadSpells(spells);
    }

    public override void Use(Item.Uses use, bool released = false){
    switch(use){
      case Uses.A: CastSpell(); break;
      case Uses.B: NextSpell(); break;
    }
  }

  public void CastSpell(){
    Item spell = hotbar.EquippedItem();

    if(spell == null){
        GD.Print("SpellCaster.CastSpell: Can't cast a null spell in slot " + hotbar.EquippedSlot());
        return;
    }

    spell.Use(Uses.A);
  }

  public void NextSpell(){
    GD.Print("NextSpell");
    hotbar.EquipNextItem();
  }

    public void LoadSpells(List<Item.Types> spells){
        List<Item> spellItems = new List<Item>();
        foreach(Item.Types spell in spells){
            Item spellItem = Item.Factory(spell);
            if(spellItem == null){
                GD.Print("SpellCaster.LoadSpells: Could not create spell " + spell);
            }
            else{
                spellItems.Add(spellItem);
            }
        }

        hotbar = new HotBar(spellItems.Count);

        for(int i = 0; i < spellItems.Count; i++){
            hotbar.SetItemSlot(i, spellItems[i]);
            AddChild(spellItems[i]);
            spellItems[i].Equip(this);
            spellItems[i].Mode = RigidBody.ModeEnum.Static;
        }
    }

    public override void Equip(object wielder){
        ItemBaseEquip(wielder);
        
        foreach(Item spellItem in hotbar.GetEveryItem()){
            spellItem.wielder = wielder;
        }
    }

    public string ToString(){
        string ret = "SpellCaster: " + hotbar.ActiveSlotsCount() + " spells loaded.\n";
        ret += hotbar.ToString();

        return ret;
    }

    public override string GetInfo(){
        Item spell = hotbar.EquippedItem();
        if(spell == null){
            return "Magic Staff: No spell";
        }
        return  name + ":\n" + spell.name + "\n(" + spell.manaCost + " mana)\n" + spell.description;
        
    }

    public static void SpellCasterTests(){
        SpellCaster caster = new SpellCaster();

        GD.Print("Blank caster looks like this" + caster.ToString());

        caster = new SpellCaster( new List<Item.Types>{ Item.Types.Crossbow, Item.Types.Spear, Item.Types.Knife });

        GD.Print("Caster with three spells looks like this." + caster.ToString());
    }
}
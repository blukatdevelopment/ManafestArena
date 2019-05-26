/*
    Each class implementing IItem is responsible for a certain category of items.
    The verbage here is Archetype

    What this ItemFactory provides is the player's notion of an item, To the player a
    crossbow and double crossbow are two very-different items, even though they share a
    class and differ by only a single setting. ItemFactory.Items connects those identities
    with the appropriate class's factory. 

    In other words, this factory maps an enum to a particular config for a particular item class.
*/
using System;
using System.Collections.Generic;
using Godot;

public class ItemFactory {

    public enum Items {
        None,
        // MeleeStabItem
        Knife,
        Claws,
        // ProjectileItem
        MusketBall,
        CrossbowBolt,
        // ProjectileLauncher
        Crossbow
    };

    public static IItem Factory(Items item){
        foreach(IItem factory in GetDelegateFactories()){
            List<ItemFactory.Items> supported = factory.GetSupportedItems();
            if(supported.IndexOf(item) != -1){
                return factory.Factory(item);
            }
        }

        return null;
    }

    public static IItem Factory(string name, string json){
        return null;
    }

    public static IItem Factory(Items item, string json = ""){
        IItem ret = Factory(item);
        if(ret != null && json != ""){
            ret.LoadJson(json);
        }
        return ret;
    }

    /* Add an instance of each class that implements IItem to this list */
    public static List<IItem> GetDelegateFactories(){
        List<IItem> ret = new List<IItem>();
        ret.Add(new Item() as IItem);
        ret.Add(new MeleeStabItem() as IItem);
        ret.Add(new RangedProjectileItem() as IItem);
        ret.Add(new ProjectileItem() as IItem);
        return ret;
    }

}
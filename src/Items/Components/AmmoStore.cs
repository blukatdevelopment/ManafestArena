/*
    An ammo store exists as loadedAmmo that overflows into reserveAmmo.
    Reloading replenishes loadedAmmo with reserveAmmo.
*/
using Godot;
using System;
using System.Collections.Generic;

public class AmmoStore {
    IItem item;
    Inventory loadedAmmo, reserveAmmo;

    public int ammoCapacity;

    public AmmoStore(IItem item){
        this.item   = item;
        loadedAmmo  = new Inventory();
        reserveAmmo = new Inventory();
    }

    public void Config(
        int ammoCapacity,
        List<ItemData> ammo
    ){
        this.ammoCapacity = ammoCapacity;
    }

    // Trickle fill loadedAmmo and trickle to reserveAmmo
    // Don't feed it stacked items, and never after midnight
    public void LoadAmmo(List<ItemData> ammo){
        loadedAmmo.StoreItemDataRange(ammo);

        while(loadedAmmo.ItemCount() > ammoCapacity){
            reserveAmmo.StoreItemData(loadedAmmo.RetrieveItem(0));
        }
    }

    public void Reload(){
        int ammoNeeded = ammoCapacity - loadedAmmo.Count;

        for(int i = 0; i < ammoNeeded; i++){
            if(reserveAmmo.ItemCount() < 1){
                return;
            }
            else{
                loadedAmmo.StoreItemData(reserveAmmo.RetrieveItem(0));
            }
        }
    }

    public int LoadedAmmoCount(){
        return loadedAmmo.ItemCount();
    }

    public int ReserveAmmoCount(){
        return reserveAmmo.ItemCount();
    }

    public bool CanExpendAmmo(){
        return loadedAmmo.ItemCount() > 0;
    }

    // Attempts to expend loaded ammo, returns true if successful
    public bool ExpendAmmo(){
        if(loadedAmmo.ItemCount() < 1){
            return false;
        }

        loadedAmmo.RetrieveItem(0);
        return true;
    }

}
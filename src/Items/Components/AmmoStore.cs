/*
    An ammo store exists as loadedAmmo that overflows into reserveAmmo.
    Reloading replenishes loadedAmmo with reserveAmmo.
*/
using Godot;
using System;
using System.Collections.Generic;

public class AmmoStore {
    IItem item;
    List<ItemData> loadedAmmo, reserveAmmo;

    public int ammoCapacity;

    public AmmoStore(IItem item){
        this.item   = item;
        loadedAmmo  = new List<ItemData>();
        reserveAmmo = new List<ItemData>();
    }

    public void Config(
        int ammoCapacity,
        List<ItemData> ammo
    ){
        this.ammoCapacity = ammoCapacity;
        LoadAmmo(ammo);
    }

    // Trickle fill loadedAmmo and trickle to reserveAmmo
    // Don't feed it stacked items, and never after midnight
    public void LoadAmmo(List<ItemData> ammo){
        loadedAmmo.AddRange(ammo);

        while(loadedAmmo.Count > ammoCapacity){
            reserveAmmo.Add(loadedAmmo[0]);
            loadedAmmo.RemoveAt(0);
        }
    }

    public void Reload(){
        int ammoNeeded = ammoCapacity - loadedAmmo.Count;

        for(int i = 0; i < ammoNeeded; i++){
            if(reserveAmmo.Count < 1){
                return;
            }
            else{
                loadedAmmo.Add(reserveAmmo[0]);
                reserveAmmo.RemoveAt(0);
            }
        }
    }

    public int AmmoCapacity(){
        return ammoCapacity;
    }

    public int LoadedAmmoCount(){
        return loadedAmmo.Count;
    }

    public int ReserveAmmoCount(){
        return reserveAmmo.Count;
    }

    public bool CanExpendAmmo(){
        return loadedAmmo.Count > 0;
    }

    // Attempts to expend loaded ammo, returns true if successful
    public bool ExpendAmmo(){
        if(loadedAmmo.Count < 1){
            return false;
        }

        loadedAmmo.RemoveAt(0);
        return true;
    }

}
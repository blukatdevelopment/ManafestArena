/*
  A ranged weapon that fires a projectile and consumes ammo from an ammostore.
*/
using Godot;
using System;
using System.Collections.Generic;

public class RangedProjectileItem : Item, IWeapon, IRangedWeapon {
  ProjectileLauncher launcher;
  AmmoStore ammoStorage;
  bool reloadActive, fireActive;
  float reloadTimer, reloadDelay, fireTimer, fireDelay;

  public RangedProjectileItem(){}

  public RangedProjectileItem(
    string name, 
    string description,
    string meshPath,
    ItemFactory.Items projectileType, 
    Damage damage, 
    float launchImpulse,
    int ammoCapacity,
    float reloadDelay,
    float fireDelay
  ){
    this.name = name;
    Name = name;
    this.description = description;
    this.meshPath = meshPath;
    this.reloadDelay = reloadDelay;
    InitNodeStructure();

    launcher = new ProjectileLauncher(this);
    launcher.Config(projectileType, damage, launchImpulse, speaker);
    
    ammoStorage = new AmmoStore(this);
    ammoStorage.Config(ammoCapacity, new List<ItemData>());
  }

  private List<ItemData> CreateAmmo(int count, ItemFactory.Items projectileType){
    List<ItemData> ammo = new List<ItemData>();

    ItemData dat = new ItemData(projectileType);
    for(int i = 0; i < count; i++){
      ammo.Add(dat);
    }

    return ammo;
  }

  public override void Update(float delta){
    if(fireActive){
      fireTimer -= delta;
      if(fireTimer <= 0f){
        fireActive = false;
      }
    }
    if(reloadActive){
      reloadTimer -= delta;
      if(reloadTimer <= 0f){
        EndReload();
      }
    }
  }

  public override string GetInfo(){
    string ret = name + ":\n";
    ret += "[" + ammoStorage.LoadedAmmoCount() + "/" + ammoStorage.AmmoCapacity();
    ret += "/" + ammoStorage.ReserveAmmoCount() + "]";
    return ret;
  }

  public override void Use(MappedInputEvent inputEvent){
    if(inputEvent.inputType != MappedInputEvent.Inputs.Press || reloadActive || fireActive){
      return;
    }
    Item.ItemInputs input = (Item.ItemInputs)inputEvent.mappedEventId;
    switch(input){
      case Item.ItemInputs.A:
        if(ammoStorage.ExpendAmmo()){
          launcher.Fire();
          fireTimer = fireDelay;
          fireActive = true;
          if(ammoStorage.LoadedAmmoCount() == 0 && ammoStorage.ReserveAmmoCount() > 0){
            StartReload();
          }
        }
        else if(ammoStorage.ReserveAmmoCount() != 0){
          StartReload();
        }
      break;
      case Item.ItemInputs.C:
        StartReload();
      break;
    }
  }

  private void StartReload(){
    reloadActive = true;
    reloadTimer = reloadDelay;
  }

  private void EndReload(){
    reloadActive = false;
    ammoStorage.Reload();
  }

  public void LoadAmmo(List<ItemData> ammo){
    ammoStorage.LoadAmmo(ammo);
  }

  public override List<ItemFactory.Items> GetSupportedItems(){
    return new List<ItemFactory.Items>(){ ItemFactory.Items.Crossbow };
  }

  public Damage GetDamage(){
    return launcher.damage;
  }

  public float AttackDelay(){
    return fireDelay;
  }

  public float GetEffectiveRange(){
    // FIXME: Calculate bullet drop from height here
    float effectiveRange = 20f;
    return effectiveRange;
  }

  public int GetAmmo(){
    return ammoStorage.LoadedAmmoCount() + ammoStorage.ReserveAmmoCount(); 
  }

  public override IItem Factory(ItemFactory.Items item){
    RangedProjectileItem ret = null;
    Damage damage = new Damage();
    switch(item){
      case ItemFactory.Items.Crossbow:
      damage.health = 100;
      ret = new RangedProjectileItem(
        "Crossbow",
        "Fires a metal bolt at high speeds.",
        "res://Assets/Models/crossbow.obj",
        ItemFactory.Items.CrossbowBolt,
        damage,
        50f,
        1,
        1f,
        0.5f
      );
      ret.LoadAmmo(CreateAmmo(12, ItemFactory.Items.CrossbowBolt));
      break;
    }
    return ret as IItem;
  }
}
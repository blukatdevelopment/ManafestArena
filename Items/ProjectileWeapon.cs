/*
  Instantiates and fires projectiles, launching them forward. 
  Limited by a store of ammo wich must be reloaded with delay.
*/
using Godot;
using System.Collections.Generic;
using System;

public class ProjectileWeapon : Item, IWeapon, IHasAmmo, IEquip {
  
  const int BaseDamage = 10;
  const float ProjectileOffset = 0.1f;
  const float ImpulseStrength = 50f;
  const float DefaultReloadDelay = 2f;
  public int healthDamage;
  public string ammoType = "Bullet";
  public Inventory inventory;
  public Inventory reserve;
  public int maxAmmo = 10;
  public float busyDelay = 0f;
  public bool busy = false;
  public delegate void OnBusyEnd();
  public OnBusyEnd busyEndHandler;
  public float reloadDelay;
  
  public ProjectileWeapon(){
    inventory = new Inventory();
    reserve = new Inventory();
    healthDamage = BaseDamage;
    reloadDelay = DefaultReloadDelay;
  }

  // Load weapon without associating it with an external inventory.
  public void LoadInternalReserve(ItemData projectile, int projectileCount){
    for(int i = 0; i < projectileCount; i++){
      if(i < maxAmmo){
        inventory.StoreItemData(ItemData.Clone(projectile));
      }
      else{
        reserve.StoreItemData(ItemData.Clone(projectile));
      }
    }
  }
  
  public override void _Process(float delta){
    if(busy){
      busyDelay -= delta;
      
      if(busyDelay <= 0f){
        busy = false;
        busyDelay = 0f;
        busyEndHandler();
      }
    }  
  }
  
  public override ItemData GetData(){
    ItemData ret = ItemGetData();
    
    ret.description += "\nDamage: " + healthDamage + "\n";
    ret.description += "Capacity: " + maxAmmo + "\n";
    ret.description += "Ammo Type: " + ammoType + "\n";
    ret.description += "Range: " + ImpulseStrength;
    
    return ret;
  }

  public override string GetInfo(){
    string ret = name + "[" + inventory.ItemCount() + "/" + maxAmmo;
    int ammoTotal = CheckReserveAmmo(ammoType, 0);

    if(wielder != null){
      IHasAmmo ammoHolder = wielder as IHasAmmo;
      if(ammoHolder != null){
        ammoTotal += ammoHolder.CheckAmmo(ammoType, 0);
      }
    }
    ret += "/" + ammoTotal;
    
    ret += "]";
    return ret; 
  }
  
  public Damage GetBaseDamage(){
    return new Damage(healthDamage);
  }
  
  public int CheckAmmo(string ammoType, int max = 0){
    int quantity = inventory.GetQuantity(Item.Types.Ammo, ammoType); 
    if(max > 0 && quantity > max){
      return max;
    }
    return quantity;
  }
  
  public List<ItemData> RequestAmmo(string ammoType, int max = 0){
    return inventory.RetrieveItems(Item.Types.Ammo, ammoType, max);
  }
  
  public List<ItemData> StoreAmmo(List<ItemData> ammo){
    if(busy){
      return ammo;
    }
    
    List<ItemData> ret = new List<ItemData>();
    
    foreach(ItemData data in ammo){
      if(inventory.ItemCount() < maxAmmo && data.type == Item.Types.Ammo && data.name == ammoType){
        inventory.StoreItemData(data);
      }
      else{
        ret.Add(data);
      }
    }
    
    return ret;
  }
  
  public string[] AmmoTypes(){
    return new string[]{ ammoType };
  }
  
  void StartReload(){

    if(!ReloadNeeded()){
      return;
    }
    speaker.PlayEffect(Sound.Effects.RifleReload);
    busy = true;
    busyDelay = reloadDelay;

    OnBusyEnd loadAmmo = () => { 
      Reload();
    };

    busyEndHandler = loadAmmo;
  }
  
  public override bool IsBusy(){
    return busy;
  }
  
  public override void Use(Item.Uses use, bool released = false){
    if(busy){
      return;
    }

    switch(use){
      case Uses.A: Fire(); break;
      case Uses.B: break; // Aim logic goes here.
      case Uses.D: StartReload(); break;
    }
  }
  
  protected virtual void Fire(){
    if(inventory.ItemCount() < 1 || (Session.NetActive() && !Session.IsServer() )){
      return;
    }
    
    string name = Session.NextItemName();

    if(Session.IsServer()){
      DeferredFire(name);
      Rpc(nameof(DeferredFire), name);
    }
    else{
      DeferredFire(name);
    }
  }

  public bool ExpendAmmo(){
    if(inventory.ItemCount() > 0){
      inventory.RetrieveItem(0, 1);
      return true;
    }
    
    return false;
  } 

  [Remote]
  public void DeferredFire(string name){
    if(!ExpendAmmo()){
      return;
    }
    
    speaker.PlayEffect(Sound.Effects.RifleShot);
    Item projectile = Item.Factory(Item.Types.Bullet);
    projectile.Name = name;
    Projectile proj = projectile as Projectile;

    if(proj != null){
      proj.healthDamage = healthDamage;
    }

    Actor wielderActor = wielder as Actor;
    
    if(wielderActor != null){
      proj.sender = wielderActor.NodePath();
    }
    
    Vector3 projectilePosition = ProjectilePosition();
    Vector3 globalPosition = this.ToGlobal(projectilePosition);
    Spatial gameNode = (Spatial)Session.GameNode();
    
    Vector3 gamePosition = gameNode.ToLocal(globalPosition);
    projectile.Translation = gamePosition;
    gameNode.AddChild(projectile);

    Transform start = this.GetGlobalTransform();
    Transform destination = start;
    destination.Translated(new Vector3(0, 0, 1));
    
    Vector3 impulse = start.origin - destination.origin;
    projectile.SetAxisVelocity(impulse * ImpulseStrength);
  }

  public override void Equip(object wielder){
    ItemBaseEquip(wielder);
    IHasAmmo ammoHolder = wielder as IHasAmmo;
    
    if(ammoHolder != null){
      List<ItemData> newAmmo = ammoHolder.RequestAmmo(ammoType, maxAmmo);
      
      foreach(ItemData ammo in newAmmo){
        inventory.StoreItemData(ammo);
      }
    }
    
  }

  public override void Unequip(){
    IHasAmmo ammoHolder = wielder as IHasAmmo;
    
    busyDelay = -1f;
    busy = false;

    if(ammoHolder != null){
      ammoHolder.StoreAmmo(inventory.RetrieveAllItems());
    }

    ItemBaseUnequip();
  }
  
  private void Reload(){
    
    List<ItemData> receivedAmmo = new List<ItemData>();

    if(!ReloadNeeded()){
      GD.Print("Reload not needed");
      return;
    }
    
    IHasAmmo ammoHolder = wielder as IHasAmmo;
    if(ammoHolder != null){
      receivedAmmo = ammoHolder.RequestAmmo(ammoType, ReloadAmmoNeeded());
    }
    
    if(receivedAmmo.Count == 0){
      receivedAmmo = RequestReserveAmmo(ReloadAmmoNeeded());
    }

    if(receivedAmmo.Count == 0){
      GD.Print("No reload possible");
      return;
    }

    foreach(ItemData ammo in receivedAmmo){
      inventory.StoreItemData(ammo);
    }
  }

  public bool ReloadNeeded(){
    return ReloadAmmoNeeded() > 0;
  }

  public int ReloadAmmoNeeded(){
    return maxAmmo - inventory.ItemCount();
  }
  
  public List<ItemData> RequestReserveAmmo(int needed){
    List<ItemData> ret = new List<ItemData>();

    for(int i = 0; i < needed; i++){
      ItemData dat = reserve.RetrieveItem(0);
      if(dat != null){
        ret.Add(dat);
      }
    }

    return ret;
  }

  public int CheckReserveAmmo(string ammoType, int max = 0){
    int quantity = reserve.GetQuantity(Item.Types.Ammo, ammoType);
    
    if(max > 0 && max > quantity){
      return quantity;
    }
    else if(max <= 0){
      return quantity;
    }
    
    return max;
  }

  private Vector3 ProjectilePosition(){
    Vector3 current = Translation; 
    Vector3 forward = -Transform.basis.z;
    forward *= ProjectileOffset;
    
    return current + forward;
  }
  
}

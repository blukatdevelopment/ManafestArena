/*
  Moves forward, damaging the first thing it hits for it's duration, and then draws back.
*/
using Godot;
using System;

public class MeleeWeapon : Item, IWeapon {
  
  public const int DefaultDamage = 10;
  public const float DefaultSwingSpeed = 0.5f;
  public int healthDamage;
  public Vector3 wieldedPosition;
  public Vector3 forwardPosition;
  public bool swinging = false;
  public float busyDelay = 0f;
  public bool busy = false;
  public delegate void OnBusyEnd();
  public OnBusyEnd busyEndHandler;
  public float swingSpeed;

  
  public override ItemData GetData(){
    ItemData ret = ItemGetData();
    ret.description += "\nDamage: " + healthDamage + "\n";
    return ret;
  }
  
  public MeleeWeapon(){
    healthDamage = DefaultDamage;
    swingSpeed = DefaultSwingSpeed;
  }

  public void Init(){
    
  }
  
  public override void Equip(object wielder){
    ItemBaseEquip(wielder);
    this.wieldedPosition = GetTranslation();
    this.forwardPosition = this.wieldedPosition + new Vector3(0, 0, -1);
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
  
  public Damage GetBaseDamage(){
    return new Damage(healthDamage);
  }
  
  public override bool IsBusy(){
    return busy;
  }
  
  public override void Use(Item.Uses use, bool released = false){
    switch(use){
      case Uses.A: Swing(); break;
    }
  }
  
  [Remote]
  public override void DoOnCollide(object body){
    if(!swinging){
      return;
    }
    
    IReceiveDamage receiver = body as IReceiveDamage;
    IReceiveDamage wielderDamage = wielder as IReceiveDamage;
    
    if(receiver != null && receiver != wielderDamage){
      Strike(receiver);
    }
  }
  
  public void Strike(IReceiveDamage receiver){
    GiveDamage(receiver);
    swinging = false;
  }
  
  public void GiveDamage(IReceiveDamage receiver){
    Damage damage = new Damage(healthDamage);

    Node wielderNode = wielder as Node;

    if(wielderNode != null){
      damage.sender = wielderNode.GetPath();
    }
    receiver.ReceiveDamage(damage);
  }
  
  public void Swing(){
    if(!busy && !swinging && ExpendResources()){
      StartSwing();
    }
  }

  /* Returns true on success */
  public bool ExpendResources(){
    StatsManager stats = GetStats();
    if(manaCost != 0 && !stats.ConsumeStat(StatsManager.Stats.Mana, manaCost)){
      return false;
    }
    if(staminaCost != 0 && !stats.ConsumeStat(StatsManager.Stats.Stamina, staminaCost)){
      return false;
    }
    
    return true;
  }
  
  public void StartSwing(){
    speaker.PlayEffect(Sound.Effects.FistSwing);
    
    busy = true;
    busyDelay = swingSpeed;
    OnBusyEnd endSwing = EndSwing;
    busyEndHandler = endSwing;
    swinging = true;
    Translation = forwardPosition;
    
    SetCollision(true);
  }
   
  public virtual void EndSwing(){
    swinging = false;
    busy = false;
    Translation = wieldedPosition;
    
    SetCollision(false);
  }
}

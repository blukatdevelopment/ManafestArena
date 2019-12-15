/*
  This is an helper script that can be attached to an area in the scene 
  making it an HitBox linked to the animation specified by the given animationName 
*/
using Godot;
using System;
using System.Collections.Generic;

public class Hitbox : Area, IItem, IHasDamage, IWeapon {
  
  public Actor actor;
  private object wielder;
  public int id;
  public ItemMeleeAttacker attacker;
  public StatConsumer consumer;
  public Speaker speaker;
  [Export] public String animationName = "";
  public String name, description;
  public float attackSpeed;
  public bool isActive;

  public override void _Ready(){

    if(animationName == ""){
      Free();
      return;
    }
    isActive = false;
    SetMonitorable(false);
    SetRayPickable(false);

    this.Connect("body_entered", this, nameof(OnCollide));
    speaker = new Speaker();
    AddChild(speaker);
    actor = FindActor();
    if(actor == null){
      return;
    }
    wielder = actor.GetBody();
    actor.hotbar.AddItemToNextSlot(this);
    Config();
    SetCollision(false);
  }

  private Actor FindActor(){

    Spatial parent = GetParentSpatial();
    while(parent != null){
      IBody body = parent as IBody;
      if(body != null){
        return body.GetActor();
      }
      parent = parent.GetParentSpatial();
    }
    return null;
  }

  public void Config(){
    attacker = new ItemMeleeAttacker(this, animationName);
    Damage damage = new Damage(100);
    attacker.Config(
      1,
      damage,
      speaker,
      Sound.Effects.None,
      Sound.Effects.FistImpact
    );
    consumer = new StatConsumer(0, 0, 0);
  }

  public void Update(float delta){
    attacker.Update(delta);
  }

  public void Equip(object wielder){
    attacker.OnUpdateWielder();
  }

  public void Unequip(){
    attacker.OnUpdateWielder();
  }
  
  public void Use(MappedInputEvent inputEvent){
    Item.ItemInputs input = (Item.ItemInputs)inputEvent.mappedEventId;
      if(input == Item.ItemInputs.A && inputEvent.inputType == MappedInputEvent.Inputs.Press){
          Attack();
      }
  }

  public void Attack(){
    if(attacker.CanStartAttack() && consumer.ConsumeStats()){
      attacker.StartAttack();
    }
  }

  public int GetId(){
    return id;
  }

  public void SetId(int id){
    this.id = id;
  }
  public object GetWielder(){
    wielder = actor.GetBody();
    return wielder;
  }

  public Node GetNode(){
    return this;
  }

  public Damage GetDamage(){
    return attacker.damage;
  }

  public void SetCollision(bool val){
    
    isActive = val;
    Godot.Collections.Array shapeOwners = GetShapeOwners();
    for (int i = 0; i < shapeOwners.Count; i++)
    {
      int ownerId = (int) shapeOwners[i];
      ShapeOwnerSetDisabled(ownerId, !val);
    }
    
  }

  public float AttackDelay(){
    return attacker.attackSpeed;
  }

  public void OnCollide(object body){
    if(isActive){
      attacker.OnCollide(body);
    }
  }

  public List<ItemFactory.Items> GetSupportedItems(){
    return new List<ItemFactory.Items>();
  }

  public IItem Factory(ItemFactory.Items item){
    return null;
  }

  public ItemFactory.Items GetItemEnum()
  {
    return ItemFactory.Items.None;
  }

  public void LoadJson(string json){}

  public string GetJson()
  {
    return "";
  }
}

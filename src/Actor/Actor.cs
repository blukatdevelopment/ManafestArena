/*
  TODO: Update this description
  An Actor is a living entity rendered in the 3D world and controlled by a Brain,
  which can either be an AI, or an input handler listening to a device.
*/

using Godot;
using System;
using System.Collections.Generic;

public class Actor : KinematicBody, IReceiveDamage, IHasInfo, ILook, IHasStats {
  
  public enum Brains{
    Player1, // Local player leveraging keyboard input.
    Ai,      // Computer player
    Remote   // Remote player controlled via RPC calls
  };
  
  private Brain brain;
  public Brains brainType;
  private Spatial eyes;
  
  const int maxY = 90;
  const int minY = -40;
  const float GravityAcceleration = -9.81f;
  const float TerminalVelocity = -53;
  
  private bool grounded = false; //True when Actor is standing on surface.
  public bool sprinting = false;
  private float gravityVelocity = 0f;
  
  public bool menuActive = false;
  public bool paused = false;

  // Child nodes
  public Speaker speaker;
  public MeshInstance meshInstance;
  public CollisionShape collisionShape;

  private StatsManager stats;
  
  // Inventory
  private Item activeItem;
  private Item hand; // Weapon for unarmed actors.
  private bool unarmed = true;
  public HotBar hotbar;
  
  // Handpos
  private float HandPosX = 0;
  private float HandPosY = 0;
  private float HandPosZ = -1.5f;

  public int id;
  public string name;

  // These can change when detailed actor models with animations are added in.
  public const string ActorMeshPath = "res://Assets/Models/Actor.obj";

  public Actor(){
    brainType = Brains.Ai;
    InitChildren();
    InitBrain(brainType);
    InitHand();
    id = -1;
    stats = new StatsManager();
  }

  public Actor(Brains b){
    brainType = b;
    InitChildren();
    InitBrain(b);
    InitHand();
    id = -1;
    stats = new StatsManager();
  }

  public void InitBrain(Brains b){
    this.brainType = b;
    switch(b){
      case Brains.Player1:
        brain = (Brain)new ActorInputHandler(this);
        break;
      case Brains.Ai: 
        brain = (Brain)new StateAi(this); 
        break;
      case Brains.Remote:
        brain = null;
        break;
    }
  }

  /* Construct node tree. */
  protected void InitChildren(){
    // MeshInstance
    string meshPath = ActorMeshPath;
    meshInstance = new MeshInstance();
    meshInstance.Mesh = ResourceLoader.Load(meshPath) as Mesh;
    AddChild(meshInstance);

    // CollisionShape
    collisionShape = new CollisionShape();
    AddChild(collisionShape);
    collisionShape.MakeConvexFromBrothers();

    // Eyes
    Spatial eyesInstance;

    if(brainType == Brains.Player1){
      Camera cam = new Camera();
      cam.Far = 1000f; // Render EVERYTHING possible
      eyesInstance = (Spatial)cam;
    }
    else{
      eyesInstance = new Spatial();
    }

    eyesInstance.Name = "Eyes";
    AddChild(eyesInstance);
    Vector3 eyesPos = EyesPos();
    eyesInstance.TranslateObjectLocal(eyesPos);
    eyes = eyesInstance;
    eyes.SetRotationDegrees(new Vector3(0, 0, 0));

    // Speaker
    speaker = new Speaker();
    AddChild(speaker);
  }

  public void SetMaterial(Material material){
    ArrayMesh mesh = meshInstance.Mesh as ArrayMesh;
    mesh.SurfaceSetMaterial(0, material);
  }

  public void LoadStats(StatsManager stats){
    GD.Print("Loading stats " + stats.GetFact(StatsManager.Facts.Name));
    hotbar = new HotBar(stats);
    if(hotbar != null && hotbar.EquipItem(0) != null){
      EquipItem(hotbar.EquipItem(0));
    }
    this.stats = stats;
    Brains brain = (Brains)stats.GetStat(StatsManager.Stats.Brain);
  }
  
  public Item PrimaryItem(){
    return activeItem;
  }

  public void InitHand(){
    if(hand != null){
      GD.Print("Hand already initialized.");
    }
    //hand = Item.Factory(Item.Types.Hand);
    //EquipHand(); 
  }

  public void NameHand(string handName){
    hand.Name = handName;
  }

  public StatsManager GetStats(){
    return stats;
  }

  /* Return global position of eyes(if available) or body */
  public Vector3 GlobalHeadPosition(){
    if(eyes != null){
      return eyes.GlobalTransform.origin;
    }
    return GlobalTransform.origin;
  }

  /* Returns global transform of eyes(if available) or body */
  public Transform GlobalHeadTransform(){
    if(eyes != null){
      return eyes.GlobalTransform;
    }
    return GlobalTransform;
  }

  /* Global space 
    Point at end of ray in looking direction.
  */
  public Vector3 Pointer(float distance = 100f){
    if(IsPaused()){
      return new Vector3();
    }
    Vector3 start = GlobalHeadPosition();
    Transform headTrans = GlobalHeadTransform();
    Vector3 end = Util.TForward(headTrans);
    end *= distance;
    end += start;
    return end;
  }

  public object VisibleObject(){
    if(IsPaused()){
      return null;
    }
    Vector3 start = GlobalHeadPosition();
    Vector3 end = Pointer();
    World world = GetWorld();
    return Util.RayCast(start, end, world);
  }
  
  public string GetInfo(){
    switch(brainType){
      case Brains.Player1:
        return "You.";
        break;
      case Brains.Ai:
        return "AI";
        break;
      case Brains.Remote:
        return "Online player";
        break;
    }
    return "Actor";
  }

  public string GetMoreInfo(){
    return "A character in this game.";
  }
  
  public Vector3 HeadPosition(){
    if(eyes != null){
      return eyes.ToGlobal(eyes.Translation);
    }
    return this.ToGlobal(Translation);
  }
  
  public Vector3 Forward(){
    if(eyes != null){
      return Util.TForward(eyes.Transform);
    }
    return Util.TForward(this.Transform);
  }
  
  public Vector3 Up(){
    if(eyes != null){
      return Util.TUp(eyes.Transform);
    }
    return Util.TUp(this.Transform);
  }
  
  public Vector3 RotationDegrees(){
    if(eyes != null){
      return eyes.GetRotationDegrees();
    }
    return GetRotationDegrees();
  }
  
  public Transform GetLookingTransform(){
    if(eyes != null){
      return eyes.Transform;
    }
    return Transform;
  }
  
  public void SetSprint(bool val){
    sprinting = val;
    int sprintInt = sprinting ? 1 : 0;
    stats.SetBaseStat(StatsManager.Stats.Sprinting, sprintInt);
    GD.Print("Sprinting set to " + sprintInt + ", " + stats.GetStat(StatsManager.Stats.Sprinting));
  }
  
  public float GetMovementSpeed(){
    float speed = 5f;

    int stamina = stats.GetStat(StatsManager.Stats.Stamina);
    if(sprinting && stamina > 0){
      speed *= 2f;
    }
    return speed;
  }

  public void EquipHotbarItem(int slot){
    EquipItem(hotbar.EquipItem(slot));
  }
  
  /* Equip item to hotbar immediately. */
  public void PickUpAndEquipItem(Item item){
    int slot = hotbar.FirstEmptySlot();

    if(slot == -1){
      GD.Print("PickUpAndEquipItem: No empty slot");
      return;
    }

    hotbar.SetItemSlot(slot, item);
    hotbar.EquipItem(slot);

    EquipItem(item);

  }

  public void EquipItem(Item item){
    if(item == null){
      GD.Print("Actor.EquipItem: Can't equip null item");
      return;
    }
    if(unarmed){
      StashHand();
    }
    else{
      StashItem();
    }

    if(eyes == null){
      GD.Print("Actor.EquipItem: No eyes.");
      return;
    }

    eyes.AddChild(item);
    

    item.Mode = RigidBody.ModeEnum.Static;
    item.Transform = GetItemTransform();
    //item.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem = item;
    activeItem.Equip(this);
    unarmed = false;
    GD.Print("Successfully equipped item " + item.Name);
  }

  public Transform GetItemTransform(){
    Vector3 x = new Vector3(1f, 0f, 0f);
    Vector3 y = new Vector3(0f, 1f, 0f);
    Vector3 z = new Vector3(0f, 0f, 1f);

    Basis basis = new Basis(x, y, z);

    Vector3 origin = new Vector3(HandPosX, HandPosY, HandPosZ);

    Transform ret = new Transform(basis, origin);

    return ret;
  }

  public void EquipNextItem(){
    EquipItem(hotbar.EquipNextItem());
  }
  
  /* Removes activeItem from hands. */
  public void StashItem(){
    if(unarmed){
      return;
    }

    if(activeItem == null){
      return;
    }
    
    if(activeItem.GetParent() == null){
      return;
    }

    DeferredStashItem();
    
    if(Session.NetActive() && Session.IsServer()){
      Rpc(nameof(DeferredStashItem));
    }
    else if(Session.NetActive()){
      RpcId(1, nameof(ServerStashItem), Session.session.netSes.selfPeerId);
    }
  }

  public void DropItem(Item item){
    Transform itemTrans = item.GetGlobalTransform();
    
    eyes.RemoveChild(item);
    Session.GameNode().AddChild(item);
    
    item.GlobalTransform = itemTrans;
    item.Mode = RigidBody.ModeEnum.Rigid;
    activeItem = null;
    hotbar.DropEquippedItem();
  }

  [Remote]
  public void ServerStashItem(int caller){
    DeferredStashItem();

    foreach(KeyValuePair<int, PlayerData> entry in Session.session.netSes.playerData){
      if(caller != entry.Key){
        RpcId(entry.Key, nameof(DeferredStashItem));
      }
    }
  }

  [Remote]
  public void DeferredStashItem(){
    activeItem.Unequip();
    //inventory.ReceiveItem(activeItem);
    eyes.RemoveChild(activeItem);

    //activeItem.QueueFree();
    activeItem = null;
    //EquipHand();
  }

  /* Remove hand in preparation of equipping an item */
  public void StashHand(){
    if(activeItem != hand){
      GD.Print("Can't stash hand when it's not active.");
      return;
    }

    eyes.RemoveChild(activeItem);
    activeItem = null;
    unarmed = false;
  }

  /* Equip hand when unarmed.  */
  public void EquipHand(){
    if(activeItem == hand){
      return;
    }

    activeItem = hand;
    eyes.AddChild(activeItem);
    activeItem.Translation = new Vector3(HandPosX, HandPosY, HandPosZ);
    activeItem.Mode = RigidBody.ModeEnum.Static;
    activeItem.Equip(this);
    unarmed = true;
  }

  public void Use(Item.ItemInputs use, bool released = false){
    if(activeItem == null){
      return;
    }

    activeItem.Use(use);
  }

  public static Vector3 EyesPos(){
    return new Vector3(0, 2, 0);
  }
    
  public override void _Process(float delta){
      if(IsDead()){
        return;
      }
      if(brainType != Brains.Remote){ 
        brain.Update(delta); 
        Gravity(delta);
      }
      StatsUpdate(delta);
      KillActorsThatFallOutOfTheMap();
  }

  public void KillActorsThatFallOutOfTheMap(){
    if(GetTranslation().y < -10){
      GD.Print("I fell out of the map!");
      Die();
    }
  }
  
  public void Gravity(float delta){ 
    float gravityForce = GravityAcceleration * delta;
    gravityVelocity += gravityForce;

    if(gravityVelocity < TerminalVelocity){
      gravityVelocity = TerminalVelocity;
    }
    
    Vector3 grav = new Vector3(0, gravityVelocity, 0);
    Move(grav, delta);
  }

  public void Move(Vector3 movement, float moveDelta = 1f){
      movement *= moveDelta;
      
      Transform current = GetTransform();
      Transform destination = current; 
      destination.Translated(movement);
      
      Vector3 delta = destination.origin - current.origin;
      KinematicCollision collision = MoveAndCollide(delta);
      
      if(collision != null && collision.Collider != null){
        ICollide collider = collision.Collider as ICollide;
        
        if(collider != null){
          Node colliderNode = collider as Node;
          collider.OnCollide(this as object);
        }
      }
      
      if(!grounded && collision != null && collision.Position.y < GetTranslation().y){
        if(gravityVelocity < 0){
          grounded = true;
          gravityVelocity = 0f;
        }
      }
  }
  
  public void Jump(){
    if(!grounded){ return; }
    
    float jumpForce = 10;
    gravityVelocity = jumpForce;
    grounded = false; 
    int jumpCost = stats.GetStat(StatsManager.Stats.JumpCost);
    Damage dmg = new Damage();
    dmg.stamina = jumpCost;
    ReceiveDamage(dmg);
  }

  [Remote]
  public void RemoteReceiveDamage(string json){
    Damage dmg = null;
    
    if(dmg != null){
      ReceiveDamage(dmg);
    }
  }
  
  public void ReceiveDamage(Damage damage){
    int health = GetHealth();
    if(health <= 0){
      return;
    }

    stats.ReceiveDamage(damage);

    HandleDeath(GetHealth() - health, damage);
  }

  public string GetStatusText(){
    return stats.GetStatusText();
  }

  public void StatsUpdate(float delta){
    int health = GetHealth();
    stats.Update(delta);

    HandleDeath(GetHealth() - health, null);
  }

  private void HandleDeath(int healthDelta, Damage damage){
    int health = GetHealth();

    if(health <= 0){
      speaker.PlayEffect(Sound.Effects.ActorDeath);
      string sender = "";
      
      if(damage != null){
        sender = damage.sender;
      }
      
      Die(sender);
    }
    else if(healthDelta < 0){
      speaker.PlayEffect(Sound.Effects.ActorDamage);
    }
  }
  
  public void Die(string source = ""){
    Transform = Transform.Rotated(new Vector3(0, 0, 1), 1.5f);
    string path = NodePath();
    SessionEvent evt = SessionEvent.ActorDiedEvent(path, source);
    Session.Event(evt);
  }

  public string NodePath(){
    NodePath path = GetPath();
    return path.ToString();
    
  }

  public string ToString(){
    string ret = "Actor: \n";
    ret += "\tName: " + name + "\n";
    ret += "\tHealth: " + GetHealth() + "/" +  GetHealthMax() + "\n";
    ret += "\tID: " + id + "\n";
    ret += "\t" + brain.ToString() + "\n";
    if(hotbar != null){
      ret += "\t" + hotbar.ToString() + "\n";
    }
    return ret;
  }

  public bool IsDead(){
    return GetHealth() <= 0;
  }

  public int GetHealth(){
    return stats.GetStat(StatsManager.Stats.Health);
  }

  public int GetHealthMax(){
    return stats.GetStat(StatsManager.Stats.HealthMax);
  }
  
  public void SyncPosition(){
    Vector3 pos = GetTranslation();
    RpcUnreliable(nameof(SetPosition), pos.x, pos.y, pos.z);
  }

  public void SyncAim(){
    Vector3 headRot = RotationDegrees();
    Vector3 bodyRot = GetRotationDegrees();

    float x = bodyRot.y;
    float y = headRot.x;

    RpcUnreliable(nameof(SetRotation), x, y);
  }

  [Remote]
  public void SetRotation(float x, float y){
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y = x;
    this.SetRotationDegrees(bodyRot);

    Vector3 headRot = RotationDegrees();
    headRot.x = y;
    if(headRot.x < minY){
      headRot.x = minY;  
    }
    if(headRot.x > maxY){
      headRot.x = maxY;
    }
    eyes.SetRotationDegrees(headRot);
  }

  [Remote]
  public void SetPosition(float x, float y, float z){
    Vector3 pos = new Vector3(x, y, z);
    SetTranslation(pos);
  }

  public void Turn(float x, float y){
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += x;
    this.SetRotationDegrees(bodyRot);
    
    Vector3 headRot = eyes.GetRotationDegrees();
    headRot.x += y;

    if(headRot.x < minY){
      headRot.x = minY;
    }

    if(headRot.x > maxY){
      headRot.x = maxY;
    }

    eyes.SetRotationDegrees(headRot);
  }
  
  public void SetPos(Vector3 pos){
    SetTranslation(pos);
  }
  
  public bool HasItem(string item){
    return false;
  }

  public string ItemInfo(){
    if(activeItem != null){
      return activeItem.GetInfo();
    }

    return "Unequipped";
  }
  
  public bool IsPaused(){
    return paused;
  }

  public void SetMenuActive(bool val){
    menuActive = val;

    if(menuActive){
      Input.SetMouseMode(Input.MouseMode.Visible);
    }
    else{
      Session.ChangeMenu(Menu.Menus.HUD);
      Input.SetMouseMode(Input.MouseMode.Captured);

    }
    
  }

  public List<Item> GetHotbarItems(){
    return hotbar.GetEveryItem();
  }
  
  public void TogglePause(){
    SetPaused(!paused);
  }

  public void Pause(){
    SetPaused(true);
  }

  public void Unpause(){
    SetPaused(false);
  }

  public void SetPaused(bool val){
    paused = val;

    if(brain as ActorInputHandler == null){
      return;
    }

    menuActive = val;
    if(menuActive){
      Session.ChangeMenu(Menu.Menus.Pause);
      Input.SetMouseMode(Input.MouseMode.Visible);
    }
    else{
      Session.ChangeMenu(Menu.Menus.HUD);
      Input.SetMouseMode(Input.MouseMode.Captured);
    }

  }
  

  public static Actor Factory(Brains brain = Brains.Player1, ActorData data = null){
    
    Actor actor = new Actor(brain);

    // TODO: Fix this to work

    return actor;
  }
}
using Godot;
using System;
using System.Collections.Generic;

public class HumanoidBody : KinematicBody , IBody, IReceiveDamage {
    
  Actor actor;
  Spatial eyes, hand;
  Speaker speaker;
  string rootPath;
  MeshInstance meshInstance;
  Dictionary<CollisionKey, CollisionShape> dummyCollisions;
  Dictionary<CollisionKey, int>  collisions;
  Skeleton skeleton;
  AnimationTree animationTree;
  AnimationNodeStateMachinePlayback stateMachine;
  public bool dead;
  const float ChangeAnimDelay = 0.05f;
  IncrementTimer ChangeAnimTimer;
  bool grounded, crouched;
  Vector3 velocity = new Vector3();
  const int maxY = 90;
  const int minY = -40;

  const float GravityForceY = -9.81f;
  Vector3 GravityAcceleration = new Vector3(0,GravityForceY,0);
  const float TerminalVelocity = -53;
  const float SightSize = 100f;

  public HumanoidBody(Actor actor, string rootPath = "res://Assets/Scenes/Actors/beast.tscn"){
    this.actor = actor;
    this.rootPath = rootPath;
    this.dead = false;
    ChangeAnimTimer = new IncrementTimer(ChangeAnimDelay);
    InitChildren();
  }

  enum CollisionKey{
    Idle,
    Crouch,
    Run,
    Dead
  }

  private void ChangeCollision(CollisionKey key){
    if(!collisions.ContainsKey(key)){
      return;
    }
    foreach (KeyValuePair<CollisionKey, int> pair in collisions)
    {
      int ownerId  = pair.Value;
      if(pair.Key == key){
        ShapeOwnerSetDisabled(ownerId,false);
      }
      else{
        ShapeOwnerSetDisabled(ownerId,true);
      }
    }
  }

  public void AnimationTrigger(string triggerName){
    GD.Print("Animation trigger " + triggerName);
    triggerName = triggerName.ToLower();
    if(triggerName=="claw"){
      if(crouched){
        stateMachine.Travel("claw_crouched");
      }
      else{
        stateMachine.Travel("claw");
      }
    }

    if(triggerName=="stab"){
      if(crouched){
        stateMachine.Travel("stab_crouched");
      }
      else{
        stateMachine.Travel("stab");
      }
    }

    if(triggerName=="hold"){
      animationTree.Set("parameters/hold/blend_amount", 1);
    }

    if(triggerName=="release"){
      animationTree.Set("parameters/hold/blend_amount", 0);
    }

  }

  public Actor GetActor(){
    return actor;
  }

  private void InitChildren(){
    
    PackedScene ps = (PackedScene)GD.Load(rootPath);
    Node rootNode = ps.Instance();
    AddChild(rootNode);
    
    speaker = new Speaker();
    AddChild(speaker);

    eyes = rootNode.FindNode("Eyes") as Spatial;
    meshInstance = rootNode.FindNode("Body") as MeshInstance;
    skeleton = rootNode.FindNode("Skeleton") as Skeleton;
    hand = rootNode.FindNode("Hand") as Spatial;
    animationTree = rootNode.FindNode("AnimationTree") as AnimationTree;
    grounded = true;
    dummyCollisions = new Dictionary<CollisionKey, CollisionShape>();
    dummyCollisions.Add(CollisionKey.Idle, rootNode.FindNode("CollisionIdle") as CollisionShape);
    dummyCollisions.Add(CollisionKey.Crouch, rootNode.FindNode("CollisionCrouch") as CollisionShape);
    dummyCollisions.Add(CollisionKey.Run, rootNode.FindNode("CollisionRun") as CollisionShape);
    dummyCollisions.Add(CollisionKey.Dead, rootNode.FindNode("CollisionDead") as CollisionShape);
    initAnimTree();
  }

  private void initAnimTree(){
    animationTree.SetActive(true);
    stateMachine = animationTree.Get("parameters/StateMachine/playback") as AnimationNodeStateMachinePlayback;
    setBlendPosition(0);
  }

  private void initCollisions(){
    collisions = new Dictionary<CollisionKey, int>();
    foreach (KeyValuePair<CollisionKey, CollisionShape> pair in dummyCollisions)
    {
      CollisionShape dummy = pair.Value;
      if(dummy != null){
        int ownerId = CreateShapeOwner(this);
        ShapeOwnerAddShape(ownerId, dummy.Shape);
        ShapeOwnerSetTransform(ownerId, dummy.GetGlobalTransform());
        ShapeOwnerSetDisabled(ownerId, dummy.Disabled);
        collisions.Add(pair.Key, ownerId);
      }
    }
  }

  public override void _Ready(){
    initCollisions();
    ChangeCollision(CollisionKey.Idle);
  }

  public void setBlendPosition(float blendPosition =0){
    animationTree.Set("parameters/StateMachine/walk/blend_position", blendPosition);
    animationTree.Set("parameters/StateMachine/crouch/blend_position", blendPosition);
    if(blendPosition!=0){
      ChangeAnimTimer.StartTimer();
    }
  }

  public void ReceiveDamage(Damage damage){
    if(dead){
      return;
    }
    
    IReceiveDamage receiver = actor.stats as IReceiveDamage;
    if(receiver == null){
      return;
    }

    GD.Print("Received Damage " + Util.ToJson(damage));
    receiver.ReceiveDamage(damage);

    if(GetHealth() < 1){
      GD.Print("Died because health was" + GetHealth());
      
      string dead = this.GetPath();
      string killer = damage.sender;
      SessionEvent evt = SessionEvent.ActorDiedEvent(dead, killer);
      Session.session.HandleEvent(evt);

      Die();
    }
  }

  public int GetHealth(){
    IReceiveDamage receiver = actor.stats as IReceiveDamage;
    return receiver == null ? 0 : receiver.GetHealth();
  }

  public List<Node> GetHands(){ 
    return new List<Node>(){hand};
  }
  
  public Node GetNode(){ 
    return this as Node; 
  }
  
  public void InitCam(int index){
    if(index != 0){
      return; // Splitscreen cameras not implemented
    }
    
    Camera cam = new Camera();
    cam.Far = 1000f;
    eyes.AddChild(cam);
  }
  public void Move(Vector3 movement, float moveDelta = 1f, bool ignoreAnimator = true, bool sprint = false){

      if(!ignoreAnimator && grounded){
        if(movement.z != 0){ 
           if(movement.z < 0){
            setBlendPosition(1);
          }
          else{
            setBlendPosition(-1);
          }
          ChangeAnimTimer.StartTimer();
        }
      }

      movement *= moveDelta;
      
      Transform current = GetTransform();
      Transform destination = current; 
      destination.Translated(movement);
      
      Vector3 delta = destination.origin - current.origin;
      KinematicCollision collision = MoveAndCollide(delta);
      
      if(collision == null){
        return;
      }

      if(collision.Collider != null){
        ICollide collider = collision.Collider as ICollide;
        
        if(collider != null){
          collider.OnCollide(this as object);
        }
      }

  }

  public void ToggleCrouch(){
    crouched = !crouched;
    if(crouched){
      stateMachine.Travel("crouch");
      ChangeCollision(CollisionKey.Crouch);
    }
    else{
      stateMachine.Travel("walk");
      ChangeCollision(CollisionKey.Idle);
    }
  }

  public void Turn(Vector3 movement, float moveDelta){
    float factor = 20;
    movement *= moveDelta;
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += movement.x;
    this.SetRotationDegrees(bodyRot);

    float blendPosition = (float) animationTree.Get("parameters/look/blend_position");
    blendPosition += movement.y / factor;
    blendPosition = Mathf.Clamp(blendPosition, -1, 1);

    animationTree.Set("parameters/look/blend_position", blendPosition);

  }

  public void Update(float delta){
    float margin = 0.05f;
    float landMargin = 3f;
    
    if(dead){
      return;
    }
    
    ApplyGravity(delta);
    
    if(ChangeAnimTimer.CheckTimer(delta))
      setBlendPosition(0);
    if(TestMove(Transform, margin * Vector3.Down)){
      if(velocity.y < 0){
        if(!grounded){
          grounded = true;
          velocity = new Vector3();
        }
        if(grounded && stateMachine.GetCurrentNode() == "jump"){
          stateMachine.Travel("walk");
        }
      }
    }
    else if(TestMove(Transform, landMargin * Vector3.Down)){
      if(velocity.y < 0){
        if(stateMachine.GetCurrentNode() == "jump"){
          stateMachine.Travel("land");
        }
      }
    }
    
  }

  public void HoldItem(int hand, IItem item){
    Node node = item.GetNode();
    this.hand.AddChild(node);
    AnimationTrigger("hold");
  }

  public void ReleaseItem(int hand, IItem item){
    if(this.hand.IsAParentOf(item.GetNode())){
      this.hand.RemoveChild(item.GetNode());
    }
    AnimationTrigger("release");
  }

  private void DieWhenOutOfBounds(){
    float minimumYBounds = -100;
    
    if(actor.stats != null && GetTranslation().y < minimumYBounds){
      Damage damage = new Damage();
      damage.health = actor.stats.Health;
      actor.stats.ReceiveDamage(damage);
    }

  }

  private void ApplyGravity(float delta){
    Vector3 gravityForce = GravityAcceleration * delta;
    velocity += gravityForce;

    if(velocity.Length() < TerminalVelocity){
      velocity = TerminalVelocity*velocity.Normalized();
    }
    
    Move(velocity, delta, true);

    DieWhenOutOfBounds();
  }

  public void Jump(){
    if(!grounded){ 
      return; 
    }
    crouched=false;
    Vector3 jumpForce = new Vector3(0, 11, 0);
    
    if(actor.stats == null){
      velocity = jumpForce;
      grounded = false;
      stateMachine.Travel("jump");
      return;
    }

    if(actor.stats != null){
      int jumpCost = actor.stats.JumpCost;
      if(actor.stats.ConsumeCondition(Stats.Conditions.Stamina, jumpCost)){
        velocity = jumpForce;
        grounded = false;
        stateMachine.Travel("jump");
      }
    }
  }

  public Speaker GetSpeaker(){
    return speaker;
  }

  public MeshInstance GetMesh(){
    return meshInstance;
  }

  public bool IsDead(){
    return dead;
  }

  public void Die(){
    dead = true;
    stateMachine.Travel("die");
    ChangeCollision(CollisionKey.Dead);
  }

  public List<Actor> ActorsInSight(){
    Vector3 start = eyes.GlobalTransform.origin;
    Vector3 end = Get3DCursor();
    
    Spatial spat = this as Spatial;
    Vector3 offset = new Vector3(SightSize, SightSize, SightSize);
    Vector3 min = spat.Transform.origin - offset;
    Vector3 max = spat.Transform.origin + offset;
    List<object> objects = Util.SiblingBoxCast(spat, min, max);
    List<Actor> ret = new List<Actor>();
    foreach(object obj in objects){
      Node node = obj as Node;
      Actor sightedActor = Actor.GetActorFromNode(node);
      if(sightedActor != null && sightedActor != actor){
        ret.Add(sightedActor);
      }
    }
    return ret;
  }

  public Vector3 Get3DCursor(float distance = 100f){
    Vector3 start = eyes.GlobalTransform.origin;
    Transform headTrans = eyes.Transform;
    Vector3 end = Util.TForward(headTrans);
    end *= distance;
    end += start;
    return end;
  }

  public Vector3 LookingDegrees(){
    Vector3 torsoRot = GetRotationDegrees();
    Vector3 eyesRot = eyes.GetRotationDegrees();
    torsoRot.x = eyesRot.x;
    return torsoRot;
  }
}

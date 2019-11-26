using Godot;
using System;
using System.Collections.Generic;

public class HumanoidBody : KinematicBody , IBody, IReceiveDamage {
    
  Actor actor;
  Spatial eyes, hand;
  Speaker speaker;
  string rootPath;
  MeshInstance meshInstance;
  CollisionShape collisionShape;
  Godot.Collections.Array<CollisionShape> dummyCollisions;
  Godot.Collections.Array<CollisionShape> collisions;
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

  public HumanoidBody(Actor actor, string rootPath = "res://Assets/Scenes/Actors/debug2.tscn"){
    this.actor = actor;
    this.rootPath = rootPath;
    this.dead = false;
    ChangeAnimTimer = new IncrementTimer(ChangeAnimDelay);
    InitChildren();
  }

  public void AnimationTrigger(string triggerName){
    GD.Print("Animation trigger " + triggerName);
    triggerName = triggerName.ToLower();
    if(triggerName=="claw"){
      stateMachine.Travel("claw");
    }
    if(triggerName=="stab"){
      stateMachine.Travel("stab");
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
    initAnimTree();
  }

  public void initAnimTree(){
    animationTree.SetActive(true);
    stateMachine = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
    setBlendPosition(0);
  }

  public void setBlendPosition(float blendPosition =0){
    animationTree.Set("parameters/walk/blend_position",blendPosition);
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
    }
    else{
      stateMachine.Travel("walk");
    }
  }

  public void Turn(Vector3 movement, float moveDelta = 1f){
    movement *= moveDelta;
    Vector3 bodyRot = this.GetRotationDegrees();
    bodyRot.y += movement.x;
    this.SetRotationDegrees(bodyRot);
    
    Vector3 headRot = eyes.GetRotationDegrees();
    headRot.x += movement.y;

    if(headRot.x < minY){
      headRot.x = minY;
    }

    if(headRot.x > maxY){
      headRot.x = maxY;
    }

    eyes.SetRotationDegrees(headRot);

  }

  public override void _Ready(){
    dummyCollisions = new Godot.Collections.Array<CollisionShape>();
    collisions = new Godot.Collections.Array<CollisionShape>();
    foreach(System.Object obj in GetTree().GetNodesInGroup("Collisions")){
      CollisionShape dummy = obj as CollisionShape;
      if(dummy != null && IsAParentOf(dummy)){
        dummyCollisions.Add(dummy);
        CollisionShape collision = new CollisionShape();
        collision.Shape = dummy.Shape;
        AddChild(collision);
        collision.SetGlobalTransform(dummy.GetGlobalTransform());
        collision.Disabled = dummy.Disabled;
        collisions.Add(collision);
      }
    }
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

  public void UpdateCollision(float delta){
    for (int i = 0; i < collisions.Count; i++)
    {
      CollisionShape collision = collisions[i];
      CollisionShape dummy = dummyCollisions[i];
      collision.SetGlobalTransform(dummy.GetGlobalTransform());
      collision.Disabled=dummy.Disabled;
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
    Transform trans = Transform;
    Vector3 pos = trans.origin;
    trans = trans.Rotated(new Vector3(0, 0, 1), 1.5f);
    trans.origin = pos;
    Transform = trans;
    dead = true;
    //FIX ME: play dead anim
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

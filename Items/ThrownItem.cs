/*
    An item such as a spear that might be meleed with, or thrown.
    This should be used for any minor piece of debris that has no \
    intrinsic use.
*/
using Godot;
using System;

public class ThrownItem : MeleeWeapon, IWeapon, IEquip {

    public static float DefaultImpulseStrength = 25.0f;
    public bool meleeEnabled;
    public bool thrown;
    public float impulseStrength;
    public object pastWielder;

    public ThrownItem(){
        healthDamage = DefaultDamage;
        meleeEnabled = true;
        thrown = false;
        impulseStrength = DefaultImpulseStrength;
        swingSpeed = DefaultSwingSpeed;
    }

    public override void Use(Item.Uses use, bool released = false){
        switch(use){            
            case Uses.A: 
                if(meleeEnabled){
                    Swing();
                } 
                break;
            case Uses.B:
                Throw();
                break;
        }

    }

    public void Throw(){
        GD.Print("Throw item");
        if(thrown){
            GD.Print("Can only throw item once.");
            return;
        }
        thrown = true;

        Actor actor = wielder as Actor;
        pastWielder = wielder;

        if(actor != null){
            actor.DropItem(this);
            SetCollision(true);
            wielder = null;
            actor.EquipNextItem();
        }

        Transform start = this.GetGlobalTransform();
        Transform destination = start;
        destination.Translated(new Vector3(0, 0, 1));
        
        Vector3 impulse = start.origin - destination.origin;

        this.SetAxisVelocity(impulse * impulseStrength);

        swinging = true;
    }

  public override void DoOnCollide(object body){
    HandlePickup(body);

    if(!swinging){
      return;
    }
    if(thrown){
        swinging = false;
        this.SetAxisVelocity(new Vector3());
    }
    
    IReceiveDamage receiver = body as IReceiveDamage;
    IReceiveDamage wielderDamage = wielder as IReceiveDamage;
    if(wielderDamage == null){
        wielderDamage = pastWielder as IReceiveDamage;
    }

    if(receiver != null && receiver != wielderDamage){
      Strike(receiver);
    }
    else if(receiver == wielderDamage){
        swinging = true;
    }
  }

  public override void GiveDamage(IReceiveDamage receiver){
    Damage damage = new Damage(healthDamage);

    Node wielderNode = wielder as Node;
    Node pastWielderNode = pastWielder as Node;

    if(wielderNode != null){
      damage.sender = wielderNode.GetPath();
    }
    else if(pastWielderNode != null){
      damage.sender = pastWielderNode.GetPath();
    }

    GD.Print("Sender : " + damage.sender);
    receiver.ReceiveDamage(damage);
  }

  public void HandlePickup(object body){
    if(!thrown || swinging){
        return;
    }

    Actor pastActor = pastWielder as Actor;
    Actor currentActor = body as Actor;
    
    if(pastActor == null || currentActor == null){
        return;
    }

    if(pastActor == currentActor){
        GD.Print("Picking item back up");
        thrown = false;
        if(GetParent() != null){
            GetParent().RemoveChild(this);
        }
        
        currentActor.PickUpAndEquipItem(this);
    }
    else{
        GD.Print("Only the original actor can pick it up.");
    }
    
  }

  public override void EndSwing(){
    swinging = false;
    busy = false;

    if(wielder != null){
        GD.Print("Wielder " + wielder);
        Translation = wieldedPosition;
        SetCollision(false);    
    }
  }
    
}
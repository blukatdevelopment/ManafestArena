/*
    An item such as a spear that might be meleed with, or thrown.
    This should be used for any minor piece of debris that has no \
    intrinsic use.
*/
using Godot;
using System;

public class ThrownItem : MeleeWeapon, IWeapon, IEquip {

    public static float DefaultImpulseStrength = 50.0f;
    public bool meleeEnabled;
    public bool thrown;
    public float impulseStrength;

    public ThrownItem(){
        healthDamage = DefaultDamage;
        meleeEnabled = true;
        thrown = false;
        impulseStrength = DefaultImpulseStrength;
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

        if(actor != null){
            actor.DropItem(this);
            SetCollision(true);
            wielder = null;
        }

        Transform start = this.GetGlobalTransform();
        Transform destination = start;
        destination.Translated(new Vector3(0, 0, 1));
        
        Vector3 impulse = start.origin - destination.origin;

        this.SetAxisVelocity(impulse * impulseStrength);

        swinging = true;
    }

    public override void DoOnCollide(object body){
    if(!swinging){
      return;
    }
    if(thrown){
        swinging = false;
        this.SetAxisVelocity(new Vector3());
    }
    
    IReceiveDamage receiver = body as IReceiveDamage;
    IReceiveDamage wielderDamage = wielder as IReceiveDamage;
    
    if(receiver != null && receiver != wielderDamage){
      Strike(receiver);
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
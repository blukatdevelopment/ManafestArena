using Godot;
using System;
using System.Collections.Generic;

public class ItemThrower {
    IItem item;

    public const float DefaultImpulseStrength = 25f;
    public bool thrown;
    public float thrownImpulseStrength;
    public object pastWielder;
    public bool damageActive;
    public Damage damage;

    public ItemThrower(IItem item){
        this.item = item;
    }

    public void Config(
        float thrownImpulseStrength,
        Damage damage
    ){
        this.thrownImpulseStrength = thrownImpulseStrength;
        this.damage = damage;
    }

    public void Throw(){
        if(thrown){
            return;
        }
        thrown = true;

        object wielder = item.GetWielder();
        Actor actor = wielder as Actor;
        pastWielder = wielder;

        if (actor != null) {
            actor.DropItem(this);
            SetCollision(true);
            wielder = null;
            actor.EquipNextItem();
        }

        Transform start = item.GetNode().GetGlobalTransform();
        Transform destination = start;
        destination.Translated(new Vector3(0, 0, 1));
        
        Vector3 impulse = start.origin - destination.origin;

        RigidBody rigidBody = item.GetNode() as RigidBody;
        if(rigidBody != null){
            rigidBody.SetAxisVelocity(impulse * thrownImpulseStrength);
        }
    }

    public void OnUpdateWielder(){
        object wielder = item.GetWielder();
        if(wielder == false){
            damage.sender = "";
            return;
        }
        Node wielderNode = wielder as Node;
        if(wielderNode != null){
            damage.sender = wielderNode.GetPath();
        }
    }

    public void OnCollide(object body){
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

    public void HandlePickup(object body){
        if(!thrown || damageActive){
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
            GD.Print("Only the original actor can pick it up at this point in time.");
        }
        
    }

    private Node GetParent(){
        return item.GetNode().GetParent();
    }
}
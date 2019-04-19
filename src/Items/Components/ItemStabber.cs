/*
    Item snaps forward to damage an enemy,
    then snaps back when either a target
    is hit or the stab time runs out.
*/
using Godot;
using System;
using System.Collections.Generic;

public class ItemStabber {
    IItem item;
    public Vector3 wieldedPosition, forwardPosition;
    public bool stabbing = false;
    public float stabSpeed, stabTimer;
    public Damage damage;
    public Speaker speaker;
    public Sound.Effects stabSound, impactSound;

    public ItemStabber(IItem item){
        this.item = item;
        this.stabSound = Sound.Effects.FistSwing;
    }

    public void Config(
        float stabSpeed, // How long a stab takes to complete
        Damage damage, // Damage object passed to stabbed target
        Speaker speaker, // Use this to make noise
        Sound.Effects stabSound,
        Sound.Effects impactSound
    ){
        this.stabSpeed  = stabSpeed;
        this.damage     = damage;
        this.speaker    = speaker;
        this.stabSound = stabSound;
        this.impactSound = impactSound;
        this.forwardPosition = new Vector3();
        this.wieldedPosition = new Vector3();
    }

    public void Update(float delta){
        this.item = item;
        if(stabbing){
            stabTimer -= delta;
            if(stabTimer <= 0f){
                stabbing = false;
                EndStab();
            }
        }
    }

    public void OnUpdateWielder(){
        EndStab();

        object wielder = item.GetWielder();
        if(wielder == null){
            damage.sender = "";
            return;
        }
        Node wielderNode = wielder as Node;
        if(wielderNode != null){
            damage.sender = wielderNode.GetPath();
        }
        
        Spatial itemSpatial = item.GetNode() as Spatial;
        if(itemSpatial == null){
            return;
        }
        
        wieldedPosition = itemSpatial.GetTranslation();
        forwardPosition = wieldedPosition + new Vector3(0, 0, -1);
    }

    public bool CanStartStab(){
        return !stabbing;
    }

    public void StartStab(){
        if(stabbing){
            return;
        }

        stabbing = true;
        stabTimer = stabSpeed;

        if(speaker != null){
            speaker.PlayEffect(stabSound);
        }

        Spatial itemSpatial = item.GetNode() as Spatial;
        if(itemSpatial != null){
            itemSpatial.Translation = forwardPosition;
        }
        item.SetCollision(true);
    }

    public void EndStab(){
        if(!stabbing){
            return;
        }

        stabbing = false;
        stabTimer = 0f;
        
        Spatial itemSpatial = item.GetNode() as Spatial;
        if(itemSpatial != null){
            itemSpatial.Translation = wieldedPosition;
        }
        item.SetCollision(false);
    }

    public void OnCollide(object body){
        if(!stabbing){
            return;
        }
        if(damage == null){
            EndStab();
            return;
        }

        IReceiveDamage receiver = body as IReceiveDamage;
        if(receiver != null){
            receiver.ReceiveDamage(damage);

            if(speaker != null){
                speaker.PlayEffect(impactSound);
            }
        }

    }
}
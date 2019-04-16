/*
    Provides functionality for damage on contact.
*/
using Godot;
using System;
using System.Collections.Generic;

public class CollisionDamager {
    public Damage damage;
    public IItem item;
    public bool active = true;
    public Speaker speaker;
    public Sound.Effects impactSound;
    public bool damageOnce; // Stop giving damage after first time

    public CollisionDamager(IItem item){
        this.item = item;
    }

    public void Config(
        Damage damage, 
        Speaker speaker, 
        Sound.Effects impactSound,
        bool damageOnce = true
    ){
        this.damage = damage;
        this.speaker = speaker;
        this.impactSound = impactSound;
        this.damageOnce = damageOnce;
    }

    public void OnCollide(object Body){
        if(!active || damage == null){
            return;
        }

        if(speaker != null){
            speaker.PlayEffect(impactSound);
        }

        IReceiveDamage receiver = body as IReceiveDamage;
        if(receiver != null){
            receiver.ReceiveDamage(receiver);
        }

        if(damageOnce){
            active = false;
        }
    }
}
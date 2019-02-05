/*
    An item such as a spear that might be meleed with, or thrown.
    This should be used for any minor piece of debris that has no \
    intrinsic use.
*/
using Godot;
using System;

public class ThrownItem : MeleeWeapon, IWeapon, IEquip {

    public bool meleeEnabled;

    public ThrownItem(){
        healthDamage = DefaultDamage;
        this.meleeEnabled = true;

    }

    public override void Use(Item.Uses use, bool released = false){
        GD.Print("Item used with  " + use + "use melee " + meleeEnabled );
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
    }

    
}
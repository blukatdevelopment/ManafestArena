// For when using an item requires stamina, mana, or health
using Godot;
using System;
using System.Collections.Generic;

public class StatConsumer{
    public int health, stamina, mana;
    public object wielder;

    public StatConsumer(){
        health = stamina = mana = 0;
    }
    
    public StatConsumer(int health, int stamina, int mana){
        this.health = health;
        this.stamina = stamina;
        this.mana = mana;
    }

    public bool ConsumeStats(){
        // FIXME: Need to be reimplemented with IStats
        return true;
    }
}
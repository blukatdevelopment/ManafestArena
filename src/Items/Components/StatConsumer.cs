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
        IHasStats statsHolder = wielder as IHasStats;
        if(statsHolder == null){
            return false;
        }
        StatsManager stats = statsHolder.GetStats();

        bool healthCheck  = stats.CanConsumeStat(StatsManager.Stats.Health, health);
        bool staminaCheck = stats.CanConsumeStat(StatsManager.Stats.Stamina, stamina);
        bool manaCheck    = stats.CanConsumeStat(StatsManager.Stats.Mana, mana);
        
        if(!healthCheck || !staminaCheck || !manaCheck){
            return false;
        }

        stats.ConsumeStat(StatsManager.Stats.Health, health);
        stats.ConsumeStat(StatsManager.Stats.Stamina, stamina);
        stats.ConsumeStat(StatsManager.Stats.Mana, mana);

        return true;
    }
}
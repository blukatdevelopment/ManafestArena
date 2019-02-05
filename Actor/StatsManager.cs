/*
    Basically a character sheet + dice to make calculations + game status.
    Persists a wide range of data to a csv file until JSON or SQL storage are
    made available.
*/
using Godot;
using System;
using System.Collections.Generic;

public class StatsManager {
    public enum Archetypes{ // Types of characters
        None,
        One,
        Two,
        Three,
        EnemyOne,
        EnemyTwo
    };

    public enum Effects{ // Perks and status conditions
        None,
        Bleed
    };

    public enum Stats{
        // ICEPAWS Attributes
        Intelligence,
        Charisma,
        Endurance,
        Perception,
        Agility,
        Willpower,
        Strength,

        // DERIVED stats
        Health,
        HealthMax,
        HealthRegenDelay,
        HealthRegenAmount,
        Stamina,
        StaminaMax,
        StaminaRegenDelay,
        StaminaRegenAmount,
        Mana,
        ManaMax,
        ManaRegenDelay,
        ManaRegenAmount,
        Speed,
        UnarmedDamage,
        DamageResistance,
        DamageThreshold,
        SlotsMax,

        // Identity
        Brain,

        // GameState
        LastEncounter,
        CurrentNode,
        CurrentLevel,
        LastNode,
        NodeInProgress,
        Victory
    };

    public enum Abilities{
        None,
        Unarmed,
        DoubleJump
    };

    public enum Facts{
        None,
        Name,
        Archetype,
        Slot1,
        Slot2,
        Slot3,
        Slot4,
        Slot5,
        Slot6,
        Slot7,
        Slot8,
        Slot9,
        Slot10
    };

    System.Collections.Generic.Dictionary<Stats, int> baseStats;
    System.Collections.Generic.Dictionary<Stats, int> statBuffs;
    System.Collections.Generic.Dictionary<Effects, int> effects;
    System.Collections.Generic.Dictionary<Abilities, int> abilities;

    System.Collections.Generic.Dictionary<Facts, string> facts;

    public StatsManager(){
        baseStats = new System.Collections.Generic.Dictionary<Stats, int>();
        statBuffs = new System.Collections.Generic.Dictionary<Stats, int>();
        abilities = new System.Collections.Generic.Dictionary<Abilities, int>();
        effects = new System.Collections.Generic.Dictionary<Effects, int>();
        facts = new System.Collections.Generic.Dictionary<Facts, string>();
    }

    public StatsManager(string archetypeString){
        baseStats = new System.Collections.Generic.Dictionary<Stats, int>();
        statBuffs = new System.Collections.Generic.Dictionary<Stats, int>();
        abilities = new System.Collections.Generic.Dictionary<Abilities, int>();
        effects = new System.Collections.Generic.Dictionary<Effects, int>();
        facts = new System.Collections.Generic.Dictionary<Facts, string>();
        Init(archetypeString);   
    }

    public void Init(string archetypeString){
        Init(Archetype(archetypeString));
    }

    public void Init(Archetypes archetype){
        switch(archetype){
            case Archetypes.None:
                break;
            case Archetypes.One:
                BeastInit();
                break;
            case Archetypes.Two:
                MageInit();
                break;
            case Archetypes.Three:
                SoldierInit();
                break;
            case Archetypes.EnemyOne:
                GoonInit();
                break;
            case Archetypes.EnemyTwo:
                GoonInit();
                break;
        }
        baseStats.Add(Stats.CurrentLevel, -1);
        baseStats.Add(Stats.LastNode, -1);
    }

    public void BeastInit(){
        int brainInt = (int)Actor.Brains.Player1;
        SetBaseStat(Stats.Brain, brainInt);
        SetFact(Facts.Archetype, CharacterName(Archetypes.One));
        SetFact(Facts.Name, "Beast");

        SetBaseStat(Stats.Intelligence, 3);
        SetBaseStat(Stats.Charisma,     3);
        SetBaseStat(Stats.Endurance,    6);
        SetBaseStat(Stats.Perception,   9);
        SetBaseStat(Stats.Agility,      9);
        SetBaseStat(Stats.Willpower,    3);
        SetBaseStat(Stats.Strength,     9);
        
        ReplenishStats();
        GD.Print("BeastManInit");
    }

    public void MageInit(){
        int brainInt = (int)Actor.Brains.Player1;
        SetFact(Facts.Archetype, CharacterName(Archetypes.One));
        SetBaseStat(Stats.Brain, brainInt);
        SetFact(Facts.Name, "Mage");

        SetBaseStat(Stats.Intelligence, 5);
        SetBaseStat(Stats.Charisma,     5);
        SetBaseStat(Stats.Endurance,    5);
        SetBaseStat(Stats.Perception,   5);
        SetBaseStat(Stats.Agility,      5);
        SetBaseStat(Stats.Willpower,    5);
        SetBaseStat(Stats.Strength,     5);

        ReplenishStats();
        GD.Print("MageInit");
    }

    public void SoldierInit(){
        int brainInt = (int)Actor.Brains.Player1;
        SetFact(Facts.Archetype, CharacterName(Archetypes.Three));
        SetBaseStat(Stats.Brain, brainInt);
        SetFact(Facts.Name, "Soldier");

        SetBaseStat(Stats.Intelligence, 5);
        SetBaseStat(Stats.Charisma,     5);
        SetBaseStat(Stats.Endurance,    5);
        SetBaseStat(Stats.Perception,   5);
        SetBaseStat(Stats.Agility,      5);
        SetBaseStat(Stats.Willpower,    5);
        SetBaseStat(Stats.Strength,     5);

        ReplenishStats();
        GD.Print("SoldierInit");
    }

    public void GoonInit(){
        int brainInt = (int)Actor.Brains.Ai;
        SetFact(Facts.Archetype, CharacterName(Archetypes.EnemyOne));
        SetBaseStat(Stats.Brain, brainInt);
        SetFact(Facts.Name, "Goon");

        SetBaseStat(Stats.Intelligence, 5);
        SetBaseStat(Stats.Charisma,     5);
        SetBaseStat(Stats.Endurance,    5);
        SetBaseStat(Stats.Perception,   5);
        SetBaseStat(Stats.Agility,      5);
        SetBaseStat(Stats.Willpower,    5);
        SetBaseStat(Stats.Strength,     5);

        ReplenishStats();
        GD.Print("GoonInit"); 
    }

    // Raise all stats with a max to that max value.
    public void ReplenishStats(){
        int healthMax = GetStat(Stats.HealthMax);
        SetBaseStat(Stats.Health, healthMax);
    }

    // Returns a stat before buffs are applied
    public int GetBaseStat(Stats stat){
        bool derived = StatIsDerived(stat);
        if(baseStats.ContainsKey(stat) && !derived){
            return baseStats[stat];
        }
        else if(derived){
            return GetDerivedStat(stat);
        }
        GD.Print("Stat " + stat + " not set");
        return 0;
    }

    public void SetBaseStat(Stats stat, int value){
        if(stat == Stats.HealthMax){
          GD.Print("SetBaseStat: " + stat + ", " + value);
        }
        if(baseStats.ContainsKey(stat)){
            if(stat == Stats.HealthMax){
                GD.Print("baseStats already contains " + stat);
            }
            baseStats[stat] = value;
            return;
        }
        baseStats.Add(stat, value);
        if(stat == Stats.HealthMax){
            GD.Print("baseStats does not already contain " + stat);
            GD.Print(this.ToString());
        }
    }

    // Returns a stat derived from other stats
    public int GetDerivedStat(Stats stat){
        switch(stat){
            case Stats.HealthMax: return HealthMaxFormula(GetStat(Stats.Endurance)); break;
            case Stats.HealthRegenAmount:
                break;
            case Stats.HealthRegenDelay:
                break;
            case Stats.StaminaMax:
                break;
            case Stats.StaminaRegenAmount:
                break;
            case Stats.StaminaRegenDelay:
                break;
            case Stats.ManaMax:
                break;
            case Stats.ManaRegenAmount:
                break;
            case Stats.ManaRegenDelay:
                break;
            case Stats.Speed:
                break;
            case Stats.UnarmedDamage:
                break;
            case Stats.DamageResistance:
                break;
            case Stats.DamageThreshold:
                break;
            case Stats.SlotsMax: return SlotsMaxFormula(GetStat(Stats.Strength)); break;
        }
        return 0;
    }

    public static int HealthMaxFormula(int endurance){
        return 50 + (endurance * 10);
    }

    public static int SlotsMaxFormula(int strength){
        int ret = strength / 2; // 2 strength = 1 slot
        ret += 1; // round up to give 0 strength 1 slot
        return ret;
    }

    public int GetStatBuff(Stats stat){
        if(statBuffs.ContainsKey(stat)){
            return statBuffs[stat];
        }
        return 0;
    }

    public void SetStatBuff(Stats stat, int value){
        if(statBuffs.ContainsKey(stat)){
            statBuffs[stat] = value;
            return;
        }
        statBuffs.Add(stat, value);
    }

    public int GetStat(Stats stat){
        int baseStat = GetBaseStat(stat);
        int buff = GetStatBuff(stat);
        return baseStat + buff;
    }

    public string GetFact(Facts fact){
        if(facts.ContainsKey(fact)){
            return facts[fact];
        }
        return "";
    }

    public static bool StatIsDerived(Stats stat){
        List<Stats> derived = new List<Stats>{
            Stats.HealthMax,
            Stats.StaminaMax,
            Stats.ManaMax,
            Stats.Speed,
            Stats.UnarmedDamage,
            Stats.SlotsMax
        };

        if(derived.IndexOf(stat) != -1){
            return true;
        }
        return false;
    }

    public bool HasPerk(Effects effect){
        if(effects.ContainsKey(effect)){
            return true;
        }

        return false;
    }

    public void SetEffect(Effects effect, int value){
        if(effects.ContainsKey(effect)){
            effects[effect] = value;
            return;
        }
        effects.Add(effect, value);
    }

    public void SetAbility(Abilities ability, int value){
        if(abilities.ContainsKey(ability)){
            abilities[ability] = value;
            return;
        }
        abilities.Add(ability, value);
    }

    public void SetFact(Facts fact, string value){
        if(facts.ContainsKey(fact)){
            facts[fact] = value;
            return;
        }
        facts.Add(fact, value);
    }    

    public int EffectsLevel(Effects effect){
        if(effects.ContainsKey(effect)){
            return effects[effect];
        }
        return 0;
    }

    public bool HasAbility(Abilities ability){
        if(abilities.ContainsKey(ability)){
            return true;
        }

        return false;
    }

    public System.Collections.Generic.Dictionary<int, string[]> GetRows(){
        System.Collections.Generic.Dictionary<int, string[]> ret;
        ret = new System.Collections.Generic.Dictionary<int, string[]>();
        int i = 0;
        foreach(Stats key in baseStats.Keys){
            ret.Add(i, new List<string>{"stats", "" + key, "" + baseStats[key]}.ToArray());
            i++;
        }

        foreach(Stats key in statBuffs.Keys){
            ret.Add(i, new List<string>{"buffs", "" + key, "" + statBuffs[key]}.ToArray());
            i++;
        }

        foreach(Effects key in effects.Keys){
            ret.Add(i, new List<string>{"effects", "" + key, "" + effects[key]}.ToArray());
            i++;
        }

        foreach(Abilities key in abilities.Keys){
            ret.Add(i, new List<string>{"abilities", "" + key, "" + abilities[key]}.ToArray());
            i++;
        }

        foreach(Facts key in facts.Keys){
            ret.Add(i, new List<string>{"facts", "" + key, facts[key]}.ToArray());
            i++;
        }

        return ret;

    }

    public static StatsManager FromRows(System.Collections.Generic.Dictionary<int, string[]> rows){
        StatsManager ret = new StatsManager();
        foreach(int key in rows.Keys){
            ret.LoadRow(rows[key]);
        }
        return ret;
    }

    public void LoadRow(string[] row){
        string type = row[0];
        string key = row[1];
        string value = row[2];

        switch(type){
            case "stats":
                LoadStat(key, value);
                break;
            case "buffs":
                LoadBuff(key, value);
                break;
            case "effects":
                LoadEffect(key, value);
                break;
            case "abilities":
                LoadAbility(key, value);
                break;
            case "facts":
                LoadFact(key, value);
                break;
        }
    }

    public void LoadStat(string key, string value){
        Stats keyStat = (Stats) Enum.Parse(typeof(Stats), key, true);
        int valueInt = Util.ToInt(value);
        SetBaseStat(keyStat, valueInt);
    }

    public void LoadBuff(string key, string value){
        Stats keyStat = (Stats) Enum.Parse(typeof(Stats), key, true);
        int valueInt = Util.ToInt(value);
        SetStatBuff(keyStat, valueInt);   
    }

    public void LoadEffect(string key, string value){
        Effects keyEffect = (Effects) Enum.Parse(typeof(Effects), key, true);
        int valueInt = Util.ToInt(value);
        SetEffect(keyEffect, valueInt);
    }

    public void LoadAbility(string key, string value){
        Abilities keyEffect = (Abilities) Enum.Parse(typeof(Abilities), key, true);
        int valueInt = Util.ToInt(value);
        SetAbility(keyEffect, valueInt);
    }

    public void LoadFact(string key, string value){
        Facts keyFact = (Facts) Enum.Parse(typeof(Facts), key, true);
        SetFact(keyFact, value);
    }

    public static Archetypes Archetype(string characterName){
        switch(characterName){
            case "fred":
                return Archetypes.One;
                break;
            case "velma":
                return Archetypes.Two;
                break;
            case "scoob":
                return Archetypes.Three;
                break;
            case "old man rivers":
                return Archetypes.EnemyOne;
                break;
            case "old man jenkins":
                return Archetypes.EnemyTwo;
                break;
        }
        GD.Print("StatsManager.Archetypes: Invalid character name " + characterName);
        return Archetypes.None;
    }

    public static string CharacterName(Archetypes archetype){
        switch(archetype){
            case Archetypes.One:
                return "fred";
                break;
            case Archetypes.Two:
                return "velma";
                break;
            case Archetypes.Three:
                return "scoob";
                break;
            case Archetypes.EnemyOne:
                return "old man rivers";
                break;
            case Archetypes.EnemyTwo:
                return "old man jenkins";
                break;
        }
        return "NULL";
    }

    public string ToString(){
        string ret = "StatsManager:\n";
        ret += "BaseStats: \n";
        foreach(Stats stat in baseStats.Keys){
            ret += "" + stat + ":" + baseStats[stat] + "\n"; 
        }
        return ret;
    }

    public static void StatsTests(){
        
        IcepawsTests();
        FormulaTests();
        DerivedTests();
        ReplenishTest();

        Test.PrintFails();
    }

    public static void IcepawsTests(){
        StatsManager sm = GetTestStatsManager();
        
        Test.Assert(sm.GetStat(Stats.Intelligence) == 5, "Intelligence not set correctly.");
        Test.Assert(sm.GetStat(Stats.Charisma) == 5, "Charisma not set correctly.");
        Test.Assert(sm.GetStat(Stats.Endurance) == 5, "Endurance not set correctly.");
        Test.Assert(sm.GetStat(Stats.Perception) == 5, "Perception not set correctly.");
        Test.Assert(sm.GetStat(Stats.Agility) == 5, "Agility not set correctly.");
        Test.Assert(sm.GetStat(Stats.Willpower) == 5, "Willpower not set correctly.");
        Test.Assert(sm.GetStat(Stats.Strength) == 5, "Strength not set correctly.");
    }

    public static void FormulaTests(){

        int actual, expected;

        actual = HealthMaxFormula(5);
        expected = 100;
        Test.Assert(actual == expected, "HealthmaxFormula got " + actual + " and expected " + expected + ".");

        actual = SlotsMaxFormula(5);
        expected = 3;
        Test.Assert(actual == expected, "SlotsMaxFormula got " + actual + " and expected " + expected + ".");
    }

    public static void DerivedTests(){
        StatsManager sm = GetTestStatsManager();
        
        TestDerived(sm, Stats.HealthMax, 100);
        TestDerived(sm, Stats.SlotsMax, 3);
    }

    public static void TestDerived(StatsManager sm, Stats stat, int expected){
        int actual = sm.GetStat(stat);
        Test.Assert( actual == expected, stat + "derived as " +  actual + " when expecting " + expected + ".");
    }

    public static void ReplenishTest(){
        StatsManager sm = GetTestStatsManager();

        int health = sm.GetStat(Stats.Health);
        Test.Assert(health == 0, "Health should be 0 by default.");

        sm.ReplenishStats();

        health = sm.GetStat(Stats.Health);
        int healthMax = sm.GetStat(Stats.HealthMax);
        Test.Assert(health == healthMax, "Health should be replenished to " + healthMax + " got: " + health);
    }

    public static StatsManager GetTestStatsManager(){
        StatsManager sm = new StatsManager();

        sm.SetBaseStat(Stats.Intelligence, 5);
        sm.SetBaseStat(Stats.Charisma, 5);
        sm.SetBaseStat(Stats.Endurance, 5);
        sm.SetBaseStat(Stats.Perception, 5);
        sm.SetBaseStat(Stats.Agility, 5);
        sm.SetBaseStat(Stats.Willpower, 5);
        sm.SetBaseStat(Stats.Strength, 5);
        return sm;
    }
}
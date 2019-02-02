/*
	The dormant representation of an actor that can be serialized and stuffed into
	sqlite tables.
*/
using Godot;
using System;
using System.Collections.Generic;

public class ActorData {
	public Actor.Brains brain;
	public string name;
	public int id, health, healthMax;
	public Vector3 pos, rot;
	public Inventory inventory;

	// Extra data
	public System.Collections.Generic.Dictionary<string, string> extra;

	public ActorData(){
		id = -1;
		health = healthMax = 0;
		inventory = new Inventory();
		pos = new Vector3();
		rot = new Vector3();
		extra = new System.Collections.Generic.Dictionary<string, string>();
	}

	public override string ToString(){
		int x = (int)pos.x;
		int y = (int)pos.y;
		int z = (int)pos.z;

		string ret = "Actor[" + id + "]:\n";
		
		ret += "\tname: " + name + "\n";
		ret += "\tbrain: " + brain + "\n";
		ret += "\tPos: [" + x + "," + y + "," + z + "] \n";

		return ret;
	}


	// Load this data from an existing stats manager
	public void LoadStats(StatsManager sm){
		health = sm.GetStat(StatsManager.Stats.Health);
		healthMax = sm.GetStat(StatsManager.Stats.HealthMax);
		name = sm.GetFact(StatsManager.Facts.Name);
	}

	// Save this data to an existing stats manager
	public void SaveStats(StatsManager sm){
		sm.SetBaseStat(StatsManager.Stats.Health, health);
		sm.SetBaseStat(StatsManager.Stats.HealthMax, healthMax);
		sm.SetFact(StatsManager.Facts.Name, name);
	}

	// Return a new stats manager containing this data
	public StatsManager GetStats(){
		StatsManager ret = new StatsManager();
		SaveStats(ret);
		return ret;
	}

	public static ActorData FromJson(string json){
		return null;
		//return JsonConvert.DeserializeObject<ActorData>(json);
	}

	public static string ToJson(ActorData dat){
		return "";
		//return JsonConvert.SerializeObject(dat, Formatting.Indented);
	}


	public static ActorData Factory(StatsManager.Archetypes archetype){
		switch(archetype){
			case StatsManager.Archetypes.One:
				return ArchetypeOne();
				break;
			case StatsManager.Archetypes.Two:
				return ArchetypeTwo();
				break;
			case StatsManager.Archetypes.Three:
				return ArchetypeThree();
				break;
			case StatsManager.Archetypes.EnemyOne:
				return ArchetypeEnemyOne();
				break;
			case StatsManager.Archetypes.EnemyTwo:
				return ArchetypeEnemyTwo();
				break;
		}
		GD.Print("ActorData.Factory: How'd you get here?!");
		return null;
	}

	public static ActorData ArchetypeOne(){
		ActorData ret = new ActorData();
		ret.name = "ArchetypeOne";
		ret.brain = Actor.Brains.Player1;
		ret.healthMax = ret.health = 100;
		return ret;
	}

	public static ActorData ArchetypeTwo(){
		ActorData ret = new ActorData();
		ret.name = "ArchetypeTwo";
		ret.brain = Actor.Brains.Player1;
		ret.healthMax = ret.health = 100;
		return ret;
	}

	public static ActorData ArchetypeThree(){
		ActorData ret = new ActorData();
		ret.name = "ArchetypeThree";
		ret.brain = Actor.Brains.Player1;
		ret.healthMax = ret.health = 100;
		return ret;
	}

	public static ActorData ArchetypeEnemyOne(){
		ActorData ret = new ActorData();
		ret.name = "ArchetypeEnemyOne";
		ret.brain = Actor.Brains.Ai;
		ret.healthMax = ret.health = 100;
		return ret;
	}

	public static ActorData ArchetypeEnemyTwo(){
		ActorData ret = new ActorData();
		ret.name = "ArchetypeEnemyTwo";
		ret.brain = Actor.Brains.Ai;
		ret.healthMax = ret.health = 100;
		return ret;
	}
}
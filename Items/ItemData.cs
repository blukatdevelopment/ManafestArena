/* 
	A serializeable storage class for items.
*/
using System;
using System.Collections.Generic;
using Godot;

public class ItemData : IHasInfo {
	
	public int id;
	public Item.Types type;
	public string name;
	public string description;
	public int weight;
	public Vector3 pos, rot;
	public bool held;
	public System.Collections.Generic.Dictionary<string, string> extra;
	public List<int> stack;
	public bool stackable;

	public ItemData(){
		pos = new Vector3();
		rot = new Vector3();

		extra = new System.Collections.Generic.Dictionary<string, string>();
		stack = new List<int>();
	}

	public string GetExtra(string key){
		if(!extra.ContainsKey(key)){
			return "";
		}

		return extra[key];
	}

	public void SetExtra(string key, string val){
		if(extra.ContainsKey(key)){
			extra[key] = val;
			return;
		}

		extra.Add(key, val);
	}

	public string GetInfo(){
		return name;
	}

	public string GetMoreInfo(){
		return description;
	}

	public static ItemData Clone(ItemData original){
		ItemData ret = new ItemData();
		ret.type = original.type;
		ret.name = original.name;
		ret.pos = original.pos;
		ret.rot = original.rot;
		ret.stack = new List<int>(original.stack.ToArray());
    ret.stackable = original.stackable;

		foreach(string key in original.extra.Keys){
			ret.SetExtra(key, original.extra[key]);
		}

		ret.description = original.description;

		return ret;
	}

	public string ToString(){
		string ret = name;
    if(stack.Count > 0){
      int quantity = 1 + stack.Count;
      ret += "(" + quantity + ")";
    }
		return ret;
	}

	public List<int> GetStack(){
    return stack;
  }

  public bool Push(ItemData item){
  	if(!stackable || item.type != this.type || item.name != this.name){
      return false;
    }

    stack.Add(item.id);
    stack.AddRange(item.GetStack());
    return true;
  }

  public bool Push(int id){
    if(!stackable){
      return false;
    }

    stack.Add(id);
    return true;
  }

  public ItemData Pop(int quantity = 1){
    if(stack.Count == 0){
      return null;
    }

    int effectiveQuantity = quantity;
    if(quantity > stack.Count){
      effectiveQuantity = stack.Count + 1;
    }

    ItemData ret = Clone(this);
    ret.stack = new List<int>();

    ret.id = stack[0];
    stack.RemoveAt(0);

    for(int i = 1; i < effectiveQuantity; i++){
      ret.stack.Add(stack[0]);
      stack.RemoveAt(0);
    }

    return ret;
  }

  public int GetWeight(){
    int ret = weight;
    ret += weight * stack.Count;
    return ret;
  }

  public int GetQuantity(){
    return 1 + stack.Count;
  }

  public static ItemData FromJson(string json){
    return null;//JsonConvert.DeserializeObject<ItemData>(json);
  }

  public static string ToJson(ItemData dat){
    return "";//JsonConvert.SerializeObject(dat, Formatting.Indented);
  }


  public static ItemData Factory(string name){
    ItemData ret = new ItemData();
    ret.name = "NULL";
    ret.stackable = false;
    ret.extra.Add("shop_name", name);

    switch(name.ToLower()){
      case "sword":
        ret.name = "sword";
        ret.description = "A little sharp around the edges.";
        break;
      case "magic_rifle":
        ret.name = "Magic Rifle";
        ret.description = "Blast 'em with mana!";
        break;
      case "magic_beans":
        ret.name = "Magic Beans";
        ret.description = "Don't plant these!";
        break;
      case "bow_and_arrows":
        ret.name = "Bow and arrows(6)";
        ret.description = "Fight quietly.";
        break;
      case "magic_talisman":
        ret.name = "Magical Talisman";
        ret.description = "It's pretty cool looking.";
        break;
      case "old_fish":
        ret.name = "Old Fish";
        ret.description = "Smells pretty bad.";
        break;
      case "nutriloaf":
        ret.name = "Nutriloaf";
        ret.description = "Vegetable matter mingling with bread! It's green, mean, and packed with nutrients every growing champion needs!";
        break;
      case "fire_tome":
        ret.name = "Fire Tome";
        ret.description = "Cast more fireballs for less mana!";
        break;
    }

    if(ret.name == "NULL"){
      GD.Print("ItemData.Factory: " + name + " was an invalid name.");
    }

    return ret;
  }
}
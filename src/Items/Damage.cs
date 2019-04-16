/*
	Contains information about the damage an IReceiveDamage will
	accept.
*/

using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;


public class Damage{
  public int health; // HP loss, Negative values heal.
  public int stamina;
  public int mana;
  
	public string sender; // NodePath of actor that is responsible for this damage

    public Damage(int health = 0, string sender = ""){
      this.health = health;
      this.sender = sender;
    }

}

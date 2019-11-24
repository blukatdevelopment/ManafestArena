using System;
using System.Collections.Generic;

public struct ActorRecord {
  string name, description, body, inputController;
  int intelligence, charisma, endurance, perception, agility, willpower;
  List<string> startingGear, abilities;

  public ActorRecord(
    string name,
    string description,
    string body,
    string inputController,
    int intelligence,
    int charisma,
    int endurance,
    int perception,
    int agility,
    int willpower,
    List<string> startingGear,
    List<string> abilities
  ){
    this.name = name;
    this.description = description;
    this.body = body;
    this.inputController = inputController;
    this.intelligence = intelligence;
    this.charisma = charisma;
    this.endurance = endurance;
    this.perception = perception;
    this.agility = agility;
    this.willpower = willpower;
    this.startingGear = startingGear;
    this.abilities = abilities;
  }
}
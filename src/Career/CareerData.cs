// Simple serializeable class meant to store persistent career data
using Godot;
using System;
using System.Collections.Generic;

public class CareerData {
  public string heroName, fileName;
  public List<string> cards, relics, potions, encounters;

  public CareerData(){
    fileName = "";
    heroName = "DebugHeroName";
    cards = new List<string>{"debugcard1", "debugcard2", "debugcard3"};
    relics = new List<string>{"debugrelic1", "debugrelic2", "debugrelic3"};
    potions = new List<string>{"debugpotion1", "debugpotion2", "debugpotion3"};
    encounters = new List<string>{"debugencounter1", "debugencounter2", "debugencounter3"};
  }
}
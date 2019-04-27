/*
  An IStats is the business logic behind RPG elements of an in-game character.
*/
using System.Collections.Generic;

public interface IStats {

  // Use this to determine compatible interactions
  bool HasStat(string stat);
  List<string> GetStatList();

  // Use this for skills, attributes, perks, modifiers, ability levels, status effects, etc, etc
  int GetStat(string stat);
  void SetStat(string stat, int val);

  // Use this for facts such as name, description
  string GetFact(string fact);
  string SetFact(string fact, string val);

  // Use these for worldly interactions
  bool StatCheck(string stat, int difficulty);
  bool ConsumeStat(string stat, int amount); // returns true when successful
  void ReceiveDamage(Damage damage);
  void Update(float delta);
}
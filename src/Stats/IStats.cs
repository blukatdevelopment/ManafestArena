/*
  An IStats is the business logic behind RPG elements of an in-game character.
*/
using System.Collections.Generic;

public interface IStats {

  // Use this to determine compatible interactions
  bool HasStat(string stat); // Differentiate legitimate 0 stats from non-existent ones.
  List<string> GetStatList();

  // Use this for skills, attributes, perks, modifiers, ability levels, status effects, etc, etc
  int GetStat(string stat);
  void SetStat(string stat, int val);

  // Use this for facts such as name, description
  string GetFact(string fact);
  void SetFact(string fact, string val);

  // Use these for worldly interactions
  bool StatCheck(string stat, int difficulty);
  
  /*
    Use ConsumeStat when an ability requires a certain
    ammount of a certain stat to be used. (ie magic using mana)
    will return false instead of creating a deficit
  */
  bool ConsumeStat(string stat, int amount); 

  /*
    The caller doesn't care about the outcome of ReceiveDamage.
  */
  void ReceiveDamage(Damage damage);
  void Update(float delta);
}
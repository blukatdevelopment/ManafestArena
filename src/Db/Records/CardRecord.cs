public struct CardRecord {
  string name, cardClass, icon;
  int damage, uses;

  public CardRecord(string name, string cardClass, string icon, int damage, int uses){
    this.name = name;
    this.cardClass = cardClass;
    this.icon = icon;
    this.damage = damage;
    this.uses = uses;
  }
}
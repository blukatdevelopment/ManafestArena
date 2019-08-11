/*
  An entire cards system crammed into one class.
*/
using Godot;
using System;
using System.Collections.Generic;

public class HandOfCards : Item {
  bool init = false;
  List<string> deck, drawPile, discardPile;
  List<string> handCards;
  List<int> handStacks;
  int selectedCard, crossbowShotsQueued;

  HUDMenu hud;
  IncrementTimer useDelayTimer, // Delay between using cards 
    scrollDelayTimer, //  Delay between switchin cards
    dealCardTimer,  // Delay between dealing each card
    autoCrossbowTimer;

  List<string> dealQueue;

  IStats stats;
  Speaker speaker;
  ProjectileLauncher launcher;
  RaycastDamager raycastWeapon;


  public HandOfCards(List<string> deck){
    launcher = new ProjectileLauncher(this);
    raycastWeapon = new RaycastDamager(this);

    speaker = new Speaker();
    AddChild(speaker);

    this.deck = deck;
    drawPile = new List<string>();
    discardPile = new List<string>();
    
    handCards = new List<string>();
    handStacks = new List<int>();

    useDelayTimer = new IncrementTimer(0.5f);
    scrollDelayTimer = new IncrementTimer(0.3f);
    dealCardTimer = new IncrementTimer(1f);
    autoCrossbowTimer = new IncrementTimer(0.15f);
  }

  public override void Use(MappedInputEvent inputEvent){
    if(stats == null){
      Actor actor = Actor.GetActorFromNode(wielder as Node);
      if(actor != null){
        stats = actor.stats;
      }
    }

    Item.ItemInputs input = (Item.ItemInputs)inputEvent.mappedEventId;
    if(inputEvent.inputType != MappedInputEvent.Inputs.Press){
      return;
    }
    switch(input){
      case Item.ItemInputs.A:
        if(dealQueue != null || !useDelayTimer.CheckTimerReady()){
          GD.Print("Busy or not ready");
          return;
        }
        int staminaCost = CardStamina(handCards[selectedCard]);
        if(stats == null || stats.ConsumeStat("stamina", staminaCost)){          
          PlayCard();
        }
        else{
          GD.Print("Could not consume " + staminaCost + " stamina");
        }
      break;
      case Item.ItemInputs.C:
        if(dealQueue == null){
          DrawHand();
        }
      break;
      case Item.ItemInputs.F:
        if(scrollDelayTimer.CheckTimerReady()){
          NextCard();
        }
      break;
      case Item.ItemInputs.G:
        if(scrollDelayTimer.CheckTimerReady()){
          PreviousCard();
        }
      break;
    }
  }

  public void PlayCard(){
    if(handCards.Count == 0){
      GD.Print("No cards to play.");
      return;
    }

    string card = handCards[selectedCard];
    if(handStacks[selectedCard] > 1){
      handStacks[selectedCard]--;
    }
    else{
      handCards.RemoveAt(selectedCard);
      handStacks.RemoveAt(selectedCard);
      if(selectedCard >= handCards.Count){
        PreviousCard();
      }
    }

    CardEffect(card);

    discardPile.Add(card);

    if(handCards.Count == 0){
      DrawHand();
    }
    UpdateDisplayedCards();
  }

  public void NextCard(){
    selectedCard++;
    if(selectedCard >= handCards.Count){
      selectedCard = 0;
    }
    UpdateDisplayedCards();
  }

  public void PreviousCard(){
    selectedCard--;
    if(selectedCard < 0){
      selectedCard = handCards.Count-1;
      if(selectedCard < 0){
        selectedCard = 0;
      }
    }
    UpdateDisplayedCards();
  }

  public void DrawHand(){
    DiscardHand();
    GD.Print("Drawhand");
    List<string> queue = new List<string>();
    for(int i = 0; i < 3; i++){
      string card = RequestCardFromDrawPile();
      if(card != ""){
        queue.Add(card);
      }
    }
    if(queue.Count > 0){
      dealQueue = queue;
    }
  }

  public void DiscardHand(){
    for(int i = 0; i < handCards.Count; i++){
      for(int j = 0; j < handStacks[i]; j++){
        discardPile.Add(handCards[i]);
      }
    }
    handCards = new List<string>();
    handStacks = new List<int>();
  }

  public string RequestCardFromDrawPile(){
    string ret = "";
    if(drawPile.Count > 0){
      ret = drawPile[0];
      drawPile.RemoveAt(0);
    }
    else if(discardPile.Count > 0){
      drawPile = new List<string>(discardPile);
      drawPile = Shuffle(drawPile);
      discardPile = new List<string>();
      ret = drawPile[0];
      drawPile.RemoveAt(0);
    }

    return ret;
  }

  public override void Update(float delta){
    useDelayTimer.UpdateTimerReady(delta);
    scrollDelayTimer.UpdateTimerReady(delta);
    DealCards(delta);
    if(crossbowShotsQueued > 0 && autoCrossbowTimer.CheckTimer(delta)){
      crossbowShotsQueued--;
      CardEffect("crossbow");
    }
  }

  public override string GetInfo(){
    if(!init){ // Do init here because this is when HUDMenu definitely exists
      init = true;
      hud = Session.session.activeMenu as HUDMenu;
      drawPile = new List<string>(deck);
      drawPile = Shuffle(drawPile);
      UpdateDisplayedCards();
      DrawHand();
    }
    return "Hand of cards";
  }

  public void DealCards(float delta){
    if(dealQueue == null || !dealCardTimer.CheckTimer(delta)){
      return;
    }

    string card = dealQueue[0];
    dealQueue.RemoveAt(0);

    DealSingleCard(card);
    UpdateDisplayedCards();
    if(dealQueue.Count == 0){
      dealQueue = null;
    }
  }

  public void DealSingleCard(string card){
    for(int i = 0; i < handCards.Count; i++){
      if(handCards[i].Equals(card)){
        handStacks[i]++;
        return;
      }
    }

    handCards.Add(card);
    handStacks.Add(1);
  }

  public void UpdateDisplayedCards(){
    List<string> stackedCards = new List<string>();
    for(int i = 0; i < handCards.Count; i++){
      string card = handCards[i];
      if(handStacks[i] > 1){
        card += "(" + handStacks[i] + ")";
      }
      card += "\nCost: " + (CardStamina(handCards[i])/10); 
      if(i == selectedCard){
        card += "\n^^^^^^";
      }
      stackedCards.Add(card);
    }
    hud.UpdateHandOfCards(stackedCards);
    hud.UpdateDiscardPile(discardPile.Count);
    hud.UpdateDrawPile(drawPile.Count);
  }


  public List<string> Shuffle(List<string> cards){
    List<string> ret = new List<string>();
    while(cards.Count > 0){
      int choice = Util.RandInt(0, cards.Count);
      ret.Add(cards[choice]);
      cards.RemoveAt(choice);
    }
    return ret;
  }

  public int CardStamina(string card){
    switch(card){
      case "strike":
        return 250;
      break;
      case "defend":
        return 250;
      break;
      case "crossbow":
        return 250;
      break;
      case "autocrossbow":
        return 250;
      break;
      case "musket":
        return 250;
      break;
    }
    return 0;
  }

  public void CardEffect(string card){
    Damage dmg = new Damage();
    switch(card){
      case "strike":
        dmg.health = 100;
        raycastWeapon.Config(30f, dmg, speaker);
        raycastWeapon.Fire();
      break;
      case "defend":
        stats.ConsumeStat("block", -10);
      break;
      case "crossbow":
        dmg.health = 100;
        launcher.Config(
          ItemFactory.Items.CrossbowBolt,
          dmg,
          50f,
          speaker
        );
        launcher.Fire();
      break;
      case "autocrossbow":
        crossbowShotsQueued = 5;
      break;
      case "musket":
        dmg.health = 150;
        launcher.Config(
          ItemFactory.Items.CrossbowBolt,
          dmg,
          100f,
          speaker
        );
        launcher.Fire();
      break;
    }
  }
}
/*
  An entire cards system crammed into one class.
*/
using Godot;
using System;
using System.Collections.Generic;

public class HandOfCards {
  Actor actor;
  bool init = false;
  List<string> deck;
  List<string> handCards;

  bool handFull = true;
  int selectedCard;
  float drawTimer = 20;

  HUDMenu hud;
  IncrementTimer useDelayTimer, // Delay between using cards 
    scrollDelayTimer, //  Delay between switchin cards
    dealCardTimer,  // Delay between dealing each card
    furyTimer, // Duration of fury buff
    rushTimer; // duration of rush buff

  int fury, rush;

  List<string> dealQueue;

  IStats stats;
  Speaker speaker;
  //ProjectileLauncher launcher;
  //RaycastDamager raycastWeapon;


  public HandOfCards(Actor actor, List<string> deck){

    this.actor  =actor;
    this.deck = deck;
    
    handCards = new List<string>();

    useDelayTimer = new IncrementTimer(0.5f);
    scrollDelayTimer = new IncrementTimer(0.3f);
    dealCardTimer = new IncrementTimer(1f);
    furyTimer = new IncrementTimer(15f);
    rushTimer = new IncrementTimer(15f);
  }

  public void Use(MappedInputEvent inputEvent){
    if(stats == null){
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
        DiscardSelectedCard();
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
    
    handFull = false;
    handCards.RemoveAt(selectedCard);
    if(selectedCard >= handCards.Count){
      PreviousCard();
    }

    CardEffect(card);

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

  public void DrawHand(int num){
    //DiscardHand();
    List<string> queue = new List<string>();
    for(int i = 0; (i < num)&&!handFull; i++){
      string card = GetRandomCardFromDeck();
      handCards.Add(card);
      if(handCards.Count>=5){
          handFull = true;
      }
    }
  }

  public void DiscardSelectedCard(){
    if((selectedCard < handCards.Count)&&(selectedCard>=0)){
      handCards.RemoveAt(selectedCard);
    }
    if(selectedCard>=handCards.Count){
      selectedCard = 0;
    }
    UpdateDisplayedCards();
  }

  public string GetRandomCardFromDeck(){
    return deck[Util.RandInt(0,deck.Count)];
  }

  public void Update(float delta){
    
    if(hud!=null){
      hud.UpdateDrawTime((int)drawTimer);
    }
    if(drawTimer>0){
      drawTimer-=delta;
    }
    else if(!handFull){
      DrawHand(1);
      UpdateDisplayedCards();
      drawTimer = 20;
    }
    useDelayTimer.UpdateTimerReady(delta);
    scrollDelayTimer.UpdateTimerReady(delta);
    if(rush > 0 && rushTimer.CheckTimer(delta)){
      int currentAgility = stats.GetStat("agility");
      stats.SetStat("agility", currentAgility - rush);
      rush = 0;
    }
    if(fury > 0 && furyTimer.CheckTimer(delta)){
      int currentEndurance = stats.GetStat("endurance");
      stats.SetStat("endurance", currentEndurance - fury);
      fury = 0;
    }
  }

  public void ToggleActive(bool active){
    hud.ToggleHandOfCards(active);
  }

  public string GetInfo(){
    if(!init){ // Do init here because this is when HUDMenu definitely exists
      init = true;
      hud = Session.session.activeMenu as HUDMenu;
      handFull = false;
      DrawHand(3);
      ToggleActive(false);
      UpdateDisplayedCards();
    }
    return "Hand of cards";
  }

  public void UpdateDisplayedCards(){
    List<string> stackedCards = new List<string>();
    for(int i = 0; i < handCards.Count; i++){
      string card = handCards[i];
      card += "\nCost: " + (CardStamina(handCards[i])/10); 
      if(i == selectedCard){
        card += "\n^^^^^^";
      }
      stackedCards.Add(card);
    }
    hud.UpdateHandOfCards(stackedCards);
    hud.UpdateDrawTime((int)drawTimer);
  }


  public int CardStamina(string card){
    switch(card){
      case "knife":
        return 250;
      break;
      case "defend":
        return 250;
      break;
      case "crossbow2":
        return 250;
      break;
      case "crossbow5":
        return 250;
      break;
      case "blade":
        return 250;
      break;
      case "rush":
        return 250;
      break;
      case "fury":
        return 250;
      break;
    }
    return 0;
  }

  public void CardEffect(string card){
    Damage dmg = new Damage();
    switch(card){
      case "knife":
        actor.hotbar.SwitchItem(ItemFactory.Factory(ItemFactory.Items.Knife));
        actor.hotbar.handOfCardsActive =false;
        ToggleActive(false);
      break;
      case "defend":
        stats.ConsumeStat("block", -10);
      break;
      case "crossbow2":
        actor.hotbar.SwitchItem(ItemFactory.Factory(ItemFactory.Items.Crossbow2));
        actor.hotbar.handOfCardsActive =false;
        ToggleActive(false);
      break;
      case "crossbow5":
        actor.hotbar.SwitchItem(ItemFactory.Factory(ItemFactory.Items.Crossbow5));
        actor.hotbar.handOfCardsActive =false;
        ToggleActive(false);
      break;
      case "blade":
        actor.hotbar.SwitchItem(ItemFactory.Factory(ItemFactory.Items.Blade));
        actor.hotbar.handOfCardsActive =false;
        ToggleActive(false);

      break;
      case "fury":
        int currentEndurance = stats.GetStat("endurance");
        stats.SetStat("endurance", currentEndurance + 15);
        fury += 15;
      break;
      case "rush":
        int currentAgility = stats.GetStat("agility");
        stats.SetStat("agility", currentAgility + 10);
        rush += 10;
      break;
    }
  }
}
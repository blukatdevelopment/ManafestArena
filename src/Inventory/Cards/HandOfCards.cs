/*
  Represents a hand of cards drawn from a draw pile and discarded into a 
  discard pile.
*/

using System;
using System.Collections.Generic;


public class HandOfCards {
  const int DefaultMaxHandSize = 7;
  int maxHandSize;

  List<string> deck, drawPile, discardpile;
  Dictionary<string, int> handCards;

  public HandOfCards(List<string> deck, int maxHandSize = DefaultMaxHandSize){
    this.deck = new List<string>(deck);
    this.drawPile = new List<string>(deck);
    this.maxHandSize = maxHandSize;
  }

  public void Draw(int cards){
    
  }

  public void Discard(int cards){

  }

  public void DrawHand(){

  }

  public void DiscardHand(){

  }


}
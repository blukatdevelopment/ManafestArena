using System;
using System.Collections.Generic;

public struct PlaylistRecord {
  string name;
  List<string> songs;

  public PlaylistRecord(
    string name,
    List<string> songs
  ){
    this.name = name;
    this.songs = songs;
  }
}
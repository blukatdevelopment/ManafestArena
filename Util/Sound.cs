/* 
  This a centralized storage place for sound effects and music filenames separate
  from business logic.
*/
using Godot;
using System.Collections.Generic;

public class Sound {
  public enum Effects{
    None,
    RifleShot,
    RifleReload,
    FistSwing,
    FistImpact,
    ActorDamage,
    ActorDeath
  };

  public enum Songs{
    None,
    // Main menu music
    Menu1,
    Menu2,
    Menu3,
    Menu4,
    // Arena music
    Arena1,
    Arena2,
    Arena3,
    Arena4,
    Arena5,
    Arena6,
    Arena7,
    Arena8,
    Arena9,
    Arena10,
    Arena11
  };

  public enum Playlists{
    None,
    Menu,
    Arena
  };

  public static string SongFile(Songs song){
    string ret = "";
    switch(song){
      case Songs.Menu1: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/adam_spc.ogg";
        break;
      case Songs.Menu2: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/ssss.ogg";
        break;
      case Songs.Menu3: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/t.ogg";
        break;
      case Songs.Menu4: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/you_will_know.ogg";
        break;
      case Songs.Arena1: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/bowser_nights.ogg";
        break;
      case Songs.Arena2: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/dr.ogg";
        break;
      case Songs.Arena3: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/FDBT.ogg";
        break;
      case Songs.Arena4: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/ffff.ogg";
        break;
      case Songs.Arena5: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/heated_swimmer.ogg";
        break;
      case Songs.Arena6: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/hot_shit.ogg";
        break;
      case Songs.Arena7: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/mbtb_away.ogg";
        break;
      case Songs.Arena8: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/md.ogg";
        break;
      case Songs.Arena9: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/MODR_something_big.ogg";
        break;
      case Songs.Arena10: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/true_fossil_soul_beta.ogg";
        break;
      case Songs.Arena11: 
        ret = "res://Audio/Songs/HallyLabs2014-current-selected-research/you_bet.ogg";
        break;

    }
    return ret;
  }
  
  public static string EffectFile(Effects effect){
    string ret = "";
    switch(effect){
      case Effects.RifleShot:
        ret = "res://Audio/Effects/pew.wav";
        break;
      case Effects.RifleReload:
        ret = "res://Audio/Effects/chtcht.wav";
        break;
      case Effects.FistSwing:
        ret = "res://Audio/Effects/swing.wav";
        break;
      case Effects.FistImpact:
        ret = "res://Audio/Effects/impact.wav";
        break;
      case Effects.ActorDamage:
        ret = "res://Audio/Effects/actor_damage.wav";
        break;
      case Effects.ActorDeath:
        ret = "res://Audio/Effects/actor_die.wav";
        break;
    }
    return ret;
  }

  public static void RefreshVolume(){
    float musicVolume = Sound.VolumeMath(Session.session.musicVolume);
    Session.session.jukeBox.VolumeDb = musicVolume;
  }

  public static float VolumeMath(float val){
    val *= Session.session.masterVolume;
    val *= 100f;
    
    float remainder = 100f - val; // The distance from 100%
    val = 0f - remainder; // volume should be -distance decibals
    return val;
  }

  public static List<Songs> GetPlaylist(Playlists playlist){
    switch(playlist){
      case Playlists.Menu:
        return new List<Songs>{
          Songs.Menu1,
          Songs.Menu2,
          Songs.Menu3,
          Songs.Menu4
        };
        break;
      case Playlists.Arena:
        return new List<Songs>{
          Songs.Arena1,
          Songs.Arena2,
          Songs.Arena3,
          Songs.Arena4,
          Songs.Arena5,
          Songs.Arena6,
          Songs.Arena7,
          Songs.Arena8,
          Songs.Arena9,
          Songs.Arena10,
          Songs.Arena11
        };
    }

    return new List<Songs>();
  }

  public static void PlayRandomSong(List<Songs> playlist){
    if(playlist == null || playlist.Count == 0){
      GD.Print("Tried to play bad playlist");
      return;
    }
    int choice = Util.RandInt(0, playlist.Count);
    Sound.PlaySong(playlist[choice]);
  }
  
  public static void PlaySong(Songs song){
    if(song == Songs.None){
      return;
    }
    
    if(song == Session.session.currentSong){
      GD.Print("Don't restart the current song");
      return;
    }
    GD.Print("Changing " + Session.session.currentSong + " to " + song);

    string fileName = SongFile(song);
    AudioStreamOGGVorbis stream = (AudioStreamOGGVorbis)GD.Load(fileName);
    
    Session.InitJukeBox();
    Session.session.jukeBox.Stream = stream;
    Session.session.jukeBox.Playing = true;
    Session.session.currentSong = song;
  }
  
  public static void PauseSong(){
    if(Session.session.jukeBox == null){
      return;
    }
    Session.session.jukeBox.Playing = false;
  }
}
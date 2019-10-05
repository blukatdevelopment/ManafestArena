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
    ActorDeath,
    Click,
    Coins
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

  public static System.Collections.Generic.Dictionary<string, AudioStreamOGGVorbis> loadedMusic;
  public static System.Collections.Generic.Dictionary<string, AudioStreamSample> loadedEffects;

  public static void LoadSoundFiles(){
    System.Collections.Generic.Dictionary<string, string> music = SoundDb.GetMusic();
    loadedMusic = new System.Collections.Generic.Dictionary<string, AudioStreamOGGVorbis>();
    foreach(string key in music.Keys){
      loadedMusic.Add(key, (AudioStreamOGGVorbis)GD.Load(music[key]));
    }

    System.Collections.Generic.Dictionary<string, string> effects = SoundDb.GetEffects();
    loadedEffects = new System.Collections.Generic.Dictionary<string, AudioStreamSample>();
    foreach(string key in effects.Keys){
      loadedEffects.Add(key, (AudioStreamSample)GD.Load(effects[key]));
    }
  }

  public static AudioStreamSample LoadEffect(Effects effect){
    string effectName = "" + effect;
    if(loadedEffects == null){
      LoadSoundFiles();
    }
    if(!loadedEffects.ContainsKey(effectName)){
      GD.Print("No such sound effect " + effectName);
      return null;
    }
    return loadedEffects[effectName];
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
    List<Songs> ret = new List<Songs>();
    switch(playlist){
      case Playlists.Menu:
        ret = new List<Songs>{
          Songs.Menu1,
          Songs.Menu2,
          Songs.Menu3,
          Songs.Menu4
        };
        break;
      case Playlists.Arena:
        ret = new List<Songs>{
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
        break;
    }

    return ret;
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
    if(song == Songs.None || song == Session.session.currentSong){
      GD.Print("Not playing song " + song);
      return;
    }
    if(loadedMusic == null){
      LoadSoundFiles();
    }
    string songName = "" + song;

    if(!loadedMusic.ContainsKey(songName)){
      GD.Print("Song: " + songName + " does not exist");
      return;
    }
    
    GD.Print("Changing " + Session.session.currentSong + " to " + song);

    AudioStreamOGGVorbis stream = loadedMusic[songName];
    
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

  public static void PlayEffect(Effects effect){
    if(loadedEffects == null){
      LoadSoundFiles();
    }
    string effectName = "" + effect;

    if(!loadedEffects.ContainsKey(effectName)){
      GD.Print("Effect " + effectName + " does not exist");
      return;
    }

    AudioStreamPlayer player = GetSfxPlayer();
    player.VolumeDb = Sound.VolumeMath(Session.session.sfxVolume);
    player.Stream = loadedEffects[effectName];
    player.Play();
  }

  // Need more than one channel active in case there are multiple
  // simultaneous sound effects
  public static AudioStreamPlayer GetSfxPlayer(){
    if(Session.session.sfxPlayers == null){
      List<AudioStreamPlayer> sfxPlayers = new List<AudioStreamPlayer>();
      sfxPlayers.Add(new AudioStreamPlayer());
      Session.session.sfxPlayers = sfxPlayers;
      Session.session.AddChild(sfxPlayers[0]);
      return Session.session.sfxPlayers[0];
    }

    foreach(AudioStreamPlayer player in Session.session.sfxPlayers){
      if(player != null && !player.Playing){
        return player;
      }
    }

    AudioStreamPlayer newPlayer = new AudioStreamPlayer();
    Session.session.sfxPlayers.Add(newPlayer);
    Session.session.AddChild(newPlayer);
    return newPlayer;
  }

}
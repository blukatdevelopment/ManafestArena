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

  public enum Playlists{
    None,
    Menu,
    Arena
  };

  public static Dictionary<string, AudioStreamOGGVorbis> loadedMusic;
  public static Dictionary<string, AudioStreamSample> loadedEffects;
  public static Dictionary<Playlists, PlaylistRecord> loadedPlaylists;

  public static void LoadSoundFiles(){
    loadedMusic = new Dictionary<string, AudioStreamOGGVorbis>();

    foreach(Songrecord song in ConfigsDb.songs){
      loadedMusic.Add(song.title, (AudioStreamOGGVorbis)GD.Load(song.file)));
    }

    loadedEffects = new Dictionary<string, AudioStreamSample>();

    foreach(SoundRecord sound in ConfigsDb.sounds){
      loadedEffects.Add(sound.name, (AudioStreamSample)GD.Load(sound.file));
    }
    
    loadedPlaylists = new Dictionary<Playlists, Playlistrecord>();

    foreach(PlaylistRecord playlist in ConfigsDb.playlists){
      Playlists playlistEnum = Enum.Parse(typeof(Playlists), playlist.name);
      loadedPlaylists.Add(playlistEnum, playlist);
    }
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

  public static void PlayRandomSong(Playlists playlistEnum){
    if(playlistEnum == Playlists.None){
      PauseSong();
      return;
    }

    PlaylistRecord playlist = loadedPlaylists[playlistEnum];

    if(playlist.songs.Count == 0){
      GD.Print("No songs in this playlist");
      return;
    }

    int choice = Util.RandInt(0, playlist.songs.Count);
    Sound.PlaySong(playlist.songs[choice]);
  }
  
  public static void PlaySong(string song){
    if(!loadedMusic.ContainsKey(song)){
      GD.Print("'" + song + "' is an invalid song");
    }

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
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SoundDb {
  const string SFXFile = "Configs/sfx.json";
  const string MusicFile = "Configs/music.json";

  public static Dictionary<string, string> GetEffects(){
    string rawText = System.IO.File.ReadAllText(SFXFile);
    return Util.FromJson<Dictionary<string, string>>(rawText);
  }

  public static Dictionary<string, string> GetMusic(){
    string rawText = System.IO.File.ReadAllText(MusicFile);
    return Util.FromJson<Dictionary<string, string>>(rawText); 
  }
}
/*
  Apparently external libraries are just not an option, so this is neither
  going to be sqlite nor json. Settings will be stored in a csv file.
*/
using Godot;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class SettingsDb{
    const string SavesDirectory = "Saves/";
    const string SettingsPath = "Saves/settings.json";
    System.Collections.Generic.Dictionary<string, string> settings;

    public SettingsDb(){
        settings = new System.Collections.Generic.Dictionary<string, string>();
        FetchSettings();
    }

    public void StoreSettings(){    
        string settingsText = Util.ToJson(settings);

        System.IO.File.WriteAllText(SettingsPath, settingsText);
    }

    public void FetchSettings(){
        if(!System.IO.File.Exists(SavesDirectory)){
            System.IO.Directory.CreateDirectory(SavesDirectory);
        }

        if(!System.IO.File.Exists(SettingsPath)){
            InitSettings();
        }

        string rawText = System.IO.File.ReadAllText(SettingsPath);
        settings = Util.FromJson<System.Collections.Generic.Dictionary<string, string>>(rawText);
    }

    public void InitSettings(){
        settings.Add("master_volume", "1.0");
        settings.Add("sfx_volume", "1.0");
        settings.Add("music_volume", "1.0");
        settings.Add("username", "New Player");
        settings.Add("first_login", DateTime.Today.ToString("MM/dd/yyyy"));
        settings.Add("mouse_sensitivity_x", "1.0");
        settings.Add("mouse_sensitivity_y", "1.0");
        StoreSettings();
    }

    public void StoreSetting(string name, string val){
        if(settings.ContainsKey(name)){
            settings[name] = val;
        }
        else{
            settings.Add(name, val);
        }
        StoreSettings();
    }

    public string SelectSetting(string name){
        if(!settings.ContainsKey(name)){
            return "";
        }
        return settings[name];
    }

    public static bool SaveExists(string saveName){
        return false;
    }

    // Dummy method
    public static List<String> GetAllSaves(string type){
        return new List<string>();
    }
}
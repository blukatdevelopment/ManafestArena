/*
 This class manages a JSON text file to store settings.
 Performance is a tertiary concern at this point, so a settings file is
 read in the constructor, and every save results in saving the entire JSON text
 to disk.
*/
using Godot;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SettingsDb{
    const string SettingsPath = "Saves/settings.json";
    System.Collections.Generic.Dictionary<string, string> settings;

    public SettingsDb(){
        settings = new System.Collections.Generic.Dictionary<string, string>();
        FetchJson();
    }

    public void StoreJson(){
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        System.IO.File.WriteAllText(SettingsPath, json);
    }

    public void FetchJson(){

        if(!System.IO.File.Exists(SettingsPath)){
            InitSettings();
        }

        string json = System.IO.File.ReadAllText(SettingsPath);
        settings = JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, string>>(json);
    }

    public void InitSettings(){
        settings.Add("master_volume", "1.0");
        settings.Add("sfx_volume", "1.0");
        settings.Add("music_volume", "1.0");
        settings.Add("username", "New Player");
        settings.Add("first_login", DateTime.Today.ToString("MM/dd/yyyy"));
        settings.Add("mouse_sensitivity_x", "1.0");
        settings.Add("mouse_sensitivity_y", "1.0");
        StoreJson();
    }

    public void StoreSetting(string name, string val){
        if(settings.ContainsKey(name)){
            settings[name] = val;
        }
        else{
            settings.Add(name, val);
        }
        StoreJson();
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
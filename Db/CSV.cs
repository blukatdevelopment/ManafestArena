/*
  Simple class to retrieve and store contents of CSV files.
*/
using Godot;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CSV {
  System.Collections.Generic.Dictionary<int, List<string>> rows;

  public static System.Collections.Generic.Dictionary<int, string[]> ReadRows(string filePath, bool skipValidation = false){
    if(!System.IO.File.Exists(filePath)){
      return null;
    }

    string rawText = System.IO.File.ReadAllText(filePath);
    System.Collections.Generic.Dictionary<int, string[]> ret;
    ret = ParseRawText(rawText);

    if(skipValidation || ValidateRows(ret)){
      return ret;
    }
    return null;
  }

  public static bool ValidateRows(System.Collections.Generic.Dictionary<int, string[]> rows){
    bool ret = true;
    if(rows == null){
      GD.Print("ValidateRows: Rows dictionary null.");
      return false;
    }

    if(rows.Count < 1){
      GD.Print("ValidateRows: No rows to validate!");
      return false;
    }

    int columnCount = -1;
    foreach(int key in rows.Keys){
      if(columnCount == -1){
        columnCount = rows[key].Length;
      }
      else if(columnCount != rows[key].Length){
        GD.Print("Row " + key + " doesn't match first row column length.");
        ret = false;
      }
    }

    return ret;
  }

  public static System.Collections.Generic.Dictionary<int, string[]> ParseRawText(string rawText){
    string[] lines = rawText.Split(new Char [] {'\n'});

    System.Collections.Generic.Dictionary<int, string[]> ret;
    ret = new System.Collections.Generic.Dictionary<int, string[]>();

    for(int i = 0; i < lines.Length; i++){
      string[] columns = lines[i].Split(new Char [] { ',' });

      ret.Add(i, columns);
    }

    return ret;
  }

  public static void WriteToFile(string filePath, System.Collections.Generic.Dictionary<int, string[]> rows){
    string outputText = OutputText(rows);
    System.IO.File.WriteAllText(filePath, outputText);
  }

  public static string OutputText(System.Collections.Generic.Dictionary<int, string[]> rows){
    string ret = "";

    foreach(int key in rows.Keys){
      int length = rows[key].Length;
      string row = String.Join(",", rows[key], 0, length);
      ret += row + "\n"; 
    }

    ret = ret.Remove(ret.Length -1);

    return ret;
  }

}
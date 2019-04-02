/*
  A collection of static graphics-related methods much like Util.
*/
using Godot;
using System;
using System.Collections.Generic;

public class GFX {

  // For when you want a spatial material that represents a color.
  public static Material GetColorSpatialMaterial(Vector3 color){
    SpatialMaterial ret = new SpatialMaterial();
    
    ret.AlbedoColor = Color(color);
    return ret;
  }

  // Assume a is 1, because why not?
  public static Color Color(Vector3 color){
    return new Color(color.x, color.y, color.z, 1f);
  }
}
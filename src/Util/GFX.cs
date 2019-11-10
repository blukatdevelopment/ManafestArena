/*
  A collection of static graphics-related methods much like Util.
*/
using System;
using Godot;

public class GFX {

  public static Material GetColorSpatialMaterial(Vector3 color){
    SpatialMaterial ret = new SpatialMaterial();
    
    ret.AlbedoColor = Color(color);
    return ret;
  }

  public static Color Color(Vector3 color){
    float absolutelyOpaque = 1f;
    return new Color(color.x, color.y, color.z, absolutelyOpaque);
  }
}
/*
  Menu utilities
*/
using Godot;
using System;

public class Menu{
  
  public static Button Button(Node parent, string text = "", Action onClick = null){
    Button button = Button(text, onClick);
    parent.AddChild(button);
    return button;
  }

  public static Button Button(string text = "", Action onClick = null){
    Button button = new Button();
    
    if(text != ""){ 
      button.SetText(text); 
    }
    if(onClick != null){ 
      button.SetOnClick(onClick); 
    }

    button.AddStyleboxOverride("normal", ColorStyleBox("normal"));
    button.AddStyleboxOverride("pressed", ColorStyleBox("pressed"));
    button.AddStyleboxOverride("hover", ColorStyleBox("hover"));
    button.AddStyleboxOverride("disabled", ColorStyleBox("disabled"));

    button.AddColorOverride("font_color",           GFX.Color(new Vector3()));
    button.AddColorOverride("font_color_pressed",   GFX.Color(new Vector3()));
    button.AddColorOverride("font_color_hover",     GFX.Color(new Vector3()));
    button.AddColorOverride("font_color_disabled",  GFX.Color(new Vector3()));

    return button;
  }

  public static TexturedButton TexturedButton(
    Node parent,
    string normalTexturePath = "",
    string selectTexturePath = "",
    Action onClick = null
  ){
    TexturedButton button = TexturedButton(normalTexturePath, selectTexturePath, onClick);
    parent.AddChild(button);
    return button;    
  }

  public static TexturedButton TexturedButton(
      string normalTexturePath = "",
      string selectTexturePath = "",
      Action onClick = null
  ){
    TexturedButton tb = new TexturedButton();
    
    if(normalTexturePath != ""){ 
      tb.TextureNormal = ResourceLoader.Load(normalTexturePath) as Texture; 
    }

    if(selectTexturePath != ""){ 
      Texture t = ResourceLoader.Load(selectTexturePath) as Texture;
      tb.TextureFocused = t;
      tb.TextureHover = t;
    }

    if(onClick != null){ 
      tb.SetOnClick(onClick); 
    }

    tb.Expand = true;
    return tb;
  }
  
  public static Color BoxColor(string state){
    Vector3 rgb = new Vector3();
    switch(state){
      case "normal":        rgb = new Vector3(0.211f, 0.219f, 0.349f); break;
      case "pressed":       rgb = new Vector3(0.090f, 0.101f, 0.227f); break;
      case "hover":         rgb = new Vector3(0.203f, 0.207f, 0.258f); break;
      case "disabled":      rgb = new Vector3(0.466f, 0.470f, 0.513f); break;
      case "completion":    rgb = new Vector3(0.090f, 0.101f, 0.227f); break;
      case "panel":         rgb = new Vector3(0.466f, 0.470f, 0.513f); break;
      case "focus":         rgb = new Vector3(0.211f, 0.219f, 0.349f); break;
      case "read_only":     rgb = new Vector3(0.211f, 0.219f, 0.349f); break;
      case "slider":        rgb = new Vector3(0.211f, 0.219f, 0.349f); break;
      case "grabber_area":  rgb = new Vector3(0.211f, 0.219f, 0.349f); break;
      case "editable":      rgb = new Vector3(0.466f, 0.470f, 0.513f); break;
      case "background":    rgb = new Vector3(0.086f, 0.086f, 0.172f); break;
    }

    return GFX.Color(rgb);
  }

  public static StyleBox ColorStyleBox(string state){
    return ColorStyleBox(BoxColor(state));
  }

  public static StyleBox ColorStyleBox(Color bgColor){
    StyleBoxFlat box = new StyleBoxFlat();
    box.BgColor = bgColor;
    box.BorderColor = GFX.Color(new Vector3());
    int border = 5;
    box.BorderWidthLeft = border;
    box.BorderWidthRight = border;
    box.BorderWidthTop = border;
    box.BorderWidthBottom = border;
    return box;
  }

  public static TextEdit TextBox(Node parent, string val = "", bool readOnly = true, bool wordWrap = false){
    TextEdit textBox = TextBox(val, readOnly, wordWrap);
    parent.AddChild(textBox);
    return textBox;
  }

  public static TextEdit TextBox(string val = "", bool readOnly = true, bool wordWrap = false){    
    TextEdit textBox = new TextEdit();
    textBox.SetText(val);
    textBox.Readonly = readOnly;
    textBox.AddStyleboxOverride("normal", ColorStyleBox("editable"));
    textBox.AddStyleboxOverride("focus", ColorStyleBox("focus"));
    textBox.AddStyleboxOverride("panel", ColorStyleBox("panel"));
    textBox.AddStyleboxOverride("read_only", ColorStyleBox("read_only"));

    return textBox;
  }

  public static TextEdit BackgroundBox(Node parent){
    TextEdit textBox = BackgroundBox();
    parent.AddChild(textBox);

    return textBox;
  }

  public static TextEdit BackgroundBox(){
    TextEdit ret = TextBox("");
    StyleBox box = ColorStyleBox(BoxColor("background")); 
    ret.AddStyleboxOverride("normal", box);
    ret.AddStyleboxOverride("focus", box);
    ret.AddStyleboxOverride("panel", box);
    ret.AddStyleboxOverride("read_only", box);

    return ret;
  }

  public static HSlider HSlider(Node parent, float min, float max, float val, float step){
    HSlider slider = HSlider(min, max, val, step);
    parent.AddChild(slider);
    return slider;
  }

  public static HSlider HSlider(float min, float max, float val, float step){    
    HSlider slider = new HSlider();
    slider.MinValue = min;
    slider.MaxValue = max;
    slider.Value = val;
    slider.Step = step;
    
    slider.AddIconOverride("grabber", ResourceLoader.Load("res://Assets/Textures/UI/grabber.png") as Texture);
    slider.AddIconOverride("grabber_highlight", ResourceLoader.Load("res://Assets/Textures/UI/grabber_highlight.png") as Texture);

    slider.AddStyleboxOverride("slider", ColorStyleBox("slider"));
    slider.AddStyleboxOverride("focus", ColorStyleBox("focus"));
    slider.AddStyleboxOverride("grabber_area", ColorStyleBox("grabber_area"));

    return slider;
  }

  public static Label Label(Node parent, string text = ""){
    Label label = Label(text);
    parent.AddChild(label);
    return label;
  }

  public static Label Label(string text = ""){
    Label label = new Label();
    label.Text = text;   
    return label;
  }
  
  public static Node MenuFactory(string name){
    object menuObj = Activator.CreateInstance(Type.GetType(name));
    return menuObj as Node;
  }

  public static void ScaleControl(Control control, float width, float height, float x, float y){
    if(control == null){ return; }
    
    control.SetSize(new Vector2(width, height)); 
    control.SetPosition(new Vector2(x, y)); 
  }

  public static void TogglePause(){
    Node menuNode = Session.session.activeMenu;
    HUDMenu hudMenu = menuNode as HUDMenu;
    if(hudMenu != null){
      hudMenu.TogglePause();
    }
    else{
      GD.Print("HUDMenu not active. Doing nothing.");
    }
  }

  public static bool HudActive(){
    Node menuNode = Session.session.activeMenu;
    HUDMenu hudMenu = menuNode as HUDMenu;
    if(hudMenu != null){
      return !hudMenu.Paused();
    }
    return false;
  }

}
using UnityEngine;
using System.Collections;

public class SceneFade : MonoBehaviour {

    //Source video https://www.youtube.com/watch?v=0HwZQt94uHQ

    public Texture2D fadeOutTexture;

    public float fadeSpeed = 0.8f;

    private int drawDepth = -1000; // Textures draw order: a low number means it renders on top
    private float alpha = 1.0f; //The textures alpha
    private int fadeDir = -1; //The direction to fade in

    void OnGUI ()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
    }

    public float BeginFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }

    void OnLevelWasLoaded ()
    {
        BeginFade(-1);
    }

    
}

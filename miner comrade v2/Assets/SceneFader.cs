using UnityEngine;
using System.Collections;

public class SceneFader : MonoBehaviour {

	// Use this for initialization
	public IEnumerator ChangeLevel()
    {
        float fadeTime = GameObject.Find("GameManager").GetComponent<SceneFade>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        Application.LoadLevel(Application.loadedLevel - 1);
    }
}

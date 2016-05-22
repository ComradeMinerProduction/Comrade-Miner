using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour {

	// Use this for initialization
	public IEnumerator ChangeLevel()
    {
        float fadeTime = GameObject.Find("GameManager").GetComponent<SceneFade>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(SceneManager.GetSceneAt(0).ToString());
    }
}

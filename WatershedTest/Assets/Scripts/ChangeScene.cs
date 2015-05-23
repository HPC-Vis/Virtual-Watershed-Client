using UnityEngine;
using UnityEditor;
using System.Collections;

public class ChangeScene : MonoBehaviour {

    public GameObject GUI;
    IEnumerator loadScene(string scene)
    {
        yield return new WaitForSeconds(0);
        Application.LoadLevel(scene);
    }


    public void LoadScene(string scene)
    {
        GUI.SetActive(true);
        //StartCoroutine(loadScene(scene));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
public class EndingCutscene_ver1 : MonoBehaviour
{
    public GameObject endingCanvas;
    public PlayableDirector endingDirector;

    public void PlayEnding()
    {
        endingCanvas.SetActive(true);
        endingDirector.Play();
    }

    private void Awake()
    {
        endingCanvas.SetActive(false);
        endingDirector.stopped += OnEndingStopped;
    }

    private void Oestroy()
    {
        endingDirector.stopped -= OnEndingStopped;        
    }

    private void OnEndingStopped(PlayableDirector obj)
    {
        SceneManager.LoadScene("TestScene");   //切换成下一个场景: TestScene
    }
}

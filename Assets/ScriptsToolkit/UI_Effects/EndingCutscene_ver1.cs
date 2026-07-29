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
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(true);
        }

        if (endingDirector != null)
        {
            endingDirector.Play();
        }
    }

    private void Awake()
    {
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(false);
        }

        if (endingDirector != null)
        {
            endingDirector.stopped += OnEndingStopped;
        }
    }

    private void OnDestroy()
    {
        if (endingDirector != null)
        {
            endingDirector.stopped -= OnEndingStopped;
        }
    }

    private void OnEndingStopped(PlayableDirector obj)
    {
        SceneManager.LoadScene("TestScene");   //切换成下一个场景: TestScene
    }
}

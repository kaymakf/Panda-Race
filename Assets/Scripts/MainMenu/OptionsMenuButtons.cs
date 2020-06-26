using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuButtons : MonoBehaviour
{
    public GameObject MainMenuObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBack()
    {
        gameObject.SetActive(false);
        MainMenuObject.SetActive(true);
    }

    public void Sound()
    {
        //AudioListener.pause = !AudioListener.pause;
        //AudioListener.volume = 0.0;
    }
}

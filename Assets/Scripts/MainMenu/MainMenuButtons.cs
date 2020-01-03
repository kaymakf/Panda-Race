using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject OptionsMenuObject;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnOptions()
    {
        OptionsMenuObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnLogOut()
    {
        SceneManager.LoadScene("Login");

    }

    public void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }
}

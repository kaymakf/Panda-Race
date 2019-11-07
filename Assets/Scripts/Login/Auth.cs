using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Auth : MonoBehaviour
{
    public void GuestLogin()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

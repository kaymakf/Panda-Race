using UnityEngine;
using UnityEngine.SceneManagement;

public class Demo_GenerateLevel : MonoBehaviour
{
    public void RefreshLevel() {
        SceneManager.LoadScene(Application.loadedLevel);
    }
}

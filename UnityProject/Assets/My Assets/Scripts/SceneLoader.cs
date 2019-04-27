// Jack Hamilton        - 100550931
// Daniel MacCormick    - 100580519
// Wen Bo Yu            - 100579309
// Rob Savaglio         - 100591436

// *****
// SceneLoader.cs
// This simple script handles switching between the scenes
// It is really only used in the main menu to load the main scene
// *****

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
         SceneManager.LoadScene(sceneName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
  public static MainUIController Instance { get; private set; }
    
    void Awake() {
        MakeSingleton();
    }

    public void PlayLocalGame() {
        SceneManager.LoadScene("GamePlay");
    }
    
    void MakeSingleton() {
        if (Instance == null) {
            Instance = GetComponent<MainUIController>();
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(Instance.gameObject);
        }
    }
}

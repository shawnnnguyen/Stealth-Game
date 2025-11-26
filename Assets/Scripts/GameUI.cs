using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
   [SerializeField] private GameObject gameLoseUI;
   [SerializeField] private GameObject gameWinUI;

   private bool gameIsOver;

   private void Start()
   {
       Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
       FindObjectOfType<Player>().OnReachFinish += ShowGameWinUI;
   }

   private void Update()
   {
       if (gameIsOver)
       {
           if (Input.GetKeyDown(KeyCode.Space))
           {
               SceneManager.LoadScene(0);
           }
       }
   }
   
   private void ShowGameLoseUI()
   {
        OnGameOverUI(gameLoseUI);
   }

   private void ShowGameWinUI()
   {
       OnGameOverUI(gameWinUI);
   }
   
   private void OnGameOverUI(GameObject gameOverUI)
   {
       gameOverUI.SetActive(true);
       gameIsOver = true;
       Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
       FindObjectOfType<Player>().OnReachFinish -= ShowGameWinUI;
   }
}

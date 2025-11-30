using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
   [SerializeField] private GameObject gameLoseUI;
   [SerializeField] private GameObject gameWinUI;

   private Player _player;
   private bool _gameIsOver;

   private void Awake()
   {
       _player = FindFirstObjectByType<Player>();
   }
   
   private void Start()
   {
       Guard.OnGuardHasSpottedPlayer += ShowGameLoseUI;
       _player.OnReachFinish += ShowGameWinUI;
   }

   private void Update()
   {
       if (_gameIsOver)
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
       _gameIsOver = true;
       Guard.OnGuardHasSpottedPlayer -= ShowGameLoseUI;
       _player.OnReachFinish -= ShowGameWinUI;
   }
}

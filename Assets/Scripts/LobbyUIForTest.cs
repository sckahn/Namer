using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIForTest : MonoBehaviour
{
   public void OnGameStartButton()
   {
      GameManager.getGameManager.ChangeGameState(GameStates.InGame);
   }
}

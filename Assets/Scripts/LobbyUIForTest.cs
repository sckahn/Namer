using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIForTest : MonoBehaviour
{
   public void OnGameStartButton()
   {
      GameManager.GetInstance.ChangeGameState(GameStates.InGame);
   }

   public void OnResetButton() => GameManager.GetInstance.Reset();
}

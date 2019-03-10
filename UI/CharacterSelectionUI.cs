using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionUI : MonoBehaviour
{
    public GameObject menuCanvas;
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            menuCanvas.SetActive(true);
            gameObject.SetActive(false);
        }    
    }

    public void StartGame(int characterIndex)
    {
        SceneController.Instance.SetCharacterIndex(characterIndex);
        SceneController.TransitionToScene("DevRoom");
    }
}

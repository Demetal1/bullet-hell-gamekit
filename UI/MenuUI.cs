using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the menu flow
public class MenuUI : MonoBehaviour
{
    public GameObject characterSelectCanvas;

    public Transform selector;
    public Vector2 offset;
    
    //Start index is 3
    public int m_SelectedIndex = 3;

    void Update()
    {
        if(ScreenFader.IsFading)
            return;

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(m_SelectedIndex == 3)
            {
                m_SelectedIndex = 0;
                selector.transform.position -= 3 * (Vector3)offset;
                return; 
            }

            m_SelectedIndex++;
            selector.transform.position += (Vector3)offset;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(m_SelectedIndex == 0)
            {
                m_SelectedIndex = 3;
                selector.transform.position += 3 * (Vector3)offset;
                return;
            }

            m_SelectedIndex--;
            selector.transform.position -= (Vector3)offset;
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            switch(m_SelectedIndex)
            {
                case 3:
                    characterSelectCanvas.SetActive(true);
                    gameObject.SetActive(false);
                    break;
                case 2:
                    Debug.Log("Options");
                    break;
                case 1:
                    Debug.Log("Credits");
                    break;
                case 0:
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                    break;
            }
        }
    }
}

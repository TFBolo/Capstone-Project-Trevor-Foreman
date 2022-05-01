using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject savesMenu;

    public GameObject optionsPanel;
    public GameObject optionsMovePointLeft;
    public GameObject optionsMovePointRight;

    public GameObject savePanel;
    public GameObject savesMovePointUp;
    public GameObject savesMovePointDown;

    public GameObject continueButton;
    public GameObject newGameButton;

    public GameObject loadGameButton;
    public GameObject loadMovePointLeft;
    public GameObject loadMovePointRight;

    public GameObject optionsButton;
    public GameObject optionsPointLeft;
    public GameObject optionsPointRight;

    public GameObject quitButton;

    public bool optionsOut;
    public bool savesOut;

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        SaveData.instance.hubSpawnPoint = 0;
        if (!PlayerPrefs.HasKey("LastSave"))
        {
            continueButton.SetActive(false);
        }
    }

    private void Update()
    {
        HideSaves();
        HideOptions();
    }

    public void Continue()
    {
        if (PlayerPrefs.GetInt("LastSave") == 1)
        {
            if (SaveSystem.LoadPlayer("Save1") != null)
            {
                SaveData.instance.SetData(SaveSystem.LoadPlayer("Save1"));
                SaveData.instance.LoadInstance();
                SaveData.instance.hubSpawnPoint = 1;
                SceneManager.LoadScene("HubWorld");
            }
        }
        else if (PlayerPrefs.GetInt("LastSave") == 2)
        {
            if (SaveSystem.LoadPlayer("Save2") != null)
            {
                SaveData.instance.SetData(SaveSystem.LoadPlayer("Save2"));
                SaveData.instance.LoadInstance();
                SaveData.instance.hubSpawnPoint = 1;
                SceneManager.LoadScene("HubWorld");
            }
        }
        else if (PlayerPrefs.GetInt("LastSave") == 3)
        {
            if (SaveSystem.LoadPlayer("Save3") != null)
            {
                SaveData.instance.SetData(SaveSystem.LoadPlayer("Save3"));
                SaveData.instance.LoadInstance();
                SaveData.instance.hubSpawnPoint = 1;
                SceneManager.LoadScene("HubWorld");
            }
        }
    }

    public void NewGame()
    {
        // Change save variables
        SceneManager.LoadScene("HubWorld");
    }

    public void LoadGame1()
    {
        if (SaveSystem.LoadPlayer("Save1") != null)
        {
            SaveData.instance.SetData(SaveSystem.LoadPlayer("Save1"));
            SaveData.instance.LoadInstance();
            SaveData.instance.hubSpawnPoint = 1;
            SceneManager.LoadScene("HubWorld");
        }
    }

    public void LoadGame2()
    {
        if (SaveSystem.LoadPlayer("Save2") != null)
        {
            SaveData.instance.SetData(SaveSystem.LoadPlayer("Save2"));
            SaveData.instance.LoadInstance();
            SaveData.instance.hubSpawnPoint = 1;
            SceneManager.LoadScene("HubWorld");
        }
    }

    public void LoadGame3()
    {
        if (SaveSystem.LoadPlayer("Save3") != null)
        {
            SaveData.instance.SetData(SaveSystem.LoadPlayer("Save3"));
            SaveData.instance.LoadInstance();
            SaveData.instance.hubSpawnPoint = 1;
            SceneManager.LoadScene("HubWorld");
        }
    }

    public void Exit()
    {
        Debug.Log("Exit Application");
        Application.Quit();
    }

    public void DisableButtons()
    {
        continueButton.GetComponent<Button>().interactable = false;
        newGameButton.GetComponent<Button>().interactable = false;
        loadGameButton.GetComponent<Button>().interactable = false;
        optionsButton.GetComponent<Button>().interactable = false;
        quitButton.GetComponent<Button>().interactable = false;
    }
    
    public void EnableButtons()
    {
        continueButton.GetComponent<Button>().interactable = true;
        newGameButton.GetComponent<Button>().interactable = true;
        loadGameButton.GetComponent<Button>().interactable = true;
        optionsButton.GetComponent<Button>().interactable = true;
        quitButton.GetComponent<Button>().interactable = true;
    }

    public void OpenSavePanel()
    {
        DisableButtons();
        StartCoroutine("MoveLoadButtonRight");
        StartCoroutine("MoveSavesMenuUp");
    }

    public void OpenOptionsPanel()
    {
        DisableButtons();
        StartCoroutine("MoveOptionsButtonRight");
        StartCoroutine("MoveOptionsMenuLeft");
    }

    private void HideSaves()
    {
        if (Input.GetMouseButton(0) && Vector3.Distance(savesMenu.transform.position, savesMovePointUp.transform.position) < 0.05f && !RectTransformUtility.RectangleContainsScreenPoint(savePanel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        {
            StartCoroutine("MoveSavesMenuDown");
        }
    }

    private void HideOptions()
    {
        if (Input.GetMouseButton(0) && Vector3.Distance(optionsMenu.transform.position, optionsMovePointLeft.transform.position) < 0.05f && !RectTransformUtility.RectangleContainsScreenPoint(optionsPanel.GetComponent<RectTransform>(), Input.mousePosition, Camera.main))
        {
            StartCoroutine("MoveOptionsMenuRight");
        }
    }

    public IEnumerator MoveSavesMenuDown()
    {
        while (Vector3.Distance(savesMenu.transform.position, savesMovePointDown.transform.position) > 0.1f)
        {
            savesMenu.transform.position = Vector3.MoveTowards(savesMenu.transform.position, savesMovePointDown.transform.position, 2f);
            yield return new WaitForSeconds(0.001f);
        }
        while (Vector3.Distance(loadGameButton.transform.position, loadMovePointLeft.transform.position) > 0.1f)
        {
            loadGameButton.transform.position = Vector3.MoveTowards(loadGameButton.transform.position, loadMovePointLeft.transform.position, 0.5f);
            yield return new WaitForSeconds(0.001f);
        }
        EnableButtons();
    }

    public IEnumerator MoveSavesMenuUp()
    {
        while (Vector3.Distance(savesMenu.transform.position, savesMovePointUp.transform.position) > 0.1f)
        {
            savesMenu.transform.position = Vector3.MoveTowards(savesMenu.transform.position, savesMovePointUp.transform.position, 2f);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator MoveOptionsMenuRight()
    {
        while (Vector3.Distance(optionsMenu.transform.position, optionsMovePointRight.transform.position) > 0.1f)
        {
            optionsMenu.transform.position = Vector3.MoveTowards(optionsMenu.transform.position, optionsMovePointRight.transform.position, 2f);
            yield return new WaitForSeconds(0.001f);
        }
        while (Vector3.Distance(optionsButton.transform.position, optionsPointLeft.transform.position) > 0.1f)
        {
            optionsButton.transform.position = Vector3.MoveTowards(optionsButton.transform.position, optionsPointLeft.transform.position, 0.5f);
            yield return new WaitForSeconds(0.001f);
        }
        EnableButtons();
    }

    public IEnumerator MoveOptionsMenuLeft()
    {
        while (Vector3.Distance(optionsMenu.transform.position, optionsMovePointLeft.transform.position) > 0.1f)
        {
            optionsMenu.transform.position = Vector3.MoveTowards(optionsMenu.transform.position, optionsMovePointLeft.transform.position, 2f);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator MoveLoadButtonRight()
    {
        while (Vector3.Distance(loadGameButton.transform.position, loadMovePointRight.transform.position) > 0.1f)
        {
            loadGameButton.transform.position = Vector3.MoveTowards(loadGameButton.transform.position, loadMovePointRight.transform.position, 0.5f);
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator MoveOptionsButtonRight()
    {
        while (Vector3.Distance(optionsButton.transform.position, optionsPointRight.transform.position) > 0.1f)
        {
            optionsButton.transform.position = Vector3.MoveTowards(optionsButton.transform.position, optionsPointRight.transform.position, 0.5f);
            yield return new WaitForSeconds(0.001f);
        }
    }
}

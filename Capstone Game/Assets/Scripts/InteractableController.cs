using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class InteractableController : MonoBehaviour
{
    private bool openMenu = false;
    public bool hasLoaded = false;

    public GameObject pausePanel;
    public Image dashCharge;
    public Image petStamina;

    public TextMeshProUGUI moneyGot;
    private float currentTime = 7f;

    public GameObject inventoryPanel;
    public GameObject dashItem;
    public GameObject moneyItem;
    public GameObject healthItem;

    public ThirdPersonMovement playerInfo;
    public InventoryObject inventory;
    public PetAI petAI;
    public HealthSystem playerHealth;

    public GameObject dialoguePanel;
    public GameObject dialogueName;
    public GameObject dialogueSentence;
    public GameObject button1;
    public GameObject button2;
    public GameObject continueButton;
    public int buttonSetup;

    public GameObject shopPanel;
    public GameObject shopMoney;
    public GameObject itemInfo;
    public GameObject itemName;
    public GameObject itemDescription;
    public GameObject itemCost;
    public List<string> itemNames = new(); //I'll do this better later
    public List<string> itemDescriptions = new(); // I'll fix this as well
    public List<int> itemCosts = new(); // Oh good another thing to fix later
    private int itemChoice;

    public GameObject trainPanel;
    public GameObject trainMoney;
    public GameObject powerCost;
    public GameObject speedCost;
    public GameObject bondCost;
    public GameObject enduranceCost;

    public GameObject powerStat;
    public GameObject speedStat;
    public GameObject bondStat;
    public GameObject enduranceStat;

    public DialogueDatabase dialogues;
    public Queue<string> sentences;

    public List<Vector3> spawnPoints = new();

    private void Start()
    {
        spawnPoints.Add(new Vector3(19f, 2.5f, -40f));
        spawnPoints.Add(new Vector3(19.5f, 9.25f, 48f));
        spawnPoints.Add(new Vector3(184f, -2.5f, 48f));
        if (SaveData.instance.hubSpawnPoint == 0)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoints[0];
            GameObject.FindGameObjectWithTag("Pet").GetComponent<NavMeshAgent>().Warp(spawnPoints[0]);
        }
        else if (SaveData.instance.hubSpawnPoint == 1)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoints[1];
            GameObject.FindGameObjectWithTag("Pet").GetComponent<NavMeshAgent>().Warp(spawnPoints[1]);
        }
        else if (SaveData.instance.hubSpawnPoint == 2)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnPoints[2];
            GameObject.FindGameObjectWithTag("Pet").GetComponent<NavMeshAgent>().Warp(spawnPoints[2]);
        }
        sentences = new Queue<string>();
        petAI = GameObject.FindGameObjectWithTag("Pet").GetComponent<PetAI>();
        playerInfo = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonMovement>();
    }

    private void Update()
    {
        if (SaveData.instance.shouldLoad && !hasLoaded)
        {
            inventory.Load(SaveData.instance.inventory);
            hasLoaded = true;
        }

        if (Input.GetButtonDown("Pause"))
        {
            if (openMenu && pausePanel.activeInHierarchy)
            {
                UnPause();
            }
            else if (!openMenu)
            {
                Time.timeScale = 0;
                pausePanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                openMenu = true;
            }
            
        }
        else if (Input.GetButtonDown("Inventory"))
        {
            if (openMenu && inventoryPanel.activeInHierarchy)
            {
                CloseInventory();
            }
            else if (!openMenu)
            {
                Time.timeScale = 0;
                inventoryPanel.SetActive(true);
                BuildInventory();
                Cursor.lockState = CursorLockMode.None;
                openMenu = true;
            }
            
        }

        if (inventory.invChange || inventory.invLoad)
        {
            for (int i = 0; i < inventory.container.Count; i++)
            {
                if (inventory.container[i].ID == 0)
                {
                    playerInfo.baseDash = 2.5f + ((float)inventory.container[i].amount / 2);
                    playerInfo.dashLimit = playerInfo.baseDash * 2f;
                    playerInfo.dashCharge = playerInfo.baseDash * 0.7f;
                }
                else if (inventory.container[i].ID == 3)
                {
                    int currentHealth = playerHealth.maxHealth;
                    playerHealth.maxHealth = 5 + inventory.container[i].amount;
                    if (inventory.invLoad)
                    {
                        playerHealth.healthPoints = playerHealth.maxHealth;
                    }
                    else if (playerHealth.maxHealth != currentHealth)
                    {
                        playerHealth.healthPoints = playerHealth.maxHealth;
                    }playerHealth.UpdateHealth();
                }
            }
            if (inventory.invLoad)
            {
                petAI.currentStamina = petAI.maxStamina; // Temporary fix
            }
            inventory.invChange = false;
            inventory.invLoad = false;
        }

        if (currentTime < 6f)
        {
            currentTime += Time.deltaTime;
            moneyGot.color = new Color(moneyGot.color.r, moneyGot.color.g, moneyGot.color.b, (1f - (currentTime / 6f)) * 1f);
        }
        else
        {
            moneyGot.color = new Color(moneyGot.color.r, moneyGot.color.g, moneyGot.color.b, 0);
        }
        
        dashCharge.rectTransform.sizeDelta = new Vector2(200 * ((playerInfo.dash - playerInfo.baseDash)/(playerInfo.dashLimit - playerInfo.baseDash)), 40);
        petStamina.rectTransform.sizeDelta = new Vector2(150 * (petAI.currentStamina / petAI.maxStamina), 50);
    }

    public void MoneyAmount(int amount)
    {
        moneyGot.text = "+" + amount.ToString() + " Money";
        currentTime = 0;
        moneyGot.color = new Color(moneyGot.color.r, moneyGot.color.g, moneyGot.color.b, 1);
    }

    public void OpenInteraction(int objectNum)
    {
        if (objectNum == 1 && !openMenu) // go to dungeon from hubworld
        {
            dialoguePanel.SetActive(true);
            openMenu = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;

            sentences.Clear();
            foreach (string sentence in dialogues.dialogues[0].sentences)
            {
                sentences.Enqueue(sentence);
            }
            dialogueName.GetComponent<TextMeshProUGUI>().text = dialogues.dialogues[0].name;
            buttonSetup = 0;
            continueButton.SetActive(true);
            DisplayNextSentence();
        }
        else if (objectNum == 2 && !openMenu) //go to hubworld from dungeon start
        {
            dialoguePanel.SetActive(true);
            openMenu = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;

            sentences.Clear();
            foreach (string sentence in dialogues.dialogues[1].sentences)
            {
                sentences.Enqueue(sentence);
            }
            dialogueName.GetComponent<TextMeshProUGUI>().text = dialogues.dialogues[1].name;
            buttonSetup = 1;
            continueButton.SetActive(true);
            DisplayNextSentence();
        }
        else if (objectNum == 3 && !openMenu)
        {
            dialoguePanel.SetActive(true);
            openMenu = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;

            sentences.Clear();
            foreach (string sentence in dialogues.dialogues[2].sentences)
            {
                sentences.Enqueue(sentence);
            }
            dialogueName.GetComponent<TextMeshProUGUI>().text = dialogues.dialogues[2].name;
            buttonSetup = 2;
            continueButton.SetActive(true);
            DisplayNextSentence();
        }
        else if (objectNum == 4 & !openMenu)
        {
            dialoguePanel.SetActive(true);
            openMenu = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;

            sentences.Clear();
            foreach (string sentence in dialogues.dialogues[3].sentences)
            {
                sentences.Enqueue(sentence);
            }
            dialogueName.GetComponent<TextMeshProUGUI>().text = dialogues.dialogues[3].name;
            buttonSetup = 3;
            continueButton.SetActive(true);
            DisplayNextSentence();
        }
    }

    public void BuildInventory()
    {
        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == 0)
            {
                dashItem.SetActive(true);
                dashItem.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString();
            }
            else if (inventory.container[i].ID == 1)
            {
                moneyItem.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString("n0");
            }
            else if (inventory.container[i].ID == 3)
            {
                healthItem.SetActive(true);
                healthItem.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString();
            }
        }
    }

    public void CloseInventory()
    {
        Time.timeScale = 1;
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        openMenu = false;
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        openMenu = false;
    }

    public void CloseShop()
    {
        Time.timeScale = 1;
        itemInfo.SetActive(false);
        shopPanel.SetActive(false);
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        openMenu = false;
    }

    public void CloseTrain()
    {
        Time.timeScale = 1;
        trainPanel.SetActive(false);
        dialoguePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        openMenu = false;
    }

    public void SaveGame1()
    {
        PlayerPrefs.SetInt("LastSave", 1);
        PetAI petAI = GameObject.FindGameObjectWithTag("Pet").GetComponent<PetAI>();
        SaveSystem.SavePlayer(inventory, petAI, "Save1");
    }

    public void SaveGame2()
    {
        PlayerPrefs.SetInt("LastSave", 2);
        PetAI petAI = GameObject.FindGameObjectWithTag("Pet").GetComponent<PetAI>();
        SaveSystem.SavePlayer(inventory, petAI, "Save2");
    }

    public void SaveGame3()
    {
        PlayerPrefs.SetInt("LastSave", 3);
        PetAI petAI = GameObject.FindGameObjectWithTag("Pet").GetComponent<PetAI>();
        SaveSystem.SavePlayer(inventory, petAI, "Save3");
    }

    public void CheatLevels()
    {
        PetAI petAI = GameObject.FindGameObjectWithTag("Pet").GetComponent<PetAI>();
        petAI.power.AddExperience(500);
        petAI.speed.AddExperience(500);
        petAI.bond.AddExperience(500);
        petAI.endurance.AddExperience(500);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            continueButton.SetActive(false);
            return;
        }

        string nextSentence = sentences.Dequeue();
        dialogueSentence.GetComponent<TextMeshProUGUI>().text = nextSentence;
    }

    public void EndDialogue()
    {
        if (buttonSetup == 0)
        {
            button1.SetActive(true);
            button1.GetComponentInChildren<TextMeshProUGUI>().text = "Go to Dungeon";
            button2.SetActive(true);
            button2.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
        }
        else if (buttonSetup == 1)
        {
            button1.SetActive(true);
            button1.GetComponentInChildren<TextMeshProUGUI>().text = "Return to HubWorld";
            button2.SetActive(true);
            button2.GetComponentInChildren<TextMeshProUGUI>().text = "Cancel";
        }
        else if (buttonSetup == 2)
        {
            shopPanel.SetActive(true);
            for (int i = 0; i < inventory.container.Count; i++)
            {
                if (inventory.container[i].ID == 1)
                {
                    shopMoney.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString("n0");
                }
            }
        }
        else if (buttonSetup == 3)
        {
            trainPanel.SetActive(true);
            for (int i = 0; i < inventory.container.Count; i++)
            {
                if (inventory.container[i].ID == 1)
                {
                    trainMoney.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString("n0");
                }
            }

            CurrentStats();

            powerCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.power.GetLevelNumber() + 5).ToString();
            speedCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.speed.GetLevelNumber() + 5).ToString();
            bondCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.bond.GetLevelNumber() + 5).ToString();
            enduranceCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.endurance.GetLevelNumber() + 5).ToString();
        }
    }

    public void SaveHold()
    {
        PetAI petAI = GameObject.FindGameObjectWithTag("Pet").GetComponent<PetAI>();
        SaveSystem.SavePlayer(inventory, petAI, "SaveHold");
        SaveData.instance.SetData(SaveSystem.LoadPlayer("SaveHold"));
        SaveData.instance.LoadInstance();
    }

    public void DialogueButton1()
    {
        if (buttonSetup == 0)
        {
            SaveHold();
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;

            SaveData.instance.hubSpawnPoint = 3;

            SceneManager.LoadScene("Dungeon");
        }
        else if (buttonSetup == 1)
        {
            SaveHold();
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;

            SaveData.instance.hubSpawnPoint = 2;

            SceneManager.LoadScene("HubWorld");
        }
    }
    

    public void DialogueButton2()
    {
        if (buttonSetup == 0 || buttonSetup == 1)
        {
            button1.SetActive(false);
            button2.SetActive(false);
            Time.timeScale = 1;
            dialoguePanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            openMenu = false;
        }
    }

    public void ShopChoice(int itemNum)
    {
        int itemID = 0;
        if (itemNum == 0)
        {
            itemID = 0;
        }
        else if (itemNum == 1)
        {
            itemID = 3;
        }
        string itemAmount = "1";
        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == itemID)
            {
                if (inventory.container[i].amount >= 5)
                {
                    itemAmount = "MAX";
                }
                else
                {
                    itemAmount = (inventory.container[i].amount + 1).ToString();
                }
            }
        }
        itemInfo.SetActive(true);
        itemName.GetComponent<TextMeshProUGUI>().text = itemNames[itemNum] + " Mk." + itemAmount;
        itemDescription.GetComponent<TextMeshProUGUI>().text = itemDescriptions[itemNum];
        itemCost.GetComponent<TextMeshProUGUI>().text = "COST: " + itemCosts[itemNum].ToString();
        itemChoice = itemNum;
    }

    public void ShopPurchase()
    {
        int playerMoney = 0;
        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == 1)
            {
                playerMoney = inventory.container[i].amount;
            }

        }
        if (itemChoice == 0) // Dash Booster
        {
            int dashAmount = 0;
            for (int i = 0; i < inventory.container.Count; i++)
            {
                if (inventory.container[i].ID == 0)
                {
                    dashAmount = inventory.container[i].amount;
                }

            }
            if (playerMoney >= itemCosts[0] && dashAmount < 5)
            {
                inventory.AddItem(inventory.database.getItem[0], 1);
                inventory.AddItem(inventory.database.getItem[1], -itemCosts[0]);

                ShopChoice(itemChoice);
            }
            else if (dashAmount >= 5)
            {
                itemCost.GetComponent<TextMeshProUGUI>().text = "MAX";
            }
            else 
            {
                itemCost.GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH";
            }
        }
        else if (itemChoice == 1)
        {
            int healthAmount = 0;
            for (int i = 0; i < inventory.container.Count; i++)
            {
                if (inventory.container[i].ID == 3)
                {
                    healthAmount = inventory.container[i].amount;
                }
            }
            if (playerMoney >= itemCosts[1] && healthAmount < 5)
            {
                inventory.AddItem(inventory.database.getItem[3], 1);
                inventory.AddItem(inventory.database.getItem[1], -itemCosts[1]);

                ShopChoice(itemChoice);
            }
            else if (healthAmount >= 5)
            {
                itemCost.GetComponent<TextMeshProUGUI>().text = "MAX";
            }
            else 
            {
                itemCost.GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH";
            }
        }

        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == 1)
            {
                shopMoney.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString("n0");
            }
        }
    }

    public void TrainStat(int statNum)
    {
        int playerMoney = 0;
        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == 1)
            {
                playerMoney = inventory.container[i].amount;
                break;
            }
        }

        if (statNum == 0)
        {
            if (playerMoney >= petAI.power.GetLevelNumber() + 5)
            {
                inventory.AddItem(inventory.database.getItem[1], -(petAI.power.GetLevelNumber() + 5));
                petAI.power.AddExperience(100);
                powerCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.power.GetLevelNumber() + 5).ToString();
            }
            else
            {
                powerCost.GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH";
            }
        }
        else if (statNum == 1)
        {
            if (playerMoney >= petAI.speed.GetLevelNumber() + 5)
            {
                inventory.AddItem(inventory.database.getItem[1], -(petAI.speed.GetLevelNumber() + 5));
                petAI.speed.AddExperience(100);
                speedCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.speed.GetLevelNumber() + 5).ToString();
            }
            else
            {
                speedCost.GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH";
            }
        }
        else if (statNum == 2)
        {
            if (playerMoney >= petAI.bond.GetLevelNumber() + 5)
            {
                inventory.AddItem(inventory.database.getItem[1], -(petAI.bond.GetLevelNumber() + 5));
                petAI.bond.AddExperience(100);
                bondCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.bond.GetLevelNumber() + 5).ToString();
            }
            else
            {
                bondCost.GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH";
            }
        }
        else if (statNum == 3)
        {
            if (playerMoney >= petAI.endurance.GetLevelNumber() + 5)
            {
                inventory.AddItem(inventory.database.getItem[1], -(petAI.endurance.GetLevelNumber() + 5));
                petAI.endurance.AddExperience(100);
                enduranceCost.GetComponent<TextMeshProUGUI>().text = "COST: " + (petAI.endurance.GetLevelNumber() + 5).ToString();
            }
            else
            {
                enduranceCost.GetComponent<TextMeshProUGUI>().text = "NOT ENOUGH";
            }
        }

        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == 1)
            {
                trainMoney.GetComponentInChildren<TextMeshProUGUI>().text = inventory.container[i].amount.ToString("n0");
            }
        }
        CurrentStats();
    }

    public void YouDied()
    {
        int moneyNum = 0;
        for (int i = 0; i < inventory.container.Count; i++)
        {
            if (inventory.container[i].ID == 1)
            {
                moneyNum = Mathf.FloorToInt(inventory.container[i].amount / 4);
                break;
            }
        }

        inventory.AddItem(inventory.database.getItem[1], -moneyNum);

        SaveHold();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;

        SaveData.instance.hubSpawnPoint = 2;

        SceneManager.LoadScene("HubWorld");
    }

    public void CurrentStats()
    {
        powerStat.GetComponent<TextMeshProUGUI>().text = "Power Level: " + petAI.power.GetLevelNumber().ToString();
        speedStat.GetComponent<TextMeshProUGUI>().text = "Speed Level: " + petAI.speed.GetLevelNumber().ToString();
        bondStat.GetComponent<TextMeshProUGUI>().text = "Bond Level: " + petAI.bond.GetLevelNumber().ToString();
        enduranceStat.GetComponent<TextMeshProUGUI>().text = "Endurance Level: " + petAI.endurance.GetLevelNumber().ToString();
    }
}
using System;
using System.Collections;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public int score = 0;
    public bool isGameOver = false;
    
    public float startingTime;
    public float timeLeft = 60f;
    
    public GUIController GUIController;
    public GameObject winScreen;
    public GameObject lossScreen;
    
    public GameObject player;
    public GameObject playerStartingPosition;
    private PlayerController playerScript;

    private PickupItem[] _pickups;
    private TimeBoostItem[] _timeItems;

    private void Start()
    {
        isGameOver = false;
        startingTime = timeLeft;
        winScreen.SetActive(false);
        lossScreen.SetActive(false);
        playerScript = player.GetComponent<PlayerController>();

    }

    void Update()
    {
        //update timer
        //GUIController.UpdateTimer(timeLeft);
        //Update score
        //GUIController.UpdateScore(score);
        //Update Player Health
        GUIController.UpdateHealth();

        if (score >=50)
        {
            WinGame();
        }
        
        if (!isGameOver)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                EndGame();
            }
        }

        if (playerScript.health <= 0)
        {
            EndGame();
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }
    
    public void ResetProgress()
    {
        Debug.Log("RESET!");
        // Reset player's score to 0
        score = 0;
        //GUIController.UpdateScore(score);

        // Reset the timer to the starting time
        timeLeft = startingTime;
        //GUIController.UpdateTimer(timeLeft);

        // Reset player position to starting position.
        player.transform.position = playerStartingPosition.transform.position;
        // Reset player health.
        playerScript.health = playerScript.maxHealth;
        
        //find all pickup objects and adding to array
        _pickups = PickupItem.GetAllInactivePickupItems();
        _timeItems = TimeBoostItem.GetAllInactiveTimeItems();
        //iterate through all pickups and time boost items and set state to active
        foreach (PickupItem pickup in _pickups)
        {
            //Debug.Log((pickup.name));
            pickup.gameObject.SetActive(true);
        }

        foreach (TimeBoostItem timeItem in _timeItems)
        {
            timeItem.gameObject.SetActive(true);
        }

        if (!isGameOver)
        {
            startingTime = timeLeft;
            winScreen.SetActive(false);
            lossScreen.SetActive(false);
        }
    }

    void EndGame()
    {
        isGameOver = true;
        if (isGameOver)
        {
            lossScreen.SetActive(true);
            StartCoroutine(ResetGame());
        }
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(2f);
        isGameOver = false;
        ResetProgress();
    }

    void WinGame()
    {
        isGameOver = true;
        if (isGameOver)
        {
            winScreen.SetActive(true);
        }
    }
}
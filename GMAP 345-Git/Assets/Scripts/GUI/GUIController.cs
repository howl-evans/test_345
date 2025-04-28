using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
   // public TextMeshProUGUI scoreText;
    //public TextMeshProUGUI timerText;
    public Image healthBar;
    
    private GameObject player;
    private PlayerController playerScript;
    private GameManager2 _gameManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        _gameManager = FindObjectOfType<GameManager2>();
    }

    /*
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void UpdateTimer(float time)
    {
        timerText.text = "Time: " + Mathf.Round(time).ToString();
    }
    */
    
    public void UpdateHealth()
    {
        healthBar.fillAmount = playerScript.health / playerScript.maxHealth;
    }

    public void UpdateDashCooldown()
    {
        
    }
}
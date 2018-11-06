using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//spawn players when they join
public class JoinSceneManager : MonoBehaviour
{
    [HideInInspector]
    public int playerControllerNumber = 0;//can only be equal to 2 or 4

    private int playerCount;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Transform[] playerSpawnPoints;

    [SerializeField]
    private GameObject[] joinUIs;

    [SerializeField]
    private Sprite ps4Icon;

    [SerializeField]
    private Sprite xboxIcon;

    [SerializeField]
    private Text countDownText;

    private GameObject playerInstance;
    private MovePlayer moveScript;

    [HideInInspector]
    public List<GameObject> players { get; private set; } // Use this for initialization

    [SerializeField]
    [Tooltip("NekuroRed, then Nekuro, then Napoleon, then NapoleonRed")]
    private RuntimeAnimatorController[] animatorControllers;

    [SerializeField]
    private Text errorText;

    [SerializeField]
    private ReadyText[] readyTextScripts;

    private List<string> joinButtons = new List<string>();
    private bool coroutineStarted;
    private AnimateCountdown animateScript;

    // Use this for initialization
    void Start()
    {
        joinButtons.Add("XboxJump1");
        joinButtons.Add("XboxJump2");
        joinButtons.Add("XboxJump3");
        joinButtons.Add("XboxJump4");
        joinButtons.Add("Ps4Jump1");
        joinButtons.Add("Ps4Jump2");
        joinButtons.Add("Ps4Jump3");
        joinButtons.Add("Ps4Jump4");
        joinButtons.Add("KeyboardJump1");
        joinButtons.Add("KeyboardJump2");

        animateScript = countDownText.GetComponent<AnimateCountdown>();

        players = new List<GameObject>();
        errorText.gameObject.SetActive(false);//should be invisible at start
        countDownText.gameObject.SetActive(false);
        GlobalVars.isJoinScreen = true;//turn this off at the game manager start
          
    }

    // Update is called once per frame
    void Update()
    {
        foreach (string i in joinButtons)//goes through my list of possible buttons that could be connected
        {
            if (Input.GetButtonDown(i))//if it detects one, it needs to spawn a player
            {
                SpawnPlayer(i);
                joinButtons.Remove(i);
                foreach (GameObject g in players)
                {
                    for (int a = 0; a < players.Count; a++)
                    {
                        Physics2D.IgnoreCollision(g.GetComponent<BoxCollider2D>(), players[a].GetComponent<BoxCollider2D>());
                    }
                }
            }
        }

        //check if the countdown needs to be started
        if (CheckIfReady() == true)
        {
            if(!coroutineStarted)
            StartCoroutine(StartCountDown());

            //StartButton.Select();
        }
        if (!CheckIfReady())
        {
            StopCoroutine(StartCountDown());
            ResetCountdown();
        }

    }

    private void SpawnPlayer(string joinName)//use this var in the future to figure out what the player number is
    {
        string tempString = joinName.Substring(joinName.Length - 1, 1);//get the last number from the input string 
        playerControllerNumber = int.Parse(tempString) - 1;//this is now our player number. This is so the joystick number isn't overrriden by the order that a player joins.

        if (!GlobalVars.playerInts.Contains(playerControllerNumber))
        {
            switch (playerControllerNumber)// I think this is a bit more readable if we're going to be editing this
            {
                case 0:
                    playerInstance = Instantiate(playerPrefab, playerSpawnPoints[0].transform.position, playerPrefab.transform.rotation);
                    moveScript = playerInstance.GetComponent<MovePlayer>();
                    moveScript.myTeam = MovePlayer.Team.rightTeam;
                    moveScript.playerNum = 1;
                    break;
                case 1:
                    playerInstance = Instantiate(playerPrefab, playerSpawnPoints[1].transform.position, playerPrefab.transform.rotation);
                    moveScript = playerInstance.GetComponent<MovePlayer>();
                    moveScript.myTeam = MovePlayer.Team.leftTeam;
                    moveScript.playerNum = 2;
                    break;
                case 2:
                    playerInstance = Instantiate(playerPrefab, playerSpawnPoints[2].transform.position, playerPrefab.transform.rotation);
                    moveScript = playerInstance.GetComponent<MovePlayer>();
                    moveScript.myTeam = MovePlayer.Team.rightTeam;
                    moveScript.playerNum = 3;
                    break;
                case 3:
                    playerInstance = Instantiate(playerPrefab, playerSpawnPoints[3].transform.position, playerPrefab.transform.rotation);
                    moveScript = playerInstance.GetComponent<MovePlayer>();
                    moveScript.myTeam = MovePlayer.Team.leftTeam;
                    moveScript.playerNum = 4;
                    break;
            }
            GlobalVars.playerInts.Add(playerControllerNumber);

            //AUDIO player joined in the join screen
            playerInstance.GetComponent<Animator>().runtimeAnimatorController = animatorControllers[playerControllerNumber];

            //set player canvas stuff
            playerInstance.GetComponentInChildren<ImageActive>().xboxIcon = xboxIcon;
            playerInstance.GetComponentInChildren<ImageActive>().ps4Icon = ps4Icon;
            playerInstance.GetComponentInChildren<HealthDotManager>().gameObject.SetActive(false);

            joinUIs[playerControllerNumber].gameObject.SetActive(false);//turn off the join prompt

            //set movement stuff
            moveScript.CheckWhichControllerConnected();//has to call the controller set up here to avoid things getting called out of order
            moveScript.SetInputBasedOnController();

            players.Add(playerInstance);

            readyTextScripts[playerControllerNumber].playerHasJoined = true;//tells the ready scripts that the player has joined
            playerCount++;
        }
    }

    private bool CheckIfReady()
    {
        if (players.Count == 2 || players.Count == 4)//if we have enough players, keep going
        {
            if (errorText.gameObject.activeSelf)//if error was up before, put it down
                errorText.gameObject.SetActive(false);

            foreach (ReadyText r in readyTextScripts)
            {
                if (r.playerHasJoined)//checks if a player has joined if false, don't worry about if they are ready
                {
                    if (r.playerIsReady == false)
                    {
                        errorText.text = "All players must be ready!";
                        errorText.gameObject.SetActive(true);
                        return false;
                    }
                }
            }

            if (errorText.gameObject.activeSelf)//if error was up before, put it down
                errorText.gameObject.SetActive(false);

            return true;
        }
        else//if we don't have enough players, print an appropriate error message and return false;
        {
            errorText.text = "There must be an even number of players!";
            if (!errorText.gameObject.activeSelf)
                errorText.gameObject.SetActive(true);//or else we put up error text
            return false;
        }
    }

    private void ResetCountdown()
    {
        countDownText.gameObject.SetActive(false);
        coroutineStarted = false;
    }

    IEnumerator StartCountDown()
    {
        //this is going to check and make sure it's not being interrupted by new players joining
        //use the fish script for reference.
        coroutineStarted = true;
        countDownText.gameObject.SetActive(true);

        countDownText.text = "3";
        StartCoroutine(animateScript.AnimateCountdownText());
        yield return new WaitForSeconds(1);


        countDownText.text = "2";
        StartCoroutine(animateScript.AnimateCountdownText());
        yield return new WaitForSeconds(1);

        countDownText.text = "1";
        StartCoroutine(animateScript.AnimateCountdownText());
        yield return new WaitForSeconds(1);

        yield return new WaitForSeconds(.2f);//lil buffer time
        SceneManager.LoadScene("MainLevel");
        GlobalVars.numOfControllers = playerCount;//this is where we need to check number of controllers
    }   
}

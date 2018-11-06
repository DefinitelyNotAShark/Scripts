using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//This script does: Shows a text alert with "NEW MISSION: " at the beginning and animates it from off screen right to left on a panel
//This script goes on: The panel with the text child
//This script requires: That it has a text child. Might need to move the numbers in fixed update to fit another size. Works with FadeScript to fade it out once text off screen


public class MissionPanelUI : MonoBehaviour
{
    private GameObject missionPanel;
    private Text missionText;
    private RectTransform textLocation;

    private bool moveText = false;
    private FadeScript fadeScript;

    // Use this for initialization
    void Start()
    {
        missionPanel = this.gameObject;
        missionText = GetComponentInChildren<Text>();
        textLocation = missionText.gameObject.GetComponent<RectTransform>();
        missionPanel.gameObject.SetActive(false);

        fadeScript = this.gameObject.GetComponent<FadeScript>();
    }

    public void StartMission(string missionString)
    {
        missionPanel.gameObject.SetActive(true);
        missionText.text = "New Mission: " + missionString;//set text to current mission

        missionText.gameObject.GetComponent<RectTransform>().position = textLocation.position;//resets text location
        moveText = true;
    }

    private void FixedUpdate()
    {
        if (moveText == true)
        {
            textLocation.position = new Vector2(textLocation.position.x - 120 * Time.deltaTime, textLocation.position.y);
            if (textLocation.position.x <= -100)
            {
                moveText = false;
                fadeScript.FadeOut();
            }
        }
    }
}

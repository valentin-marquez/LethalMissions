using System;
using System.Runtime;
using LethalMissions.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItem : MonoBehaviour
{
    [NonSerialized]
    public MissionType Type;
    public string Name;
    public string Objective;
    public MissionStatus Status;

    private GameObject LeftContainer;
    private GameObject MidleContainer;
    private GameObject RightContainer;

    [NonSerialized]
    private TextMeshProUGUI MissionInfo;
    [NonSerialized]
    private Image CheckMark;

    private Image Icon;
    private void Awake()
    {
        LeftContainer = transform.GetChild(0).gameObject;
        MidleContainer = transform.GetChild(1).gameObject;
        RightContainer = transform.GetChild(2).gameObject;

        MissionInfo = MidleContainer.GetComponentInChildren<TextMeshProUGUI>();
        CheckMark = RightContainer.transform.GetChild(0).GetComponent<Image>();
        Icon = LeftContainer.transform.GetChild(0).GetComponent<Image>();
    }

    public void SetMissionInfo(MissionType mType, string missionName, string missionDescription, MissionStatus missionStatus = MissionStatus.Incomplete, Sprite sprite = null)
    {
        this.Type = mType;
        this.Name = missionName;
        this.Objective = missionDescription;
        MissionInfo.text = missionName + "\n" + missionDescription;
        if (missionStatus == MissionStatus.Complete)
        {
            SetMissionCompleted();
        }
        else
        {
            SetMissionUncompleted();
        }

        if (sprite != null)
        {
            Icon.sprite = sprite;
        }
    }

    // SetMissionStatus

    public void SetMissionStatus(MissionStatus missionStatus)
    {
        if (missionStatus == MissionStatus.Complete)
        {
            SetMissionCompleted();
        }
        else
        {
            SetMissionUncompleted();
        }
    }

    public void SetMissionCompleted()
    {
        Status = MissionStatus.Complete;
        CheckMark.enabled = true;
    }

    public void SetMissionUncompleted()
    {
        Status = MissionStatus.Incomplete;
        CheckMark.enabled = false;
    }


}
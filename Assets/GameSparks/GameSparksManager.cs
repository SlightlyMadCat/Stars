using UnityEngine;
using System.Collections;
using GameSparks.Api;

/// <summary>
/// Created for GameSparks tutorial, October 2015, Sean Durkan
/// This class sets up the GameSparksUnity script with a persistant gameobjec
/// </summary>
using GameSparks.Core;
using System.Linq;
using UnityEngine.UI;

using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;


public class GameSparksManager : MonoBehaviour
{
    /// <summary>The GameSparks Manager singleton</summary>
    private static GameSparksManager instance = null;
    //public Text tokenText;

    public Economics economics;
    public LevelManager levelManager;
    public CloneCenter cloneCenter;
    public UIManager uiManager;

    public GameObject[] playerInTable;
    public Sprite defaultPlayer;
    public Sprite curPlayer;
    public Button board;

    public GameObject loadingText;
    public double minutes;
    public Text totalPlayersNum;

    void Awake()
    {
        /*if (instance == null) // check to see if the instance has a refrence
        {
            instance = this; // if not, give it a refrence to this class...
            DontDestroyOnLoad(this.gameObject); // and make this object persistant as we load new scenes
        }
        else // if we already have a refrence then remove the extra manager from the scene
        {
            Destroy(this.gameObject);
        }*/
        GS.GameSparksAvailable += OnAvailable;

        //board.onClick.AddListener(() => LoadBoard());
    }

    void OnAvailable(bool _true)
    {
        new GameSparks.Api.Requests.DeviceAuthenticationRequest()
            .Send((responses) => {
                if (!responses.HasErrors)
                {
                    Debug.Log("Device Authenticated...");
                    SaveManager.SharedInstance.LoadDataFromCloud();
                }
                else
                {
                    Debug.Log("Error Authenticating Device...");
                }
                // uiManager.CheckFps();
            });
    }

    public void SendDataToCloud()
    {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("SAVE_PLAYER")
            .SetEventAttribute("MONEY", economics.totalMoney.ToString())
            .SetEventAttribute("CLONES", cloneCenter.clonesNum)
            .SetEventAttribute("LEVEL", levelManager.currentLevel)
            .SetEventAttribute("EXIT", System.DateTime.Now.Minute)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("WELL DONE");
                }
                else
                {
                    print(response.Errors);
                    Debug.Log("SOME ERRORS");
                }
            });
    }

    public void LoadDataFromCloud()
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("LOAD_PLAYER").Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Received Player Data From GameSparks...");
                GSData data = response.ScriptData.GetGSData("player_Data");
                print("Player ID: " + data.GetString("playerID"));
                print("Player money: " + data.GetString("playerMoney"));
                print("Player clones: " + data.GetInt("playerClones"));
                print("Player level: " + data.GetInt("playerLevel"));
                print("Player exit time: " + data.GetLong("playerExitTime"));
            }
            else
            {
                Debug.Log("Error Loading Player Data...");
            }
        });
    }

    public void SendBoardToCloud()
    {
        //minutes = SaveManager.SharedInstance.gameLoad.totalInGameMinutes;
        //print(minutes);
        minutes = SaveManager.SharedInstance.SessionTime();

        //count = (1+minutes*0.1f)*(1+level)*(1+totalGold*0.1f)
        minutes = System.Math.Round(minutes, 2);
        float score = (float)(1 + minutes * 2) * (1 + levelManager.currentLevel * 10) * (1 + economics.totalGoldCoins);
        //print((long)score);

        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("SCORE_EVT")
            .SetEventAttribute("SCORE_ATTR", (long)score)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("WELL DONE");
                    LoadBoard();
                }
                else
                {
                    print(response.Errors);
                    Debug.Log("SOME ERRORS");
                }
            });
    }

    public void LoadBoard()
    {
        new GameSparks.Api.Requests.AroundMeLeaderboardRequest()
            .SetEntryCount(7)
            .SetLeaderboardShortCode("HIGH_SCORE_LB")
            .Send((response) => {
                if (!response.HasErrors)
                {
                    //Debug.Log("WELL DONE");
                    loadingText.SetActive(false);

                    GSRequestData parsedJson = new GSRequestData(response.JSONData);
                    for (int i = 0; i < playerInTable.Length; i++)
                    {
                        if (i < parsedJson.GetGSDataList("data").Count)
                        {
                            playerInTable[i].SetActive(true);
                            playerInTable[i].transform.GetChild(0).GetComponent<Text>().text = parsedJson.GetGSDataList("data")[i].GetString("country") + " / " + parsedJson.GetGSDataList("data")[i].GetString("city");
                            if ((double)parsedJson.GetGSDataList("data")[i].GetInt("SCORE_ATTR") > 0)
                                playerInTable[i].transform.GetChild(2).GetComponent<Text>().text = economics.getShortIndex((double)parsedJson.GetGSDataList("data")[i].GetInt("SCORE_ATTR"));
                            else
                                playerInTable[i].transform.GetChild(2).GetComponent<Text>().text = "0";
                            playerInTable[i].transform.GetChild(4).GetComponent<Text>().text = economics.getShortIndex((double)parsedJson.GetGSDataList("data")[i].GetInt("rank"));

                            if (SaveManager.SharedInstance.playerID.text == parsedJson.GetGSDataList("data")[i].GetString("userId"))
                                playerInTable[i].GetComponent<Image>().sprite = curPlayer;
                            else
                                playerInTable[i].GetComponent<Image>().sprite = defaultPlayer;
                        }
                        else
                        {
                            playerInTable[i].SetActive(false);
                        }
                    }
                    //totalPlayersNum.text = economics.getShortIndex(parsedJson.GetGSDataList("data").Count);
                }
                else
                {
                    playerInTable[0].SetActive(false);
                    loadingText.SetActive(true);
                    print(response.Errors);
                    Debug.Log("SOME ERRORS");
                }
            });
    }

    public void ShowLeaderBoard()
    {
        SendBoardToCloud();
        uiManager.ShowLeadScreen(1);
        //LoadBoard();
    }

    public void Start()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;

        Firebase.Messaging.FirebaseMessaging.Subscribe("/topics/example");
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        //tokenText.text = "Received Registration Token: " + token.Token;
        //print("Received Registration Token: " + token.Token);

        new PushRegistrationRequest()
        .SetDeviceOS("FCM")
        .SetPushId(token.Token)
        .Send((response) => {
            string registrationId = response.RegistrationId;
            GSData scriptData = response.ScriptData;

            if (!response.HasErrors)
            {
                // tokenText.text = "ZBS";
                //sendPushNotif();
            }
        });

        print("send");
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        // tokenText.text = "Received a new message from: " + e.Message.From;
        print("Received a new message from: " + e.Message.From);
    }

    public void sendPushNotif()
    {
        new GameSparks.Api.Requests.LogEventRequest()
        .SetEventKey("PUSH_NOTIF")
        .Send((response) =>
        {
            if (!response.HasErrors)
            {
                Debug.Log("WELL DONE");
                //tokenText.text = "send";
            }
            else
            {
                print(response.Errors);
                Debug.Log("SOME ERRORS");
            }
        });
    }
}


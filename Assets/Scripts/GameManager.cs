using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject setStart;
    [SerializeField] private bool tutStartSim = false;
    [SerializeField] private List<GameObject> startPanels;
    [SerializeField] private Sprite rebuildGuildBuilding;
    [SerializeField] private TextAsset nameBlocked;
    
    // 각각의 중요패널들 등록
    [SerializeField] private GameObject showBuildingPanel;
    [SerializeField] private GameObject MakeGuildPanel;
    [SerializeField] private GameObject InGamelLoadingPanel;

    [SerializeField] private string serverAddress = "localhost";

    [SerializeField] private List<GameObject> firstShowObjs;
    private void Awake()
    {
        // 최초 실행시 무조건 보여야 될 오브젝트 보이기.
        foreach (var vObj in firstShowObjs)
        {
            if (vObj != null) vObj.SetActive(value: true);
        }
    }

    public void UserAwake()
    {
        ProgramStart();
    }

    public string GetServerAddress()
    {
        return serverAddress;
    }

    /// <summary>
    /// 기본적으로 프로그램을 시작시 꼭 해야되는 부분.
    /// </summary>
    private void ProgramStart()
    {
        
        Debug.Log("Start Game");

        // 에디터에서만 동작
        // 게임 시작시 튜토리얼 시뮬레이션을 할것인지 확인하는 부분
#if UNITY_EDITOR
        if (tutStartSim == true)
        {
            Debug.Log("Delete the Player Preference");
            PlayerPrefs.DeleteAll();
        }
#endif
        // 프로그램 시작을 튜토리얼로 할지 아닐지를 모르니 우선은 튜토리얼과 스타트 둘다 가린다.
        // 이 곳에는 되도록이면 스타트 시 무조건 하이드 시켜야 되는 것들만 넣도록 한다.
//        foreach (var t in startPanels)
//        {
//            Debug.Log("Initializing Panels");
//            if (t != null) t.SetActive(false);
//        }

        // 튜토리얼 완료여부를 여기서 확인한다. 
        if (PlayerPrefs.HasKey("TutComplete"))
        {
            Debug.Log("Has Key");
            if (PlayerPrefs.GetString("TutComplete") == "No")
            {
                // 튜토리얼 버튼을 보여준다.
                Debug.Log("Tut Incomplete");
                ShowStart(0);
            }
            else
            {
                // 일반 시작 버트을 보여준다.
                Debug.Log("Tut Complete");
                ShowStart(1);
            }
        }
        else
        {
            // 신규 유저 최초 접속 또는 재설치.
            // 튜토리얼 버튼을 보여준다.
            Debug.Log("No Key");
            SetDefaultData();
            ShowStart(0);
        }
    }

    private void SetDefaultData()
    {
        PlayerPrefs.SetString("TutComplete","No");
    }

    /// <summary>
    /// 새로운 시작인지 아니면 이미 게임을 하고 있는지에 따라 다른 인자값을 받아서 처리하는 메소드
    /// </summary>
    /// <param name="panNum"></param>
    private void ShowStart(int panNum)
    {
        Debug.Log(panNum == 0 ? "Tut Start" : "Game Start");
        startPanels[panNum].SetActive(true);    
    }

    /// <summary>
    /// 사용자가 생성한 길드이름의 정상여부 판단
    /// </summary>
    /// <param name="nameText"></param>
    public void CheckTxtAvailable(TextMeshProUGUI nameText)
    {
        string guildName = nameText.text;
        string[] textToWord = guildName.Split(' ');
        string[] filterWord = nameBlocked.text.Split(',');
        foreach (var w in textToWord)
        {
  
            // zero width space 문자 삭제하는 방법 유니코드 \u200B 아스키코드 8203
            string wReplacement = Regex.Replace(w, "\u200B", "");
            
            foreach (var f in filterWord)
            {
                Debug.Log("_Word: "+ w+"::W");
                Debug.Log("_Word: "+ f+"::F");
                
                
                if (String.Equals(wReplacement, f))
                {
                    Debug.Log("허용되지 않는 이름입니다. 길드 이름을 다시 지정하세요.");
                    return;
                }
                else
                {
                    Debug.Log("허용되는 이름입니다.");
                    continue;
                }
            }
        }

        // foreach문을 모두 통과했다는 것은 문제되는 단어가 없다는 뜻이니 다음 함수를 실행하여 길드 생성을 완료한다.
        MakeGuild(guildName);
    }
    
    /// <summary>
    /// 정상적인 이름을 사용했을 때 이곳으로 온다.
    /// </summary>
    /// <param name="txt"></param>
    private void MakeGuild(string txt)
    {
        Debug.Log("길드 이름 만들기 성공");
        
        // 길드 만들기 패널을 숨긴다.
        // 서버에 생성된 길드(사용자)를 저장하는 함수를 실행한다.
        SendGuildNameToServerAndSave();

        // 인게임로딩패널을 보인다.
        InGamelLoadingPanel.SetActive(true);
        // 메이크길드패널을 가린다.
        MakeGuildPanel.SetActive(false);
        
    }


    private void SendGuildNameToServerAndSave()
    {
        Debug.Log("길드 생성이 완료 되었음을 서버에 전달하고 서버에서 사용자의 정보를 생성 저장후 고유식별코드를 서버에서 생성 후 클라이언트로 전달한다.");
        // 이 곳에서 서버에 정보 전달과 생성된 사용자 고유식별 코드를 받아서 적용하는 코드를 작성한다.
    }

    /// <summary>
    /// 폐허가 된 길드 건물을 클릭했을때 해야될 작업
    /// 1. 이미지 변경
    /// 2. 튜토리얼 완료 표시
    /// </summary>
    public void ChangeImage(Image img)
    {
        img.sprite = rebuildGuildBuilding;
        img.SetNativeSize();
    }
    
    public  void ShowMakeGuildPanel()
    {
        showBuildingPanel.SetActive(false);
        MakeGuildPanel.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// 이 클래스는 실제 게임의 구성이라기보다는 최초 실행시 보이는 화면의 구성을 만드는 것이고
/// 최초 프로그램이 실행될 때 빼고는 더이상 나오는 않는 부분이기에 서버에서 다운로드가 끝나면
/// 바로 디스트로이하는 구조를 가진다.
/// 이후 실제 게임을 총괄 컨트롤하는 것은 GameManager가 된다.
/// </summary>
public class IntroImagesControl : MonoBehaviour
{
    [SerializeField] private List<Sprite> introImages;
    [SerializeField] private Image introImagePanel;

    private float _spendTime, _delayTime;
    private int _introNumber =0;

    [SerializeField] private float splashTime = 2f;
    [SerializeField]private float defaultDelayTime = 3f;

    [SerializeField] private bool isServer = false;
    [SerializeField] private Slider dnSlider;

    private Action _upAction;

    [SerializeField] private GameManager _gameManager;
    
    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _delayTime = _spendTime + defaultDelayTime;
        introImagePanel.sprite = introImages[_introNumber];
        introImagePanel.color = Color.white;
        if (isServer == true) DnDataFromServer();
        else _upAction = FakeDownload;
    }

    private void Update()
    {
        //지정된 시간이 지났으니 메서드를 호출하여 화면을 바꾸고 시간을 초기화한다.
        if (_spendTime > _delayTime) IncreaseIntroNumber();
        else _spendTime += Time.deltaTime;

        _upAction?.Invoke();
    }

    
    // 외부에서 클릭하여 화면이 바뀌어야 할때 필요하여 메서드로 분리.
    public void IncreaseIntroNumber()
    {
        _spendTime = 0f;
        _delayTime = defaultDelayTime;
        _introNumber = ++_introNumber % introImages.Count;
        introImagePanel.sprite = introImages[_introNumber];
    }

    private void FakeDownload()
    {
        dnSlider.value += Time.deltaTime * 0.2f;
        if (dnSlider.value >= 1f)
        {
            _gameManager.UserAwake();
            DestroyImmediate(this.gameObject);
        }
    }

    /// <summary>
    /// 서버에서 데이터를 이곳으로 보낼 수 있도록 한다.
    /// </summary>
    public void DnDataFromServer()
    {
        string serverAddress = _gameManager.GetServerAddress();
        // 서버 주소를 받아왔으니 이 것을 이용하여 서버랑 통신으로 데이터를 받아온다.
        // 차후 서버가 구성되면 만들어야 될 부분.
    }
}

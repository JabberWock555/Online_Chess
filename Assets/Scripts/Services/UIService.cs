using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIService : MonoSingleton < UIService >
{
    private int inGameMenuID;
    private int onlineMenuID;
    private int hostMenuID;
    private int startMenuID;

    [SerializeField] private GameObject[] cameraAngles; 
    [SerializeField] private InputField addressInput;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    private Animator animator;
    public Action<bool> SetLocalGame;

    private void Start() {
        AssignAnimations();
        animator = GetComponent<Animator>();
        RegisterToEvent();
    }

    private void AssignAnimations()
    {
        inGameMenuID = Animator.StringToHash("InGameMenu");
        onlineMenuID = Animator.StringToHash("OnlineMenu");
        hostMenuID = Animator.StringToHash("HostMenu");
        startMenuID = Animator.StringToHash("StartMenu");
    }

    private void RegisterToEvent(){
        NetUtility.C_START_GAME += OnStartGame_Client;
    }

    private void UnregisterToEvents(){
        NetUtility.C_START_GAME += OnStartGame_Client;
    }

    private void OnStartGame_Client(NetMessage message)
    {
        animator.SetTrigger(inGameMenuID);
    }

    public void ChangeCamera(ChessTeam index)
    {
        foreach(GameObject cam in cameraAngles)
            cam.SetActive(false);

        cameraAngles[(int)index].SetActive(true);
    }

    public void OnLocalGameButton() 
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007); 
        SetLocalGame?.Invoke(true);
        animator.SetTrigger(inGameMenuID); 
    }

    public void OnOnlineGameButton() 
    {
        SetLocalGame?.Invoke(false);
        animator.SetTrigger(onlineMenuID); 
    }

    public void OnOnlineHostButton() 
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);

        animator.SetTrigger(hostMenuID); 
    }

    public void OnOnlineConnectButton() 
    {
        client.Init(addressInput.text, 8007);
    }

    public void OnOnlineBackButton() {  animator.SetTrigger(startMenuID); }

    public void OnHostBackButton() 
    {
        server.ShutDown();
        client.ShutDown();  
        animator.SetTrigger(onlineMenuID); 
    }

    public void OnLeaveFromGameButton()
    {
        ChangeCamera(ChessTeam.NONE);
        animator.SetTrigger(startMenuID);

    }

    private void OnDestroy() {
        UnregisterToEvents();
    }
}

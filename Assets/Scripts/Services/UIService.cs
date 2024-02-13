using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIService : MonoSingleton < UIService >
{
    [SerializeField] private GameObject[] cameraAngles; 
    [SerializeField] private InputField addressInput;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    private Animator animator;
    public Action<bool> SetLocalGame;

    private void Start() {
        animator = GetComponent<Animator>();
        RegisterToEvent();
    }

    private void RegisterToEvent(){
        NetUtility.C_START_GAME += OnStartGame_Client;
    }

    private void UnregisterToEvents(){
        NetUtility.C_START_GAME += OnStartGame_Client;
    }

    private void OnStartGame_Client(NetMessage message)
    {
        animator.SetTrigger("InGameMenu");
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
        animator.SetTrigger("InGameMenu"); 
    }

    public void OnOnlineGameButton() 
    {
        SetLocalGame?.Invoke(false);
        animator.SetTrigger("OnlineMenu"); 
    }

    public void OnOnlineHostButton() 
    {
        server.Init(8007);
        client.Init("127.0.0.1", 8007);

        animator.SetTrigger("HostMenu"); 
    }

    public void OnOnlineConnectButton() 
    {
        client.Init(addressInput.text, 8007);
    }

    public void OnOnlineBackButton() {  animator.SetTrigger("StartMenu"); }

    public void OnHostBackButton() 
    {
        server.ShutDown();
        client.ShutDown();  
        animator.SetTrigger("OnlineMenu"); 
    }

    private void OnDestroy() {
        UnregisterToEvents();
    }
}

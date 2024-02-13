using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoSingleton < Client>
{
   
    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;

    public Action connectionDropped;


// --------- Methods
    public void Init(string ip, ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);

        connection = driver.Connect(endpoint);

        Debug.Log("Attempting to connect to Server on " + endpoint.Address);

        isActive = true;

        RegisterToEvent();
    }

    public void ShutDown(){
        if(isActive){
            UnregisterToEvent();
            driver.Dispose();
            connection = default(NetworkConnection);
            isActive = false;
        }
    }

    private void OnDestroy() 
    {
        ShutDown();
    }

    private void Update() { 
       
        if(!isActive)
            return;

        driver.ScheduleUpdate().Complete();

        CheckAlive();

        UpdateMessagePumpe();


    }

    private void CheckAlive()
    {
        if(!connection.IsCreated && isActive)
        {
            Debug.Log("Something went wrong, lost connection to server");
            connectionDropped?.Invoke();
            ShutDown();
        }
    }

    private void UpdateMessagePumpe()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                SendToServer(new NetWelcome());
                Debug.Log("We are Connected!");
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, connection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server! ");
                connection = default(NetworkConnection);
                connectionDropped?.Invoke();
                ShutDown();   // This does not happen usually just for 2 player game.
            }
        }
    }


    // ---- Client Specific

    private void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer );
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    // --- Event Parsing

    private void RegisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE += OnKeepAlive;
    }

    private void UnregisterToEvent()
    {
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
    }

    private void OnKeepAlive(NetMessage message)
    {
        //Send it back, to keep both side alive
        SendToServer(message);
    }


}

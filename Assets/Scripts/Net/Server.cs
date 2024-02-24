using System;
using System.Security.Cryptography;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

public class Server : MonoSingleton < Server >
{
    public NetworkDriver driver;
    private NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastkeepAlive;

    public Action connectionDropped;

// --------- Methods
    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
        endpoint.Port = port;

        if(driver.Bind(endpoint) != 0)
        {
            Debug.Log("Unable to bind on port " + endpoint.Port);
            return;
        }
        else
        {
            driver.Listen();
            Debug.Log("Currently listening to port " + endpoint.Port);
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
    }
    
    public void ShutDown(){
        if(isActive){
            driver.Dispose();
            connections.Dispose();
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
        
        KeepAlive();

        driver.ScheduleUpdate().Complete();

        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePumpe();


    }

    private void KeepAlive()
    {
        if(Time.time - lastkeepAlive > keepAliveTickRate)
        {
            lastkeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }

    private void CleanupConnections()
    {
        for(int i = 0; i < connections.Length; i++)
        {
            if(!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    private void AcceptNewConnections()
    {
        //Accepts New Connections
        NetworkConnection c;
        while((c = driver.Accept()) != default(NetworkConnection)){
            connections.Add(c);
        }
    }

    private void UpdateMessagePumpe()
    {
        DataStreamReader stream;
        for(int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, connections[i], this);
                }
                else if( cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server! ");
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    ShutDown();   // This does not happen usually just for 2 player game.
                }
            }
        }
    }

    #region  //  ------ Sever Specific

        public void SendToClient(NetworkConnection connection, NetMessage msg)
        {
            DataStreamWriter writer;
            driver.BeginSend(connection, out writer );
            msg.Serialize(ref writer);
            driver.EndSend(writer);
        }

        public void Broadcast(NetMessage msg)
        {
            for(int i = 0; i < connections.Length; i++){
                if(connections[i].IsCreated)
                {
                    //Debug.Log($"Sending {msg.Code} to : {connections[i].InternalId}");
                    SendToClient(connections[i], msg);
                }
            }
        }
    #endregion

}

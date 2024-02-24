using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetRematch : NetMessage
{
    public int teamID;
    public byte wantRematch;
    
    public NetRematch(){
        Code = OpCode.REMATCH;
    }

    public NetRematch(DataStreamReader reader){
        Code = OpCode.REMATCH;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte) Code);
        writer.WriteInt(teamID);    
        writer.WriteByte(wantRematch);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        //  We already read the byte in the NetUtility :: OnDate
        teamID = reader.ReadInt();   
        wantRematch = reader.ReadByte();
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection connection)
    {
        NetUtility.S_REMATCH?.Invoke(this, connection);
    }

} 

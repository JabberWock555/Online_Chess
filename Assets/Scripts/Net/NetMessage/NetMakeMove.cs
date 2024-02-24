
using Unity.Collections;
using Unity.Networking.Transport;

public class NetMakeMove : NetMessage
{
    public int originalX;
    public int originalY;
    public int destinationX;
    public int destinationY;
    public int teamID;

    public NetMakeMove() // <------- Making the Message
    {
        Code = OpCode.MAKE_MOVE;
    }

    public NetMakeMove( DataStreamReader reader) // <--- Recieveing the mesaage
    {
        Code = OpCode.MAKE_MOVE;
        Deserialize(reader);

    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);

        writer.WriteInt(originalX);
        writer.WriteInt(originalY);
        writer.WriteInt(destinationX);
        writer.WriteInt(destinationY);
        writer.WriteInt(teamID);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        originalX = reader.ReadInt();
        originalY = reader.ReadInt();
        destinationX = reader.ReadInt();
        destinationY = reader.ReadInt();
        teamID = reader.ReadInt();
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection connection)
    {
        NetUtility.S_MAKE_MOVE?.Invoke(this, connection);
    }

}

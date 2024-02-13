
using Unity.Collections;
using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage
{
    public NetKeepAlive() // <------- Making the Message
    {
        Code = OpCode.KEEP_ALIVE;
    }
    public NetKeepAlive( DataStreamReader reader) // <--- Recieveing the mesaage
    {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);

    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        base.Serialize(ref writer);
    }

    public override void Deserialize(DataStreamReader reader)
    {
    }

    public override void RecievedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void RecievedOnServer(NetworkConnection connection)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, connection);
    }

}

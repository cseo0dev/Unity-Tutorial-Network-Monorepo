using System;
using System.Text;

public class Packet
{
    public byte[] buffer { get; private set; }
    public int position { get; private set; }

    public Packet()
    {
        this.buffer = new byte[4096];
        this.position = Defines.HEADERSIZE;
    }

    public Packet(byte[] buffer)
    {
        this.buffer = buffer;
        this.position = Defines.HEADERSIZE;
    }

    public Packet(PROTOCOL protocol) : this()
    {
        Push((short)protocol);
    }

    public void RecordSize()
    {
        short body_size = (short)(this.position - Defines.HEADERSIZE);
        byte[] header = BitConverter.GetBytes(body_size);
        header.CopyTo(this.buffer, 0);
    }

    public void Push(short value)
    {
        byte[] data = BitConverter.GetBytes(value);
        data.CopyTo(this.buffer, this.position);
        this.position += data.Length;
    }

    public void Push(string value)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);
        data.CopyTo(this.buffer, this.position);
        this.position += data.Length;
    }

    public void Push(byte value)
    {
        this.buffer[this.position] = value;
        this.position++;
    }

    public void Push(byte[] value)
    {
        value.CopyTo(this.buffer, this.position);
        this.position += value.Length;
    }

    public short PopShort()
    {
        short value = BitConverter.ToInt16(this.buffer, this.position);
        this.position += 2;
        return value;
    }

    public byte PopByte()
    {
        byte value = this.buffer[this.position];
        this.position++;
        return value;
    }
}
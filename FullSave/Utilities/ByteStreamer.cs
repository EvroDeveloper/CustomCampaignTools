using System;
using UnityEngine;

namespace FullSave.Utilities;

public class ByteStreamer
{
    private List<byte> _bytes = [];
    private int _readerIndex;

    public ByteStreamer()
    {
        
    }

    public ByteStreamer(byte[] bytes)
    {
        _bytes.AddRange(bytes);
    }

    private byte[] ReadData(int length)
    {
        if(_readerIndex + length > _bytes.Count)
            throw new IndexOutOfRangeException("you tried to read more than what was there. fuck you");
        
        byte[] output = new byte[length];
        for(int i = 0; i < length; i++)
        {
            output[i] = _bytes[_readerIndex+i];
        }
        _readerIndex += length;

        return output;
    }

    private void WriteData(byte[] bytes)
    {
        _bytes.AddRange(bytes);
    }

    public void WriteBool(bool value)
    {
        WriteData(BitConverter.GetBytes(value));
    }

    public bool ReadBool()
    {
        byte[] nextByte = ReadData(sizeof(bool)); // 1 byte
        return BitConverter.ToBoolean(nextByte);
    }

    public void WriteInt(int value)
    {
        WriteData(BitConverter.GetBytes(value));
    }

    public int ReadInt()
    {
        byte[] nextBytes = ReadData(sizeof(int)); // 4 bytes
        return BitConverter.ToInt32(nextBytes);
    }

    public void WriteFloat(float value)
    {
        WriteData(BitConverter.GetBytes(value));
    }

    public float ReadFloat()
    {
        byte[] nextBytes = ReadData(sizeof(float)); // 4 bytes
        return BitConverter.ToSingle(nextBytes);
    }

    public void WriteChar(char value)
    {
        WriteData(BitConverter.GetBytes(value));
    }

    public char ReadChar()
    {
        byte[] nextBytes = ReadData(sizeof(char)); // 2 bytes
        return BitConverter.ToChar(nextBytes);
    }

    public void WriteString(string value) // oh boy this will be fun. write int for length, then write length number of chars
    {
        WriteInt(value.Length);
        foreach(char c in value)
        {
            WriteChar(c);
        }
    }

    public string ReadString()
    {
        string outputString = "";
        int stringLength = ReadInt();
        for(int i = 0; i < stringLength; i++)
        {
            outputString += ReadChar();
        }
        return outputString;
    }

    public void WriteVector3(Vector3 value)
    {
        WriteFloat(value.x);
        WriteFloat(value.y);
        WriteFloat(value.z);
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
    }

    public byte[] ToByteArray()
    {
        return [.. _bytes];
    }
}

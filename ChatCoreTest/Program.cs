using System;
using System.Text;

namespace ChatCoreTest
{
  internal class Program
  {
    private static byte[] m_PacketData;
    private static uint m_Pos;
    private static int intData;
    private static float fData;
    private static string stringData;
    private static int stringLenght;
    public static void Main(string[] args)
    {
      m_PacketData = new byte[1024];
      m_Pos = 0;

      Write(109);
      Write(109.99f);
      Write("Hello!");

      AddLen();
      Console.WriteLine($"Output Byte array(length:{m_Pos}): ");
      for (var i = 0; i < m_Pos; i++)
      {
          Console.Write(m_PacketData[i] + ", ");
      }

      Receive();
      Console.WriteLine("");
      Console.WriteLine(intData);
      Console.WriteLine(fData);
      Console.WriteLine(stringData);
    }

    // write an integer into a byte array
    private static bool Write(int i)
    {
      // convert int to byte array
      var bytes = BitConverter.GetBytes(i);
      _Write(bytes);
      return true;
    }

    // write a float into a byte array
    private static bool Write(float f)
    {
        // convert int to byte array
        var bytes = BitConverter.GetBytes(f);
        _Write(bytes);
        return true;
    }

    // write a string into a byte array
    private static bool Write(string s)
    {
      // convert string to byte array
      var bytes = Encoding.Unicode.GetBytes(s);

      // write byte array length to packet's byte array
      if (Write(bytes.Length) == false)
      {
        return false;
      }
      _Write(bytes);
      return true;
    }

    // write a byte array into packet's byte array
    private static void _Write(byte[] byteData)
    {
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(byteData);
        }

        byteData.CopyTo(m_PacketData, m_Pos);
        m_Pos += (uint)byteData.Length;
    }
    private static void Receive()
    {
        byte[] readInt;
        byte[] readFloat;
        byte[] stringLen;
        byte[] readString;
        int len = Convert.ToInt32(m_Pos);
        byte[] bData = new byte[len];

        readInt = Read(m_PacketData, 5, 8);
        readFloat = Read(m_PacketData, 9, 12);
        stringLen = Read(m_PacketData, 13, 16);

        Array.Reverse(readInt, 0, readInt.Length);
        Array.Reverse(readFloat, 0, readFloat.Length);
        Array.Reverse(stringLen, 0, stringLen.Length);
            
        intData = BitConverter.ToInt32(readInt, 0);
        fData = BitConverter.ToSingle(readFloat, 0);
        stringLenght = BitConverter.ToInt32(stringLen, 0);
        readString = Read(m_PacketData, 17, 17+ stringLenght-1);
        Array.Reverse(readString, 0, readString.Length);
        stringData = System.Text.Encoding.Unicode.GetString(readString);
    }
    private static byte[] Read(byte[] bb,int start, int end )
    {
       byte[] aaa = new byte[end -start +1];
       int temp = 0;
       for (int i = start-1; i <= end-1; i++)
       {
            aaa[temp] = bb[i];
            temp++;           
       }
       temp = 0;
       return aaa;
    }

    private static void AddLen()
    {
       Array.Reverse(m_PacketData, 0, (int)m_Pos);
       var bytes = BitConverter.GetBytes((int)m_Pos);
       bytes.CopyTo(m_PacketData, m_Pos);
       m_Pos += (uint)bytes.Length;
       Array.Reverse(m_PacketData, 0, (int)m_Pos);
    }
  }
}

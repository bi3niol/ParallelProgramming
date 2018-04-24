using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace WUT.ParallelProgramming.EX3.Jankiel.Messages
{
    [Serializable]
    public abstract class Message
    {
        public string From { get; protected set; }
        public Message(string from)
        {
            From = from;
        }

        public abstract void ProcessMessage(JankielManager jankiel);

        public static Message GetMessage(byte[] contet)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(contet))
            {
                return bf.Deserialize(stream) as Message;
            }
        }
        public byte[] GetBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                bf.Serialize(stream, this);
                return stream.GetBuffer();
            }
        }
    }
}

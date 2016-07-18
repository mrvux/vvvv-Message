﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using VVVV.Packs.Messaging.Serializing;
using VVVV.Utils.VMath;



namespace VVVV.Packs.Messaging.Tests
{

    using Time = VVVV.Packs.Time.Time;
    using System.IO;


    [TestClass]
    public class MessageSerialisationTest
    {

        [TestMethod]
        public void MessageToString()
        {
          var message = new Message();

          message.Topic = "Test";
          message.Init("foo", "bar");
          message.TimeStamp = Time.MinUTCTime();
          		
           Assert.AreEqual("Message Test (01.01.0001 01:00:00 [UTC])\n foo \t: bar \r\n", message.ToString());
        }

        [TestMethod]
        public void MessageToJson()
        {

            var message = new Message();
            message.Init("stringy", "bar");
            message.Init("inty", 1, 2);
            message.Init("float", 1.0f, 2.0f);
            message.Init("double", 1.0d, 2.0d);

            message.Init("raw", new MemoryStream());
            message.Init("time", Time.MinUTCTime(), Time.StringAsTime("UTC", "2001", "yyyy"));

            message.Init("V2", new Vector2D(), new Vector2D(1, 2));
            message.Init("V3", new Vector3D(), new Vector3D(1, 2, 3));
            message.Init("V4", new Vector4D(), new Vector4D(1, 2, 3, 4));

            message.Init("Matrix", new Matrix4x4(), new Matrix4x4(new Vector4D(1, 2, 3, 4)));

            var innerMessage = new Message("InnerMessage2");
            innerMessage.Init("Foo", "bar");
            innerMessage.TimeStamp = Time.MinUTCTime();
            message.Init(innerMessage.Topic, innerMessage);


            message.Topic = "Test";
            message.TimeStamp = Time.MinUTCTime();

            var settings = new JsonSerializerSettings { Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.Auto };
            string json = JsonConvert.SerializeObject(message, settings);
//            Assert.AreEqual("{\"Topic\":\"Test\",\"stringy\":\"bar\",\"inty\":[1,2],\"float<float>\":[1.0,2.0],\"double\":[1.0,2.0],\"raw<Raw>\":\"\",\"time<Time>\":[{\"UTC\":\"0001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"},{\"UTC\":\"2001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"}],\"V2<Vector2d>\":[{\"x\":0.0,\"y\":0.0},{\"x\":1.0,\"y\":2.0}],\"V3<Vector3d>\":[{\"x\":0.0,\"y\":0.0,\"z\":0.0},{\"x\":1.0,\"y\":2.0,\"z\":3.0}],\"V4<Vector4d>\":[{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":0.0},{\"x\":1.0,\"y\":2.0,\"z\":3.0,\"w\":4.0}],\"Matrix<Transform>\":[{\"Values\":[0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0]},{\"Values\":[1.0,-2.0,-3.0,-4.0,2.0,1.0,4.0,-3.0,3.0,-4.0,1.0,2.0,4.0,3.0,-2.0,1.0]}],\"Message<Message>\":{\"Topic\":\"InnerMessage2\",\"Foo\":\"bar\",\"Stamp\":{\"UTC\":\"0001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"}},\"Stamp\":{\"UTC\":\"0001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"}}", json);
            Assert.AreEqual("{\"Topic\":\"Test\",\"stringy\":\"bar\",\"inty\":[1,2],\"float<float>\":[1.0,2.0],\"double\":[1.0,2.0],\"raw<Raw>\":\"\",\"time<Time>\":[{\"UTC\":\"0001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"},{\"UTC\":\"2001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"}],\"V2<Vector2d>\":[{\"x\":0.0,\"y\":0.0},{\"x\":1.0,\"y\":2.0}],\"V3<Vector3d>\":[{\"x\":0.0,\"y\":0.0,\"z\":0.0},{\"x\":1.0,\"y\":2.0,\"z\":3.0}],\"V4<Vector4d>\":[{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":0.0},{\"x\":1.0,\"y\":2.0,\"z\":3.0,\"w\":4.0}],\"Matrix<Transform>\":[{\"Values\":[0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0]},{\"Values\":[1.0,-2.0,-3.0,-4.0,2.0,1.0,4.0,-3.0,3.0,-4.0,1.0,2.0,4.0,3.0,-2.0,1.0]}],\"InnerMessage2\":{\"Topic\":\"InnerMessage2\",\"Foo\":\"bar\",\"Stamp\":{\"UTC\":\"0001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"}},\"Stamp\":{\"UTC\":\"0001-01-01 00:00:00.0000\",\"ZoneId\":\"UTC\"}}", json);

            var newMessage = JsonConvert.DeserializeObject<Message>(json);
            Assert.AreEqual(message.ToString(), newMessage.ToString());


            Assert.IsInstanceOfType(newMessage["stringy"].First, typeof(string));
            Assert.IsInstanceOfType(newMessage["inty"].First, typeof(int));
            Assert.IsInstanceOfType(newMessage["float"].First, typeof(float));
            Assert.IsInstanceOfType(newMessage["double"].First, typeof(double));
            Assert.IsInstanceOfType(newMessage["raw"].First, typeof(Stream));
            Assert.IsInstanceOfType(newMessage["time"].First, typeof(Time));

            Assert.IsInstanceOfType(newMessage["V2"].First, typeof(Vector2D));
            Assert.IsInstanceOfType(newMessage["V3"].First, typeof(Vector3D));
            Assert.IsInstanceOfType(newMessage["V4"].First, typeof(Vector4D));
            Assert.IsInstanceOfType(newMessage["Matrix"].First, typeof(Matrix4x4));

            Assert.IsInstanceOfType(newMessage["InnerMessage2"].First, typeof(Message));


        }

        [TestMethod]
        public void JsonToMessage()
        {

            string json = "{'Int':42, 'NullTyped<string>':null, 'Null':null, 'String':'Hello World'}";

            var newMessage = JsonConvert.DeserializeObject<Message>(json);
            Assert.AreEqual(42, newMessage["Int"].First);
            Assert.AreEqual("Hello World", newMessage["String"].First);

            Assert.AreEqual(2, newMessage.Fields.Count());

            Assert.IsNull(newMessage["NullTyped"]);
            Assert.IsNull(newMessage["Null"]);
        

        }






    }
}

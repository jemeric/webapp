﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace webapp.Util.Extensions
{
    // see https://dejanstojanovic.net/aspnet/2018/may/using-idistributedcache-in-net-core-just-got-a-lot-easier/
    public static class Serialization
    {
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        public static T FromByteArray<T>(this byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default(T);
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            {
                object cachedObj = binaryFormatter.Deserialize(memoryStream);
                if(cachedObj is T tValue)
                {
                    return tValue;
                } else
                {
                    return default(T);
                }
            }
        }
    }
}

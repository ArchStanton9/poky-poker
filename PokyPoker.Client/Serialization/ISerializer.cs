using System;
using System.Collections.Generic;
using System.IO;

namespace PokyPoker.Client.Serialization
{
    public interface ISerializer
    {
        void Serialize(object item, Stream outputStream);
        byte[] Serialize(object item);

        T Deserialize<T>(Stream inputStream);
        T Deserialize<T>(byte[] bytes);

        bool TryDeserialize<TValue>(Stream inputStream, out TValue value);
        bool TryDeserialize<TValue>(byte[] bytes, out TValue value);


        string ContentType { get; }
    }

}

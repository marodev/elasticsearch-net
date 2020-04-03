#region Utf8Json License https://github.com/neuecc/Utf8Json/blob/master/LICENSE
// MIT License
//
// Copyright (c) 2017 Yoshifumi Kawai
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;

namespace Elasticsearch.Net.Utf8Json
{
	internal static class ByteArrayPool
	{
		public const int DefaultBufferLength = 1024;

		public static byte[] Resize(byte[] array, int newSize)
		{
			byte[] rented = Rent(newSize);
			Buffer.BlockCopy(array, 0, rented, 0, array.Length > newSize ? newSize : array.Length);
			Return(array);
			return rented;
		}

        public static void EnsureCapacity(ref byte[] bytes, int offset, int appendLength)
        {
            var newLength = offset + appendLength;

            // If null (most case first time) fill byte.
            if (bytes == null)
            {
                throw new Exception("bytes can never be null");
            }

            // like MemoryStream.EnsureCapacity
            var current = bytes.Length;
            if (newLength > current)
            {
                int num = newLength;
                if (num < 256)
                {
                    num = 256;
                    bytes = Resize(bytes, num);
                    return;
                }

                if (current == BinaryUtil.ArrayMaxSize)
                {
                    throw new InvalidOperationException("byte[] size reached maximum size of array(0x7FFFFFC7), can not write to single byte[]. Details: https://msdn.microsoft.com/en-us/library/system.array");
                }

                var newSize = unchecked(current * 2);
                if (newSize < 0) // overflow
                {
                    num = BinaryUtil.ArrayMaxSize;
                }
                else
                {
                    if (num < newSize)
                    {
                        num = newSize;
                    }
                }

                bytes = Resize(bytes, num);
            }
        }

		public static byte[] Rent(int minLength = DefaultBufferLength) =>
			System.Buffers.ArrayPool<byte>.Shared.Rent(minLength);

		public static void Return(byte[] bytes) =>
			System.Buffers.ArrayPool<byte>.Shared.Return(bytes);
	}


    /// <summary>
    /// High-Level API of Utf8Json.
    /// </summary>
	internal static partial class JsonSerializer
    {
        static IJsonFormatterResolver defaultResolver;

        /// <summary>
        /// FormatterResolver that used resolver less overloads. If does not set it, used StandardResolver.Default.
        /// </summary>
        public static IJsonFormatterResolver DefaultResolver
        {
            get { return defaultResolver ??= StandardResolver.Default; }
        }

        /// <summary>
        /// Set default resolver of Utf8Json APIs.
        /// </summary>
        /// <param name="resolver"></param>
        public static void SetDefaultResolver(IJsonFormatterResolver resolver)
        {
            defaultResolver = resolver;
        }

        /// <summary>
        /// Serialize to binary with default resolver.
        /// </summary>
        public static byte[] Serialize<T>(T obj)
        {
            return Serialize(obj, defaultResolver);
        }

        /// <summary>
        /// Serialize to binary with specified resolver.
        /// </summary>
        public static byte[] Serialize<T>(T value, IJsonFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;
			var buffer = ByteArrayPool.Rent();
			try
			{
				var writer = new JsonWriter(buffer);
				var formatter = resolver.GetFormatterWithVerify<T>();
				formatter.Serialize(ref writer, value, resolver);
				return writer.ToUtf8ByteArray();
			}
			finally
			{
				ByteArrayPool.Return(buffer);
			}
        }

        public static void Serialize<T>(ref JsonWriter writer, T value)
        {
            Serialize<T>(ref writer, value, defaultResolver);
        }

        public static void Serialize<T>(ref JsonWriter writer, T value, IJsonFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;

            var formatter = resolver.GetFormatterWithVerify<T>();
            formatter.Serialize(ref writer, value, resolver);
        }

        /// <summary>
        /// Serialize to stream.
        /// </summary>
        public static void Serialize<T>(Stream stream, T value)
        {
            Serialize(stream, value, defaultResolver);
        }

        /// <summary>
        /// Serialize to stream with specified resolver.
        /// </summary>
        public static void Serialize<T>(Stream stream, T value, IJsonFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;

            var buffer = SerializeUnsafe(value, resolver);
            stream.Write(buffer.Array, buffer.Offset, buffer.Count);
        }

        /// <summary>
        /// Serialize to stream(write async).
        /// </summary>
        public static Task SerializeAsync<T>(Stream stream, T value)
        {
            return SerializeAsync<T>(stream, value, defaultResolver);
        }

        /// <summary>
        /// Serialize to stream(write async) with specified resolver.
        /// </summary>
        public static async Task SerializeAsync<T>(Stream stream, T value, IJsonFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;

            var buf = ByteArrayPool.Rent();
            try
            {
                var writer = new JsonWriter(buf);
                var formatter = resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref writer, value, resolver);
                var buffer = writer.GetBuffer();
                await stream.WriteAsync(buffer.Array, buffer.Offset, buffer.Count).ConfigureAwait(false);
            }
            finally
            {
                ByteArrayPool.Return(buf);
            }
        }

        /// <summary>
        /// Serialize to binary. Get the raw memory pool byte[]. The result can not share across thread and can not hold, so use quickly.
        /// </summary>
        public static ArraySegment<byte> SerializeUnsafe<T>(T obj)
        {
            return SerializeUnsafe(obj, defaultResolver);
        }

        /// <summary>
        /// Serialize to binary with specified resolver. Get the raw memory pool byte[]. The result can not share across thread and can not hold, so use quickly.
        /// </summary>
        public static ArraySegment<byte> SerializeUnsafe<T>(T value, IJsonFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;
			var buffer = ByteArrayPool.Rent();
			try
			{
				var writer = new JsonWriter(buffer);
				var formatter = resolver.GetFormatterWithVerify<T>();
				formatter.Serialize(ref writer, value, resolver);
				var arraySegment = writer.GetBuffer();
				return new ArraySegment<byte>(BinaryUtil.ToArray(ref arraySegment));
			}
			finally
			{
				ByteArrayPool.Return(buffer);
			}
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return Deserialize<T>(bytes, defaultResolver);
        }

        public static T Deserialize<T>(byte[] bytes, IJsonFormatterResolver resolver)
        {
            return Deserialize<T>(bytes, 0, resolver);
        }

        public static T Deserialize<T>(byte[] bytes, int offset)
        {
            return Deserialize<T>(bytes, offset, defaultResolver);
        }

        public static T Deserialize<T>(byte[] bytes, int offset, IJsonFormatterResolver resolver)
        {
			if (bytes == null || bytes.Length == 0)
				return default;

            if (resolver == null)
				resolver = DefaultResolver;

            var reader = new JsonReader(bytes, offset);
            var formatter = resolver.GetFormatterWithVerify<T>();
            return formatter.Deserialize(ref reader, resolver);
        }

        public static T Deserialize<T>(ref JsonReader reader)
        {
            return Deserialize<T>(ref reader, defaultResolver);
        }

        public static T Deserialize<T>(ref JsonReader reader, IJsonFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;

            var formatter = resolver.GetFormatterWithVerify<T>();
            return formatter.Deserialize(ref reader, resolver);
        }

        public static T Deserialize<T>(Stream stream)
        {
            return Deserialize<T>(stream, defaultResolver);
        }

        public static T Deserialize<T>(Stream stream, IJsonFormatterResolver resolver)
        {
			if (stream == null || stream.CanSeek && stream.Length == 0)
				return default;

            if (resolver == null)
				resolver = DefaultResolver;

			if (stream is MemoryStream ms)
            {
				if (ms.TryGetBuffer(out var buf2))
                {
                    // when token is number, can not use from pool(can not find end line).
                    var token = new JsonReader(buf2.Array, buf2.Offset).GetCurrentJsonToken();
                    if (token == JsonToken.Number)
                    {
                        var buf3 = new byte[buf2.Count];
                        Buffer.BlockCopy(buf2.Array, buf2.Offset, buf3, 0, buf3.Length);
                        return Deserialize<T>(buf3, 0, resolver);
                    }

                    return Deserialize<T>(buf2.Array, buf2.Offset, resolver);
                }
            }
            var buf = ByteArrayPool.Rent();
			var poolBuf = buf;
			T value;

			try
			{
				var length = FillFromStream(stream, ref buf);

				if (length == 0)
					return default;


				// when token is number, can not use from pool(can not find end line).
				var token = new JsonReader(buf).GetCurrentJsonToken();
				if (token == JsonToken.Number)
				{
					buf = BinaryUtil.FastCloneWithResize(buf, length);
				}

				value = Deserialize<T>(buf, resolver);
			}
			finally
			{
				ByteArrayPool.Return(poolBuf);
			}

			return value;
        }

        public static Task<T> DeserializeAsync<T>(Stream stream)
        {
            return DeserializeAsync<T>(stream, defaultResolver);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream, IJsonFormatterResolver resolver)
        {
			if (stream == null || stream.CanSeek && stream.Length == 0)
				return default;

            if (resolver == null)
				resolver = DefaultResolver;

			if (stream is MemoryStream ms)
            {
				if (ms.TryGetBuffer(out var buf2))
                {
                    // when token is number, can not use from pool(can not find end line).
                    var token = new JsonReader(buf2.Array, buf2.Offset).GetCurrentJsonToken();
                    if (token == JsonToken.Number)
                    {
                        var buf3 = new byte[buf2.Count];
                        Buffer.BlockCopy(buf2.Array, buf2.Offset, buf3, 0, buf3.Length);
                        return Deserialize<T>(buf3, 0, resolver);
                    }

                    return Deserialize<T>(buf2.Array, buf2.Offset, resolver);
                }
            }

            var buffer = ByteArrayPool.Rent();
            var buf = buffer;
			T value;

            try
            {
                int length = 0;
                int read;
                while ((read = await stream.ReadAsync(buf, length, buf.Length - length).ConfigureAwait(false)) > 0)
                {
                    length += read;
                    if (length == buf.Length)
						buf = ByteArrayPool.Resize(buf, length * 2);
                }

				if (length == 0)
					return default;

                // when token is number, can not use from pool(can not find end line).
                var token = new JsonReader(buf).GetCurrentJsonToken();
                if (token == JsonToken.Number)
                {
                    buf = BinaryUtil.FastCloneWithResize(buf, length);
                }

                value = Deserialize<T>(buf, resolver);
            }
            finally
            {
                ByteArrayPool.Return(buffer);
            }

			return value;
        }

        static int FillFromStream(Stream input, ref byte[] buffer)
        {
            int length = 0;
            int read;
            while ((read = input.Read(buffer, length, buffer.Length - length)) > 0)
            {
                length += read;
                if (length == buffer.Length)
                {
                    buffer = ByteArrayPool.Resize(buffer, length * 2);
                }
            }

            return length;
        }
    }
}

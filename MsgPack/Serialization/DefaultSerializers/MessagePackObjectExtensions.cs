#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010-2012 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.DefaultSerializers
{
	internal static class MessagePackObjectExtensions
	{
		/// <summary>
		///		Invokes <see cref="MessagePackObject.AsInt64()"/> in deserializaton manner.
		/// </summary>
		/// <param name="source"><see cref="MessagePackObject"/>.</param>
		/// <returns>A deserialized value.</returns>
		/// <exception cref="SerializationException"><paramref name="source"/> is not expected type.</exception>
		public static long DeserializeAsInt64( this MessagePackObject source )
		{
			try
			{
				return source.AsInt64();
			}
			catch ( InvalidOperationException ex )
			{
				throw new SerializationException( String.Format( CultureInfo.CurrentCulture, "The unpacked value is not expected type. {0}", ex.Message ), ex );
			}
		}

		/// <summary>
		///		Invokes <see cref="MessagePackObject.AsString()"/> in deserializaton manner.
		/// </summary>
		/// <param name="source"><see cref="MessagePackObject"/>.</param>
		/// <returns>A deserialized value.</returns>
		/// <exception cref="SerializationException"><paramref name="source"/> is not expected type.</exception>
		public static string DeserializeAsString( this MessagePackObject source )
		{
			try
			{
				return source.AsString();
			}
			catch ( InvalidOperationException ex )
			{
				throw new SerializationException( String.Format( CultureInfo.CurrentCulture, "The unpacked value is not expected type. {0}", ex.Message ), ex );
			}
		}

		/// <summary>
		///		Invokes <see cref="MessagePackObject.AsMessagePackExtendedTypeObject()"/> in deserializaton manner.
		/// </summary>
		/// <param name="source"><see cref="MessagePackExtendedTypeObject"/>.</param>
		/// <returns>A deserialized value.</returns>
		/// <exception cref="SerializationException"><paramref name="source"/> is not expected type.</exception>
		public static MessagePackExtendedTypeObject DeserializeAsMessagePackExtendedTypeObject( this MessagePackObject source )
		{
			try
			{
				return source.AsMessagePackExtendedTypeObject();
			}
			catch ( InvalidOperationException ex )
			{
				throw new SerializationException( String.Format( CultureInfo.CurrentCulture, "The unpacked value is not expected type. {0}", ex.Message ), ex );
			}
		}
	}
}

#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010-2018 FUJIWARA, Yusuke
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

#if UNITY_5 || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS3 || UNITY_XBOX360 || UNITY_FLASH || UNITY_BKACKBERRY || UNITY_WINRT
#define UNITY
#endif

using System;
using System.Collections.Generic;

namespace MsgPack
{
	/// <summary>
	///		Implements <see cref="EqualityComparer{T}"/> of <see cref="MessagePackObject"/>.
	/// </summary>
#if !SILVERLIGHT && !NETSTANDARD1_1 && !NETSTANDARD1_3
	[Serializable]
#endif // !SILVERLIGHT && !NETSTANDARD1_1 && !NETSTANDARD1_3
#if UNITY && DEBUG
	public
#else
	internal
#endif
	sealed class MessagePackObjectEqualityComparer : IEqualityComparer<MessagePackObject>
	{
		private static readonly MessagePackObjectEqualityComparer _instance = new MessagePackObjectEqualityComparer();

#if UNITY && DEBUG
		public
#else
		internal
#endif
		static MessagePackObjectEqualityComparer Instance
		{
			get { return _instance; }
		}

		/// <summary>
		///		Initializes a new instance of the <see cref="MessagePackObjectEqualityComparer"/> class.
		/// </summary>
		public MessagePackObjectEqualityComparer() { }

		/// <summary>
		///		Determines whether two objects of type <see cref="MessagePackObject"/> are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		///		<c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
		/// </returns>
		public bool Equals( MessagePackObject x, MessagePackObject y )
		{
			return x.Equals( y );
		}

		/// <summary>
		///		Returns a hash code for the specified <see cref="MessagePackObject"/>.
		/// </summary>
		/// <param name="obj">The <see cref="MessagePackObject"/>.</param>
		/// <returns>
		///		A hash code for <paramref name="obj"/>, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public int GetHashCode( MessagePackObject obj )
		{
			return obj.GetHashCode();
		}
	}
}

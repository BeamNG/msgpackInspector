#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2015-2017 FUJIWARA, Yusuke
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

namespace MsgPack
{
	/// <summary>
	///		Defines known ext type name for MessagePack for CLI.
	/// </summary>
	/// <remarks>
	///		Note that values in this class are not guaranteed as interoperable with other implementations.
	///		These are just known by MessagePack for CLI implementation.
	/// </remarks>
	public static class KnownExtTypeName
	{
		/// <summary>
		///		Gets the ext type name which represents MsgPack timestamp.
		/// </summary>
		/// <value>
		///		"Timestamp".
		/// </value>
		public static string Timestamp
		{
			get { return "Timestamp"; }
		}

		/// <summary>
		///		Gets the ext type name which represents multidimensional array.
		/// </summary>
		/// <value>
		///		"MultidimensionalArray".
		/// </value>
		public static string MultidimensionalArray
		{
			get { return "MultidimensionalArray"; }
		}
	}
}

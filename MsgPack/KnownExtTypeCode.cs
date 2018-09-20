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
	///		Defines known ext type code for MessagePack for CLI.
	/// </summary>
	/// <remarks>
	///		Note that values in this class are not guaranteed as interoperable with other implementations.
	///		These are just known by MessagePack for CLI implementation.
	/// </remarks>
	public static class KnownExtTypeCode
	{
		/// <summary>
		///		Gets the ext type code which represents MsgPack timestamp.
		/// </summary>
		/// <value>
		///		0xFF(-1).
		/// </value>
		public static byte Timestamp
		{
			get { return 0xFF; }
		}

		/// <summary>
		///		Gets the ext type code which represents multidimensional array.
		/// </summary>
		/// <value>
		///		0x71.
		/// </value>
		public static byte MultidimensionalArray
		{
			get { return 0x71; }
		}
	}
}

﻿#region -- License Terms --
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

namespace MsgPack.Serialization
{
	/// <summary>
	///		Represents serialization method for complex types.
	/// </summary>
	public enum SerializationMethod
	{
		/// <summary>
		///		The object will be serialized as array which is ordered by member ID.
		///		This is default and more interoperable option.
		/// </summary>
		Array = 0,

		/// <summary>
		///		The object will be serialized as map which is ordered by member ID.
		///		This is a bit slower than array, but more stable for forward/backward compatibility.
		/// </summary>
		Map
	}
}

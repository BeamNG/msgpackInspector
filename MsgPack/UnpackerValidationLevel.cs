﻿#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010-2015 FUJIWARA, Yusuke
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
	///		Specifies validation behavior level of <see cref="Unpacker"/>.
	/// </summary>
	public enum UnpackerValidationLevel
	{
		/// <summary>
		///		No validation. Although this improves unpacking performance, trouble shooting becomes hard. 
		/// </summary>
		None = 0,

		/// <summary>
		///		Validates collection nesting. This options is default for backward compatibility.
		/// </summary>
		Collection
	}
}

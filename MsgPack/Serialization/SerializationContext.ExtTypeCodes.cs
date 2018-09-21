﻿#region -- License Terms --
// 
// MessagePack for CLI
// 
// Copyright (C) 2015 FUJIWARA, Yusuke
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
	partial class SerializationContext
	{
		private readonly ExtTypeCodeMapping _extTypeCodes = new ExtTypeCodeMapping();

		/// <summary>
		///		Gets the current mapping table of ext type code.
		/// </summary>
		/// <value>
		///		The <see cref="ExtTypeCodeMapping"/> which maps between known ext type names and ext type codes.
		/// </value>
		public ExtTypeCodeMapping ExtTypeCodeMapping
		{
			get { return this._extTypeCodes; }
		}
	}
}

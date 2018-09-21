#region -- License Terms --
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
	/// <summary>
	///		Defines behavior of built-in serializers to conversion of <see cref="DateTime"/> value for specific member.
	/// </summary>
	public enum DateTimeMemberConversionMethod
	{
		/// <summary>
		///		Uses systems default <see cref="DateTimeConversionMethod"/> value.
		/// </summary>
		/// <remarks>
		///		The systems default <see cref="DateTimeConversionMethod.Native"/>.
		/// </remarks>
		Default = 0,

		/// <summary>
		///		Uses <see cref="DateTime.Ticks"/> context, that is, Gregorian 0000-01-01 based, 100 nano seconds resolution. This value also preserves <see cref="DateTimeKind"/>.
		/// </summary>
		/// <remarks>
		///		As of 0.6, this value has been become default. This option prevents accidental data loss.
		/// </remarks>
		Native = 1,

		/// <summary>
		///		Uses Unix epoc context, that is, Gregirian 1970-01-01 based, milliseconds resolution.
		/// </summary>
		/// <remarks>
		///		Many binding such as Java uses this resolution, so this option gives maximom interoperability.
		/// </remarks>
		UnixEpoc = 2,

		/// <summary>
		///		Uses MsgPack timestamp format, that is, Gregirian 1970-01-01 based, nanoseconds resolution, with reserved ext type format.
		/// </summary>
		/// <remarks>
		///		<para>
		///			As of 1.0, this value became default.
		///		</para>
		///		<para>
		///			This is best choice for interoperability and prevents accidental data loss, but old implementation does not recognize this type.
		///			For backward compability purposes, use <see cref="Native"/> or <see cref="UnixEpoc"/> instead.
		///		</para>
		///		<para>
		///			Note that <see cref="DateTime"/> and <see cref="DateTimeOffset"/> cannot hold nanoseconds value.
		///			If you can depend on this assembly, consider <see cref="Timestamp"/> for date-time typed members to maximize interoperability for other languages.
		///		</para>
		/// </remarks>
		Timestamp = 3
	}
}

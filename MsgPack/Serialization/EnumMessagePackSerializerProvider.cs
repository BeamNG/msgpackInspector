#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2014-2018 FUJIWARA, Yusuke
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

namespace MsgPack.Serialization
{
	/// <summary>
	///		Implements <see cref="MessagePackSerializerProvider"/> for enums.
	///		This class accepts <see cref="EnumSerializationMethod"/> as a provider parameter.
	/// </summary>
#if UNITY && DEBUG
	public
#else
	internal
#endif
	sealed class EnumMessagePackSerializerProvider : MessagePackSerializerProvider
	{
		private readonly Type _enumType;
		private readonly object _serializerForName;
		private readonly object _serializerForIntegral;

		/// <summary>
		///		Initializes a new instance of the <see cref="EnumMessagePackSerializerProvider"/> class.
		/// </summary>
		/// <param name="enumType">A type of the enum.</param>
		/// <param name="serializer">The serializer implements <see cref="ICustomizableEnumSerializer"/>.</param>
		public EnumMessagePackSerializerProvider( Type enumType, ICustomizableEnumSerializer serializer )
		{
			this._enumType = enumType;
			this._serializerForName = serializer.GetCopyAs( EnumSerializationMethod.ByName );
			this._serializerForIntegral = serializer.GetCopyAs( EnumSerializationMethod.ByUnderlyingValue );
		}

		/// <summary>
		///		Gets a serializer instance for specified parameter.
		/// </summary>
		/// <param name="context">A serialization context which holds global settings.</param>
		/// <param name="providerParameter">A provider specific parameter.</param>
		/// <returns>	
		/// A serializer object for specified parameter.
		/// </returns>
		public override object Get( SerializationContext context, object providerParameter )
		{
			if ( ( providerParameter is EnumSerializationMethod ) )
			{
				switch ( ( EnumSerializationMethod )providerParameter )
				{
					case EnumSerializationMethod.ByName:
					{
						return this._serializerForName;
					}
					case EnumSerializationMethod.ByUnderlyingValue:
					{
						return this._serializerForIntegral;
					}
				}
			}

			switch ( 
				EnumMessagePackSerializerHelpers.DetermineEnumSerializationMethod( 
					context, this._enumType, EnumMemberSerializationMethod.Default 
				) 
			)
			{
				case EnumSerializationMethod.ByUnderlyingValue:
				{
					return this._serializerForIntegral;
				}
				default:
				{
					return this._serializerForName;
				}
			}
		}
	}
}

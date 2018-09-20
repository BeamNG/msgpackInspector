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

#if UNITY_5 || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS3 || UNITY_XBOX360 || UNITY_FLASH || UNITY_BKACKBERRY || UNITY_WINRT
#define UNITY
#endif

using System;
#if !UNITY
using System.Collections;
#endif // !UNITY
#if FEATURE_TAP
using System.Threading;
using System.Threading.Tasks;
#endif // FEATURE_TAP

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Polymorphic;

namespace MsgPack.Serialization.DefaultSerializers
{
#if !UNITY
	internal sealed class AbstractNonGenericEnumerableMessagePackSerializer<TCollection> : NonGenericEnumerableMessagePackSerializer<TCollection>
		where TCollection : IEnumerable
#else
#warning TODO: Use generic collection if possible for maintenancibility.
	internal sealed class AbstractNonGenericEnumerableMessagePackSerializer : UnityNonGenericEnumerableMessagePackSerializer
#endif // !UNITY
	{
		private readonly ICollectionInstanceFactory _concreteCollectionInstanceFactory;
		private readonly MessagePackSerializer _concreteSerializer;

		internal override SerializerCapabilities InternalGetCapabilities()
		{
			return this._concreteSerializer.Capabilities;
		}

		public AbstractNonGenericEnumerableMessagePackSerializer(
			SerializationContext ownerContext,
#if !UNITY
			Type targetType,
#else
			Type abstractType,
			Type concreteType,
#endif // !UNITY
			PolymorphismSchema schema
		)
#if !UNITY
			: base( ownerContext, schema )
#else
			: base( ownerContext, abstractType, schema, SerializerCapabilities.PackTo | SerializerCapabilities.UnpackFrom | SerializerCapabilities.UnpackTo )
#endif // !UNITY
		{
			AbstractCollectionSerializerHelper.GetConcreteSerializer(
				ownerContext,
				schema,
#if !UNITY
				typeof( TCollection ),
				targetType,
#else
				abstractType,
				concreteType,
#endif // !UNITY
				typeof( EnumerableMessagePackSerializerBase<,> ),
				out this._concreteCollectionInstanceFactory,
				out this._concreteSerializer
			);
		}

#if !UNITY
		protected internal override TCollection UnpackFromCore( Unpacker unpacker )
#else
		protected internal override object UnpackFromCore( Unpacker unpacker )
#endif // !UNITY
		{
			var polymorPhicSerializer = this._concreteSerializer as IPolymorphicDeserializer;
			if ( polymorPhicSerializer != null )
			{
				// This boxing is OK because TCollection should be reference type because TCollection is abstract class or interface.
				return 
#if !UNITY
					( TCollection )
#endif // !UNITY
					polymorPhicSerializer.PolymorphicUnpackFrom( unpacker );
			}
			else
			{
				return 
#if !UNITY
					( TCollection )
#endif // !UNITY
					this._concreteSerializer.UnpackFrom( unpacker );
			}
		}

#if FEATURE_TAP

		protected internal override async Task<TCollection> UnpackFromAsyncCore( Unpacker unpacker, CancellationToken cancellationToken )
		{
			var polymorPhicSerializer = this._concreteSerializer as IPolymorphicDeserializer;
			if ( polymorPhicSerializer != null )
			{
				// This boxing is OK because TCollection should be reference type because TCollection is abstract class or interface.
				return
					( TCollection )
						( await polymorPhicSerializer.PolymorphicUnpackFromAsync( unpacker, cancellationToken ).ConfigureAwait( false ) );
			}
			else
			{
				return
					( TCollection )
						( await this._concreteSerializer.UnpackFromAsync( unpacker, cancellationToken ).ConfigureAwait( false ) );
			}
		}

#endif // FEATURE_TAP

#if !UNITY
		protected override TCollection CreateInstance( int initialCapacity )
#else
		protected override object CreateInstance( int initialCapacity )
#endif // !UNITY
		{
			// This boxing is OK because TCollection should be reference type because TCollection is abstract class or interface.
			return 
#if !UNITY
				( TCollection )
#endif // !UNITY
				this._concreteCollectionInstanceFactory.CreateInstance( initialCapacity );
		}
	}
}
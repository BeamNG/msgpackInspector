#region -- License Terms --
// 
// MessagePack for CLI
// 
// Copyright (C) 2015-2016 FUJIWARA, Yusuke
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
#if UNITY
using System.Collections;
#endif // UNITY
using System.Collections.Generic;
using System.Linq;
#if UNITY
using System.Reflection;
#endif // UNITY
using System.Runtime.Serialization;
#if FEATURE_TAP
using System.Threading;
using System.Threading.Tasks;
#endif // FEATURE_TAP

namespace MsgPack.Serialization.CollectionSerializers
{
	/// <summary>
	///		Provides common implementation of <see cref="EnumerableMessagePackSerializerBase{TCollection, TItem}"/> 
	///		for collection types which do not implement <see cref="ICollection{T}"/>.
	/// </summary>
	/// <typeparam name="TCollection">The type of the collection.</typeparam>
	/// <typeparam name="TItem">The type of the item of collection.</typeparam>
	public abstract class EnumerableMessagePackSerializer<TCollection, TItem> : EnumerableMessagePackSerializerBase<TCollection, TItem>
		where TCollection : IEnumerable<TItem>
	{
		/// <summary>
		///		Initializes a new instance of the <see cref="EnumerableMessagePackSerializer{TCollection, TItem}"/> class.
		/// </summary>
		/// <param name="ownerContext">A <see cref="SerializationContext"/> which owns this serializer.</param>
		/// <param name="schema">
		///		The schema for collection itself or its items for the member this instance will be used to. 
		///		<c>null</c> will be considered as <see cref="PolymorphismSchema.Default"/>.
		/// </param>
		/// <exception cref="ArgumentNullException">
		///		<paramref name="ownerContext"/> is <c>null</c>.
		/// </exception>
		protected EnumerableMessagePackSerializer( SerializationContext ownerContext, PolymorphismSchema schema )
			: base( ownerContext, schema ) { }

		/// <summary>
		///		Initializes a new instance of the <see cref="EnumerableMessagePackSerializer{TCollection, TItem}"/> class.
		/// </summary>
		/// <param name="ownerContext">A <see cref="SerializationContext"/> which owns this serializer.</param>
		/// <param name="schema">
		///		The schema for collection itself or its items for the member this instance will be used to. 
		///		<c>null</c> will be considered as <see cref="PolymorphismSchema.Default"/>.
		/// </param>
		/// <param name="capabilities">A serializer calability flags represents capabilities of this instance.</param>
		/// <exception cref="ArgumentNullException">
		///		<paramref name="ownerContext"/> is <c>null</c>.
		/// </exception>
		protected EnumerableMessagePackSerializer( SerializationContext ownerContext, PolymorphismSchema schema, SerializerCapabilities capabilities )
			: base( ownerContext, schema, capabilities ) { }

		/// <summary>
		///		Serializes specified object with specified <see cref="Packer"/>.
		/// </summary>
		/// <param name="packer"><see cref="Packer"/> which packs values in <paramref name="objectTree"/>. This value will not be <c>null</c>.</param>
		/// <param name="objectTree">Object to be serialized.</param>
		/// <exception cref="SerializationException">
		///		<typeparamref name="TCollection"/> is not serializable etc.
		/// </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated by caller in base class" )]
		protected internal override void PackToCore( Packer packer, TCollection objectTree )
		{
			ICollection<TItem> asICollection;
			if ( ( asICollection = objectTree as ICollection<TItem> ) == null )
			{
				asICollection = objectTree.ToArray();
			}

			// They are AOT safe because they don't require .constraint calls.
			packer.PackArrayHeader( asICollection.Count );
			var itemSerializer = this.ItemSerializer;
			foreach ( var item in asICollection )
			{
				itemSerializer.PackTo( packer, item );
			}
		}

#if FEATURE_TAP

		/// <summary>
		///		Serializes specified object with specified <see cref="Packer"/> asynchronously.
		/// </summary>
		/// <param name="packer"><see cref="Packer"/> which packs values in <paramref name="objectTree"/>. This value will not be <c>null</c>.</param>
		/// <param name="objectTree">Object to be serialized.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
		/// <returns>
		///		A <see cref="Task"/> that represents the asynchronous operation. 
		/// </returns>
		/// <exception cref="System.Runtime.Serialization.SerializationException">
		///		Failed to serialize object.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///		<typeparamref name="TCollection"/> is not serializable even if it can be deserialized.
		/// </exception>
		/// <seealso cref="P:Capabilities"/>
		protected internal override async Task PackToAsyncCore( Packer packer, TCollection objectTree, CancellationToken cancellationToken )
		{
			ICollection<TItem> asICollection;
			if ( ( asICollection = objectTree as ICollection<TItem> ) == null )
			{
				asICollection = objectTree.ToArray();
			}

			// They are AOT safe because they don't require .constraint calls.
			await packer.PackArrayHeaderAsync( asICollection.Count, cancellationToken ).ConfigureAwait( false );
			var itemSerializer = this.ItemSerializer;
			foreach ( var item in asICollection )
			{
				await itemSerializer.PackToAsync( packer, item, cancellationToken ).ConfigureAwait( false );
			}
		}

#endif // FEATURE_TAP
	}
#if UNITY
#warning TODO: Remove if possible for maintenancibility.
	internal abstract class UnityEnumerableMessagePackSerializer : UnityEnumerableMessagePackSerializerBase
	{
		private readonly MethodInfo _getCount;

		protected UnityEnumerableMessagePackSerializer(
			SerializationContext ownerContext,
			Type targetType,
			CollectionTraits traits,
			PolymorphismSchema schema,
			SerializerCapabilities capabilities
		)
			: base( ownerContext, targetType, traits.ElementType, schema, capabilities )
		{
			this._getCount = traits.CountPropertyGetter;
		}

		protected internal override void PackToCore( Packer packer, object objectTree )
		{
			var asEnumerable = objectTree as IEnumerable;
			int count;
			if ( this._getCount == null )
			{
				// ReSharper disable once AssignNullToNotNullAttribute
				var asArray = asEnumerable.OfType<object>().ToArray();
				asEnumerable = asArray;
				count = asArray.Length;
			}
			else
			{
				count = ( int )this._getCount.InvokePreservingExceptionType( objectTree );
			}

			packer.PackArrayHeader( count );
			var itemSerializer = this.ItemSerializer;
			// ReSharper disable once PossibleNullReferenceException
			foreach ( var item in asEnumerable )
			{
				itemSerializer.PackTo( packer, item );
			}
		}
	}
#endif
}
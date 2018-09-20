#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2014-2016 FUJIWARA, Yusuke
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
#if UNITY
using System.Reflection;
#endif // UNITY
#if FEATURE_TAP
using System.Threading;
using System.Threading.Tasks;
#endif // FEATURE_TAP

using MsgPack.Serialization.CollectionSerializers;

namespace MsgPack.Serialization.DefaultSerializers
{
#if !UNITY
	/// <summary>
	///		Provides default implementation for <see cref="List{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of items of the <see cref="List{T}"/>.</typeparam>
	// ReSharper disable once InconsistentNaming
	[Preserve( AllMembers = true )]
	internal class System_Collections_Generic_List_1MessagePackSerializer<T> : MessagePackSerializer<List<T>>, ICollectionInstanceFactory
	{
		private readonly MessagePackSerializer<T> _itemSerializer;

		public System_Collections_Generic_List_1MessagePackSerializer( SerializationContext ownerContext, PolymorphismSchema itemsSchema )
			: base( ownerContext, SerializerCapabilities.PackTo | SerializerCapabilities.UnpackFrom | SerializerCapabilities.UnpackTo )
		{
			this._itemSerializer = ownerContext.GetSerializer<T>( itemsSchema );
		}

		protected internal override void PackToCore( Packer packer, List<T> objectTree )
		{
			PackerUnpackerExtensions.PackCollectionCore( packer, objectTree, this._itemSerializer );
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated internally" )]
		protected internal override List<T> UnpackFromCore( Unpacker unpacker )
		{
			if ( !unpacker.IsArrayHeader )
			{
				SerializationExceptions.ThrowIsNotArrayHeader( unpacker );
			}

			var count = UnpackHelpers.GetItemsCount( unpacker );
			var collection = new List<T>( count );
			this.UnpackToCore( unpacker, collection, count );
			return collection;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated internally" )]
		protected internal override void UnpackToCore( Unpacker unpacker, List<T> collection )
		{
			if ( !unpacker.IsArrayHeader )
			{
				SerializationExceptions.ThrowIsNotArrayHeader( unpacker );
			}

			this.UnpackToCore( unpacker, collection, UnpackHelpers.GetItemsCount( unpacker ) );
		}

		private void UnpackToCore( Unpacker unpacker, List<T> collection, int count )
		{
			for ( int i = 0; i < count; i++ )
			{
				if ( !unpacker.Read() )
				{
					SerializationExceptions.ThrowUnexpectedEndOfStream( unpacker );
				}

				if ( unpacker.IsCollectionHeader )
				{
					using ( var subTreeUnpacker = unpacker.ReadSubtree() )
					{
						collection.Add( this._itemSerializer.UnpackFrom( subTreeUnpacker ) );
					}
				}
				else
				{
					collection.Add( this._itemSerializer.UnpackFrom( unpacker ) );
				}
			}
		}

		public object CreateInstance( int initialCapacity )
		{
			return new List<T>( initialCapacity );
		}

#if FEATURE_TAP

		protected internal override Task PackToAsyncCore( Packer packer, List<T> objectTree, CancellationToken cancellationToken )
		{
			return PackerUnpackerExtensions.PackCollectionAsyncCore( packer, objectTree, this._itemSerializer, cancellationToken );
		}

		protected internal override async Task<List<T>> UnpackFromAsyncCore( Unpacker unpacker, CancellationToken cancellationToken )
		{
			if ( !unpacker.IsArrayHeader )
			{
				SerializationExceptions.ThrowIsNotArrayHeader( unpacker );
			}

			var count = UnpackHelpers.GetItemsCount( unpacker );
			var collection = new List<T>( count );
			await this.UnpackToAsyncCore( unpacker, collection, count, cancellationToken ).ConfigureAwait( false );
			return collection;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated by caller in base class" )]
		protected internal override Task UnpackToAsyncCore( Unpacker unpacker, List<T> collection, CancellationToken cancellationToken )
		{
			if ( !unpacker.IsArrayHeader )
			{
				SerializationExceptions.ThrowIsNotArrayHeader( unpacker );
			}

			return this.UnpackToAsyncCore( unpacker, collection, UnpackHelpers.GetItemsCount( unpacker ), cancellationToken );
		}

		private async Task UnpackToAsyncCore( Unpacker unpacker, List<T> collection, int count, CancellationToken cancellationToken )
		{
			for ( int i = 0; i < count; i++ )
			{
				if ( !await unpacker.ReadAsync( cancellationToken ).ConfigureAwait( false ) )
				{
					SerializationExceptions.ThrowUnexpectedEndOfStream( unpacker );
				}

				if ( unpacker.IsCollectionHeader )
				{
					using ( var subTreeUnpacker = unpacker.ReadSubtree() )
					{
						collection.Add( await this._itemSerializer.UnpackFromAsync( subTreeUnpacker, cancellationToken ).ConfigureAwait( false ) );
					}
				}
				else
				{
					collection.Add( await this._itemSerializer.UnpackFromAsync( unpacker, cancellationToken ).ConfigureAwait( false ) );
				}
			}
		}
#endif // FEATURE_TAP
	}
#else
	// ReSharper disable once InconsistentNaming
	internal class System_Collections_Generic_List_1MessagePackSerializer : NonGenericMessagePackSerializer, ICollectionInstanceFactory
	{
		private static readonly Type[] ConstructorWithCapacityParameterTypes = { typeof( int ) };
		private readonly MessagePackSerializer _itemSerializer;
		private readonly ConstructorInfo _constructor;
		private readonly MethodInfo _add;

		public System_Collections_Generic_List_1MessagePackSerializer( SerializationContext ownerContext, Type targetType, CollectionTraits traits, PolymorphismSchema itemsSchema )
			: base( ownerContext, targetType, SerializerCapabilities.PackTo | SerializerCapabilities.UnpackFrom | SerializerCapabilities.UnpackTo )
		{
			this._itemSerializer = ownerContext.GetSerializer( traits.ElementType, itemsSchema );
			this._constructor = targetType.GetConstructor( ConstructorWithCapacityParameterTypes );
			this._add = traits.AddMethod;
		}

		protected internal override void PackToCore( Packer packer, object objectTree )
		{
			var asCollection = objectTree as ICollection;
			if ( asCollection == null )
			{
				packer.PackNull();
				return;
			}

			packer.PackArrayHeader( asCollection.Count );
			foreach ( var item in asCollection )
			{
				this._itemSerializer.PackTo( packer, item );
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated internally" )]
		protected internal override object UnpackFromCore( Unpacker unpacker )
		{
			if ( !unpacker.IsArrayHeader )
			{
				SerializationExceptions.ThrowIsNotArrayHeader( unpacker );
			}

			var count = UnpackHelpers.GetItemsCount( unpacker );
			var collection = this.CreateInstance( count );
			this.UnpackToCore( unpacker, collection, count );
			return collection;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated internally" )]
		protected internal override void UnpackToCore( Unpacker unpacker, object collection )
		{
			if ( !unpacker.IsArrayHeader )
			{
				SerializationExceptions.ThrowIsNotArrayHeader( unpacker );
			}

			this.UnpackToCore( unpacker, collection, UnpackHelpers.GetItemsCount( unpacker ) );
		}

		private void UnpackToCore( Unpacker unpacker, object collection, int count )
		{
			for ( int i = 0; i < count; i++ )
			{
				if ( !unpacker.Read() )
				{
					SerializationExceptions.ThrowUnexpectedEndOfStream( unpacker );
				}
				if ( unpacker.IsCollectionHeader )
				{
					using ( var subTreeUnpacker = unpacker.ReadSubtree() )
					{
						this._add.InvokePreservingExceptionType( collection, this._itemSerializer.UnpackFrom( subTreeUnpacker ) );
					}
				}
				else
				{
					this._add.InvokePreservingExceptionType( collection, this._itemSerializer.UnpackFrom( unpacker ) );
				}
			}
		}

		public object CreateInstance( int initialCapacity )
		{
			return this._constructor.InvokePreservingExceptionType( initialCapacity );
		}
	}
#endif // !UNITY
}
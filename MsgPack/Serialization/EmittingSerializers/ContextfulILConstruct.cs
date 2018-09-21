#region -- License Terms --
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
using System.Globalization;
using System.Reflection.Emit;

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
	internal abstract class ContextfulILConstruct : ILConstruct
	{
		protected ContextfulILConstruct( TypeDefinition contextType )
			: base( contextType )
		{
		}

		public sealed override void Branch( TracingILGenerator il, Label @else )
		{
			il.TraceWriteLine( "// Brnc->: {0}", this );
			if ( this.ContextType.ResolveRuntimeType() != typeof( bool ) )
			{
				throw new InvalidOperationException(
					String.Format( CultureInfo.CurrentCulture, "Cannot branch with non boolean type '{0}'.", this.ContextType )
					);
			}

			this.BranchCore( il, @else );
			il.TraceWriteLine( "// ->Brnc: {0}", this );
		}

		protected virtual void BranchCore( TracingILGenerator il, Label @else )
		{
			this.LoadValue( il, false );
			il.EmitBrfalse( @else );
		}
	}
}
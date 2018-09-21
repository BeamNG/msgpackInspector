#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2016 FUJIWARA, Yusuke
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
using System.IO;

namespace MsgPack.Serialization
{
	/// <summary>
	///		A <see cref="CodeGenerationSink"/> which emits all codes to specified <see cref="TextWriter"/>.
	/// </summary>
	internal sealed class SingleTextWriterCodeGenerationSink : CodeGenerationSink
	{
		private readonly TextWriter _writer;

		public SingleTextWriterCodeGenerationSink( TextWriter writer )
		{
			if ( writer == null )
			{
				throw new ArgumentNullException( "writer" );
			}

			this._writer = writer;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", MessageId = "0", Justification = "Validated internally." )]
		protected override void AssignTextWriterCore( SerializerCodeInformation codeInformation )
		{
			codeInformation.SetNonFileWriter( this._writer );
		}
	}
}
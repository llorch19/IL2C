﻿/////////////////////////////////////////////////////////////////////////////////////////////////
//
// IL2C - A translator for ECMA-335 CIL/MSIL to C language.
// Copyright (c) 2016-2019 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
/////////////////////////////////////////////////////////////////////////////////////////////////

using IL2C.Internal;
using System;
using System.Diagnostics;
using System.Text;

namespace IL2C.Metadata
{
    public interface IMetadataInformation
        : IEquatable<IMetadataInformation>, IComparable<IMetadataInformation>, IComparable
    {
        string MetadataTypeName { get; }

        string UniqueName { get; }
        string Name { get; }
        string FriendlyName { get; }
        string MangledUniqueName { get; }
        string MangledName { get; }

        IMetadataContext Context { get; }
    }

    [DebuggerDisplay("{DebuggerDisplayString}")]
    internal abstract class MetadataInformation
        : IMetadataInformation
    {
        protected MetadataInformation(MetadataContext metadataContext)
        {
            this.MetadataContext = metadataContext;
        }

        internal readonly MetadataContext MetadataContext;
        IMetadataContext IMetadataInformation.Context =>
            this.MetadataContext;

        public abstract string MetadataTypeName { get; }
        public abstract string UniqueName { get; }
        public abstract string Name { get; }
        public abstract string FriendlyName { get; }

        public virtual string MangledUniqueName =>
            Utilities.GetMangledName(this.UniqueName);
        public virtual string MangledName =>
            Utilities.GetMangledName(this.Name);

        public string DebuggerDisplayString
        {
            [DebuggerStepThrough]
            get
            {
                return this.ToString();
            }
        }

        public int CompareTo(IMetadataInformation other) =>
            StringComparer.Ordinal.Compare(this.UniqueName, other.UniqueName);

        int IComparable.CompareTo(object obj) =>
            StringComparer.Ordinal.Compare(this.UniqueName, ((IMetadataInformation)obj).UniqueName);

        public bool Equals(IMetadataInformation other) =>
            StringComparer.Ordinal.Equals(this.UniqueName, other.UniqueName);

        public override bool Equals(object obj) =>
            this.Equals(obj as IMetadataInformation);

        public override int GetHashCode() =>
            this.UniqueName.GetHashCode();

        [DebuggerStepThrough]
        public override string ToString() =>
            string.Format("{0}: {1}", this.MetadataTypeName, this.FriendlyName);
    }

    internal abstract class MetadataInformation<TReference, TDefinition>
        : MetadataInformation
        where TDefinition : class, TReference
    {
        private TReference member;
        private TDefinition definition;

        protected MetadataInformation(TReference member, MetadataContext metadataContext)
            : base(metadataContext)
        {
            this.member = member;
        }

        internal TReference Member => definition ?? member;

        protected TDefinition Definition => this.GetDefinition();

        private TDefinition GetDefinition()
        {
            if (definition == null)
            {
                // Lazy evaluator for TDefinition
                lock (this)
                {
                    if (definition == null)
                    {
                        definition = this.OnResolve(member);
                    }
                }
            }

            return definition;
        }

        protected virtual TDefinition OnResolve(TReference member) =>
            (TDefinition)member;
    }
}

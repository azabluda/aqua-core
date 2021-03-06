﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.TypeSystem
{
    using Extensions;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using BindingFlags = System.Reflection.BindingFlags;

    [Serializable]
    [DataContract(Name = "Constructor", IsReference = true)]
    [DebuggerDisplay("{Name}")]
    public class ConstructorInfo : MethodBaseInfo
    {
        [NonSerialized]
        private System.Reflection.ConstructorInfo _constructor;

        public ConstructorInfo()
        {
        }

        public ConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
            : base(constructorInfo, TypeInfo.CreateReferenceTracker<Type>())
        {
            _constructor = constructorInfo;
        }

        // TODO: replace binding flags by bool flags
        public ConstructorInfo(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes)
            : base(name, declaringType, bindingFlags, genericArguments, parameterTypes, TypeInfo.CreateReferenceTracker<Type>())
        {
        }

        protected ConstructorInfo(ConstructorInfo constructorInfo)
            : base(constructorInfo, TypeInfo.CreateReferenceTracker<TypeInfo>())
        {
        }

        public override MemberTypes MemberType => MemberTypes.Constructor;

        internal System.Reflection.ConstructorInfo Constructor
        {
            get
            {
                if (ReferenceEquals(null, _constructor))
                {
                    _constructor = this.ResolveConstructor(TypeResolver.Instance);
                }
                return _constructor;
            }
        }

        public override string ToString()
        {
            return $".ctor {base.ToString()}";
        }

        public static explicit operator System.Reflection.ConstructorInfo(ConstructorInfo c)
        {
            return c.Constructor;
        }
    }
}

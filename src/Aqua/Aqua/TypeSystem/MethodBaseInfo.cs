﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using BindingFlags = System.Reflection.BindingFlags;

    [Serializable]
    [DataContract(Name = "MethodBase", IsReference = true)]
    [KnownType(typeof(ConstructorInfo)), XmlInclude(typeof(ConstructorInfo))]
    [KnownType(typeof(MethodInfo)), XmlInclude(typeof(MethodInfo))]
    public abstract class MethodBaseInfo : MemberInfo
    {
        protected MethodBaseInfo()
        {
        }

        protected MethodBaseInfo(System.Reflection.MethodBase methodInfo, Dictionary<Type, TypeInfo> referenceTracker)
            : base(methodInfo, referenceTracker)
        {
            var bindingFlags = methodInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            bindingFlags |= methodInfo.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            BindingFlags = bindingFlags;

            var genericArguments = methodInfo.IsGenericMethod ? methodInfo.GetGenericArguments() : null;
            GenericArgumentTypes = ReferenceEquals(null, genericArguments) || genericArguments.Length == 0
                ? null
                : genericArguments.Select(x => TypeInfo.Create(referenceTracker, x, includePropertyInfos: false)).ToList();

            var parameters = methodInfo.GetParameters();
            ParameterTypes = parameters.Length == 0
                ? null
                : parameters.Select(x => TypeInfo.Create(referenceTracker, x.ParameterType, includePropertyInfos: false)).ToList();
        }

        // TODO: replace binding flags by bool flags
        protected MethodBaseInfo(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, Dictionary<Type, TypeInfo> referenceTracker)
            : this(
            name, 
            TypeInfo.Create(referenceTracker, declaringType, includePropertyInfos: false), 
            bindingFlags,
            ReferenceEquals(null, genericArguments) ? null : genericArguments.Select(x => TypeInfo.Create(referenceTracker, x, includePropertyInfos: false)),
            ReferenceEquals(null, parameterTypes) ? null : parameterTypes.Select(x => TypeInfo.Create(referenceTracker, x, includePropertyInfos: false)))
        {
        }

        protected MethodBaseInfo(string name, TypeInfo declaringType, BindingFlags bindingFlags, IEnumerable<TypeInfo> genericArguments, IEnumerable<TypeInfo> parameterTypes)
            : base(name, declaringType)
        {
            BindingFlags = bindingFlags;

            GenericArgumentTypes = ReferenceEquals(null, genericArguments) || !genericArguments.Any()
                ? null
                : genericArguments.ToList();

            ParameterTypes = ReferenceEquals(null, parameterTypes) || !parameterTypes.Any()
                ? null
                : parameterTypes.ToList();
        }

        // TODO: replace binding flags by bool flags
        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public BindingFlags BindingFlags { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public List<TypeInfo> GenericArgumentTypes { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public List<TypeInfo> ParameterTypes { get; set; }

        public bool IsGenericMethod { get { return !ReferenceEquals(null, GenericArgumentTypes) && GenericArgumentTypes.Any(); } }

        public override string ToString()
        {
            var hasGenericArguments = !ReferenceEquals(null, GenericArgumentTypes) && GenericArgumentTypes.Any();
            return string.Format("{0}.{1}{3}{4}{5}({2})",
                DeclaringType,
                Name,
                ReferenceEquals(null, ParameterTypes) ? null : string.Join(", ", ParameterTypes.Select(x => x.ToString()).ToArray()),
                hasGenericArguments ? "<" : null,
                hasGenericArguments ? string.Join(", ", GenericArgumentTypes.Select(x => x.ToString()).ToArray()) : null,
                hasGenericArguments ? ">" : null);
        }
    }
}
﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.TypeSystem
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    [Serializable]
    [DataContract(Name = "Type", IsReference = true)]
    [DebuggerDisplay("{FullName}")]
    public class TypeInfo
    {
        private static readonly Regex _arrayNameRegex = new Regex(@"^.*\[,*\]$");

        [NonSerialized]
        private Type _type;

        public TypeInfo()
        {
        }

        public TypeInfo(Type type, bool includePropertyInfos = true)
            : this(type, includePropertyInfos, true, TypeInfo.CreateReferenceTracker<Type>())
        {
        }

        public TypeInfo(Type type, bool includePropertyInfos, bool setMemberDeclaringTypes)
            : this(type, includePropertyInfos, setMemberDeclaringTypes, TypeInfo.CreateReferenceTracker<Type>())
        {
        }

        private TypeInfo(Type type, bool includePropertyInfos, bool setMemberDeclaringTypes, Dictionary<Type, TypeInfo> referenceTracker)
        {
            if (!ReferenceEquals(null, type))
            {
                referenceTracker.Add(type, this);

                _type = type;

                Name = type.Name;

                Namespace = type.Namespace;

                if (type.IsArray)
                {
                    if (!IsArray)
                    {
                        throw new Exception("Name is not in expected format for array type");
                    }

                    type = type.GetElementType();
                }

                if (type.IsNested && !type.IsGenericParameter)
                {
                    DeclaringType = TypeInfo.Create(referenceTracker, type.DeclaringType, false, false);
                }

                IsGenericType = type.IsGenericType();

                if (IsGenericType && !type.GetTypeInfo().IsGenericTypeDefinition)
                {
                    GenericArguments = type
                        .GetGenericArguments()
                        .Select(x => TypeInfo.Create(referenceTracker, x, includePropertyInfos, setMemberDeclaringTypes))
                        .ToList();
                }

                IsAnonymousType = type.IsAnonymousType();

                if (IsAnonymousType || includePropertyInfos)
                {
                    Properties = type
                        .GetProperties()
                        .Select(x => new PropertyInfo(x.Name, TypeInfo.Create(referenceTracker, x.PropertyType, includePropertyInfos, setMemberDeclaringTypes), setMemberDeclaringTypes ? this : null))
                        .ToList();
                }
            }
        }

        public TypeInfo(TypeInfo typeInfo)
            : this(typeInfo, TypeInfo.CreateReferenceTracker<TypeInfo>())
        {
        }

        private TypeInfo(TypeInfo typeInfo, Dictionary<TypeInfo, TypeInfo> referenceTracker)
        {
            if (ReferenceEquals(null, typeInfo))
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            referenceTracker.Add(typeInfo, this);

            Name = typeInfo.Name;
            Namespace = typeInfo.Namespace;
            DeclaringType = ReferenceEquals(null, typeInfo.DeclaringType) ? null : Create(referenceTracker, typeInfo.DeclaringType);
            GenericArguments = typeInfo.GenericArguments?.Select(x => Create(referenceTracker, x)).ToList();
            IsGenericType = typeInfo.IsGenericType;
            IsAnonymousType = typeInfo.IsAnonymousType;
            Properties = typeInfo.Properties?.Select(x => new PropertyInfo(x, referenceTracker)).ToList();
            _type = typeInfo._type;
        }

        internal static Dictionary<T, TypeInfo> CreateReferenceTracker<T>()
        {
            return new Dictionary<T, TypeInfo>(ReferenceEqualityComparer<T>.Default);
        }

        internal static TypeInfo Create(Dictionary<Type, TypeInfo> referenceTracker, Type type, bool includePropertyInfos, bool setMemberDeclaringTypes)
        {
            TypeInfo typeInfo;
            if (!referenceTracker.TryGetValue(type, out typeInfo))
            {
                typeInfo = new TypeInfo(type, includePropertyInfos, setMemberDeclaringTypes, referenceTracker);
            }

            return typeInfo;
        }

        internal static TypeInfo Create(Dictionary<TypeInfo, TypeInfo> referenceTracker, TypeInfo type)
        {
            if (ReferenceEquals(null, type))
            {
                return null;
            }

            TypeInfo typeInfo;
            if (!referenceTracker.TryGetValue(type, out typeInfo))
            {
                typeInfo = new TypeInfo(type, referenceTracker);
            }

            return typeInfo;
        }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public string Namespace { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo DeclaringType { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public List<TypeInfo> GenericArguments { get; set; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public bool IsAnonymousType { get; set; }

        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public bool IsGenericType { get; set; }

        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public List<PropertyInfo> Properties { get; set; }

        public bool IsNested { get { return !ReferenceEquals(null, DeclaringType); } }

        public bool IsGenericTypeDefinition { get { return !GenericArguments?.Any() ?? true; } }

        public bool IsArray
        {
            get
            {
                var name = Name;
                return !ReferenceEquals(null, name) && _arrayNameRegex.IsMatch(name);
            }
        }

        public string FullName
        {
            get
            {
                if (IsNested)
                {
                    return $"{DeclaringType.FullName}+{Name}";
                }
                else
                {
                    return $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? null : ".")}{Name}";
                }
            }
        }

        /// <summary>
        /// Resolves this type info instance to it's type using the default type resolver instance.
        /// </summary>
        public Type Type
        {
            get
            {
                if (ReferenceEquals(null, _type))
                {
                    _type = TypeResolver.Instance.ResolveType(this);
                }

                return _type;
            }
        }

        public override string ToString()
            => $"{FullName}{GetGenericArgumentsString()}";

        private string GetGenericArgumentsString()
        {
            var genericArguments = GenericArguments;
            var genericArgumentsString = IsGenericType && (genericArguments?.Any() ?? false)
                ? string.Format("[{0}]", string.Join(",", genericArguments.Select(x => x.ToString()).ToArray()))
                : null;
            return genericArgumentsString;
        }

        public static explicit operator Type(TypeInfo t)
        {
            return t.Type;
        }
    }
}

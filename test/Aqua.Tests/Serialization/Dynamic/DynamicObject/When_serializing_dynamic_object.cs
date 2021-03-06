﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.Tests.Serialization.Dynamic.DynamicObject
{
    using Aqua.Dynamic;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Xunit;
    using TypeInfo = Aqua.TypeSystem.TypeInfo;

    public abstract class When_serializing_dynamic_object
    {
        public class JsonSerializer : When_serializing_dynamic_object
        {
            public JsonSerializer() : base(JsonSerializationHelper.Serialize) { }
        }

        public class DataContractSerializer : When_serializing_dynamic_object
        {
            public DataContractSerializer() : base(DataContractSerializationHelper.Serialize) { }
        }

        // NOTE: XML serialization doesn't support circular references
        //public class XmlSerializer : When_serializing_dynamic_object
        //{
        //    public XmlSerializer() : base(XmlSerializationHelper.Serialize) { }
        //}

#if NET
        public class BinaryFormatter : When_serializing_dynamic_object
        {
            public BinaryFormatter() : base(BinarySerializationHelper.Serialize) { }
        }

        public class NetDataContractSerializer : When_serializing_dynamic_object
        {
            public NetDataContractSerializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }
#endif

        class A<T>
        {
            public T Value { get; set; }
        }

        private static readonly MethodInfo SerializeMethod = typeof(When_serializing_dynamic_object)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(x => x.Name == nameof(Serialize) && x.GetGenericArguments().Length == 1);

        private static readonly MethodInfo SerializeAsPropertyMethod = typeof(When_serializing_dynamic_object)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(x => x.Name == nameof(SerializeAsProperty) && x.GetGenericArguments().Length == 1);

        private readonly Func<DynamicObject, DynamicObject> _serialize;

        protected When_serializing_dynamic_object(Func<DynamicObject, DynamicObject> serialize)
        {
            _serialize = serialize;
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        [MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_serialize_value(Type type, object value)
        {
            var result = SerializeMethod.MakeGenericMethod(type).Invoke(this, new[] { value, true });
            result.ShouldBe(value, $"type: {type.FullName}");
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        [MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_serialize_value_as_property(Type type, object value)
        {
            var result = SerializeAsPropertyMethod.MakeGenericMethod(type).Invoke(this, new[] { value, true, false });
            result.ShouldBe(value, $"type: {type.FullName}");
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        [MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_serialize_value_as_property_when_using_string_formatting(Type type, object value)
        {
            var result = SerializeAsPropertyMethod.MakeGenericMethod(type).Invoke(this, new[] { value, true, true });
            result.ShouldBe(value, $"type: {type.FullName}");
        }

        [Fact]
        public void List_of_nullable_int_should_serialize()
        {
            var result = Serialize<IEnumerable<int?>>(new List<int?> { null, 1, 11 });
        }

        [Fact]
        public void Array_of_int_should_serialize()
        {
            var result = Serialize(new int[] { 1, 11 });
        }

        [Fact]
        public void Array_of_nullable_int_should_serialize()
        {
            var result = Serialize(new int?[] { null, 1, 11 });
        }

        [Fact]
        public void Three_dimensional_array_of_nullable_int_should_serialize()
        {
            var array = new int?[,,]
            {
                {
                    { null, 8, 1, 1, 1 }, { 1, 8, 1, 1, 1 }, { 11, 8, 1, 1, 1 },
                    { 8, null, 1, 1, 1 }, { 8, 1, 1, 1, 1 }, { 8, 11, 1, 1, 1 },
                },
                {
                    { null, 4, 1, 1, 1 }, { 1, 4, 1, 1, 1 }, { 11, 4, 1, 1, 1 },
                    { null, 4, 1, 1, 1 }, { 1, 4, 1, 1, 1 }, { 11, 4, 1, 1, 1 },
                }
            };

            var result = Serialize<int?[], int?[,,]>(array);
            result.Length.ShouldBe(60);
        }

        [Fact]
        public void Array_of_char_should_serialize()
        {
            var result = Serialize(new[] { 'h', 'e', 'l', 'l', 'o' });
        }

        [Fact]
        public void Nested_anonymous_type_with_nullable_int_property_should_serialize()
        {
            int? i = 1;
            sbyte? b1 = -2;
            byte b2 = 2;

            var result = Serialize(new { B = b1, N = new { I = i, B = b2 } });
        }

        private T Serialize<T>(T value, bool setTypeFromGenericArgument = true)
            => Serialize<T, T>(value, setTypeFromGenericArgument);

        private T SerializeAsProperty<T>(T value, bool setTypeFromGenericArgument = true, bool formatValuesAsStrings = false)
            => Serialize<A<T>, A<T>>(new A<T> { Value = value }, setTypeFromGenericArgument).Value;

        private TResult Serialize<TResult, TSource>(TSource value, bool setTypeFromGenericArgument = true, bool formatValuesAsStrings = false)
        {
            var settings = new DynamicObjectMapperSettings { FormatPrimitiveTypesAsString = formatValuesAsStrings };
            var dynamicObject = new DynamicObjectMapper(settings).MapObject(value);
            if (dynamicObject != null && setTypeFromGenericArgument)
            {
                dynamicObject.Type = new TypeInfo(typeof(TSource), false);
            }

            var serializedDynamicObject = _serialize(dynamicObject);
            
            var resurectedValue = new DynamicObjectMapper().Map<TResult>(serializedDynamicObject);
            return resurectedValue;
        }
    }
}

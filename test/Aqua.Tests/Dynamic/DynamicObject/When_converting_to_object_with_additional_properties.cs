﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.Tests.Dynamic.DynamicObject
{
    using Aqua.Dynamic;
    using System;
    using Xunit;
    using Shouldly;

    public class When_converting_to_object_with_additional_properties
    {
        class CustomType
        {
            public int Int32Value { get; set; }
            public double DoubleValue { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public string StringValue { get; set; }
        }

        const int Int32Value = 11;
        const string StringValue = "eleven";

        CustomType obj;

        public When_converting_to_object_with_additional_properties()
        {
            var dynamicObject = new DynamicObject
            {
                Properties = new PropertySet
                {
                    { "Int32Value", Int32Value },
                    { "StringValue", StringValue },
                }
            };

            obj = dynamicObject.CreateObject<CustomType>();
        }

        [Fact]
        public void Should_create_an_instance()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_have_the_int_property_set()
        {
            obj.Int32Value.ShouldBe(Int32Value);
        }

        [Fact]
        public void Should_have_the_double_property_not_set()
        {
            obj.DoubleValue.ShouldBe(default(double));
        }

        [Fact]
        public void Should_have_the_date_property_not_set()
        {
            obj.NullableDateTime.ShouldBeNull();
        }

        [Fact]
        public void Should_have_the_string_property_set()
        {
            obj.StringValue.ShouldBe(StringValue);
        }
    }
}

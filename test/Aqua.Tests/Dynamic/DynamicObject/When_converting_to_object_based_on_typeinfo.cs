﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.Tests.Dynamic.DynamicObject
{
    using Aqua.Dynamic;
    using System;
    using Xunit;
    using Shouldly;

    public class When_converting_to_object_based_on_typeinfo
    {
        class CustomType
        {
            public string StringValue { get; set; }
        }

        const string StringValue = "eleven";

        CustomType obj;

        public When_converting_to_object_based_on_typeinfo()
        {
            var dynamicObject = new DynamicObject(typeof(CustomType))
            {
                Properties = new PropertySet
                {
                    { "StringValue", StringValue },
                }
            };

            obj = dynamicObject.CreateObject() as CustomType;
        }

        [Fact]
        public void Should_create_an_instance_of_the_expected_type()
        {
            obj.ShouldNotBeNull();
        }

        [Fact]
        public void Should_have_the_string_property_set()
        {
            obj.StringValue.ShouldBe(StringValue);
        }
    }
}
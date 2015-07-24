﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.Tests.TypeSystem.TypeInfo
{
    using System.Linq;
    using Aqua.TypeSystem;
    using Xunit;
    using Xunit.Should;

    public class When_creating_type_info_of_generic_type
    {
        class A<T>
        {
            public int Int32Value { get; set; }
        }

        class B
        {
            public string StringValue { get; set; }
        }

        private readonly TypeInfo typeInfo;


        public When_creating_type_info_of_generic_type()
        {
            typeInfo = new TypeInfo(typeof(A<B>));
        }

        [Fact]
        public void Type_info_should_have_is_array_false()
        {
            typeInfo.IsArray.ShouldBeFalse();
        }

        [Fact]
        public void Type_info_should_have_is_generic_true()
        {
            typeInfo.IsGenericType.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_should_have_is_nested_true()
        {
            typeInfo.IsNested.ShouldBeTrue();
        }

        [Fact]
        public void Type_info_name_should_have_array_brackets()
        {
            typeInfo.Name.ShouldBe("A`1");
        }

        [Fact]
        public void Type_info_should_contain_property()
        {
            typeInfo.Properties.Single().Name.ShouldBe("Int32Value");
        }

        [Fact]
        public void Type_info_should_contain_generic_argument_type()
        {
            typeInfo.GenericArguments.Single().Name.ShouldBe("B");
        }

        [Fact]
        public void Generic_argument_type_should_contain_property()
        {
            typeInfo.GenericArguments.Single().Properties.Single().Name.ShouldBe("StringValue");
        }
    }
}
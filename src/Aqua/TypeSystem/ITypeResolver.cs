﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.TypeSystem
{
    using System;

    public interface ITypeResolver
    {
        Type ResolveType(TypeInfo typeInfo);
    }
}

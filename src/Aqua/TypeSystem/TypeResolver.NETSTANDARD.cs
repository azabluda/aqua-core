﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if NETSTANDARD && !NETSTANDARD1_3

namespace Aqua.TypeSystem
{
    using Microsoft.Extensions.DependencyModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    partial class TypeResolver
    {
        private readonly Lazy<IEnumerable<Assembly>> _assemblies = new Lazy<IEnumerable<Assembly>>(() =>
            {
                return DependencyContext.Default.RuntimeLibraries
                    .Select(library =>
                    {
                        try
                        {
                            return Assembly.Load(new AssemblyName(library.Name));
                        }
                        catch
                        {
                            return null;
                        }
                    })
                    .Where(assembly => assembly != null)
                    .ToArray();
            }, 
            true);

        protected virtual IEnumerable<Assembly> GetAssemblies() => _assemblies.Value;
    }
}

#endif
# aqua-core

| branch | package | AppVeyor | Travis CI |
| --- | --- | --- | --- |
| `master` | [![NuGet Badge](https://buildstats.info/nuget/aqua-core?includePreReleases=true)](http://www.nuget.org/packages/aqua-core) [![MyGet Pre Release](http://img.shields.io/myget/aqua/vpre/aqua-core.svg?style=flat-square&label=myget)](https://www.myget.org/feed/aqua/package/nuget/aqua-core) | [![Build status](https://ci.appveyor.com/api/projects/status/98rc3yav530hlw1c/branch/master?svg=true)](https://ci.appveyor.com/project/6bee/aqua-core) | [![Travis build Status](https://travis-ci.org/6bee/aqua-core.svg?branch=master)](https://travis-ci.org/6bee/aqua-core?branch=master) |


Transform any object-graph into a dynamic, composed dictionaries like structure, holding serializable values and type information.


Aqua-core provides a bunch of serializable classes:
* `DynamicObject`
* `TypeInfo`
* `FieldInfo`
* `PropertyInfo`
* `MethodInfo`
* `ConstructorInfo`

Any object graph may be translated into a `DynamicObject` structure and back to it's original type using `DynamicObjectMapper`.


## Sample

Mapping an object graph into a `DynamicObject` and then back to it's original type
```C#
Blog blog = new Blog
{
    Title = ".NET Blog",
    Description = "A first-hand look from the .NET engineering teams",
    Posts = new[]
    {
        new Post
        {
            Title = "Announcing .NET Core 1.0",
            Date = new DateTime(2016, 6, 27),
            Author = "rlander"
            Text = "We are excited to announce the release of .NET Core 1.0, ASP.NET Core 1.0 and Entity Framework Core 1.0, available on Windows, OS X and Linux! .NET Core is a cross-platform, open source, and modular .NET platform [...]"
        },
        new Post
        {
            Title = "Happy 15th Birthday .NET!",
            Date = new DateTime(2017, 2, 13),
            Author = "bmassi",
            Text = "Today marks the 15th anniversary since .NET debuted to the world [...]"
        }
    }
}

DynamicObject dynamicObject = new DynamicObjectMapper().MapObject(blog);

Blog restoredBlog = new DynamicObjectMapper().Map(dynamicObject) as Blog;
```
# MeyerCorp.HateoasBuilder

A .NET Standard Library allowing convenient creation of HATEOAS for REST API models created by developers at [Meyer Corporation](https://meyerus.com).

## Background

HATEOAS or Hypermedia as the Engine of Application State is a helpful feature of REST and one many consider necessary to implement REST propertly. It can be often overlooked by backend developers as not critical but is very helpful to SPA development as API responses will contain predictable object schemas with URLs that can be used in pages of the SPA and linked pages ad infinitum.

In the following #### Example, data is returned as an array of items. Each item has a description as well as an array of links. Only one links is necessary to inform the consumer that where the details for each item can be found. Then there is an array of links describing how to retrieve the previous page or data, the next page, and this page.

```JSON
{
    "data":[
        {
            "description":"first item",
            "links":[       
                {
                    "href":"https://foo.bar/api/item/1",
                    "rel":"self",
                    "type":"GET"
                }
            ]
        },
        {
            "description":"second item",
            "links":[       
                {
                    "href":"https://foo.bar/api/item/2",
                    "rel":"self",
                    "type":"GET"
                }
            ]
        },
        {
            "description":"third item",
            "links":[       
                {
                    "href":"https://foo.bar/api/item/3",
                    "rel":"self",
                    "type":"GET"
                }
            ]
        },
    ],
    "links":[
        {
            "href":"https://foo.bar/api/main?page=2",
            "rel":"self",
            "type":"GET"
        },
        {
            "href":"https://foo.bar/api/main?page=1",
            "rel":"previous",
            "type":"GET"
        },
        {
            "href":"https://foo.bar/api/main?page=3",
            "rel":"next",
            "type":"GET"
        }
    ]
}
```

I am not trying to convince anyone that they need to use HATEOAS, but if you do, and you are creating APIs in .NET, this can make it far more convenient.

## Getting Started

Add the [nuget package](https://www.nuget.org/packages/MeyerCorp.HateoasBuilder) to your .NET web application.

When returning data as a model in a method of a Web API controller, start by using one of the extension methods to create a link array.

```C#

// Some controller method
[HttpGet("all")]
public object GetAll()
{
   // The relative URL of the link target
    var relativeUrl1 = "employees?page=1"
    var relativeUrl2 = "employees?page=2"
    // Create a link array that has links for pagination to that relative URL
    var links = HttpContext
        .AddLink("previous", relativeUrl1)
        .AddLink("next", relativeUrl2)
        .Build();

    return new
    {
        Links = links,
        Results = Enumerable.Range(1, 6).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
           
            // Here we use a format string to create our link
            // http://base.url/WeatherForecast/1
            Links = HttpContext.AddFormattedLink("self", "WeatherForecast/{0}", index).Build(),
        })
        .ToArray()
    };
}
```

By using the extension method for the `HttpContext`, the link builder is able to determine the base URL and append the employees route. This allow you to not worry what the base URL as the `HttpContext` knows this and links can be created dynamically.

## Methods

Various methods allow convenient creation of links. They are all methods of the LinkBuilder class as well as complimentary extension methods that allow initializing the LinkBuilder object starting with a base URL string or the HttpContext property of a Web API controller.

### AddLink

`AddLink` allows you to add a raw relative URL to the base URL which is either extracted from the `HttpContext` or a string.

```C#
"https://foo.bar".AddLink("label", "relativeLinkWithRoutesAndQueries");

// or
this.HttpContext
    .AddLink("previous", "data?page=1") //https://foobar/data?page=1
    .AddLink("self", "data?page=2") //https://foobar/data?page=2
    .AddLink("next", "data?page=3"); //https://foobar/data?page=3
```

### AddRouteLink

`AddRouteLink` allows you to add a relative URL to the base URL which is either extracted from the `HttpContext` or a string and add as many route items as you like appended to that.

#### #### Example

```C#
"https://foo.bar".AddRouteLink("label", "relativeUrl", "route", 1, "subroute", 2);

// or
this.HttpContext
    .AddRouteLink("employees", "id", 1, "dateOfHire") //https://foobar/employees/id/1/dateOfHire
    .AddRouteLink("locations", "id", 2, "address") //https://foobar/employees/id/2/address
    .AddRouteLink("products", "id", 3, "price"); //https://foobar/employees/id/3/price
```

### AddQueryLink

`AddRouteLink` allows you to add a relative URL to the base URL which is either extracted from the `HttpContext` or a string and add as many route items as you like appended to that.

#### Example

```C#
"https://foo.bar".AddRouteLink("label", "relativeUrl", "route", 1, "subroute", 2);

// or
this.HttpContext
    .AddRouteLink("employees", "id", 1, "dateOfHire") //https://foobar/employees/id/1/dateOfHire
    .AddRouteLink("locations", "id", 2, "address") //https://foobar/employees/id/2/address
    .AddRouteLink("products", "id", 3, "price"); //https://foobar/employees/id/3/price
```

### AddFormattedLink(s)

`AddRouteLink` allows you to add a relative URL to the base URL which is either extracted from the `HttpContext` or a string and add as many route items as you like appended to that.

#### Example

```C#
"https://foo.bar".AddRouteLink("label", "relativeUrl", "route", 1, "subroute", 2);

// or
this.HttpContext
    .AddRouteLink("employees", "id", 1, "dateOfHire") //https://foobar/employees/id/1/dateOfHire
    .AddRouteLink("locations", "id", 2, "address") //https://foobar/employees/id/2/address
    .AddRouteLink("products", "id", 3, "price"); //https://foobar/employees/id/3/price
```

### AddRoutes

`AddRouteLink` allows you to add a relative URL to the base URL which is either extracted from the `HttpContext` or a string and add as many route items as you like appended to that.

#### Example

```C#
"https://foo.bar".AddRouteLink("label", "relativeUrl", "route", 1, "subroute", 2);

// or
this.HttpContext
    .AddRouteLink("employees", "id", 1, "dateOfHire") //https://foobar/employees/id/1/dateOfHire
    .AddRouteLink("locations", "id", 2, "address") //https://foobar/employees/id/2/address
    .AddRouteLink("products", "id", 3, "price"); //https://foobar/employees/id/3/price
```

### AddParameters

`AddRouteLink` allows you to add a relative URL to the base URL which is either extracted from the `HttpContext` or a string and add as many route items as you like appended to that.

#### Example

```C#
"https://foo.bar".AddRouteLink("label", "relativeUrl", "route", 1, "subroute", 2);

// or
this.HttpContext
    .AddRouteLink("employees", "id", 1, "dateOfHire") //https://foobar/employees/id/1/dateOfHire
    .AddRouteLink("locations", "id", 2, "address") //https://foobar/employees/id/2/address
    .AddRouteLink("products", "id", 3, "price"); //https://foobar/employees/id/3/price
```

### Build()

`AddRouteLink` allows you to add a relative URL to the base URL which is either extracted from the `HttpContext` or a string and add as many route items as you like appended to that.

#### Example

```C#
"https://foo.bar".AddRouteLink("label", "relativeUrl", "route", 1, "subroute", 2);

// or
this.HttpContext
    .AddRouteLink("employees", "id", 1, "dateOfHire") //https://foobar/employees/id/1/dateOfHire
    .AddRouteLink("locations", "id", 2, "address") //https://foobar/employees/id/2/address
    .AddRouteLink("products", "id", 3, "price"); //https://foobar/employees/id/3/price
```

## Glossary

REST
SPA

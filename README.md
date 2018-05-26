# ![alt text](https://raw.githubusercontent.com/Boriszn/LogAnalyzer/Moving-To-New-MongoDb-Driver/assets/images/Logo.png "Main Dashboard") [![Build status](https://ci.appveyor.com/api/projects/status/q8yuymqhiibd39hw?svg=true)](https://ci.appveyor.com/project/Boriszn/loganalyzer)

Solution provides opportunities for retrieving and analysing log data from NoSQL data bases (MongoDb, CouchDb).

Includes functions:
 - querying and searching data
 - multidimensional objects rendering;
 - errors and info’s analyzing charts;
 - Real-Time data observing and updating;

![alt text](https://raw.githubusercontent.com/Boriszn/LogAnalyzer/Moving-To-New-MongoDb-Driver/assets/images/Index.png "Main Dashboard")

## TODO

[Project link](https://github.com/Boriszn/LogAnalyzer/projects/1)

## Data Model

JSON example
```javascript
{
  "Message": "test 2",
  "Level": "Error",
  "Name": "Test name",
  "LastInfo": { /* Can contain ANY object with ANY dimension  */
    "Level": "Low",
    "Count": 1,
    "Stack": {
      "Test": "one"
    }
  },
  "Email": "none",
  "VisitDate": "2018-05-21T02:20:00.000+0000"
}
```

## Future/To-Do

![alt text](https://raw.githubusercontent.com/Boriszn/LogAnalyzer/Moving-To-New-MongoDb-Driver/assets/images/ErrorList.png "Main Dashboard")

## Stack

Front-End
 - AngularJS, 
 - Highcharts, 
 - HTML5/CSS-3, Jquery, 

Back-End
- .NETFramework 4.6, C#, WebAPI, Swagger
- ASP.NET MVC 4
- SignalR,

DB
- MongoDB

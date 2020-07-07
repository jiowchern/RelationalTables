# Relational Tables
A common data table creation tool for the game industry.

## Introduce
Often, working in the game industry requires that the game designer take control of the game configuration.  
However, the difficulty of facing various data table-read associations was the reason for the creation of this tool.
## How to use
Define a type.
```csharp
public class TestConfig1 
{        
    public int Field1;
    public string Field2;
    public float Field3;
}
```
Query from database.
```csharp
var db = new Regulus.RelationalTables.Database(new ITableQueryable[] { /*Set up provider...*/ });
var config = db.Query<TestConfig1>().First();
```
## Set up provider.
```csharp

```



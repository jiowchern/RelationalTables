# Relational Tables
[![Maintainability](https://api.codeclimate.com/v1/badges/bdd76b4da517a29323bf/maintainability)](https://codeclimate.com/github/jiowchern/RelationalTables/maintainability)
[![Coverage Status](https://coveralls.io/repos/github/jiowchern/RelationalTables/badge.svg?branch=master)](https://coveralls.io/github/jiowchern/RelationalTables?branch=master)  
A simple game development spreadsheet conversion tool.  

## Introduce
Often, working in the game industry requires that the game designer take edit of the game configuration.  
However, the difficulty of facing various data table-read associations was the reason for the creation of this tool.

## Usage
If you have a table like it.

|Field1   |Field2   |Field3   |
|-|-|-|
|1|2|3|  

Define the class.
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
var db = new Regulus.RelationalTables.Database(new IRowProvidable[] { /*Data source ...*/ });
var config = db.Query<TestConfig1>().First();
// config.Field1 == 1
// config.Field2 == "2"
// config.Field3 == 3f
```

## Types of support
**Separate tables.**

|Field1   |Field2   |Field3   |
|-|-|-|
|1|2|3|  

```csharp
public class TestConfig1 
{        
    public int Field1;
    public string Field2;
    public float Field3;
}
```

**Array field.**

|Field1   |Field2   |Field3   |
|-|-|-|
|1|2|3|  

```csharp
public class TestConfig1 
{ 
    [Regulus.RelationalTables.Attributes.Merge("Field1","Field2","Field3")]       
    public int[] Field;    
}
```
**Related Table**

Table A

|Field1   |Field2   |Field3   |
|-|-|-|
|1|2|3|  

Table B

|Field1   |Field2   |
|-|-|
|1|1|

```csharp
public class TableA : Regulus.RelationalTables.IRelatable
{        
    public int Field1;
    public string Field2;
    public float Field3;

    bool IRelatable.Compare(string val)
    {
        int outVal;
        if (int.TryParse(val, out outVal))
        {
            return outVal == Field1;
        }
        return false;
    }

}

public class TableB
{        
    public int Field1;
    public TestConfig1 Field2;    
}
```
**Inversely related**  
Table A

|Table2s|
|-|
|1|  

Table B

|Owner|Data|
|-|-|
|1|1|
|1|2|
```csharp
class TableA
{            
    [Attributes.InverselyRelated]
    public TableB[] Table2s;
}

class TableB : IRelatable
{
    public int Owner;
    public int Data;
    bool IRelatable.Compare(string val)
    {
        int owner;
        if (int.TryParse(val , out owner))
        {
            return owner == Owner;
        }
        return false;
    }
}
```

**Inversely related by column**  
Table A

|Id|
|-|
|1|  

Table B

|Owner|Data|
|-|-|
|1|1|
|1|2|
```csharp
class TableA
{            
    [Attributes.InverselyRelatedByColumn("Id")]
    public TableB[] Table2s;
}

class TableB : IRelatable
{
    public int Owner;
    public int Data;
    bool IRelatable.Compare(string val)
    {
        int owner;
        if (int.TryParse(val , out owner))
        {
            return owner == Owner;
        }
        return false;
    }
}
```


**Custom Parser**

```csharp
public class CustomFieldParser : Regulus.RelationalTables.Attributes.FieldParser
{
    public override object Parse(FieldInfo field, IEnumerable<Column> row, ITableable table)
    {
        //todo : Implement your method... 
    }
}

public class Table
{
    [CustomFieldParser()]
    public int Field1;        
}
```
### Create database
Whether you are using Excel, CSV, Google Sheet, or another spreadsheet source, you just need to implement the following interface...  


**Row provider**  
```csharp
namespace Regulus.RelationalTables.Raw
{
    public interface IRowProvidable
    {
        Type GetTableType();
        IEnumerable<IColumnProvidable> GetRows();
    }
}
```
**Column provider**
```csharp
namespace Regulus.RelationalTables.Raw
{
    public interface IColumnProvidable
    {
        IEnumerable<Column> GetColumns();
    }
}
```
**New a database**
```csharp
var db = new Regulus.RelationalTables.Database(/* IRowProvidable */);
```



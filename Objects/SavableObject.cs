using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Library
{
  public class SavableObject<T> where T : SavableObject<T>
  {
    private int _id;
    public int id
    {
      get
      {
        return _id;
      }
      set
      {
        _id = value;
      }
    }
    public void Save()
    {
      PropertyInfo[] properties = this.GetType().GetProperties();

      List<string> parameterNameList = new List<string> {};
      List<object> parameterValueList = new List<object> {};
      string commandString = "INSERT INTO "+typeof(T).GetProperty("table").GetValue(null,null)+" (";
      for(int i=0; i<properties.Length; i++)
      {
        if(properties[i].Name != "id" && properties[i].Name != "table")
        {
          commandString += properties[i].Name + ", ";
          string parameterName = "@"+properties[i].Name;
          parameterNameList.Add(parameterName);
          parameterValueList.Add(properties[i].GetValue(this));
        }
      }
      string parameterNameString = "("+String.Join(", ", parameterNameList.ToArray())+")" ;
      commandString = commandString.Substring(0,commandString.Length-2) + ") OUTPUT INSERTED.id VALUES "+parameterNameString+";";
      DBObjects dbo = DBObjects.CreateCommand(commandString, parameterNameList, parameterValueList);
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      while(rdr.Read())
      {
        _id = rdr.GetInt32(0);
      }

      dbo.Close();
    }
    public void Update(List<string> updateColumns, List<object> updateValues)
    {
      for(int i=0; i<updateColumns.Count; i++)
      {
        string currentColumn = updateColumns[i];
        DBObjects dbo = DBObjects.CreateCommand("UPDATE "+typeof(T).GetProperty("table").GetValue(null,null)+" SET "+currentColumn+" = @NewValue WHERE id=@Id;", new List<string> {"@NewValue", "@Id"}, new List<object> {updateValues[i], _id});
        dbo.CMD.ExecuteNonQuery();
        PropertyInfo propertyInfo = this.GetType().GetProperty(currentColumn);
        propertyInfo.SetValue(this, Convert.ChangeType(updateValues[i], propertyInfo.PropertyType), null);
        dbo.Close();
      }
    }
    public static T Find(int findId)
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT * FROM "+typeof(T).GetProperty("table").GetValue(null,null)+" WHERE id=@Id;", new List<string> {"@Id"},  new List<object> {findId});
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      T result = null;
      while(rdr.Read())
      {
        result = ConstructFromDatabase(rdr);
      }

      dbo.Close();
      return result;
    }
    public void Delete(string[] tables, string[] idColumns)
    {
      DBObjects dbo = DBObjects.CreateCommand("DELETE FROM "+typeof(T).GetProperty("table").GetValue(null,null)+" WHERE id=@Id;", new List<string> {"@Id"},  new List<object> {id});
      dbo.CMD.ExecuteNonQuery();
      dbo.Close();
      for(int i=0; i<tables.Length; i++)
      {
        dbo = DBObjects.CreateCommand("DELETE FROM "+tables[i]+" WHERE "+idColumns[i]+"=@Id;", new List<string> {"@Id"},  new List<object> {id});
        dbo.CMD.ExecuteNonQuery();
        dbo.Close();
      }
    }
    public static List<T> GetAll()
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT * FROM "+typeof(T).GetProperty("table").GetValue(null,null)+";");
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      List<T> all = new List<T> {};
      while(rdr.Read())
      {
        all.Add(ConstructFromDatabase(rdr));
      }

      dbo.Close();
      return all;
    }
    public static void DeleteAll(string[] tables)
    {
      DBObjects dbo = DBObjects.CreateCommand("DELETE FROM "+typeof(T).GetProperty("table").GetValue(null,null)+";");
      dbo.CMD.ExecuteNonQuery();
      dbo.Close();
      foreach(string tableName in tables)
      {
        dbo = DBObjects.CreateCommand("DELETE FROM "+tableName+";");
        dbo.CMD.ExecuteNonQuery();
        dbo.Close();
      }
    }
    private static T ConstructFromDatabase(SqlDataReader rdr)
    {
      PropertyInfo[] propertiesArray = typeof(T).GetProperties();
      Array.Sort(propertiesArray,
        delegate(PropertyInfo propertyInfo1, PropertyInfo propertyInfo2)
          {
            return propertyInfo1.Name.CompareTo(propertyInfo2.Name);
          });

      List<PropertyInfo> properties = new List<PropertyInfo>(propertiesArray);
      properties.Remove(typeof(T).GetProperty("id"));
      properties.Remove(typeof(T).GetProperty("table"));
      object[] propertyValues = new object[properties.Count+1];
      for(int i=0; i<properties.Count; i++)
      {
        propertyValues[i] = GetDatabaseValue(rdr, i+1, properties[i].PropertyType);
      }
      propertyValues[propertyValues.Length-1] = rdr.GetInt32(0);
      return (T)Activator.CreateInstance(typeof(T), propertyValues);
    }
    private static object GetDatabaseValue(SqlDataReader rdr, int index, Type t)
    {
      if(t.ToString()=="System.Int32")
      {
        return rdr.GetInt32(index);
      }
      else if(t.ToString()=="System.String")
      {
        return rdr.GetString(index);
      }
      else if(t.ToString()=="System.DateTime")
      {
        return rdr.GetDateTime(index);
      }
      return null;
    }
  }
}

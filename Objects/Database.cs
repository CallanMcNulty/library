using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class DB
  {
    public static SqlConnection Connection()
    {
      SqlConnection conn = new SqlConnection(DBConfiguration.ConnectionString);
      return conn;
    }
  }
  public class DBObjects
  {
    public SqlConnection CONN;
    public SqlDataReader RDR;
    public SqlCommand CMD;
    public DBObjects(SqlConnection Conn, SqlDataReader Rdr, SqlCommand Cmd)
    {
      CONN = Conn;
      RDR = Rdr;
      CMD = Cmd;
    }
    public static DBObjects CreateCommand(string command, List<string> paramNames = null, List<object> paramValues = null)
    {
      paramNames = paramNames ?? new List<string> {};
      paramValues = paramValues ?? new List<object> {};
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;

      SqlCommand cmd = new SqlCommand(command, conn);

      for(int i=0; i<paramNames.Count; i++)
      {
        SqlParameter newParameter = new SqlParameter();
        newParameter.ParameterName = paramNames[i];
        newParameter.Value = paramValues[i];
        cmd.Parameters.Add(newParameter);
      }
      return new DBObjects(conn, rdr, cmd);
    }
    public void Close()
    {
      if (RDR != null)
      {
        RDR.Close();
      }
      if (CONN != null)
      {
        CONN.Close();
      }
    }
  }
}

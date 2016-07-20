using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class Patron : SavableObject<Patron>
  {
    private int _balance;
    private string _name;
    private string _notes;
    public static string table
    {
      get
      {
        return "patrons";
      }
    }
    public string name
    {
      get
      {
        return _name;
      }
      set
      {
        _name = value;
      }
    }
    public string notes
    {
      get
      {
        return _notes;
      }
      set
      {
        _notes = value;
      }
    }
    public int balance
    {
      get
      {
        return _balance;
      }
      set
      {
        _balance = value;
      }
    }
    public Patron(int Balance, string Name, string Notes, int Id=0)
    {
      id = Id;
      _name = Name;
      _notes = Notes;
      _balance = Balance;
    }
    public List<Copy> GetItemsOut()
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT * FROM copies WHERE patron_id=@Id;", new List<string> {"@Id"}, new List<object> {id});
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      List<Copy> allCopies = new List<Copy> {};
      while(rdr.Read())
      {
        allCopies.Add(new Copy(rdr.GetInt32(1), rdr.GetDateTime(2), rdr.GetInt32(3), rdr.GetInt32(0) ));
      }
      dbo.Close();
      return allCopies;
    }
    public void Checkout(int CopyId)
    {
      Copy checkoutItem = Copy.Find(CopyId);
      checkoutItem.Update(new List<string> {"due_date", "patron_id"}, new List<object> {DateTime.Today.AddDays(21), id});

      DBObjects dbo = DBObjects.CreateCommand("INSERT INTO checkouts (copy_id, patron_id) VALUES (@copy_id, @patron_id);", new List<string> {"@copy_id", "@patron_id"}, new List<object> {CopyId, id});
      dbo.CMD.ExecuteNonQuery();
      dbo.Close();
    }
    public List<Copy> GetHistory()
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT copies.* FROM patrons JOIN checkouts ON (patrons.id=checkouts.patron_id) JOIN copies ON (checkouts.copy_id=copies.id) WHERE patrons.id=@Id;", new List<string> {"@Id"}, new List<object> {id});
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      List<Copy> allCopies = new List<Copy> {};
      while(rdr.Read())
      {
        allCopies.Add(new Copy(rdr.GetInt32(1), rdr.GetDateTime(2), rdr.GetInt32(3), rdr.GetInt32(0) ));
      }
      dbo.Close();
      return allCopies;
    }
  }
}

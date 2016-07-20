using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class Author : SavableObject<Author>
  {
    private string _name;
    public static string table
    {
      get
      {
        return "authors";
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
    public Author(string Name, int Id=0)
    {
      id = Id;
      _name = Name;
    }
    public List<Book> GetWorks()
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT books.* FROM authors JOIN authors_books ON (authors.id=authors_books.author_id) JOIN books ON (authors_books.book_id=books.id) WHERE authors.id=@Id;", new List<string> {"@Id"}, new List<object> {id});
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      List<Book> works = new List<Book> {};
      while(rdr.Read())
      {
        works.Add(new Book(rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt32(0) ));
      }
      dbo.Close();
      return works;
    }
    public static Author FindByName(string Name)
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT * FROM authors WHERE name=@Name;", new List<string> {"@Name"}, new List<object> {Name});
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      Author result = null;
      while(rdr.Read())
      {
         result = new Author(rdr.GetString(1), rdr.GetInt32(0) );
      }
      dbo.Close();
      return result;
    }
  }
}

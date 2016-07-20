using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class Book : SavableObject<Book>
  {
    private string _callNumber;
    private string _collection;
    private string _title;
    public static string table
    {
      get
      {
        return "books";
      }
    }
    public string title
    {
      get
      {
        return _title;
      }
      set
      {
        _title = value;
      }
    }
    public string call_number
    {
      get
      {
        return _callNumber;
      }
      set
      {
        _callNumber = value;
      }
    }
    public string collection
    {
      get
      {
        return _collection;
      }
      set
      {
        _collection = value;
      }
    }
    public Book(string CallNumber, string Collection, string Title, int Id=0)
    {
      id = Id;
      _callNumber = CallNumber;
      _collection = Collection;
      _title = Title;
    }
    public List<Copy> GetCopies()
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT * FROM copies WHERE book_id=@Id;", new List<string> {"@Id"}, new List<object> {id});
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
    public void AddAuthor(int AuthorId)
    {
      DBObjects dbo = DBObjects.CreateCommand("INSERT INTO authors_books (author_id, book_id) VALUES (@AuthorId, @Id);", new List<string> {"@AuthorId", "@Id"}, new List<object> {AuthorId, id});
      dbo.CMD.ExecuteNonQuery();
      dbo.Close();
    }
    public List<Author> GetAuthors()
    {
      DBObjects dbo = DBObjects.CreateCommand("SELECT authors.* FROM books JOIN authors_books ON (books.id=authors_books.book_id) JOIN authors ON (authors_books.author_id=authors.id) WHERE books.id=@Id;", new List<string> {"@Id"}, new List<object> {id});
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();

      List<Author> authors = new List<Author> {};
      while(rdr.Read())
      {
        authors.Add(new Author(rdr.GetString(1), rdr.GetInt32(0) ));
      }
      dbo.Close();
      return authors;
    }
    public static List<Book> Search(string search, string searchdef)
    {
      DBObjects dbo = null;
      if(searchdef=="title")
      {
        dbo = DBObjects.CreateCommand("SELECT * FROM books WHERE title=@SearchResult;", new List<string> {"@SearchResult"}, new List<object> {search});
      }
      else if(searchdef=="author")
      {
        dbo = DBObjects.CreateCommand("SELECT books.* FROM authors JOIN authors_books ON (authors.id=authors_books.author_id) JOIN books ON (authors_books.book_id=books.id) WHERE authors.name=@SearchResult;", new List<string> {"@SearchResult"}, new List<object> {search});
      }
      SqlDataReader rdr = dbo.RDR;
      rdr = dbo.CMD.ExecuteReader();
      List<Book> searchResult = new List<Book> {};

      while(rdr.Read())
      {
        searchResult.Add(new Book(rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt32(0)));
      }
      dbo.Close();
      return searchResult;

    }
  }
}

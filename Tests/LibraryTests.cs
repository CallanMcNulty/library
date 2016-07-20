using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class SavableTests : IDisposable
  {
    public SavableTests()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library;Integrated Security=SSPI;";
    }
    [Fact]
    public void Test_GetCopies()
    {
      Book newbook = new Book("2a", "Sf", "The blob");
      newbook.Save();

      Copy newcopy = new Copy(newbook.id, new DateTime(2016,07,20), 1);
      newcopy.Save();

      Assert.Equal(newcopy.id, newbook.GetCopies()[0].id);
    }
    [Fact]
    public void Test_GetItemsOut()
    {
      Patron p = new Patron(0, "Pat", "");
      p.Save();

      Copy newcopy = new Copy(0, new DateTime(2016,07,20), p.id);
      newcopy.Save();

      Assert.Equal(newcopy.id, p.GetItemsOut()[0].id);
    }
    [Fact]
    public void Test_GetWorks()
    {
      Author a = new Author("Allen");
      a.Save();

      Book newbook = new Book("2a", "Sf", "The blob");
      newbook.Save();

      newbook.AddAuthor(a.id);

      Assert.Equal(newbook.id, a.GetWorks()[0].id);
    }
    [Fact]
    public void Test_GetAuthors()
    {
      Author a = new Author("Allen");
      a.Save();

      Book newbook = new Book("2a", "Sf", "The blob");
      newbook.Save();

      newbook.AddAuthor(a.id);

      Assert.Equal(a.id, newbook.GetAuthors()[0].id);
    }
    [Fact]
    public void Test_FindCopy()
    {
      Copy newcopy = new Copy(0, new DateTime(2016,07,20), 1);
      newcopy.Save();

      Assert.Equal(newcopy.id, Copy.Find(newcopy.id).id);
    }
    [Fact]
    public void Test_Checkout()
    {
      Copy newcopy = new Copy(0, new DateTime(2016,07,20), 1);
      newcopy.Save();

      Patron p = new Patron(0, "Pat", "");
      p.Save();

      p.Checkout(newcopy.id);

      Assert.Equal(p.id, Copy.Find(newcopy.id).patron_id);
    }
    [Fact]
    public void Test_GetHistory()
    {
      Copy newcopy = new Copy(0, new DateTime(2016,07,20), 1);
      newcopy.Save();

      Patron p = new Patron(0, "Pat", "");
      p.Save();

      p.Checkout(newcopy.id);

      Assert.Equal(1, p.GetHistory().Count);
    }
    [Fact]
    public void Test_Checkin()
    {
      Copy newcopy = new Copy(0, new DateTime(2016,06,27), 1);
      newcopy.Save();

      Patron p = new Patron(0, "Pat", "");
      p.Save();

      p.Checkout(newcopy.id);
      newcopy.Update(new List<string> {"due_date"}, new List<object> {new DateTime(2016,07,19)});
      Copy.Find(newcopy.id).Checkin();

      Assert.Equal(10, Patron.Find(p.id).balance);
    }
    [Fact]
    public void Test_overDueBooks()
    {
      Patron p = new Patron(0, "Pat", "");
      p.Save();

      Copy newcopy = new Copy(0, new DateTime(2016,06,27), 1);
      newcopy.Save();

      p.Checkout(newcopy.id);

      newcopy.Update(new List<string> {"due_date"}, new List<object> {new DateTime(2016,07,19)});

      List<Copy> isoverdue = Copy.OverdueBooks();

      Assert.Equal(1, isoverdue.Count);
    }

    [Fact]
    public void Test_Search()
    {
      Author a = new Author("Allen");
      a.Save();

      Book newbook = new Book("2a", "Sf", "The blob");
      newbook.Save();

      newbook.AddAuthor(a.id);

      List<Book> search= Book.Search("Allen", "author");

      Assert.Equal(1,search.Count);
    }

    public void Dispose()
    {
      Book.DeleteAll(new string[0]);
      Copy.DeleteAll(new string[0]);
      Patron.DeleteAll(new string[] {"checkouts"});
      Author.DeleteAll(new string[] {"authors_books"});
    }
  }
}

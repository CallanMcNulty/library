using System.Collections.Generic;
using System;
using Nancy;

namespace Library
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] =_=>{
        return View["index.cshtml"];
      };

      Get["/references"] =_=>{
        List<Book> search = new List<Book> {};
        return View["references.cshtml",search];
      };

      Get["/books/{id}"] =parameters=> {
        Book b = Book.Find(parameters.id);
        return View["book.cshtml", b];
      };
      Get["/copies/{id}"] =parameters=> {
        Copy c = Copy.Find(parameters.id);
        return View["copy.cshtml", c];
      };
      Delete["/copies/delete/{id}"] =parameters=> {
        Copy c = Copy.Find(parameters.id);
        c.Delete(new string[] {"checkouts"}, new string[] {"copy_id"});
        return View["book.cshtml", Book.Find(c.book_id)];
      };
      Patch["/books/edit/{id}"] =parameters=> {
        Book b = Book.Find(parameters.id);
        b.Update(new List<string> {"call_number", "collection", "title"}, new List<object> {(string)Request.Form["callNumber"], (string)Request.Form["collection"], (string)Request.Form["title"]});
        b.RemoveAuthors();
        Author a = Author.FindByName(Request.Form["author"]);
        if(a==null)
        {
          a = new Author(Request.Form["author"]);
          a.Save();
        }
        b.AddAuthor(a.id);
        return View["book.cshtml", b];
      };
      Get["/circulation"] =_=>{
        return View["circulation.cshtml"];
      };
      Post["/patron/new"] =_=> {
        Patron p = new Patron(0, (string)Request.Form["name"], "");
        p.Save();
        return View["patron.cshtml", p];
      };
      Post["/check-out/{id}"] =parameters=> {
        Patron p = Patron.Find(parameters.id);
        p.Checkout((int)Request.Form["itemId"]);
        return View["patron.cshtml", p];
      };
      Get["/check-in"] =_=> {
        return View["check-in.cshtml"];
      };
      Post["/check-in/new"] =_=> {
        int copyId = (int)Request.Form["id"];
        Copy c = Copy.Find(copyId);
        c.Checkin();
        return View["check-in.cshtml"];
      };
      Get["/check-out"] =_=> {
        return View["check-out.cshtml", ""];
      };
      Post["/patron"] =_=> {
        Patron p = Patron.Find((int)Request.Form["patronId"]);
        if(p==null)
        {
          return View["check-out.cshtml", "No Patron Found"];
        }
        else
        {
          return View["patron.cshtml", p];
        }
      };

      Get["/catalog"] =_=>{
        return View["catalog.cshtml"];
      };
      Post["books/new"] =_=> {
        Book newBook = new Book(Request.Form["callNumber"], Request.Form["collection"], Request.Form["title"]);
        newBook.Save();
        Copy newCopy = new Copy(newBook.id, new DateTime(1900,1,1), 0);
        newCopy.Save();
        Author a = Author.FindByName(Request.Form["author"]);
        if(a==null)
        {
          a = new Author(Request.Form["author"]);
          a.Save();
        }
        newBook.AddAuthor(a.id);
        return View["book.cshtml", newBook];
      };
      Post["copies/new"] =_=> {
        int bookId = (int)Request.Form["book_id"];
        Copy newCopy = new Copy(bookId, new DateTime(1900,1,1), 0);
        newCopy.Save();
        return View["copy.cshtml", newCopy];
      };

      Post["/search"] =_=> {
        string search = Request.Form["search"];
        string searchType = Request.Form["searchdef"];

        List<Book> booksearch = Book.Search(search,searchType);

        return View["references.cshtml",booksearch];
      };

    }
  }
}

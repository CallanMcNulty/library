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

      Get["/patrons"]=_=>{
        List<Book> search = new List<Book> {};
        return View["patrons.cshtml",search];
      };

      Get["/books/{id}"] =parameters=> {
        Book b = Book.Find(parameters.id);
        return View["book.cshtml", b];
      };
      Get["/patronview/books/{id}"] =parameters=> {
        Book b = Book.Find(parameters.id);
        return View["patronview-book.cshtml", b];
      };
      Get["/copies/{id}"] =parameters=> {
        Copy c = Copy.Find(parameters.id);
        return View["copy.cshtml", c];
      };
      Get["/patronview/copies/{id}"] =parameters=> {
        Copy c = Copy.Find(parameters.id);
        return View["patronview-copy.cshtml", c];
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
        for(int i = 0; i<Request.Form["number-of-authors"]; i++)
        {
          Author a = Author.FindByName(Request.Form["author" + i]);
          if(a==null)
          {
            a = new Author(Request.Form["author" + i]);
            a.Save();
          }
          b.AddAuthor(a.id);
        }
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
        Copy c = Copy.Find((int)Request.Form["itemId"]);
        if(c!=null)
        {
          p.Checkout(c.id);
        }
        return View["patron.cshtml", p];
      };
      Get["/check-in"] =_=> {
        return View["check-in.cshtml"];
      };
      Post["/check-in/new"] =_=> {
        int copyId = (int)Request.Form["id"];
        Copy c = Copy.Find(copyId);
        if(c!=null)
        {
          c.Checkin();
        }
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

      Post["/patronview"] =_=> {
        Patron p = Patron.Find((int)Request.Form["patronId"]);
        if(p==null)
        {
          return View["check-out.cshtml", "No Patron Found"];
        }
        else
        {
          return View["patronview.cshtml", p];
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
        for(int i = 0; i<Request.Form["number-of-authors"]; i++)
        {
          Author a = Author.FindByName(Request.Form["author" + i]);
          if(a==null)
          {
            a = new Author(Request.Form["author" + i]);
            a.Save();
          }
          newBook.AddAuthor(a.id);
        }
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
      Post["/patronview/search"] =_=> {
        string search = Request.Form["search"];
        string searchType = Request.Form["searchdef"];

        List<Book> booksearch = Book.Search(search,searchType);

        return View["patrons.cshtml",booksearch];
      };
      Get["/patron/edit/{id}"]=parameters=>{
        Patron findpatron = Patron.Find(parameters.id);

        return View["updatepatron.cshtml", findpatron];
      };

      Patch["/patron/edit/{id}"]=parameters=>{
        Patron findpatron = Patron.Find(parameters.id);

        findpatron.Update(new List<string>{"name", "notes"}, new List<object>{(string)Request.Form["patronname"], (string)Request.Form["notes"]});

        return View["patron.cshtml", findpatron];
      };
      Patch["/patron/payfines/{id}"]=parameters=> {
        Patron p = Patron.Find(parameters.id);
        p.PayFines(Request.Form["amount"]);
        return View["patron.cshtml", p];
      };

      Get["/overdue"]=_=>
      {
        List<Copy> overDueBooks = Copy.OverdueBooks();
        return View["overdue.cshtml",overDueBooks];
      };

      Get["/patron/{id}"] = parameters => {
        Patron p = Patron.Find(parameters.id);
        return View["patron.cshtml", p];
      };

    }
  }
}

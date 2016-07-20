using System.Collections.Generic;
using System;

namespace Library
{
  public class Copy : SavableObject<Copy>
  {
    private int _bookId;
    private int _patronId;
    private DateTime _dueDate;
    public static string table
    {
      get
      {
        return "copies";
      }
    }
    public int book_id
    {
      get
      {
        return _bookId;
      }
      set
      {
        _bookId = value;
      }
    }
    public DateTime due_date
    {
      get
      {
        return _dueDate;
      }
      set
      {
        _dueDate = value;
      }
    }
    public int patron_id
    {
      get
      {
        return _patronId;
      }
      set
      {
        _patronId = value;
      }
    }
    public Copy(int BookId, DateTime DueDate, int PatronId, int Id=0)
    {
      id = Id;
      _bookId = BookId;
      _dueDate = DueDate;
      _patronId = PatronId;
    }
    public void Checkin()
    {
      Patron patron =Patron.Find(patron_id);

      TimeSpan charge = DateTime.Today.Subtract(_dueDate);
      int total = Math.Max(0, (int)charge.TotalDays) * 10;

      int newBalance = patron.balance + total;

      patron.Update(new List<string> {"balance"}, new List<object> {newBalance});

      this.Update(new List<string> {"due_date", "patron_id"}, new List<object> {new DateTime(1900,1,1), 0});
    }

    public static List<Copy> OverdueBooks()
    {
      List<Patron> patrons = Patron.GetAll();
      List<Copy> OverdueBooks = new List<Copy> {};

      foreach(Patron patron in patrons)
      {
        List<Copy> listofCopies = patron.GetItemsOut();

        foreach(Copy copy in listofCopies)
        {
          TimeSpan charge = DateTime.Today.Subtract(copy.due_date);
          if((int)charge.TotalDays >0)
          {
            OverdueBooks.Add(copy);
          }
        }
      }
      return OverdueBooks;
    }
  }
}

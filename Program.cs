using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace EntityFrameworkProject
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BookContext())
            {
                var book = new BooksInMyRoom
                {
                    Name = "Dictionary"
                };
                db.Books.Add(book);
                db.SaveChanges();

                var use = new BookUseCase
                {
                    UseCase = "Hold up one of my monitors",
                    BookId = book
                };
                db.UseCases.Add(use);

                use = new BookUseCase //1:M
                {
                    UseCase = "Find big words",
                    BookId = book
                };
                db.UseCases.Add(use);
                db.SaveChanges();

                var query1 = from b in db.Books orderby b.BooksInMyRoomId select b;
                foreach (var item in query1)
                {
                    Console.WriteLine($"{item.BooksInMyRoomId} {item.Name}");
                }
                Console.WriteLine("What I use this book for: ");
                var query2 = from u in db.UseCases orderby u.BookUseCaseId select u;
                foreach (var item in query2)
                {
                    Console.WriteLine($"{item.BookUseCaseId} {item.UseCase}");
                }
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }

    public class BooksInMyRoom
    {
        public int BooksInMyRoomId { get; set; }
        public string Name { get; set; }
    }
    public class BookUseCase
    {
        public int BookUseCaseId { get; set; }
        public string UseCase { get; set; }
        public BooksInMyRoom BookId { get; set; }
    }
    public class BookContext : DbContext
    {
        public BookContext() { }
        public BookContext(string connString)
        {
            Database.Connection.ConnectionString = connString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BooksInMyRoom>().MapToStoredProcedures(
                p => p.Insert(sp => sp.HasName("sp_InsertBook").Parameter(pm => pm.Name, "name").Result
                (rs => rs.BooksInMyRoomId, "Id")).Update(sp => sp.HasName ("sp_UpdateBook").Parameter(pm => pm.Name, "name"))
                .Delete(sp => sp.HasName("sp_DeleteBook").Parameter(pm => pm.BooksInMyRoomId, "Id")));
        }
        public DbSet<BooksInMyRoom> Books { get; set; }
        public DbSet<BookUseCase> UseCases { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text.RegularExpressions;

// Класс для представления книги
public class Book
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int YearPublished { get; set; }

    public Book(string id, string title, string author, int yearPublished)
    {
        Id = id;
        Title = title;
        Author = author;
        YearPublished = yearPublished;
    }
}

// Класс для управления книгами
public class BookManager
{
    private const string ConnectionString = "Data Source=library.db;Version=3;";

    public BookManager()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Books (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Author TEXT NOT NULL,
                    YearPublished INTEGER NOT NULL
                )";
            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddBook(string title, string author, int yearPublished)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Books (Title, Author, YearPublished) VALUES (@Title, @Author, @YearPublished)";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@YearPublished", yearPublished);
                command.ExecuteNonQuery();
            }
        }
    }

    public void RemoveBook(string bookId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM Books WHERE Id = @Id";
            using (var command = new SQLiteCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", bookId);
                command.ExecuteNonQuery();
            }
        }
    }

    public Book GetBook(string bookId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string selectQuery = "SELECT Id, Title, Author, YearPublished FROM Books WHERE Id = @Id";
            using (var command = new SQLiteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", bookId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Book(
                            reader["Id"].ToString(),
                            reader["Title"].ToString(),
                            reader["Author"].ToString(),
                            Convert.ToInt32(reader["YearPublished"]));
                    }
                }
            }
        }
        return null;
    }

    public List<Book> GetAllBooks()
    {
        var books = new List<Book>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string selectQuery = "SELECT Id, Title, Author, YearPublished FROM Books";
            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new Book(
                        reader["Id"].ToString(),
                        reader["Title"].ToString(),
                        reader["Author"].ToString(),
                        Convert.ToInt32(reader["YearPublished"])));
                }
            }
        }
        return books;
    }
}

// Класс для представления читателя
public class Reader
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public Reader(string id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}

// Класс для управления читателями
public class ReaderManager
{
    private const string ConnectionString = "Data Source=library.db;Version=3;";

    public ReaderManager()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Readers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL
                )";
            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddReader(string name, string email)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string insertQuery = "INSERT INTO Readers (Name, Email) VALUES (@Name, @Email)";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.ExecuteNonQuery();
            }
        }
    }

    public void RemoveReader(string readerId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM Readers WHERE Id = @Id";
            using (var command = new SQLiteCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", readerId);
                command.ExecuteNonQuery();
            }
        }
    }

    public Reader GetReader(string readerId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string selectQuery = "SELECT Id, Name, Email FROM Readers WHERE Id = @Id";
            using (var command = new SQLiteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", readerId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Reader(
                            reader["Id"].ToString(),
                            reader["Name"].ToString(),
                            reader["Email"].ToString());
                    }
                }
            }
        }
        return null;
    }

    public List<Reader> GetAllReaders()
    {
        var readers = new List<Reader>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string selectQuery = "SELECT Id, Name, Email FROM Readers";
            using (var command = new SQLiteCommand(selectQuery, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    readers.Add(new Reader(
                        reader["Id"].ToString(),
                        reader["Name"].ToString(),
                        reader["Email"].ToString()));
                }
            }
        }
        return readers;
    }
}

// Основная программа
public class Program
{
    public static void Main(string[] args)
    {
        BookManager bookManager = new BookManager();
        ReaderManager readerManager = new ReaderManager();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1. Управление книгами");
            Console.WriteLine("2. Управление читателями");
            Console.WriteLine("3. Выход");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ManageBooks(bookManager);
                    break;
                case "2":
                    ManageReaders(readerManager);
                    break;
                case "3":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    private static void ManageBooks(BookManager bookManager)
    {
        while (true)
        {
            Console.WriteLine("\nУправление книгами:");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Найти книгу по ID");
            Console.WriteLine("3. Показать все книги");
            Console.WriteLine("4. Удалить книгу по ID");
            Console.WriteLine("5. Назад");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите название книги: ");
                    string title = Console.ReadLine();
                    Console.Write("Введите имя автора: ");
                    string author = Console.ReadLine();
                    string authorPattern = @"^[a-zA-Zа-яА-ЯёЁ\s-]+$";

                    while (!Regex.IsMatch(author, authorPattern))
                    {
                        Console.WriteLine("Имя автора не должно содержать цифры или недопустимые символы. Попробуйте снова.");
                        Console.Write("Введите имя автора: ");
                        author = Console.ReadLine();
                    }
                    Console.Write("Введите год публикации: ");
                    string yearPublished = Console.ReadLine();
                    string yearPattern = @"^\d{4}$";

                    while (!Regex.IsMatch(yearPublished, yearPattern))
                    {
                        Console.WriteLine("Год публикации должен состоять из 4 цифр. Попробуйте снова.");
                        Console.Write("Введите год публикации: ");
                        yearPublished = Console.ReadLine();
                    }

                    break;

                case "2":
                    Console.Write("Введите ID книги для поиска: ");
                    string bookId = Console.ReadLine();
                    var book = bookManager.GetBook(bookId);
                    if (book != null)
                    {
                        Console.WriteLine($"Найдена книга: {book.Title} автор: {book.Author} ({book.YearPublished})");
                    }
                    else
                    {
                        Console.WriteLine($"Книга с ID '{bookId}' не найдена.");
                    }
                    break;

                case "3":
                    var books = bookManager.GetAllBooks();
                    foreach (var b in books)
                    {
                        Console.WriteLine($"{b.Id}: {b.Title} автор: {b.Author} ({b.YearPublished})");
                    }
                    break;

                case "4":
                    Console.Write("Введите ID книги для удаления: ");
                    string removeBookId = Console.ReadLine();
                    bookManager.RemoveBook(removeBookId);
                    Console.WriteLine("Книга удалена.");
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    private static void ManageReaders(ReaderManager readerManager)
    {
        while (true)
        {
            Console.WriteLine("\nУправление читателями:");
            Console.WriteLine("1. Добавить читателя");
            Console.WriteLine("2. Найти читателя по ID");
            Console.WriteLine("3. Показать всех читателей");
            Console.WriteLine("4. Удалить читателя по ID");
            Console.WriteLine("5. Назад");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите имя читателя: ");
                    string name = Console.ReadLine();
                    Console.Write("Введите электронную почту читателя: ");
                    string email = Console.ReadLine();
                    string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                    while (!Regex.IsMatch(email, emailPattern))
                    {
                        Console.WriteLine("Некорректный адрес электронной почты. Попробуйте снова.");
                        Console.Write("Введите электронную почту читателя: ");
                        email = Console.ReadLine();
                    }
                    readerManager.AddReader(name, email);
                    Console.WriteLine("Читатель добавлен.");
                    break;

                case "2":
                    Console.Write("Введите ID читателя для поиска: ");
                    string readerId = Console.ReadLine();
                    var reader = readerManager.GetReader(readerId);
                    if (reader != null)
                    {
                        Console.WriteLine($"Найден читатель: {reader.Name}, Email: {reader.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"Читатель с ID '{readerId}' не найден.");
                    }
                    break;

                case "3":
                    var readers = readerManager.GetAllReaders();
                    foreach (var r in readers)
                    {
                        Console.WriteLine($"{r.Id}: {r.Name}, Email: {r.Email}");
                    }
                    break;

                case "4":
                    Console.Write("Введите ID читателя для удаления: ");
                    string removeReaderId = Console.ReadLine();
                    readerManager.RemoveReader(removeReaderId);
                    Console.WriteLine("Читатель удалён.");
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }
}

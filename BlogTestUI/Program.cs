using System;
using System.Collections.Generic;
using System.IO;
using BlogDataLibrary.Data;
using BlogDataLibrary.Database;
using BlogDataLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace BlogTestUI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlData db = GetConnection();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("       Blog Management System (LE3)     ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Authenticate (Login)");
                Console.WriteLine("2. Register New User");
                Console.WriteLine("3. Add Post");
                Console.WriteLine("4. List All Posts");
                Console.WriteLine("5. Show Post Details");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option (1-6): ");
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        Authenticate(db);
                        break;
                    case "2":
                        Register(db);
                        break;
                    case "3":
                        AddPost(db);
                        break;
                    case "4":
                        ListPosts(db);
                        break;
                    case "5":
                        ShowPostDetails(db);
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        static SqlData GetConnection()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            ISqlDataAccess dbAccess = new SqlDataAccess(config);
            SqlData db = new SqlData(dbAccess);
            return db;
        }

        private static UserModel GetCurrentUser(SqlData db)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();
            return db.Authenticate(username, password);
        }

        public static void Authenticate(SqlData db)
        {
            UserModel user = GetCurrentUser(db);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials.");
            }
            else
            {
                Console.WriteLine($"Welcome, {user.UserName} ({user.FirstName} {user.LastName})!");
            }
        }

        public static void Register(SqlData db)
        {
            Console.Write("Enter new username: ");
            var username = Console.ReadLine();
            Console.Write("Enter new password: ");
            var password = Console.ReadLine();
            Console.Write("Enter first name: ");
            var firstName = Console.ReadLine();
            Console.Write("Enter last name: ");
            var lastName = Console.ReadLine();

            db.Register(username, firstName, lastName, password);
            Console.WriteLine("User registered successfully!");
        }

        private static void AddPost(SqlData db)
        {
            Console.WriteLine("--- Log in to create a post ---");
            UserModel user = GetCurrentUser(db);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials. Cannot add post.");
                return;
            }

            Console.Write("Title: ");
            string title = Console.ReadLine();
            Console.Write("Write body: ");
            string body = Console.ReadLine();

            PostModel post = new PostModel
            {
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
                UserId = user.Id
            };

            db.AddPost(post);
            Console.WriteLine("Post added successfully!");
        }

        private static void ListPosts(SqlData db)
        {
            List<ListPostModel> posts = db.ListPosts();
            if (posts.Count == 0)
            {
                Console.WriteLine("No posts found.");
                return;
            }

            foreach (ListPostModel post in posts)
            {
                string snippet = post.Body.Length > 20 ? post.Body.Substring(0, 20) : post.Body;
                Console.WriteLine($"{post.Id}. Title: {post.Title} by {post.UserName} [{post.DateCreated:yyyy-MM-dd}]");
                Console.WriteLine($"{snippet}...");
                Console.WriteLine();
            }
        }

        private static void ShowPostDetails(SqlData db)
        {
            Console.Write("Enter a post ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                ListPostModel post = db.ShowPostDetails(id);
                if (post != null)
                {
                    Console.WriteLine($"Title: {post.Title}");
                    Console.WriteLine($"by {post.FirstName} {post.LastName} [{post.UserName}]");
                    Console.WriteLine();
                    Console.WriteLine(post.Body);
                    Console.WriteLine(post.DateCreated.ToString("MMM d yyyy"));
                }
                else
                {
                    Console.WriteLine("Post not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
        }
    }
}
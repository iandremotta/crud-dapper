using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using dados_dapper.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace dados_dapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=localhost,1433;Database=balta;TrustServerCertificate=True;User ID=sa;Password=1q2w3e4r@#$";

            using (var connection = new SqlConnection(connectionString))
            {
                //UpdateCategory(connection);
                //DeleteCategory(connection);
                // CreateManyCategories(connection);
                //ExecuteReadProcedure(connection);
                //ExecuteProcedure(connection);
                //ExecuteScalar(connection);
                //ReadView(connection);
                //ListCategories(connection);
                //CreateCategory(connection);
                //OneToOne(connection);
                //OneToMany(connection);
                //QueryMultiple(connection);
                //SelectIn(connection);
                //Like(connection, "backend");
                Transaction(connection);
            }

            static void ListCategories(SqlConnection connection)
            {
                var categories = connection.Query<Category>("Select [Id],[Title] FROM [Category]");
                foreach (var item in categories)
                {
                    Console.WriteLine($"{item.Id} - {item.Title}");
                }
            }

            static void CreateCategory(SqlConnection connection)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Title = "Amazon AWS";
                category.Url = "Amazon";
                category.Description = "Categoria destinada a serviços AWS";
                category.Order = 8;
                category.Summary = "AWS Cloud";
                category.Featured = false;
                var insertSql = @"INSERT INTO [Category] VALUES(
                @id,
                @title,
                @url,
                @description,
                @order,
                @summary,
                @featured
                )";

                connection.Execute(insertSql, new
                {
                    id = category.Id,
                    title = category.Title,
                    url = category.Url,
                    description = category.Description,
                    order = category.Order,
                    summary = category.Summary,
                    featured = category.Featured
                });
            }
            static void UpdateCategory(SqlConnection connection)
            {
                var updateQuery = "UPDATE [Category] SET [Title] = @title WHERE [Id] = @id";
                connection.Execute(updateQuery, new
                {
                    id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                    title = "Front End 2022"
                });
            }
            static void DeleteCategory(SqlConnection connection)
            {
                var deleteQuery = "DELETE FROM [Category] WHERE [Id] = @id";
                connection.Execute(deleteQuery, new
                {
                    id = new Guid("be4cda6c-004c-48ed-bdf4-8cd1a18baa99")
                });
            }
            static void CreateManyCategories(SqlConnection connection)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Title = "Amazon AWS";
                category.Url = "Amazon";
                category.Description = "Categoria destinada a serviços AWS";
                category.Order = 8;
                category.Summary = "AWS Cloud";
                category.Featured = false;

                var category2 = new Category();
                category2.Id = Guid.NewGuid();
                category2.Title = "Categoria Nova";
                category2.Url = "categoria-nova";
                category2.Description = "Categoria destinada a serviços novos";
                category2.Order = 9;
                category2.Summary = "Nova Cloud";
                category2.Featured = true;
                var insertSql = @"INSERT INTO [Category] VALUES(
                @id,
                @title,
                @url,
                @description,
                @order,
                @summary,
                @featured
                )";

                connection.Execute(insertSql, new[]
                {
                    new
                    {
                        id = category.Id,
                        title = category.Title,
                        url = category.Url,
                        description = category.Description,
                        order = category.Order,
                        summary = category.Summary,
                        featured = category.Featured
                    },
                    new
                    {
                        id = category2.Id,
                        title = category2.Title,
                        url = category2.Url,
                        description = category2.Description,
                        order = category2.Order,
                        summary = category2.Summary,
                        featured = category2.Featured
                    }
                });
            }
            static void ExecuteProcedure(SqlConnection connection)
            {
                var procedure = "[dbo].[spDeleteStudent]";
                var pars = new { StudentId = "8b232c43-740f-47f6-8e5d-40cbcdc9e05d" };
                var effectedRows = connection.Execute(procedure, pars, commandType: System.Data.CommandType.StoredProcedure);
                Console.WriteLine(effectedRows);
            }

            static void ExecuteReadProcedure(SqlConnection connection)
            {
                var procedure = "[dbo].[spGetCoursesByCategory]";
                var pars = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };
                var courses = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);
                foreach (var item in courses)
                {
                    Console.WriteLine($"{item.Title}");
                }

            }
            static void ExecuteScalar(SqlConnection connection)
            {
                var category = new Category();
                category.Title = "Amazon AWS";
                category.Url = "Amazon";
                category.Description = "Categoria destinada a serviços AWS";
                category.Order = 8;
                category.Summary = "AWS Cloud";
                category.Featured = false;

                var insertSql = @"INSERT INTO [Category] OUTPUT inserted.[Id] VALUES(
                        NEWID(),
                        @title,
                        @url,
                        @description,
                        @order,
                        @summary,
                        @featured
                    )";

                var id = connection.ExecuteScalar<Guid>(insertSql, new
                {
                    title = category.Title,
                    url = category.Url,
                    description = category.Description,
                    order = category.Order,
                    summary = category.Summary,
                    featured = category.Featured
                });
                Console.WriteLine($"Id da nova categoria: {id}");
            }
            static void ReadView(SqlConnection connection)
            {
                var sql = @"Select * from [vwCourses]";
                var courses = connection.Query(sql);
                foreach (var item in courses)
                {
                    Console.WriteLine($"{item.Title} - {item.Summary} ");
                }
            }
            static void OneToOne(SqlConnection connection)
            {
                var sql = @"SELECT * FROM [CareerItem] INNER JOIN [Course] ON [CareerItem].[CourseId] = [Course].[Id]";
                var items = connection.Query<CareerItem, Course, CareerItem>(sql, (carrerItem, course) =>
                {
                    carrerItem.Course = course;
                    return carrerItem;
                }, splitOn: "Id");
                foreach (var item in items)
                {
                    Console.WriteLine(item.Course.Title);
                }
            }
            static void OneToMany(SqlConnection connection)
            {
                var sql = @"
                SELECT 
                [Career].[Id], 
                [Career].[Title], 
                [CareerItem].[CareerId], 
                [CareerItem].[Title] FROM [Career] INNER JOIN [CareerItem] on [CareerItem].[CareerId] = [Career].[Id] ORDER BY [Career].[Title]";
                var careers = new List<Career>();
                var items = connection.Query<Career, CareerItem, Career>(sql, (career, item) =>
                {
                    var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                    if (car == null)
                    {
                        car = career;
                        car.Items.Add(item);
                        careers.Add(car);
                    }
                    else
                    {
                        car.Items.Add(item);
                    }
                    return career;
                }, splitOn: "CareerId");
                foreach (var career in careers)
                {
                    Console.WriteLine($"{career.Title}");
                    foreach (var item in career.Items)
                    {
                        Console.WriteLine($" - {item.Title}");
                    }
                }
            }
            static void QueryMultiple(SqlConnection connection)
            {
                var sql = @"select * from [Category]; Select * from [Course]";
                using (var multi = connection.QueryMultiple(sql))
                {
                    var categories = multi.Read<Category>();
                    var courses = multi.Read<Course>();
                    foreach (var category in categories)
                    {
                        Console.WriteLine(category.Title);
                    }

                    foreach (var course in courses)
                    {
                        Console.WriteLine(course.Title);
                    }
                }
            }
            static void SelectIn(SqlConnection connection)
            {
                var sql = @"SELECT * FROM [Career] WHERE [Id] IN @Id";
                var results = connection.Query(sql, new
                {
                    Id = new[] { "e6730d1c-6870-4df3-ae68-438624e04c72", "4327ac7e-963b-4893-9f31-9a3b28a4e72b" }
                });
                foreach (var result in results)
                {
                    Console.WriteLine(result.Title);
                }
            }
            static void Like(SqlConnection connection, string term)
            {
                var sql = @"SELECT * FROM [Course] WHERE [Title] LIKE @exp";
                var results = connection.Query<Course>(sql, new
                {
                    exp = $"%{term}%"
                });

                foreach (var result in results)
                {
                    Console.WriteLine(result.Title);
                }
            }
            static void Transaction(SqlConnection connection)
            {
                var category = new Category();
                category.Id = Guid.NewGuid();
                category.Title = "Minha categoria que não";
                category.Url = "amazon";
                category.Description = "Categoria destinada a serviços do AWS";
                category.Order = 8;
                category.Summary = "AWS Cloud";
                category.Featured = false;

                var insertSql = @"INSERT INTO 
                    [Category] 
                VALUES(
                    @Id, 
                    @Title, 
                    @Url, 
                    @Summary, 
                    @Order, 
                    @Description, 
                    @Featured)";

                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var rows = connection.Execute(insertSql, new
                    {
                        category.Id,
                        category.Title,
                        category.Url,
                        category.Summary,
                        category.Order,
                        category.Description,
                        category.Featured
                    }, transaction);

                    transaction.Commit();
                    // transaction.Rollback();

                    Console.WriteLine($"{rows} linhas inseridas");
                }
            }
        }
    }
}

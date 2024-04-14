using System.Data.SqlClient;
using Lab_06.DTOs;

namespace Lab_06.Endpoints;

using FluentValidation;

public static class AnimalsEndpoints
{
    public static void RegisterAnimalsEndpoints(this WebApplication app)
    {
        //GETs
        app.MapGet("/animals", (IConfiguration configuration, string? orderBy = "name") =>
        {
            var animals = new List<GetAnimalResponse>();
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var validColumns = new[] {"name", "description", "category", "area" };

                var sqlCommand = new SqlCommand("SELECT * FROM Animals", sqlConnection);

                if (orderBy != null && validColumns.Contains(orderBy.ToLower()))
                {
                    sqlCommand.CommandText += $" ORDER BY {orderBy}";
                } else if (orderBy != null && !validColumns.Contains(orderBy))
                {
                    return Results.BadRequest("Invalid column name");
                }

                sqlCommand.Connection.Open();
                var reader = sqlCommand.ExecuteReader();

                while (reader.Read())
                {
                    animals.Add(new GetAnimalResponse(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4))
                    );
                }

            }
            return Results.Ok(animals);
        });

        app.MapGet("/animals/{id:int}", (IConfiguration configuration, int id) =>
        {
            using var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default"));
            var sqlCommand = new SqlCommand("SELECT * FROM Animals WHERE ID = @id", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Connection.Open();
            var reader = sqlCommand.ExecuteReader();

            if (!reader.Read()) return Results.NotFound();

            return Results.Ok(new GetAnimalResponse(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4))
            );
        });

        //POST
        app.MapPost("/animals", (IConfiguration configuration, CreateAnimalRequest request, IValidator<CreateAnimalRequest> validator) =>
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand("INSERT INTO Animals (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@Name", request.Name);
                sqlCommand.Parameters.AddWithValue("@Description", request.Description);
                sqlCommand.Parameters.AddWithValue("@Category", request.Category);
                sqlCommand.Parameters.AddWithValue("@Area", request.Area);
                sqlCommand.Connection.Open();

                sqlCommand.ExecuteNonQuery();

                return Results.Created("", null);
            }
        });

        //PUT
        app.MapPut("/animals/{id:int}", (IConfiguration configuration, int id, CreateAnimalRequest request, IValidator<CreateAnimalRequest> validator) =>
        {
            var validation = validator.Validate(request);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand("UPDATE Animals SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE ID = @id", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Parameters.AddWithValue("@Name", request.Name);
                sqlCommand.Parameters.AddWithValue("@Description", request.Description);
                sqlCommand.Parameters.AddWithValue("@Category", request.Category);
                sqlCommand.Parameters.AddWithValue("@Area", request.Area);
                sqlCommand.Connection.Open();

                sqlCommand.ExecuteNonQuery();

                return Results.NoContent();
            }
        });

        //DELETE
        app.MapDelete("/animals/{id:int}", (IConfiguration configuration, int id) =>
        {
            using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
            {
                var sqlCommand = new SqlCommand("DELETE FROM Animals WHERE ID = @id", sqlConnection);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Connection.Open();

                sqlCommand.ExecuteNonQuery();

                return Results.NoContent();
            }
        });
    }
}
namespace Lab_06.DTOs;

public record GetAnimalResponse(int Id, string Name, string Description, string Category, string Area);
public record CreateAnimalRequest(string Name, string Description, string Category, string Area);
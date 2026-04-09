using Microsoft.AspNetCore.Mvc;

namespace ProfileSearch.Server.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class WeatherForecastController : ControllerBase
  {
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
      })
      .ToArray();
    }
  }
}

// What tables would we need in Dapper
// 1. Users table: Id, Name, Email, PasswordHash
// 2. CharacterImages table: Id, CharacterName, ImageUrl, TopLeftCrop, BottomRightCrop
// 3. UserCharacterImages table: Id, UserId, CharacterImageId
// 4. SearchHistory table: Id, UserId, SearchTerm, SearchDate

// What stored procedures would we need in Dapper
// 1. Add User: AddUser - takes name, email, and password hash to create a new user
// 2. Get User: GetUser - takes email and password hash to return user details for authentication
// 3. Get Character Images: GetCharacterImages - returns all character images with their crop bounds
// 4. Save User Character Image: SaveUserCharacterImage - saves a user's selected character image and crop bounds
//    Save to both CharacterImages and UserCharacterImages tables
// 5. Get User Character Images: GetUserCharacterImages - returns all character images saved by a user
// 6. Save Search History: SaveSearchHistory - saves a user's search term and date to the SearchHistory table
// 7. Get Search History: GetSearchHistory - returns a user's search history with search terms and dates
// 8. Delete User Character Image: DeleteUserCharacterImage - deletes a user's saved character image from both CharacterImages and UserCharacterImages tables
// 9. Update User Character Image: UpdateUserCharacterImage - updates a user's saved character image and crop bounds in both CharacterImages and UserCharacterImages tables
// 10. Get All Users: GetAllUsers - returns a list of all users in the system (for admin purposes)
// 11. Get All Character Images: GetAllCharacterImages - returns a list of all character images in the system (for admin purposes)
using BlogBackend.Data;
using BlogBackend.Models;

namespace BlogBackend.Initialization;

public class DataInitializationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DataInitializationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!dbContext.Communities.Any())
            {
                dbContext.Communities.Add(new Community
                {
                    Id = new Guid(),
                    CreateTime = DateTime.UtcNow,
                    Name = "Масонская ложа",
                    Description = "Место, помещение, где собираются масоны для проведения своих " +
                                  "собраний, чаще называемых работами",
                    IsClosed = true,
                    SubscribersCount = 0,
                    CommunityUsers = new List<CommunityUser>(),
                    Posts = new List<Post>()
                });
                
                dbContext.Communities.Add(new Community
                {
                    Id = new Guid(),
                    CreateTime = DateTime.UtcNow,
                    Name = "Следствие вели с Л. Каневским",
                    Description = "Без длинных предисловий: мужчина умер",
                    IsClosed = false,
                    SubscribersCount = 0,
                    CommunityUsers = new List<CommunityUser>(),
                    Posts = new List<Post>()
                });

                dbContext.Communities.Add(new Community
                {
                    Id = new Guid(),
                    CreateTime = DateTime.UtcNow,
                    Name = "IT <3",
                    Description = "Информационные технологии связаны с изучением методов " +
                                  "и средств сбора, обработки и передачи данных с целью получения информации нового " +
                                  "качества о состоянии объекта, процесса или явления",
                    IsClosed = false,
                    SubscribersCount = 0,
                    CommunityUsers = new List<CommunityUser>(),
                    Posts = new List<Post>()
                });
                
                await dbContext.SaveChangesAsync();
            }

            if (!dbContext.Tags.Any())
            {
                dbContext.Tags.Add(new Tag
                {
                    Id = new Guid(),
                    CreateTime = DateTime.UtcNow,
                    Name = "18+"
                });
                
                dbContext.Tags.Add(new Tag
                {
                    Id = new Guid(),
                    CreateTime = DateTime.UtcNow,
                    Name = "физика"
                });
                
                dbContext.Tags.Add(new Tag
                {
                    Id = new Guid(),
                    CreateTime = DateTime.UtcNow,
                    Name = "теория_заговора"
                });
                
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
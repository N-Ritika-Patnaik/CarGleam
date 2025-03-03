using Microsoft.EntityFrameworkCore;
using CarGleam.Data;
namespace CarGleam.Service
{
    public class UserInactivityService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory; // scope is a container for services, used to create an IServiceScope

        private readonly ILogger<UserInactivityService> _logger; //inject an instance of the ILogger<T> interface to logging

        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(48); // Check every 48 hours
        private readonly TimeSpan _inactivityThreshold = TimeSpan.FromDays(90); // 90 days inactivity

        //----------to check if working or not-------------
        //private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every 24 hours
        //private readonly TimeSpan _inactivityThreshold = TimeSpan.FromMinutes(3); // 90 days inactivity
        //-------------------------------------------------

        //parametarized constructor to inject the ScopeFactory and ILogger into the service
        public UserInactivityService(IServiceScopeFactory scopeFactory, ILogger<UserInactivityService> logger)
        {
            _scopeFactory = scopeFactory;  
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            _logger.LogInformation("User Inactivity Service started."); // like console write, check in cmd

            while (!stoppingToken.IsCancellationRequested) 
            {
                _logger.LogInformation("Checking for inactive users...");
                //  exception handlig to catch the error
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<EFCoreDBContext>(); // from efcoredbcontext
                        var cutoffDate = DateTime.UtcNow - _inactivityThreshold; // subtraction between the 2 dates

                        _logger.LogInformation("Cutoff Date: {CutoffDate}", cutoffDate); // like console write, to check if 90 days

                        // Find users who haven't logged in since the cutoff and are still active.
                        //linq query----------
                        var inactiveUsers = await context.Users
                               //.Where(u => u.LastLogin < cutoffDate && u.IsActive)
                               .Where(u => (u.LastLogin == null || u.LastLogin < cutoffDate) && u.IsActive)

                            .ToListAsync(stoppingToken);

                        _logger.LogInformation("{Count} inactive users found.", inactiveUsers.Count); 

                        foreach (var user in inactiveUsers) // loop to check each user
                        {
                            user.IsActive = false;
                            _logger.LogInformation("User {Email} marked as inactive.", user.Email);
                        }
                        await context.SaveChangesAsync(stoppingToken); // stop backgroung service when cancellation requested
                    }
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Error while checking user inactivity.");
                }

                // Wait for the next interval
                await Task.Delay(_checkInterval, stoppingToken); // delay for 48 hours
            }

            _logger.LogInformation("User Inactivity Service is stopping.");
        }
    }
}




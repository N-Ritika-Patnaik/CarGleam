using Microsoft.EntityFrameworkCore;
using CarGleam.Data;
namespace CarGleam.Service
{
    public class MachineStatusService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MachineStatusService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);

        public MachineStatusService(IServiceScopeFactory scopeFactory, ILogger<MachineStatusService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Machine Status Service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking machine availability...");

                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<EFCoreDBContext>();
                        var currentTime = DateTime.UtcNow;

                        var machines = await context.Machines.ToListAsync(stoppingToken);

                        foreach (var machine in machines)
                        {
                            var bookings = await context.Bookings
                                .Where(b => b.MachineId == machine.MachineId)
                                .ToListAsync(stoppingToken);

                            var isAvailable = bookings
                                .All(b => b.ServiceDate.Add(b.Machine.Duration) <= currentTime || b.ServiceDate >= currentTime);

                            machine.Status = isAvailable ? "Available" : "Unavailable";
                            _logger.LogInformation("Machine {MachineId} status updated to {Status}.", machine.MachineId, machine.Status);
                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking machine availability.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Machine Status Service is stopping.");
        }
    }
}
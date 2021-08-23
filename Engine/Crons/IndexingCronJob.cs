using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Axle.Engine.Crons
{
    public class IndexingCronJob : CronJobService
    {
        private readonly ILogger<IndexingCronJob> _logger;
        private readonly SearchEngine _engine;
        
        public IndexingCronJob(SearchEngine engine, IScheduleConfig<IndexingCronJob> config, ILogger<IndexingCronJob> logger) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _engine = engine;
            _logger = logger;
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting indexing at {0}.", DateTime.UtcNow);
            return Task.Run(() => _engine.IndexAllDocuments(), cancellationToken);
        }
    }
}
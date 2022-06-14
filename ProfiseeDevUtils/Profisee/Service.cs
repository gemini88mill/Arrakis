using ProfiseeDevUtils.Infrastructure;
using ProfiseeDevUtils.Init;

namespace ProfiseeDevUtils.Profisee
{
    internal class Service
    {
        private WinService winService;
        private string serviceName;
        public ILogger Logger { get; set; }

        public Service(bool quiet)
        {
            this.winService = new WinService(quiet);
            this.serviceName = new EnvironmentVariables(quiet).MaestroSvc;
            this.Logger = new Logger(quiet);
        }

        public static void Act(string action, bool? quiet)
        {
            var sanitizedAction = action.ToLower();
            var profService = new Service(quiet ?? false);
            profService.process(sanitizedAction);
        }

        public void Start()
        {
            this.winService.Start(this.serviceName);
        }

        public void Stop()
        {
            this.winService.Stop(this.serviceName);
        }

        private void process(string action)
        {
            var actions = new Dictionary<string, Action>
            {
                { "start", this.Start },
                { "stop", this.Stop },
            };

            if (!actions.ContainsKey(action))
            {
                this.Logger.Err($"Action {action} not found in available actions. Please use one of the following:");
                foreach (var a in actions.Keys)
                {
                    this.Logger.Err(a);
                }
                return;
            }

            actions[action]();
        }
    }
}

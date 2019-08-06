using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCodes.DotNetCore
{
    #region demos use
    
    public class MyConfig
    {
        public MyConfig()
        {
            Id = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
            Debug.WriteLine("new MyConfig() => " + this.GetHashCode() + ", Id => " + Id);
        }

        public string Id { get; set; }
        public string Foo { get; set; }
        public int Bar { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Id, Foo, Bar);
        }

        //this is a bug, it will create more than one instance!!!
        //private static readonly MyConfig Instance = new MyConfig(){Id = "AAAAAAAAAAAAAAAAAAAAAA"};
        //public static Func<MyConfig> Resolve = () => Instance;


        private static readonly Lazy<MyConfig> LazyInstance = new Lazy<MyConfig>(CreateDefault);

        private static MyConfig CreateDefault()
        {
            var myConfig = new MyConfig() { Id = "!!!Singleton!!!" };
            return myConfig;
        }

        public static Func<MyConfig> Resolve = () => LazyInstance.Value;
    }

    public static class MyConfigServiceCollectionExtensions
    {
        public static IServiceCollection AddMyConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoUpdatedConfig<MyConfig>(config, myConfig =>
            {
                config.GetSection("MyConfig").Bind(myConfig);
                MyConfig.Resolve = () => myConfig;
            });
            return services;
        }
    }

    #endregion


    public static class ConfigChangeMonitorExtensions
    {
        public static IServiceCollection AddAutoUpdatedConfig<TOptions>(
            this IServiceCollection services
            , IConfiguration config
            , Action<TOptions> configureOptions) where TOptions : class, new()
        {

            services.Configure(configureOptions);

            //setup singleton for TOptions
            services.AddSingleton(sp =>
            {
                var options = sp.GetService<IOptions<TOptions>>();
                var optionsValue = options.Value;

                //invoke only once, to create a configChangeMonitor, so notify will be hooked.
                var configChangeMonitor = ConfigChangeMonitor.Create(config, c =>
                {
                    //resolve TOptions
                    configureOptions(optionsValue);
                });
                services.AddSingleton(configChangeMonitor);
                configChangeMonitor.StartWatch();

                return optionsValue;
            });

            #region this will not got chance being invoked by user's code

            //services.AddSingleton(sp =>
            //{
            //    var configChangeMonitor = ConfigChangeMonitor.Create(config, c =>
            //    {
            //        //resolve TOptions
            //        var optionConfig = sp.GetService<TOptions>();
            //        configureOptions(optionConfig);
            //    });
            //    return configChangeMonitor;
            //});
            //configChangeMonitor.StartWatch();

            #endregion

            return services;
        }
    }

    public class ConfigChangeMonitor
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _config;
        private readonly Action<IConfiguration> _notifyChangeFunc;
        private byte[] _oldBytes = null;

        public ConfigChangeMonitor(IConfiguration config, Action<IConfiguration> notifyChangeFunc)
        {
            _config = config;
            _notifyChangeFunc = notifyChangeFunc;
            _oldBytes = CreateHash(config);
        }

        private bool _started = false;
        public void StartWatch()
        {
            if (_started)
            {
                return;
            }
            ChangeToken.OnChange(
                _config.GetReloadToken,
                OnSettingChanged,
                _config);
            _started = true;
        }

        private void OnSettingChanged(object state)
        {
            var config = (IConfiguration)state;

            var hasChanged = HasChanged(config);
            if (!hasChanged)
            {
                return;
            }
            _notifyChangeFunc(config);
        }

        public bool HasChanged(IConfiguration config)
        {
            var newBytes = CreateHash(config);
            var sequenceEqual = _oldBytes.SequenceEqual(newBytes);
            if (!sequenceEqual)
            {
                _oldBytes = newBytes;
                LogMessage("Hash Changed!");
                return true;
            }
            LogMessage("No Hash Changed!");
            return false;
        }
        private byte[] CreateHash(IConfiguration config)
        {
            var sb = new StringBuilder();
            foreach (var section in config.GetChildren())
            {
                sb.AppendFormat("{0}={1}{2}", section.Key, section.Value, Environment.NewLine);
                foreach (var child in section.GetChildren())
                {
                    sb.AppendFormat("{0}={1}{2}", child.Key, child.Value, Environment.NewLine);
                }
            }
            var trace = sb.ToString();
            var computeHash = ComputeHash(trace);
            return computeHash;
        }
        private void LogMessage(object message)
        {
            //todo replace with ILogger for config
            Debug.WriteLine("[ConfigChangeMonitor] => " + message);
        }
        private static byte[] ComputeHash(string value)
        {
            if (value == null)
            {
                return new byte[20];
            }
            var bytes = Encoding.UTF8.GetBytes(value);
            return System.Security.Cryptography.SHA1
                .Create().ComputeHash(bytes);
        }

        public static ConfigChangeMonitor Create(IConfiguration config, Action<IConfiguration> notifyChangeFunc)
        {
            return new ConfigChangeMonitor(config, notifyChangeFunc);
        }
    }
}

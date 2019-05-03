using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RealEstate.Configuration
{
    public static class Reader
    {
        public static R8Config ReadConfiguration(this Assembly assembly)
        {
            if (assembly == null)
                return default;

            var dir = Path.GetDirectoryName(assembly.Location);
            const string cfgFile = "config.inf";

            var cfg = File.ReadAllLines($"{dir}\\{cfgFile}");
            if (cfg?.Any() != true)
                return default;

            var model = new R8Config();
            var properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

            foreach (var line in cfg)
            {
                var splitter = line.Split(new[]
                {
                    "=>"
                }, StringSplitOptions.None);
                if (splitter?.Any() != true)
                    continue;

                var key = splitter[0];
                var property = properties.Find(x => x.Name == key);
                if (property == null)
                    continue;

                var startIndicator = 0;
                if (splitter[1].StartsWith(" "))
                {
                    foreach (var val in splitter[1])
                    {
                        if (val != ' ')
                            continue;

                        startIndicator++;
                    }
                }

                var value = splitter[1].Substring(startIndicator);
                property.SetValue(model, value);
            }

            return model;
        }
    }
}
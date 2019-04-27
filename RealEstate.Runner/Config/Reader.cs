using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RealEstate.Base.Config
{
    public static class Reader
    {
        public static Configuration Read()
        {
            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            const string cfgFile = "config.inf";

            var cfg = File.ReadAllLines($"{dir}\\{cfgFile}");
            if (cfg?.Any() != true)
                return default;

            var model = new Configuration();
            var dic = model.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(model, null));

            var startIndicator = 0;
            foreach (var line in cfg)
            {
                var splitter = line.Split(new[]
                {
                    "=>"
                }, StringSplitOptions.None);
                if (splitter?.Any() != true)
                    continue;

                var key = splitter[0];
                var prop = dic.FirstOrDefault(x => x.Key == key);
                if (EqualityComparer<KeyValuePair<string, object>>.Default.Equals(prop, default))
                    return default;

                if (splitter[1].StartsWith(" "))
                    foreach (var val in splitter[1])
                    {
                        if (val != ' ')
                            continue;

                        startIndicator++;
                    }

                var value = splitter[1].Substring(startIndicator);

                var type = model.GetType();
                var propType = type.GetProperty(prop.Key);
                if (propType == null)
                    continue;

                propType.SetValue(model, value);
            }

            return model;
        }
    }
}
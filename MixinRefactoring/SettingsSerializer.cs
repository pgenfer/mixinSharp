using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    /// <summary>
    /// class handles the serialization / deserialization of
    /// option values from and to the settings storage.
    /// </summary>
    public class SettingsSerializer
    {
        /// <summary>
        /// path to locate the settings inside the settings storage.
        /// </summary>
        private const string CollectionPath = "mixinSharp";

        public bool GetOption(string optionName, IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            var store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            var option = store?.GetBoolean(CollectionPath, optionName, false);
            return option ?? false;
        }

        public void SetOption(string optionName, bool value, IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            var store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!store.CollectionExists(CollectionPath))
                store.CreateCollection(CollectionPath);
            store?.SetBoolean(CollectionPath, optionName, value);
        }
    }
}

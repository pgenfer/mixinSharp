using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixinRefactoring
{
    public class Settings
    {
        private const string CollectionPath = "mixinSharp";

        public Settings(IServiceProvider serviceProvider)
        {
            ReadFromStorage(serviceProvider);
        }

        public void ReadFromStorage(IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            var store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            var createRegion = store?.GetBoolean(CollectionPath, nameof(CreateRegions), false);
            CreateRegions = createRegion ?? false;
        }

        public void WriteToStorage(IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            var store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            store?.SetBoolean(CollectionPath, nameof(CreateRegions), CreateRegions);
        }

        public bool CreateRegions { get; set; }
    }
}

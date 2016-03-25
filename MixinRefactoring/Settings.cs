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
    /// <summary>
    /// stores the settings for the code generation.
    /// The settings can be set via an options dialog.
    /// </summary>
    public class Settings
    {
        private SettingsSerializer _serializer = new SettingsSerializer();
        
        /// <summary>
        /// creates a new settings object.
        /// The service provider is needed to create the shell settings manager
        /// </summary>
        /// <param name="serviceProvider"></param>
        public Settings(IServiceProvider serviceProvider)
        {
            CreateRegions = _serializer.GetOption(nameof(CreateRegions), serviceProvider);
            IncludeDocumentation = _serializer.GetOption(nameof(IncludeDocumentation), serviceProvider);
        }

        /// <summary>
        /// default constructor does not load settings from storage,
        /// instead it uses default settings
        /// </summary>
        public Settings(
            bool createRegions=false,
            bool includeDocumentation= false)
        {
            CreateRegions = createRegions;
            IncludeDocumentation = includeDocumentation;
        }

        public bool CreateRegions { get; }
        public bool IncludeDocumentation { get; }
    }
}

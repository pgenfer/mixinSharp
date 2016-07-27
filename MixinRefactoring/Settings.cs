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
            InjectMixins = _serializer.GetOption(nameof(InjectMixins), serviceProvider);
            AddInterfacesToChild = _serializer.GetOption(nameof(AddInterfacesToChild), serviceProvider);
        }

        /// <summary>
        /// default constructor does not load settings from storage,
        /// instead it uses default settings
        /// </summary>
        public Settings(
            bool createRegions = false,
            bool includeDocumentation = false,
            bool injectMixins = false,
            bool addInterfacesToChild = false,
            bool avoidLineBreaksInProperties = true)
        {
            CreateRegions = createRegions;
            IncludeDocumentation = includeDocumentation;
            InjectMixins = injectMixins;
            AddInterfacesToChild = addInterfacesToChild;
            AvoidLineBreaksInProperties = avoidLineBreaksInProperties;
        }

        public bool CreateRegions { get; }
        public bool IncludeDocumentation { get; }
        public bool InjectMixins { get; }
        public bool AddInterfacesToChild { get; }
        /// <summary>
        /// if set, properties will avoid line breaks
        /// in accessor definitions. The whole accessor
        /// will be printed in one line, like
        /// get {return x;}
        /// set {x = value;}
        /// </summary>
        public bool AvoidLineBreaksInProperties { get; } = true;
    }
}

using System;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using System.Windows;
using MixinRefactoring;

namespace MixinSharp
{
    /// <summary>
    /// User interface dialog for mixinSharp options.
    /// The Dialog hosts a WPF user control where the options are set.
    /// The DataContext of the user control is set to the instance of this Dialog page.
    /// The options are loaded / saved to a setting storage every time when they are accessed.
    /// </summary>
    public class OptionPageGrid : UIElementDialogPage
    {
        /// <summary>
        /// path to locate the settings inside the settings storage.
        /// </summary>
        private const string CollectionPath = "mixinSharp";

        /// <summary>
        /// code here is the same as in the settings serializer, but unfortunately
        /// we have to duplicate it, otherwise the options dialog did not load (why?)
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private bool GetOption(string optionName, IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            var store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            var option = store?.GetBoolean(CollectionPath, optionName, false);
            return option ?? false;
        }

        /// <summary>
        /// code here is the same as in the settings serializer, but unfortunately
        /// we have to duplicate it, otherwise the options dialog did not load (why?)
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        private void SetOption(string optionName, bool value, IServiceProvider serviceProvider)
        {
            var shellSettingsManager = new ShellSettingsManager(serviceProvider);
            var store = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!store.CollectionExists(CollectionPath))
                store.CreateCollection(CollectionPath);
            store?.SetBoolean(CollectionPath, optionName, value);
            
        }

        /// <summary>
        /// flag controls whether region blocks should automatically
        /// be generated when mixin methods are forwarded
        /// </summary>
        public bool CreateRegions
        {
            get { return GetOption(nameof(CreateRegions),Site); }
            set { SetOption(nameof(CreateRegions), value,Site); }
        }

        /// <summary>
        /// if a mixin member has a documentation comment,
        /// it will automatically be generated for the forwarding member
        /// if this flag is set.
        /// </summary>
        public bool IncludeDocumentation
        {
            get { return GetOption(nameof(IncludeDocumentation),Site); }
            set { SetOption(nameof(IncludeDocumentation), value,Site); }
        }

        /// <summary>
        /// if set, mixin instances will be injected into 
        /// the childs constructor
        /// </summary>
        public bool InjectMixins
        {
            get { return GetOption(nameof(InjectMixins), Site); }
            set { SetOption(nameof(InjectMixins),value, Site); }
        }

        /// <summary>
        /// returns the WPF user control (which holds the actual user interface
        /// for setting the options)
        /// </summary>
        protected override UIElement Child
        {
            get
            {
                var mixinSettingsControl = new MixinSharpSettingsControl();
                mixinSettingsControl.DataContext = this;
                return mixinSettingsControl;
            }
        }
    }
}
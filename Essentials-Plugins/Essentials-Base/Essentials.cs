/*
 * MIT License
 * 
 * Copyright © 2021 ReeceDeveloper
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 * documentation files (the “Software”), to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
 * to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
 * THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */


using Newtonsoft.Json;

namespace Oxide.Plugins  {
    [Info("Essentials", "ReeceDeveloper", "1.0.0")]
    [Description("The base Essentials plugin.")]
    
    public class Essentials : CovalencePlugin {
        #region Configuration

        private class EssentialsConfig {
            [JsonProperty("(1). Enable the Essentials plugin?")]
            public bool PluginEnabled { get; private set; } = true;
        }

        private EssentialsConfig _config;

        protected override void LoadDefaultConfig() {
            Config.WriteObject(new EssentialsConfig(), true);
        }

        #endregion Configuration

        #region Localisation

        // Localisation configuration goes here.
        
        #endregion Localisation

        #region Initialisation

        void Init() {
            _config = Config.ReadObject<EssentialsConfig>();

            if (!_config.PluginEnabled) {
                Puts("- Currently disabled via the Configuration file.");
                return;
            }

            Puts("- Initialisation completed.");
        }

        #endregion Initialisation
        
        #region Uninitialisation

        void Unload() {
            Puts("- Unloading completed.");
        }
        
        #endregion Uninitialisation

        #region Essentials

        // Individual plugin modules go here.
        
        #endregion Essentials
    }
    
}
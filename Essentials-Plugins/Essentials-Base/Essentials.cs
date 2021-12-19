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
using Oxide.Core.Libraries.Covalence;
using System.Collections.Generic;

namespace Oxide.Plugins  {
    [Info("Essentials", "ReeceDeveloper", "1.0.0")]
    [Description("The base Essentials plugin.")]
    
    public class Essentials : CovalencePlugin {
        #region Configuration

        private class EssentialsConfig {
            [JsonProperty("(1). Enable the Essentials plugin?")]
            public bool PluginEnabled { get; private set; } = true;
            
            [JsonProperty("(2). Enable server whitelisting?")]
            public bool WhitelistEnabled { get; set; }

            [JsonProperty("(3). Enable server auto-broadcasts?")]
            public bool BroadcastsEnabled { get; private set; }

            [JsonProperty("(4). Set auto-broadcast interval.")]
            public float BroadcastInterval { get; private set; } = 30.0f;

            [JsonProperty("(5). Edit auto-broadcast messages.")]
            public string[] BroadcastMessages { get; private set; } = {
                "This is an example message without colour.",
                "This is an example message [#FF0000]with[/#] colour."
            };
        }

        private EssentialsConfig _config;

        protected override void LoadDefaultConfig() {
            Config.WriteObject(new EssentialsConfig(), true);
        }

        #endregion Configuration

        #region Localisation

        protected override void LoadDefaultMessages() {
            lang.RegisterMessages(new Dictionary<string, string> {
                // General.
                ["PluginReloadEvent"] = "Essentials will now reload.",
                ["PermissionDeniedEvent"] = "You do not have permission to use this command.",
                ["InsufficientArgsEvent"] = "You did not provide enough arguments.",
                ["InvalidArgsEvent"] = "One or more of your arguments was invalid.",
                ["PlayerNotFoundEvent"] = "Could not find {0}.",
                ["Disabled"] = "Disabled",
                ["Enabled"] = "Enabled",

                // Whitelist.
                ["WhitelistDenyEvent"] = "You are not whitelisted on this server.",
                ["WhitelistToggleEvent"] = "Whitelisting has now been {0}.",
                ["WhitelistRemoveEvent"] = "{0} was removed from the whitelist.",
                ["WhitelistAddEvent"] = "{0} was added to the whitelist."
            }, this);
        }
        
        #endregion Localisation

        #region Initialisation

        private void Init() {
            _config = Config.ReadObject<EssentialsConfig>();

            if (!_config.PluginEnabled) {
                Puts("- Currently disabled via the Configuration file.");
                return;
            }
            
            if(_config.WhitelistEnabled)
                InitWhitelist();
            
            if(_config.BroadcastsEnabled)
                InitAutoBroadcast();

            Puts("- Initialisation completed.");
        }

        #endregion Initialisation
        
        #region Uninitialisation

        private void Unload() {
            Puts("- Unloading completed.");
        }
        
        #endregion Uninitialisation

        #region Essentials

        #region Whitelist

        private const string WhitelistPerm = "essentials.whitelist.allow";
        private const string WhitelistAdmin = "essentials.admin.whitelist";

        private void InitWhitelist() {
            permission.RegisterPermission(WhitelistPerm, this);
            permission.RegisterPermission(WhitelistAdmin, this);

            Puts("- Whitelisting is currently enabled.");
        }

        [Command("whitelist")]
        private void WhitelistCmd(IPlayer player, string cmd, string[] args) {
            if (!player.HasPermission(WhitelistAdmin)) {
                player.Message(lang.GetMessage("PermissionDeniedEvent", this));
                return;
            }

            if (args.IsEmpty()) {
                player.Message(lang.GetMessage("InsufficientArgsEvent", this));
                return;
            }

            switch (args[0]) {
                case "toggle": {
                    _config.WhitelistEnabled = _config.WhitelistEnabled
                        ? _config.WhitelistEnabled = false
                        : _config.WhitelistEnabled = true;

                    var message = lang.GetMessage("WhitelistToggleEvent", this);
                    
                    player.Message(string.Format(message, _config.WhitelistEnabled 
                        ? lang.GetMessage("Enabled", this).ToLower() 
                        : lang.GetMessage("Disabled", this).ToLower()));
                    
                    Config.WriteObject(_config, true);
                    
                    player.Message(lang.GetMessage("PluginReloadEvent", this));
                    
                    server.Command("oxide.reload Essentials");
                    
                    break;
                } case "add": {
                    if (args.Length < 2) {
                        player.Message(lang.GetMessage("InsufficientArgsEvent", this));
                        break;
                    }

                    var selectedPlayer = players.FindPlayer(args[1]);

                    if (selectedPlayer == null) {
                        player.Message(string.Format(lang.GetMessage("PlayerNotFoundEvent", this), args[1]));
                        break;
                    }
                    
                    selectedPlayer.GrantPermission(WhitelistPerm);
                    
                    player.Message(string.Format(lang.GetMessage("WhitelistAddEvent", this), selectedPlayer.Name));
                    
                    break;
                } case "remove": {
                    if (args.Length < 2) {
                        player.Message(lang.GetMessage("InsufficientArgsEvent", this));
                        break;
                    }

                    var selectedPlayer = players.FindPlayer(args[1]);
                    
                    if (selectedPlayer == null) {
                        player.Message(string.Format(lang.GetMessage("PlayerNotFoundEvent", this), args[1]));
                        break;
                    }
                    
                    selectedPlayer.RevokePermission(WhitelistPerm);
                    
                    player.Message(string.Format(lang.GetMessage("WhitelistRemoveEvent", this), selectedPlayer.Name));
                    
                    break;
                } default: {
                  player.Message(lang.GetMessage("InvalidArgsEvent", this));
                  break;
                }
            }
        }

        private bool IsWhitelisted(string playerId) {
            var player = players.FindPlayerById(playerId);

            return player != null && permission.UserHasPermission(playerId, WhitelistPerm);
        }

        private object CanUserLogin(string name, string id) {
            if (!_config.WhitelistEnabled)
                return null;

            if (IsWhitelisted(id)) {
                return null;
            }

            return IsWhitelisted(id) ? null : lang.GetMessage("WhitelistDenyEvent", this);
        }

        #endregion Whitelist
        
        #region AutoBroadcast

        private void InitAutoBroadcast() {
            var arrayKey = 0; 
            
            if (_config.BroadcastMessages.Length == 0) {
                Puts("- Auto-Broadcasting disabled, no messages present in configuration.");
                return;
            }
            
            timer.Every(_config.BroadcastInterval, () => {
                server.Broadcast(_config.BroadcastMessages[arrayKey]);

                arrayKey++;

                if (arrayKey == _config.BroadcastMessages.Length) {
                    arrayKey = 0;
                }
            });
        }
        
        #endregion AutoBroadcast

        #endregion Essentials
    }
    
}
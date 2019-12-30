using System;
using System.Collections.Generic;
using System.Composition;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiSteamFarm.Json;
using ArchiSteamFarm.Plugins;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace ArchiSteamFarm.Cobra.RenamePlugin {
	[Export(typeof(IPlugin))]
	internal sealed class SetGameAvatarPlugin : IBotCommand {
		private static readonly Random Random = new Random();
		internal static int RandomNext(int min, int max) {
			lock (Random) {
				return Random.Next(min,max);
			}
		}
		public string Name => nameof(SetGameAvatarPlugin);
		public Version Version => typeof(SetGameAvatarPlugin).Assembly.GetName().Version;
		public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args) {
			switch (args[0].ToUpperInvariant()) {
				case "SETGAMEAVATAR" when bot.HasPermission(steamID, BotConfig.EPermission.Master):
					if (args.Length < 3) {
						return "Missing arguments!\n\nUse:\n!setgameavatar APPID AVATARID\n\n\nCheck this site to see the appid and avatarid: https://steamcommunity.com/actions/GameAvatars/ \n\n- First choose the game and hover over the 'View all xx avatars'. Look at the bottom to see the appid (.../ogg/xxx/Avatar/List)\n\n- First Avatar is id: 0 and so on. You can also look at the link after you clicked on a avatar (.../Avatar/Preview/0 as example)!";
					}
					var arg_appid_isNumeric = int.TryParse(args[1], out int arg_appid);
					var arg_avatarid_isNumeric = int.TryParse(args[2], out int arg_avatarid);
					if (!arg_appid_isNumeric) {
						return "The appid is wrong!\nCheck this site to see the appid and avatarid: https://steamcommunity.com/actions/GameAvatars/ \n\n- First choose the game and hover over the 'View all xx avatars'. Look at the bottom to see the appid (.../ogg/xxx/Avatar/List)\n\n- First Avatar is id: 0 and so on. You can also look at the link after you clicked on a avatar (.../Avatar/Preview/0 as example)!";
					}
					if (!arg_avatarid_isNumeric) {
						return "The avatarid is wrong!\nCheck this site to see the appid and avatarid: https://steamcommunity.com/actions/GameAvatars/ \n\n- First choose the game and hover over the 'View all xx avatars'. Look at the bottom to see the appid (.../ogg/xxx/Avatar/List)\n\n- First Avatar is id: 0 and so on. You can also look at the link after you clicked on a avatar (.../Avatar/Preview/0 as example)!";
					}
					string avatar_request = "/games/" + arg_appid.ToString() + "/selectAvatar";
					Dictionary<string, string> avatar_data = new Dictionary<string, string>(2) {
						{ "selectedAvatar", arg_avatarid.ToString() }
					};
					await bot.ArchiWebHandler.UrlPostToHtmlDocumentWithSession(ArchiWebHandler.SteamCommunityURL, avatar_request, avatar_data).ConfigureAwait(false);
					return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.Done);
				default:
					return null;
			}
		}
		public void OnLoaded() {
			ASF.ArchiLogger.LogGenericInfo("SetGameAvatarPlugin by Cobra");
		}
	}
}

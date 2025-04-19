using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LeeHyperrealMod.Modules.Networking
{
    internal class AchievementGranterNetworkRequest : INetMessage
    {
        public int achievementIndex;
        internal NetworkInstanceId netID;

        public AchievementGranterNetworkRequest() { }

        public AchievementGranterNetworkRequest(int achievementIndex, NetworkInstanceId netID)
        {
            this.achievementIndex = achievementIndex;
            this.netID = netID;
        }

        public void Deserialize(NetworkReader reader)
        {
            achievementIndex = reader.ReadInt32();
            netID = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            GameObject obj = Util.FindNetworkObject(netID);

            if (!obj)
            {
                return;
            }

            CharacterBody body = obj.GetComponent<CharacterBody>();
            if (!body)
            {
                return;
            }

            if (!body.hasEffectiveAuthority)
            {
                return;
            }

            CharacterMaster characterMaster = body.master;

            if (!characterMaster) 
            {
                return;
            }

            PlayerCharacterMasterController playerMasterCon = characterMaster.playerCharacterMasterController;

            if (!playerMasterCon) 
            {
                return;
            }

            ServerAchievementIndex serverAchievementIndex = new ServerAchievementIndex();
            serverAchievementIndex.intValue = achievementIndex;

            LocalUser localUser = playerMasterCon.networkUser.localUser;
            if (localUser != null)
            {
                UserAchievementManager userAchievementManager = AchievementManager.GetUserAchievementManager(localUser);
                if (userAchievementManager == null)
                {
                    return;
                }
                BaseAchievement baseAchievement = userAchievementManager.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef.serverIndex == serverAchievementIndex);
                if (baseAchievement == null)
                {
                    return;
                }
                baseAchievement.Grant();
            }


        }
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(achievementIndex);
            writer.Write(netID);
        }
    }
}

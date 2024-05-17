using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Components;

namespace PJKT.SDK2
{
    public class VrcCompsPage : SDKPage
    {
        private RequirementsInfo infoBox;
        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;
        
            if (BoothValidator.BoothsInScene.Length == 0)
            {
                topArea.Add(new NoBoothsMessage());
                return;
            }
        
            //get the report
            BoothStats pickups = BoothValidator.Report.GetStats(StatsType.Pickups);
            BoothStats portals = BoothValidator.Report.GetStats(StatsType.Portals);
            BoothStats avatars = BoothValidator.Report.GetStats(StatsType.AvatarPeds);
            BoothStats udonBehaviours = BoothValidator.Report.GetStats(StatsType.UdonBehaviours);
        
            //create the top info area
            infoBox = new RequirementsInfo(new BoothStats[] {pickups, portals, avatars, udonBehaviours});
            topArea.Add(infoBox);
            
            if (BoothValidator.SelectedBooth == null) return;
        
            //get the graphics
            Texture2D pickupIcon = PjktGraphics.GetGraphic("Pickup");
            Texture2D portalIcon = PjktGraphics.GetGraphic("Portal");
            Texture2D avatarIcon = PjktGraphics.GetGraphic("Avatar");
            Texture2D udonIcon = PjktGraphics.GetGraphic("UdonBehaviour");
            
            //create info panels for pickups
            foreach (VRCPickup pickup in pickups.ComponentList)
            {
                string pickupInfo = $"Auto Hold: {pickup.AutoHold} \n" + $"Pickupable: {pickup.pickupable}";
                InfoPanel panel = new InfoPanel(pickup.gameObject, pickupIcon, "Pickup", pickupInfo, PjktGraphics.GraphicColors["Settings"]);
                scrollView.Add(panel);   
            }

            foreach (VRCPortalMarker portal in portals.ComponentList)
            {
                string portalInfo = $"Portal ID: {portal.roomId} \n" + $"Custom Name: {portal.roomName}";
                InfoPanel panel = new InfoPanel(portal.gameObject, portalIcon, "Portal", portalInfo, PjktGraphics.GraphicColors["Particles"]);
                scrollView.Add(panel);   
            }
        
            foreach (VRCAvatarPedestal avatar in avatars.ComponentList)
            {
                string avatarInfo = $"Avatar ID: {avatar.blueprintId}";
                InfoPanel panel = new InfoPanel(avatar.gameObject, avatarIcon, "Avatar Pedestal", avatarInfo, PjktGraphics.GraphicColors["Avatar"]);
                scrollView.Add(panel);   
            }
        
            foreach (UdonInfo udon in udonBehaviours.ComponentList)
            {
                string udonInfo = $"Program Source: {udon.ProgramSource} \n"
                                  + $"Allowed in PJKT: {udon.Allowed}\n"
                                  + $"Sync Type: {udon.SyncType}\n";

                InfoPanel panel = new InfoPanel(udon.Behaviour.gameObject, udonIcon, "Udon Behaviour", udonInfo, PjktGraphics.GraphicColors["Animation"]);
                scrollView.Add(panel);   
            }
        }
    }
}
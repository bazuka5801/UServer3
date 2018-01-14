using SapphireEngine;
using UnityEngine;
using UServer3.Rust.Network;

namespace UServer3.Rust.Functions
{
    public class WallHack : SapphireType
    {
        private float m_Interval = 0f;

        public override void OnUpdate()
        {
            if (BasePlayer.LocalPlayer?.CanInteract() == true)
            {
                m_Interval += DeltaTime;

                // Every 0.1s
                if (m_Interval < 0.2f) return;
                m_Interval = 0;
                //ConsoleSystem.Log($"Storage Count => { StorageContainer.Containers}");
                foreach (var player in BasePlayer.ListPlayers)
                {
                    if (player == BasePlayer.LocalPlayer) continue;
                    DDraw.DrawBox(player.Position+player.GetCenterVector(), new Vector3(1,1.8f,1), Color.magenta, .2f );
                    DDraw.Text(player.Position+new Vector3(0, 1.8f, 0), $"PLAYER", Color.red, .2f);
                }
            }
        }
    }
}
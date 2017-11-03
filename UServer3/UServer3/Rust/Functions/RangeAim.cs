using System;
using System.Collections.Generic;
using SapphireEngine;
using UnityEngine;
using UServer3.Rust.Network;
using UServer3.Rust;
using UServer3.Rust.Struct;
using  UServer3.CSharp.ExtensionMethods;
using UServer3.Rust.Data;
using UServer3.Rust.Struct;

namespace UServer3.Rust.Functions
{
    public class RangeAim : SapphireType
    {
        public static RangeAim Instance { get; private set; } = null;
        public BasePlayer TargetPlayer = null;

        private float m_interval_update_tick = 0f;
        private float m_no_target_time = 0;
        private Stack<TargetAimInformation> m_list_players = new Stack<TargetAimInformation>();

        public override void OnAwake()
        {
            Instance = this;
        }

        public override void OnUpdate()
        {
            this.m_interval_update_tick += DeltaTime;
            if (this.m_interval_update_tick >= 0.1f)
            {
                this.m_interval_update_tick = 0f;
                ///
                if (BasePlayer.IsHaveLocalPlayer)
                {
                    if (this.m_no_target_time >= 0.5f)
                    {
                        this.m_no_target_time = 0f;
                        this.TargetPlayer = null;
                    }

                    #region [Section] Find Target
                    for (int i = 0; i < BasePlayer.ListPlayers.Count; ++i)
                    {
                        if (BasePlayer.ListPlayers[i].IsLocalPlayer == false && BasePlayer.ListPlayers[i].IsAlive)
                        {
                            float distance = Vector3.Distance(BasePlayer.ListPlayers[i].Position, BasePlayer.LocalPlayer.Position);
                            if ((BasePlayer.LocalPlayer.HasActiveItem && OpCodes.IsFireWeapon_Prefab((EPrefabUID)BasePlayer.LocalPlayer.ActiveItem.PrefabID) && distance < 150) ||  distance < 50)
                            {
                                #region [Section] Range and Radius check
                                Vector3 forward = BasePlayer.LocalPlayer.GetForward() * distance + BasePlayer.LocalPlayer.EyePos;
                                float distance_check = 5f;
                                
                                if (distance < 10)
                                    distance_check = distance / 2;
                                else  if (distance > 30)
                                    distance_check = 9;
                                float distance_point_and_playuer = Vector3.Distance(forward, BasePlayer.ListPlayers[i].Position + new Vector3(0,BasePlayer.ListPlayers[i].GetHeight()*0.5f,0));
                                if (distance_point_and_playuer < distance_check)
                                    m_list_players.Push(new TargetAimInformation { Player = BasePlayer.ListPlayers[i], DistanceCursor =  distance_point_and_playuer});
                                #endregion
                            }
                        }
                    }
                    if (this.m_list_players.Count > 0)
                    {
                        BasePlayer target = null;
                        float dist = float.MaxValue;
                        while (this.m_list_players.Count > 0)
                        {
                            TargetAimInformation player = this.m_list_players.Pop();
                            if (dist > player.DistanceCursor)
                            {
                                dist = player.DistanceCursor;
                                target = player.Player;
                            }
                        }
                        this.TargetPlayer = target;
                    }
                    else if (this.TargetPlayer != null)
                        m_no_target_time += 0.1f;

                    #endregion
                    
                    if (this.TargetPlayer != null)
                        DDraw.Text(this.TargetPlayer.Position + new Vector3(0, this.TargetPlayer.GetHeight(), 0), $"<size=32>.</size>", Color.red, 0.1f);
                    if (this.TargetPlayer != null)
                        DDraw.DrawBox(this.TargetPlayer.Position + new Vector3(0, this.TargetPlayer.GetHeight()*0.5f, 0), this.TargetPlayer.Rotation.ToQuaternion(), new Vector3(1,this.TargetPlayer.GetHeight(), 1), Color.red, 0.05f);
                }
                
                ///
            }
        }
    }
}
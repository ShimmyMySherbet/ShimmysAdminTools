using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    // Incomplete
    public class StructureESPBehaviour : MonoBehaviour
    {
        private bool awake = false;
        public int Rate = 5;
        public int Range = 500;
        public ushort Effect = 124;
        public Player Player;
        public UnturnedPlayer UPlayer;

        public void Awake()
        {
            awake = true;
            Player = GetComponentInParent<Player>();
            if (Player != null) UPlayer = UnturnedPlayer.FromPlayer(Player);
        }
        public void Stop()
        {
            awake = false;
        }
        public void OnDestroy()
        {
            Stop();
        }

        private int Enlapsed = 0;
        public void FixedUpdate()
        {
            if (awake)
            {
                Enlapsed++;
                if (Enlapsed >= Rate)
                {
                    Enlapsed = 0;
                    OnUpdate();
                }
            }
        }


        private List<Vector3> GetBarricadesInRegion(Vector3 Pos, float Rd)
        {
            List<Vector3> Result = new List<Vector3>();

            foreach(var Region in BarricadeManager.regions)
            {
                foreach(var Bar in Region.barricades)
                {
                    if (Vector3.Distance(Bar.point, Pos) <= Rd) Result.Add(Bar.point);
                }
            }
            return Result;
        }



        private void OnUpdate()
        {
            Vector3 CameraPos = Player.look.getEyesPosition();
            List<Vector3> Barricades = GetBarricadesInRegion(CameraPos, Range);
            List<Vector3> DisplayPositions = new List<Vector3>();
            foreach (Vector3 Barricade in Barricades)
            {
                Vector3 OffsetPosition = Vector3.MoveTowards(CameraPos, Barricade, 50);
                float DistanceToBarricade = Vector3.Distance(CameraPos, Barricade);
                if (Vector3.Distance(OffsetPosition, Barricade) < 5f)
                {
                    OffsetPosition = Vector3.MoveTowards(CameraPos, Barricade, DistanceToBarricade * 0.5f);
                }
                bool Allow = DistanceToBarricade > 10f;
                if (Allow)
                foreach(Vector3 Struct in DisplayPositions)
                {
                    if (Vector3.Distance(Struct, OffsetPosition) < 10f)
                        Allow = false;
                }
                if (Allow) DisplayPositions.Add(OffsetPosition);
            }

            foreach (Vector3 RenderPos in DisplayPositions)
            {
                EffectManager.sendEffectReliable(Effect, UPlayer.Player.channel.owner.transportConnection, RenderPos);
            }
        }

    }
}

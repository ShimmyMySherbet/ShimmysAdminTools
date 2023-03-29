using SDG.Unturned;
using System.Linq;
using UnityEngine;

namespace ShimmysAdminTools.Models
{
    public class RaycastResult
    {
        public RaycastHit Raycast;
        public InteractableVehicle Vehicle;
        public BarricadeData Barricade;
        public BarricadeRegion BarricadeRegion;
        public StructureData Structure;
        public StructureRegion StructureRegion;
        public bool RaycastHit = false;

        public byte BarricadeX;
        public byte BarricadeY;
        public ushort BarricadePlant;
        public ushort BarricadeIndex;


        public byte StructureX;
        public byte StructureY;
        public ushort StructureIndex;

        public Transform BarricadeRootTransform;
        public Transform StructureRootTransform;

        public RaycastResult(RaycastHit Info, bool hit)
        {
            RaycastHit = hit;
            if (hit)
            {
                Raycast = Info;
                Vehicle = TryGetEntity<InteractableVehicle>();
                Transform target = Raycast.collider?.transform;
                if (target != null)
                {
                    if (target.CompareTag("Barricade"))
                    {
                        target = DamageTool.getBarricadeRootTransform(target);
                        BarricadeRootTransform = target;
                        if (BarricadeManager.tryGetInfo(target, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion Region, out BarricadeDrop Drop))
                        {
                            BarricadeRegion = Region;
                            BarricadeX = x;
                            BarricadeY = y;
                            BarricadePlant = plant;
                            BarricadeIndex = index;
                            BarricadeData B = Region.barricades.FirstOrDefault(D => D.instanceID == Drop.instanceID);
                            if (B != null)
                            {
                                Barricade = B;
                            }
                        }
                    }
                    else if (target.CompareTag("Structure"))
                    {
                        target = DamageTool.getStructureRootTransform(target);
                        StructureRootTransform = target;
                        if (StructureManager.tryGetInfo(target, out byte x, out byte y, out ushort index, out StructureRegion Region))
                        {
                            StructureX = x;
                            StructureY = y;
                            StructureIndex = index;
                            StructureRegion = Region;
                            StructureData B = Region.structures[index];
                            if (B != null)
                            {
                                Structure = B;
                            }
                        }
                    }
                }
            }
        }

        public T TryGetEntity<T>()
        {
            return Raycast.transform.GetComponentInParent<T>();
        }

        public T[] TryGetEntities<T>()
        {
            return Raycast.transform.GetComponentsInParent<T>();
        }

		public bool ParentHasComponent<T>()
		{
			return TryGetEntity<T>() != null;
		}

		public bool HasComponent<T>()
		{
			return Raycast.transform.GetComponent<T>() != null;
		}

        public T GetComponent<T>()
        {
			return Raycast.transform.GetComponent<T>();
		}
	}
}
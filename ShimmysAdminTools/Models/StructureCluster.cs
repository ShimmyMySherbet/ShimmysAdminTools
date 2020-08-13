using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ShimmysAdminTools.Models
{
    public class StructureCluster
    {
        public Vector3 OriginPosition;
        public List<Vector3> Objects = new List<Vector3>();
        public int StorageSlots = 0;
    }
}

using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using UnityEngine;

namespace ShimmysAdminTools.Behaviors
{
    public class RepeatCommandQueue : MonoBehaviour
    {
        public static RepeatCommandQueue Instance;
        public bool awake = false;
        public List<Action> CommandQueues = new List<Action>();

        public static void EnqueueAction(Action action) => Instance.CommandQueues.Add(action);

        public void Awake()
        {
            awake = true;
            Instance = this;
        }

        public void FixedUpdate()
        {
            if (CommandQueues.Count > 0)
            {
                List<Action> Removes = new List<Action>();
                try
                {
                    foreach (var action in CommandQueues)
                    {
                        try
                        {
                            action();
                        }
                        finally
                        {
                            Removes.Add(action);
                        }
                    }
                }
                finally
                {
                    Removes.ForEach(x => CommandQueues.Remove(x));
                }
            }
        }
    }
}
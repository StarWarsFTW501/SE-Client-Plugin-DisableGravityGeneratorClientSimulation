using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Havok;
using Sandbox.Engine.Physics;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Multiplayer;
using SpaceEngineers.Game.Entities.Blocks;
using VRage.ModAPI;

namespace ClientPlugin
{
    [HarmonyPatch]
    internal class MyGamePatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MyGravityGeneratorBase), "phantom_Enter")]
        public static bool Prefix_MyGravityGeneratorBase_phantom_Enter(MyGravityGeneratorBase __instance, HkPhantomCallbackShape sender, HkRigidBody body)
        {
            if (!Sync.IsServer && Plugin.Instance.PluginConfig.DisableUpdate)
            {
                IMyEntity entity = body.GetEntity(0U);

                // Lord have mercy for what I'm about to do... I really didn't want to use the mod API but there's no other way
                if (entity == null || entity is SpaceEngineers.Game.ModAPI.IMyVirtualMass)
                    return false;
            }
            return true;
        }
    }
}

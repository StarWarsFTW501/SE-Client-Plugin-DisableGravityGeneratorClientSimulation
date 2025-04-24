using Sandbox.Game.Entities;
using Sandbox.Game.GameSystems;
using SpaceEngineers.Game.Entities.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClientPlugin
{
    internal class MyPatchUtilities
    {
        readonly static FieldInfo _myGravityProviderSystem_m_proxyIdMap_Info = typeof(MyGravityProviderSystem).GetField("m_proxyIdMap", BindingFlags.Static | BindingFlags.NonPublic);
        readonly static MethodInfo _myGravityGeneratorBase_UpdateFieldShape_Info = typeof(MyGravityGeneratorBase).GetMethod("UpdateFieldShape", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void ForceFieldUpdates()
        {
            Dictionary<IMyGravityProvider, int> m_proxyIdMap = (Dictionary<IMyGravityProvider, int>)_myGravityProviderSystem_m_proxyIdMap_Info.GetValue(null);

            foreach (IMyGravityProvider provider in m_proxyIdMap.Keys)
            {
                if (provider is MyGravityGeneratorBase gravityGenerator)
                {
                    _myGravityGeneratorBase_UpdateFieldShape_Info.Invoke(gravityGenerator, null);
                }
            }
        }
    }
}

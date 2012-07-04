using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inferables.Internal;
using Inferables.GenBindings;

namespace Inferables
{
    //Other comment
    public class Binding: Hashable<Binding>, IAllowedBinding
    {
        public IEnumerable<BindingMap> Maps { get; private set; }
        private List<BindingMap> mapList; 

        private static object[] defaultNamespaceMap = {"~"};

        public Binding(params object [] dependencyMaps)
        {
            dependencyMaps = dependencyMaps == null || dependencyMaps.Length == 0 ? defaultNamespaceMap : dependencyMaps;

            var resolvedDependencies = new List<BindingMap>();

            foreach (var dependency in dependencyMaps)
            {
                var mapConfigString = dependency as string;
                if (mapConfigString != null)
                {
                    var maps = BindingMapParser.GetMaps(mapConfigString);
                    AddRangeIfNew(resolvedDependencies, maps);
                    continue;
                }

                var binding = dependency as IAllowedBinding;

                if (binding != null)
                {
                    var maps = binding.Maps;
                    AddRangeIfNew(resolvedDependencies, maps);
                    continue;
                }

            

                throw new ArgumentException("Binding dependencies can only be a Binding type, existing modules, or a namespace map configuration string.");
            }

            //AddRangeIfNew(resolvedDependencies, defaultNameSpaceMap);

            mapList = resolvedDependencies;
            Maps = resolvedDependencies.ToEnumerable();

        }




        private static void AddRangeIfNew(List<BindingMap> resolvedDependencies, IEnumerable<BindingMap> maps)
        {
            foreach (var map in maps)
                AddIfNew(resolvedDependencies, map);
        }

        private static void AddIfNew(List<BindingMap> resolvedDependencies, BindingMap map)
        {
            if (!resolvedDependencies.Contains(map))
            {
                resolvedDependencies.Add(map);
            }
        }

        sealed protected override bool EqualsOverride(Binding compare)
        {
            if (this.mapList.Count != compare.mapList.Count)
                return false;

            for (int i = 0; i < this.mapList.Count; i++)
                if (!this.mapList[i].Equals(compare.mapList[i]))
                    return false;

            return true;

        }
    }

    public interface IBinding
    {
        IEnumerable<BindingMap> Maps { get;}
    }

    internal interface IAllowedBinding: IBinding
    {
    }

}

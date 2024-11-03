
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Dossier_Registratie.Helper
{
    public class ModelCompare
    {
        public bool AreValuesEqual<T>(T original, T modified)
        {
            if (original == null || modified == null)
                return false;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.CanRead && property.GetIndexParameters().Length == 0)
                {
                    var originalValue = property.GetValue(original); // Corrected line
                    var modifiedValue = property.GetValue(modified);

                    if (!object.Equals(originalValue, modifiedValue))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public static class CollectionCompare
    {
        public static bool AreCollectionsEqual<T>(ObservableCollection<T> collection1, ObservableCollection<T> collection2)
        {
            if (ReferenceEquals(collection1, collection2))
                return true;

            if (collection1 == null || collection2 == null)
                return false;

            if (collection1.Count != collection2.Count)
                return false;

            return collection1.SequenceEqual(collection2);
        }
    }
}

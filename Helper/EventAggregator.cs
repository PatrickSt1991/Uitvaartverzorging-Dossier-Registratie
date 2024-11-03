using System;

namespace Dossier_Registratie.Helper
{
    public static class IntAggregator
    {
        public static void Transmit(int data)
        {
            if (OnDataTransmitted != null)
            {
                OnDataTransmitted(data);
            }
        }

        public static Action<int> OnDataTransmitted;
    }
    public static class ComboAggregator
    {
        public static void Transmit()
        {
            if (OnDataTransmitted != null)
            {
                OnDataTransmitted();
            }
        }

        public static Action OnDataTransmitted;
    }
}

using System;

namespace Dossier_Registratie.Helper
{
    public static class IntAggregator
    {
        public static Action<int> OnDataTransmitted { get; set; } = _ => { };
        public static void Transmit(int data)
        {
            OnDataTransmitted?.Invoke(data);
        }
    }

    public static class ComboAggregator
    {
        public static Action OnDataTransmitted { get; set; } = () => { };
        public static void Transmit()
        {
            OnDataTransmitted?.Invoke();
        }
    }
}

namespace Saint.Ikki.Fx.Shared.Models
{ 
    public static class StatusProcessor
    {
        public static string Procesando => "Procesando";
        public static string Procesado => "Procesado";
        public static string NoProceso => "";
    }

    public enum ProcessStatus
    {
        Completed = 1,
        WithErrors = 2,
        InExecution = 3,
        Blocked = 4,
    }
}

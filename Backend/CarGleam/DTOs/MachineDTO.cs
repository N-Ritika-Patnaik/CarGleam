using System.ComponentModel.DataAnnotations;

namespace CarGleam.DTOs
{
    public class MachineDTO
    {
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public string Status { get; set; }
        public string MachineType { get; set; }
        public TimeSpan Duration { get; set; } 
    }
}
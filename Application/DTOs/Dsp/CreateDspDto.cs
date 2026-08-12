using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Dsp
{
    public class CreateDspDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class DspResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}

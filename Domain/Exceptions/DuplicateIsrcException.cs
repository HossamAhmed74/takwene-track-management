using System;

namespace Domain.Exceptions 
{
    public class DuplicateIsrcException : Exception
    {
        public DuplicateIsrcException(string isrc)
            : base($"A track with the ISRC '{isrc}' already exists.")
        {
        }
    }
}
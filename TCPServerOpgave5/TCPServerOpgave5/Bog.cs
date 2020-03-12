using System;

namespace TCPServerOpgave5
{
    public class Bog
    {
        public string Titel { get; }
        public string Forfatter { get; }
        public int Sidetal { get; }
        public string Isbn13 { get; }


        public Bog(string titel, string forfatter, int sidetal, string isbn13)
        {
            if (string.IsNullOrWhiteSpace(titel))
                throw new ManglendeTitelException();

            if (forfatter.Length < 2)
                throw new ForfatterForKortException();
            
            if (sidetal < 4 || sidetal > 1000)
                throw new UgyldigtSidetalException();

            if (isbn13.Length != 13)
                throw new UgyldigtIsbnException();

            Titel = titel;
            Forfatter = forfatter;
            Sidetal = sidetal;
            Isbn13 = isbn13;
        }
    }

    public class ManglendeTitelException : Exception
    {
    }

    public class ForfatterForKortException : Exception
    {
    }

    public class UgyldigtSidetalException : Exception
    {
    }

    public class UgyldigtIsbnException : Exception
    {
    }
}

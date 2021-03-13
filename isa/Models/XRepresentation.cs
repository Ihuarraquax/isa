namespace isa
{
    public class XRepresentation
    {
        public int Index { get; set; }
        public decimal GeneratedXReal { get; set; }
        public int XIntFromGeneratedXReal { get; set; }
        public string XBinFromXInt { get; set; }
        public int XIntFromXBin { get; set; }
        public decimal XRealFromXInt { get; set; }
        public decimal XFunction { get; set; }
    }
}
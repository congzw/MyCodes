using System.Collections.Generic;

namespace UsefulCodes.AsciiTrees
{
    public class AsciiTree
    {
        public string Name { get; set; }
        public List<AsciiTree> Children { get; } = new List<AsciiTree>();
    }
}
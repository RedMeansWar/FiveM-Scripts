using CitizenFX.Core;

namespace NearestPostal.Client
{
    internal class Postal
    {
        public string Code { get; set; }
        public Vector2 Location { get; set; }
        
        public Postal(string code, float x, float y)
        {
            Code = code;
            Location = new(x, y);
        }
    }
}

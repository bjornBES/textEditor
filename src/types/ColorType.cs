
public struct ColorType
{
    public string color
    {
        get
        {
            uint uintColor = BitConverter.ToUInt32([r, g, b, a]);
            string hex = Convert.ToString(uintColor, 16).PadRight(6, '0');
            return hex;
        }
    }
    internal byte r { get; private set; }
    internal byte g { get; private set; }
    internal byte b { get; private set; }
    internal byte a { get; private set; }

    public ColorType(string color)
    {
        byte[] colors = Convert.FromHexString(color);
        r = colors[0];
        g = colors[1];
        b = colors[2];
        a = colors[3];
    }
}
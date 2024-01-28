namespace csharp_gameboy.Util
{
    public class MathUtil
    {
        public static byte WrapToByte(int a)
        {
            return (byte)WrapTo(a, 0xFF);
        }

        public static int WrapTo(int a, int max)
        {
            return (a % max + max) % max;
        }

        public static bool IsBitSet(byte b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }

        public static bool IsBitSet(ushort b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }

        public static byte SetOrClearBit(byte b, bool c, int pos)
        {
            return c ? SetBit(b, pos) : ClearBit(b, pos);
        }

        public static ushort SetOrClearBit(ushort b, bool c, int pos)
        {
            return c ? SetBit(b, pos) : ClearBit(b, pos);
        }

        public static byte SetBit(byte b, int pos)
        {
            b |= (byte)(1 << pos);
            return b;
        }

        public static ushort SetBit(ushort b, int pos)
        {
            b |= (ushort)(1 << pos);
            return b;
        }

        public static byte ClearBit(byte b, int pos)
        {
            b &= (byte)~(1 << pos);
            return b;
        }

        public static ushort ClearBit(ushort b, int pos)
        {
            b &= (ushort)~(1 << pos);
            return b;
        }

        public static unsafe byte BoolAsByte(bool b)
        {
            return *(byte*)&b;
        }

        public static ushort BytesToU16(byte a, byte b)
        {
            return (ushort)((b << 8) + a);
        }

        public static sbyte ToSigned(byte a)
        {
            return unchecked((sbyte)a);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using static Utils.Program;

namespace Day16
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // ReadByte input
            string stringInput = Console.ReadLine()!;
            BitArray bits = HexadecimalToBits(stringInput);
            int start = 0;
            (Packet topPacket, start) = ParsePacket(bits, start);
            
            if (!PeekWhetherEnd(bits, start)) throw new ArgumentException("Packet not finished!");
            Debug.WriteLine(topPacket);
            
            // Calculate sum of versions through recursion
            Console.WriteLine($"Total version sum: {topPacket.GetTotalVersion()}");

            // Calculate sum of values
            Console.WriteLine($"Total value: {topPacket.GetValue()}");
        }
    
        private static BitArray HexadecimalToBits(string stringInput)
        {
            return MakeNibbles(stringInput, c => byte.Parse(c.ToString(), NumberStyles.HexNumber));
        }

        private static BitArray MakeNibbles<T>(IEnumerable<T> items, Func<T, byte> parseToNibble)
        {
            List<T> enumerable = items.ToList();
            BitArray bits = new BitArray(enumerable.Count * 4);
            for (int chunk = 0; chunk < enumerable.Count; chunk++)
            {
                byte nibble = parseToNibble(enumerable[chunk]);
                for (int bitInNibble = 0; bitInNibble < 4; bitInNibble++)
                {
                    bits.Set(bitInNibble + chunk * 4, ((nibble >> (3 - bitInNibble)) & 0b1) == 1);
                }
            }

            return bits;
        }
        
        private abstract class Packet
        {
            public byte version;
            public byte type;

            protected Packet(byte version, byte type)
            {
                this.version = version;
                this.type = type;
            }

            public abstract long GetTotalVersion();

            public abstract BigInteger GetValue();
        }

        private class LiteralPacket : Packet
        {
            public BitArray literalValue;
            public LiteralPacket(byte version, byte type, BitArray literalValue) : base(version, type)
            {
                this.literalValue = literalValue;
            }

            public override string ToString()
            {
                return $"Literal {ExtractValue()}";
            }

            public override long GetTotalVersion()
            {
                return version;
            }

            public override BigInteger GetValue()
            {
                return ExtractValue();
            }

            public BigInteger ExtractValue()
            {
                BigInteger value = 0;
                for (int i = 0; i < literalValue.Count; i++)
                {
                    value <<= 1;
                    if (literalValue[i]) value |= 0b1;
                }
                return value;
            }
        }

        private class OperatorPacket : Packet
        {
            public List<Packet> children;
            public OperatorPacket(byte version, byte type, List<Packet> children) : base(version, type)
            {
                this.children = children;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder($"Operator {type} [");
                if (children.Count > 0)
                {
                    builder.Append(children.First());
                    for (int i = 1; i < children.Count; i++)
                    {
                        builder.Append(", ").Append(children[i]);
                    }
                }

                return builder.Append(']').ToString();
            }

            public override long GetTotalVersion()
            {
                return version + children.Sum(c => c.GetTotalVersion());
            }

            public override BigInteger GetValue()
            {
                return type switch
                {
                    0 => children.Aggregate<Packet, BigInteger>(0, (integer, packet) => integer + packet.GetValue()),
                    1 => children.Aggregate<Packet, BigInteger>(1, (integer, packet) => integer * packet.GetValue()),
                    2 => children.Min(p => p.GetValue()),
                    3 => children.Max(p => p.GetValue()),
                    5 => children[0].GetValue() > children[1].GetValue() ? 1 : 0,
                    6 => children[0].GetValue() < children[1].GetValue() ? 1 : 0,
                    7 => children[0].GetValue() == children[1].GetValue() ? 1 : 0,
                    _ => throw new ArgumentException($"Invalid type {type}")
                };
            }
        }

        public static (byte result, int end) ReadByte(this BitArray bits, int start, byte length)
        {
            if (length > 8) throw new ArgumentException("Too long!");
            byte result = 0;
            for (int i = start; i < start + length; i++)
            {
                result = (byte)(result << 1 | (bits[i] ? 0b1 : 0b0));
            }

            return (result, start + length);
        }

        public static (int result, int end) ReadInt(this BitArray bits, int start, byte length)
        {
            if (length > 16) throw new ArgumentException("Too long!");
            int result = 0;
            for (int i = start; i < start + length; i++)
            {
                result = result << 1 | (bits[i] ? 0b1 : 0b0);
            }

            return (result, start + length);
        }

        public static (bool result, int end) ReadBool(this BitArray bits, int start)
        {
            return (bits[start], start + 1);
        }

        private static (Packet packet, int end) ParsePacket(BitArray bits, int start)
        {
            (byte version, start) = bits.ReadByte(start, 3);
            (byte type, start) = bits.ReadByte(start, 3);
            if (type == 4) return ParseLiteral(bits, start, version, type).Trace(pair => Debug.WriteLine($"Parsed: {pair.packet}"));
            else return ParseOperator(bits, start, version, type).Trace(pair => Debug.WriteLine($"Parsed: {pair.packet}"));
        }
        
        private static (Packet packet, int end) ParseLiteral(BitArray bits, int start, byte version, byte type)
        {
            List<byte> nibbleValues = new();
            bool shouldContinue = true;
            while (shouldContinue)
            {
                (shouldContinue, start) = bits.ReadBool(start);
                (byte value, start) = bits.ReadByte(start, 4);
                nibbleValues.Add(value);
            }

            BitArray literalValue = MakeNibbles(nibbleValues, Id);
            return (new LiteralPacket(version, type, literalValue), start);
        }

        private static (Packet packet, int end) ParseOperator(BitArray bits, int start, byte version, byte type)
        {
            List<Packet> children = new();
            (bool length11, start) = bits.ReadBool(start);
            if (length11)
            {
                (int packetsUpcoming, start) = bits.ReadInt(start, 11);
                Debug.WriteLine($"Operator contains {packetsUpcoming} packets");
                for (int i = 0; i < packetsUpcoming; i++)
                {
                    (Packet child, start) = ParsePacket(bits, start);
                    children.Add(child);
                }
            }
            else
            {
                (int lengthInBits, start) = bits.ReadInt(start, 15);
                Debug.WriteLine($"Operator contains {lengthInBits} bits of packets");
                int expectedEnd = start + lengthInBits;
                while (start < expectedEnd)
                {
                    (Packet child, start) = ParsePacket(bits, start);
                    children.Add(child);
                }
                Debug.WriteLine($"Finished at {start}. Expected {expectedEnd}");
            }

            return (new OperatorPacket(version, type, children), start);
        }

        private static bool PeekWhetherEnd(BitArray bits, int start)
        {
            for (int i = start; i < bits.Count; i++)
            {
                if (bits[i]) return false;
            }

            // If all coming bits are false, we have reached the end.
            return true;
        }
    }
}
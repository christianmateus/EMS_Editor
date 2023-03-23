using System.Collections.Generic;

namespace EMS_Editor
{
    public class EMS
    {
        public long[] EMSOffsets = new long[]{
            // r100+
            0x4117A9E0, // r100
            0x41371F80, // r101
            0x41516260, // r102
            0x416502C0, // r103
            0x41798E00, // r104
            0x418FB900, // r105
            // r200+
            0x41B7F6C0, // r200
            0x41D9DCC0, // r201
            0x41EB9E20, // r202
            0x41F8D440, // r203
            0x420B6E80, // r204
            0x422101C0, // r205
            0x423A2D80, // r206
            // r300+
            0x426958C0, // r300
            0x428591C0, // r301
            0x4293A240, // r302
            0x42A2FA00, // r303
            0x42C11400, // r304
            0x42D1D860, // r305
            // r400+
            0x42DA6F80, // r400
            0x42E7CF20, // r401
            0x42F48A80, // r402
            0x43019660, // r403
            0x430D14C0, // r404
            0x43203F40, // r405
            0x432DCDE0, // r406
            0x433A7C40, // r407
            0x434ACC00, // r408
            0x435FEF00, // r409
            // r500+
            0x4374D5C0, // r500
            0x438E35C0, // r501
            0x43AD3E00, // r503
            0x43C3CCA0, // r504
            0x43E41200, // r505
            0x43F85540, // r506
            0x440CC5C0, // r507
            0x44260CC0, // r508
            0x4445BE80, // r509
            // r600+
            0x44630780, // r600
            0x44766C80, // r601
            0x44845D80, // r602
            0x44967100, // r603
            0x44BFBA60, // r604
            0x44C71200, // r605
            0x44E29DA0, // r606
            0x44F6B820, // r607
            0x451EA420, // r608
            // r700+
            0x4546E100, // r700
            0x455E8F80, // r701
            0x456912C0, // r702
            0x4577BA60, // r703
            0x45812EE0, // r704
            // r800+
            0x45A654A0, // r801
            0x45C14640, // r802
            0x45CE1D80, // r803
            0x45DD5440, // r804
            0x45EC3D80, // r805
            0x45FC0000, // r806
        };
        public struct EMS_Struct
        {
            public uint Magic;
            public uint EnemyCount;
            public uint Unk1;
            public float PosX;
            public float PosY;
            public float PosZ;
            public float Padding1;
            public float RotX;
            public float RotY;
            public float RotZ;
            public float Padding2;
            public byte Weapon;
            public byte Unk2;
            public byte Action;
            public byte isSpawned;
            public uint Variation;
            public byte Animation;
            public byte EnemySubtype;
            public byte EnemyType;
            public byte Index;
            public uint Unk3;
            public uint Unk4;
            public uint Unk5;
            public uint Unk6;
            public uint Unk7;
        }

        public Dictionary<byte, byte[]> EnemiesList = new Dictionary<byte, byte[]>
        {
            {0x00, new byte[] {0x00, 0x02, 0x06, 0x0A, 0x1A}},
            {0x01, new byte[] {0x00, 0x0C, 0x1A}},
            {0x02, new byte[] {0x00, 0x02, 0x03, 0x0A, 0x0F}},
            {0x04, new byte[] {0x00, 0x03, 0x0A}},
            {0x05, new byte[] {0x00, 0x03, 0x0A, 0x0C, 0x1A}},
            {0x07, new byte[] {0x05, 0x1B, 0x42}},
            {0x08, new byte[] {0x00, 0x02, 0x05, 0x06, 0x0F}},
            {0x09, new byte[] {0x09}},
            {0x0A, new byte[] {0x07, 0x1D, 0x40, 0x48}},
            {0x0B, new byte[] {0x00, 0x04, 0x05, 0x1E, 0x45}},
            {0x0C, new byte[] {0x01, 0x04, 0x05, 0x0C, 0x1A}},
            {0x0D, new byte[] {0x01, 0x06, 0x10, 0x1A, 0x25}},
            {0x0E, new byte[] {0x03, 0x16, 0x1B}},
            {0x0F, new byte[] {0x07, 0x1A, 0x41, 0x46, 0x49}},
            {0x10, new byte[] {0x1A, 0x1D, 0x49}},
            {0x11, new byte[] {0x00, 0x03, 0x11}},
            {0x14, new byte[] {0x0A, 0x0C, 0x0D, 0x47}},
            {0x15, new byte[] {0x02, 0x05, 0x06, 0x1A}},
            {0x16, new byte[] {0x07, 0x0D, 0x1A, 0x1D}},
            {0x17, new byte[] {0x02, 0x03, 0x16, 0x17}},
            {0x18, new byte[] {0x18, 0x44, 0x3A}},
            {0x1A, new byte[] {0x05, 0x16, 0x1B, 0x42, 0x46}},
            {0x1B, new byte[] {0x02, 0x16, 0x1B, 0x45}},
            {0x1C, new byte[] {0x01, 0x02, 0x03, 0x16, 0x48}},
            {0x1D, new byte[] {0x03, 0x16, 0x24, 0x25, 0x26}},
            {0x1E, new byte[] {0x01, 0x0D, 0x24, 0x25, 0x4F}},
            {0x20, new byte[] {0x20, 0x21, 0x22}},
            {0x23, new byte[] {0x23}},
            {0x24, new byte[] {0x00, 0x04, 0x10, 0x45, 0x1E}},
            {0x25, new byte[] {0x01, 0x04, 0x06, 0x1A}},
            {0x30, new byte[] {0x30}},
            {0x31, new byte[] {0x31}},
            {0x32, new byte[] {0x32}},
            {0x3A, new byte[] {0x04, 0x25, 0x3A, 0x42, 0x44, 0x47, 0x5B}},
            {0x3B, new byte[] {0x3B}},
            {0x40, new byte[] {0x1E, 0x40, 0x41, 0x45, 0x49}},
            {0x41, new byte[] {0x00, 0x01, 0x04, 0x25, 0x47}},
            {0x42, new byte[] {0x01, 0x06, 0x1A, 0x25, 0x42}},
            {0x43, new byte[] {0x05, 0x0C, 0x1A, 0x42, 0x47}},
            {0x44, new byte[] {0x1A, 0x1C, 0x3A, 0x44, 0x4F}},
            {0x46, new byte[] {0x05, 0x16, 0x1B, 0x46}},
            {0x47, new byte[] {0x47}},
            {0x4F, new byte[] {0x0C, 0x40, 0x46, 0x49, 0x4F}},
            {0x52, new byte[] {0x04, 0x1E}},
            {0x5A, new byte[] {0x5A, 0x5B}},
            {0x70, new byte[] {0x02, 0x0C}},
            {0x76, new byte[] {0x0D, 0x0F, 0x24, 0x26}},
            {0x78, new byte[] {0x0F}},
            {0xA5, new byte[] {0xA2, 0xA4, 0xA5, 0xA7, 0xA9, 0xAB}},
            {0xAD, new byte[] {0xAD}}
    };

        public Dictionary<byte, byte[]> BossesList = new Dictionary<byte, byte[]>
        {
            {0x03, new byte[]{0x50, 0x51}}, // Elvis
            {0x04, new byte[]{0x14, 0x15}}, // Gold & Silver
            {0x06, new byte[]{0x08, 0x0B, 0x1C}}, // Bruce, Conchita & Felix
            {0x11, new byte[]{0x11}}, // 
            {0x13, new byte[]{0x0E, 0x13, 0x17}}, // Guitarrists
            {0x1F, new byte[]{0x1F}}, // Ninja 
            {0x44, new byte[]{0x44, 0x4F}}, // Black power + Ninja gordo
            {0x52, new byte[]{0x52}}, // Shannon
            {0x56, new byte[]{0x56}}, // Azel
            {0x60, new byte[]{0x60}}, // Elvis Demon
            {0x64, new byte[]{0x64}}, // Shannon Demon
            {0x65, new byte[]{0x65}}, // Arcan Demon
            {0x6A, new byte[]{0x6A}}, // Angra
            {0x70, new byte[]{0x70, 0x74}}, // Power rangers
            {0x75, new byte[]{0x75}}, // Dr. Ion
            {0x76, new byte[]{0x76}}, // Dr. Ion DESERT
            {0x78, new byte[]{0x78}}, // Gorilla
            {0x79, new byte[]{0x79}}, // Gorilla 
            {0x7E, new byte[]{0x7E}}  // Double God Hand
        };

        public byte[] WeaponsList = new byte[]
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x18, 0x20, 0x40, 0x80
        };
    }
}

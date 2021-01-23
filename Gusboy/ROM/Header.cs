namespace Gusboy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Everything related to parsing ROM headers.
    /// </summary>
    public partial class ROM
    {
        private const int OFFSET_TITLE = 0x134;
        private const int OFFSET_TYPE = 0x147;
        private const int OFFSET_ROM_SIZE = 0x148;
        private const int OFFSET_RAM_SIZE = 0x149;
        private const int OFFSET_DESTINATION_CODE = 0x14A;
        private const int OFFSET_OLD_LICENSEE_CODE = 0x14B;
        private const int OFFSET_NEW_LICENSEE_CODE = 0x144;
        private const int OFFSET_SGB = 0x146;
        private const int OFFSET_MASKROM_VERSION = 0x14C;
        private const int OFFSET_CGB = 0x143;
        private const int OFFSET_HEADER_CHECKSUM = 0x14D;
        private const int OFFSET_GLOBAL_CHECKSUM = 0x14E;

        private readonly int[] ramSizeTable = { 0x00, 0x800, 0x2000, 0x8000, 0x20000, 0x10000 };

        private readonly Dictionary<int, string> newLicensee = new Dictionary<int, string>
        {
            { 0x00, "None" },
            { 0x01, "Nintendo R&D1" },
            { 0x08, "Capcom" },
            { 0x13, "Electronic Arts" },
            { 0x18, "Hudson Soft" },
            { 0x19, "B-ai" },
            { 0x20, "Kss" },
            { 0x22, "Pow" },
            { 0x24, "PCM Complete" },
            { 0x25, "San-x" },
            { 0x28, "Kemco Japan" },
            { 0x29, "Seta" },
            { 0x30, "Viacom" },
            { 0x31, "Nintendo" },
            { 0x32, "Bandai" },
            { 0x33, "Ocean/Acclaim" },
            { 0x34, "Konami" },
            { 0x35, "Hector" },
            { 0x37, "Taito" },
            { 0x38, "Hudson" },
            { 0x39, "Banpresto" },
            { 0x41, "Ubisoft" },
            { 0x42, "Atlus" },
            { 0x44, "Malibu" },
            { 0x46, "Angel" },
            { 0x47, "Bullet-Proof" },
            { 0x49, "Irem" },
            { 0x51, "Acclaim" },
            { 0x52, "Activision" },
            { 0x53, "American sammy" },
            { 0x54, "Konami" },
            { 0x55, "Hi tech entertainment" },
            { 0x56, "LJN" },
            { 0x57, "Matchbox" },
            { 0x58, "Mattel" },
            { 0x59, "Milton Bradley" },
            { 0x60, "Titus" },
            { 0x61, "Virgin" },
            { 0x64, "LucasArts" },
            { 0x67, "Ocean" },
            { 0x69, "Electronic Arts" },
            { 0x70, "Infogrames" },
            { 0x71, "Interplay" },
            { 0x72, "Broderbund" },
            { 0x73, "sculptured" },
            { 0x75, "Sci" },
            { 0x78, "THQ" },
            { 0x79, "Accolade" },
            { 0x80, "Misawa" },
            { 0x83, "Lozc" },
            { 0x86, "Tokuma Shoten Intermedia" },
            { 0x87, "Tsukada Original" },
            { 0x91, "Chunsoft" },
            { 0x92, "Video system" },
            { 0x93, "Ocean/Acclaim" },
            { 0x95, "Varie" },
            { 0x96, "Yonezawa/s'pal" },
            { 0x97, "Kaneko" },
            { 0x99, "Pack in soft" },
            { 0xA4, "Konami (Yu-Gi-Oh!)" },
        };

        private readonly Dictionary<int, string> oldLicensee = new Dictionary<int, string>
        {
            { 0x00, "None" },
            { 0x01, "Nintendo" },
            { 0x08, "Capcom" },
            { 0x09, "HOT-B" },
            { 0x0A, "Jaleco" },
            { 0x0B, "Coconuts" },
            { 0x0C, "Elite Systems" },
            { 0x13, "Electronic Arts" },
            { 0x18, "Hudson Soft" },
            { 0x19, "ITC Entertainment" },
            { 0x1A, "Yanoman" },
            { 0x1D, "Clary" },
            { 0x1F, "Virgin" },
            { 0x20, "KSS" },
            { 0x24, "PCM Complete" },
            { 0x25, "San-X" },
            { 0x28, "Kotobuki Systems" },
            { 0x29, "SETA" },
            { 0x30, "Infogrames" },
            { 0x31, "Nintendo" },
            { 0x32, "Bandai" },
            { 0x34, "Konami" },
            { 0x35, "Hector" },
            { 0x38, "Capcom" },
            { 0x39, "Banpresto" },
            { 0x3C, "Entertainment i(truncated)" },
            { 0x3E, "Gremlin" },
            { 0x41, "Ubisoft" },
            { 0x42, "Atlus" },
            { 0x44, "Malibu" },
            { 0x46, "Angel" },
            { 0x47, "Spectrum Holoby" },
            { 0x49, "Irem" },
            { 0x4A, "Virgin" },
            { 0x4D, "Malibu" },
            { 0x4F, "U.S. Gold" },
            { 0x50, "Absolute" },
            { 0x51, "Acclaim" },
            { 0x52, "Activision" },
            { 0x53, "American Sammy" },
            { 0x54, "GameTek" },
            { 0x55, "Park Place" },
            { 0x56, "LJN" },
            { 0x57, "Matchbox" },
            { 0x59, "Milton Bradley" },
            { 0x5A, "Mindscape" },
            { 0x5B, "Romstar" },
            { 0x5C, "Naxat Soft" },
            { 0x5D, "Tradewest" },
            { 0x60, "Titus" },
            { 0x61, "Virgin" },
            { 0x67, "Ocean" },
            { 0x69, "Electronic Arts" },
            { 0x6E, "Elite Systems" },
            { 0x6F, "Electro Brain" },
            { 0x70, "Infogrammes" },
            { 0x71, "Interplay" },
            { 0x72, "Brøderbund" },
            { 0x73, "Sculptered Soft" },
            { 0x75, "The Sales Curve" },
            { 0x78, "THQ" },
            { 0x79, "Accolade" },
            { 0x7A, "Triffix Entertainment" },
            { 0x7C, "Microprose" },
            { 0x7F, "Kemco" },
            { 0x80, "Misawa Entertainment" },
            { 0x83, "LOZC" },
            { 0x86, "Tokuma Shoten Intermedia" },
            { 0x8B, "Bullet-Proof Software" },
            { 0x8C, "Vic Tokai" },
            { 0x8E, "Ape" },
            { 0x8F, "I'Max" },
            { 0x91, "Chun Soft" },
            { 0x92, "Video System" },
            { 0x93, "Tsuburava" },
            { 0x95, "Varie" },
            { 0x96, "Yonezawa/S'Pal" },
            { 0x97, "Kaneko" },
            { 0x99, "Arc" },
            { 0x9A, "Nihon Bussan" },
            { 0x9B, "Tecmo" },
            { 0x9C, "Imagineer" },
            { 0x9D, "Banpresto" },
            { 0x9F, "Nova" },
            { 0xA1, "Hori Electric" },
            { 0xA2, "Bandai" },
            { 0xA4, "Konami" },
            { 0xA6, "Kawada" },
            { 0xA7, "Takara" },
            { 0xA9, "Technos Japan" },
            { 0xAA, "Brøderbund" },
            { 0xAC, "Toei Animation" },
            { 0xAD, "Toho" },
            { 0xAF, "Namco" },
            { 0xB0, "Acclaim" },
            { 0xB1, "Ascii/Nexoft" },
            { 0xB2, "Bandai" },
            { 0xB4, "Enix" },
            { 0xB6, "HAL" },
            { 0xB7, "SNK" },
            { 0xB9, "pony Canyon" },
            { 0xBA, "culture brain o(truncated)" },
            { 0xBB, "Sunsoft" },
            { 0xBD, "Sony Imagesoft" },
            { 0xBF, "Sammy" },
            { 0xC0, "Taito" },
            { 0xC2, "Kemco" },
            { 0xC3, "Squaresoft" },
            { 0xC4, "Tokuma Shoten Intermedia" },
            { 0xC5, "Data East" },
            { 0xC6, "Tonkin House" },
            { 0xC8, "Koei" },
            { 0xC9, "UFL" },
            { 0xCA, "Eltra" },
            { 0xCB, "VAP" },
            { 0xCC, "USE" },
            { 0xCD, "Meldac" },
            { 0xCE, "Pony Canyon" },
            { 0xCF, "Angel" },
            { 0xD0, "Taito" },
            { 0xD1, "Sofel" },
            { 0xD2, "Quest" },
            { 0xD3, "Sigma Enterprises" },
            { 0xD4, "Ask Kodansha" },
            { 0xD6, "Naxat Soft" },
            { 0xD7, "Copya Systems" },
            { 0xD9, "Banpresto" },
            { 0xDA, "Tomy" },
            { 0xDB, "LJN" },
            { 0xDD, "NCS" },
            { 0xDE, "Human" },
            { 0xDF, "Altron" },
            { 0xE0, "Jaleco" },
            { 0xE1, "Towachiki" },
            { 0xE2, "Uutaka" },
            { 0xE3, "Varie" },
            { 0xE5, "Epoch" },
            { 0xE7, "Athena" },
            { 0xE8, "Asmik" },
            { 0xE9, "Natsume" },
            { 0xEA, "King Records" },
            { 0xEB, "Atlus" },
            { 0xEC, "Epic/Sony Secords" },
            { 0xEE, "IGS" },
            { 0xF0, "A Wave" },
            { 0xF3, "Extreme Entertainment" },
            { 0xFF, "LJN" },
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Unecessary")]
        public enum CGBType
        {
            No = 0x00,
            Both = 0x80,
            Yes = 0xC0,
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Unecessary")]
        public enum CartridgeType
        {
            ROM_ONLY = 0x00,
            MBC1 = 0x01,
            MBC1_RAM = 0x02,
            MBC1_RAM_BATTERY = 0x03,
            MBC2 = 0x05,
            MBC2_BATTERY = 0x06,
            ROM_RAM = 0x08,
            ROM_RAM_BATTERY = 0x09,
            MMM01 = 0x0B,
            MMM01_RAM = 0x0C,
            MMM01_RAM_BATTERY = 0x0D,
            MBC3_TIMER_BATTERY = 0x0F,
            MBC3_TIMER_RAM_BATTERY = 0x10,
            MBC3 = 0x11,
            MBC3_RAM = 0x12,
            MBC3_RAM_BATTERY = 0x13,
            MBC4 = 0x15,
            MBC4_RAM = 0x16,
            MBC4_RAM_BATTERY = 0x17,
            MBC5 = 0x19,
            MBC5_RAM = 0x1A,
            MBC5_RAM_BATTERY = 0x1B,
            MBC5_RUMBLE = 0x1C,
            MBC5_RUMBLE_RAM = 0x1D,
            MBC5_RUMBLE_RAM_BATTERY = 0x1E,
            MBC6 = 0x20,
            MBC7_SENSOR_RUMBLE_RAM_BATTERY = 0x22,
            POCKET_CAMERA = 0xFC,
            BANDAI_TAMAS = 0xFD,
            HUC3 = 0xFE,
            HUC1_RAM_BATTERY = 0xFF,
        }

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
        public string hTitle { get; set; }

        public CartridgeType hType { get; set; }

        public int hRomSize { get; set; }

        public int hRamSize { get; set; }

        public CGBType hCGB { get; set; }

        public string hLicensee { get; set; }

        public bool hSGB { get; set; }

        public bool hDestination { get; set; }

        public byte hMaskRomVersion { get; set; }

        public byte hHeaderChecksum { get; set; }

        public int hGlobalChecksum { get; set; }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter

        public static string GetRomString(byte[] romFile, int offset, int maxLength)
        {
            return Encoding.ASCII.GetString(romFile[offset..(offset + maxLength)].TakeWhile(b => b != 0x80 && b != 0xC0 && b != 0x00).ToArray());
        }

        public void ReadHeader(byte[] romFile)
        {
            this.hTitle = ROM.GetRomString(romFile, OFFSET_TITLE, 16);

            this.hCGB = romFile[OFFSET_CGB] switch
            {
                (byte)CGBType.Both => CGBType.Both,
                (byte)CGBType.Yes => CGBType.Yes,
                _ => CGBType.No,
            };

            if (romFile[OFFSET_OLD_LICENSEE_CODE] == 0x33)
            {
                int licenseeNumber = ((romFile[OFFSET_NEW_LICENSEE_CODE] & 0b1111) << 4) | romFile[OFFSET_NEW_LICENSEE_CODE + 1] & 0b1111;

                this.hLicensee = this.newLicensee.ContainsKey(licenseeNumber) ? this.newLicensee[licenseeNumber] : $"Unknown (0x{licenseeNumber:X2})";
            }
            else
            {
                int licenseeNumber = romFile[OFFSET_OLD_LICENSEE_CODE];

                this.hLicensee = this.oldLicensee.ContainsKey(licenseeNumber) ? this.oldLicensee[licenseeNumber] : $"Unknown (0x{licenseeNumber:X2})";
            }

            this.hSGB = romFile[OFFSET_SGB] == 0x03;

            this.hType = (CartridgeType)romFile[OFFSET_TYPE];

            this.hRomSize = 0x8000 << romFile[OFFSET_ROM_SIZE];

            this.hRamSize = (this.hType == CartridgeType.MBC2 || this.hType == CartridgeType.MBC2_BATTERY) ? 0x100 : this.ramSizeTable[romFile[OFFSET_RAM_SIZE]];

            this.hDestination = romFile[OFFSET_DESTINATION_CODE] == 0x01;

            this.hMaskRomVersion = romFile[OFFSET_MASKROM_VERSION];

            this.hHeaderChecksum = romFile[OFFSET_HEADER_CHECKSUM];

            this.hGlobalChecksum = (romFile[OFFSET_GLOBAL_CHECKSUM] << 8) | romFile[OFFSET_GLOBAL_CHECKSUM + 1];
        }
    }
}

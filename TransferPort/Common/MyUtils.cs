using System.Linq;
using System.Text;
using TUNING;

namespace RsTransferPort {
    public static class MyUtils {
        public static MyIdGenerate ID = new MyIdGenerate();

        public static string UniqueSaveName(string name) {
            return name + "-" + ID.Next();
        }

        public static BuildingDef BaseBuildingDef(
            string id,
            string anim,
            float[] construction_mass,
            string[] construction_materials
        ) {
            var buildingDef = global::BuildingTemplates.CreateBuildingDef(
                id,
                1,
                1,
                anim,
                30,
                15,
                construction_mass,
                construction_materials,
                2400f,
                BuildLocationRule.Anywhere,
                BUILDINGS.DECOR.NONE,
                NOISE_POLLUTION.NONE
            );
            buildingDef.DefaultAnimState = "idle";
            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            return buildingDef;
        }
    }


    public class MyIdGenerate {
        private const string ID_T = "0123456789abcdefghijklnmopqrstuvwxyzABCDEFGHIJKLNMOPQRSTUVWXYZ";
        private const int ID_LEN = 62;

        private int nextBit;
        private long lastTime;

        public static long CurrentSecond => System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //(System.DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

        public static string NumberToAscall(long number) {
            var builder = new StringBuilder();
            while (number > 0) {
                var n = (int)(number % ID_LEN);
                number /= ID_LEN;
                builder.Append(ID_T[n]);
            }

            return new string(builder.ToString().Reverse().ToArray());
        }

        public string Next() {
            var currentSecond = CurrentSecond;

            if (lastTime == currentSecond) {
                nextBit++;
            }
            else {
                nextBit = 0;
                lastTime = currentSecond;
            }

            var id = currentSecond * 100 + nextBit;

            return NumberToAscall(id);
        }
    }
}
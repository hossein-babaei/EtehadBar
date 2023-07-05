using EtehadBar.Domain;
using System.Collections.Generic;

namespace EtehadBar.Infra.Data
{
    public static class LoadFactorCreatorStaticData
    {
        public static List<OriginModel> GetOrigins()
        {
            return new List<OriginModel>()
            {
                new OriginModel() {Name = "جاده مخصوص"},
                new OriginModel() {Name = "جاده قدیم"},
                new OriginModel() {Name = "باقر شهر"},
                new OriginModel() {Name = "کردان"},
                new OriginModel() {Name = "شمس آباد"},
                new OriginModel() {Name = "گرم دره"},
                new OriginModel() {Name = "شهرک استقلال"},
                new OriginModel() {Name = "رباط کریم"},
                new OriginModel() {Name = "صفادشت"},
                new OriginModel() {Name = "شور آباد"}
            };
        }

        public static List<DestinationModel> GetDestinations()
        {
            return new List<DestinationModel>()
            {
                new DestinationModel() { Name = "خراسان"},//13500
                new DestinationModel() { Name = "کرمانشاه"},//10500
                new DestinationModel() { Name = "کاشان"},//7000
                new DestinationModel() { Name = "بابلسر"},//8000
                new DestinationModel() { Name = "تبریز"},//12000
                new DestinationModel() { Name = "رشت"},//9000
                new DestinationModel() { Name = "اصفهان"},//8500
                new DestinationModel() { Name = "پلاسکو قزوین"},//4500
                new DestinationModel() { Name = "خاوران"},//2500
            };
        }

        public static List<PriceRangeModel> GetPriceRanges()
        {
            return new List<PriceRangeModel> {
                new PriceRangeModel()
            {
                 Minimum = 150000000,
                 Maximum = 200000000,
                 Divider = 3
            },
                new PriceRangeModel()
            {
                 Minimum = 200000001,
                 Maximum = 300000000,
                 Divider = 4
            },
                new PriceRangeModel()
            {
                 Minimum = 300000001,
                 Maximum = 450000000,
                 Divider = 5
            },
                new PriceRangeModel()
            {
                 Minimum = 450000001,
                 Maximum = 550000000,
                 Divider = 6
            },
                new PriceRangeModel()
            {
                 Minimum = 550000001,
                 Maximum = 700000000,
                 Divider = 7
            },
                new PriceRangeModel()
            {
                 Minimum = 700000001,
                 Maximum = 750000000,
                 Divider = 8
            },
                new PriceRangeModel()
            {
                 Minimum = 750000001,
                 Maximum = 800000000,
                 Divider = 9
            },
                new PriceRangeModel()
            {
                 Minimum = 800000001,
                 Maximum = 850000000,
                 Divider = 10
            },
                new PriceRangeModel()
            {
                 Minimum = 850000001,
                 Maximum = 900000000,
                 Divider = 11
            },
                new PriceRangeModel()
            {
                 Minimum = 900000001,
                 Maximum = 950000000,
                 Divider = 12
            },
                new PriceRangeModel()
            {
                 Minimum = 950000001,
                 Maximum = 1050000000,
                 Divider = 13
            },
                new PriceRangeModel()
            {
                 Minimum = 1050000001,
                 Maximum = 1200000000,
                 Divider = 14
            },
                new PriceRangeModel()
            {
                 Minimum = 1200000001,
                 Maximum = 1400000000,
                 Divider = 15
            },
                new PriceRangeModel()
            {
                 Minimum = 1400000001,
                 Maximum = 1500000000,
                 Divider = 16
            },
                new PriceRangeModel()
            {
                 Minimum = 1500000001,
                 Maximum = 1550000000,
                 Divider = 17
            },
                new PriceRangeModel()
            {
                 Minimum = 1550000001,
                 Maximum = 1650000000,
                 Divider = 18
            },
                new PriceRangeModel()
            {
                 Minimum = 1650000001,
                 Maximum = 1750000000,
                 Divider = 19
            },
                new PriceRangeModel()
            {
                 Minimum = 1750000001,
                 Maximum = 1850000000,
                 Divider = 20
            },
                new PriceRangeModel()
            {
                 Minimum = 1850000001,
                 Maximum = 1950000000,
                 Divider = 22
            },
                new PriceRangeModel()
            {
                 Minimum = 1950000001,
                 Maximum = 2050000000,
                 Divider = 24
            },
                new PriceRangeModel()
            {
                 Minimum = 2050000001,
                 Maximum = 2500000000,
                 Divider = 28
            },
                new PriceRangeModel()
            {
                 Minimum = 2500000001,
                 Maximum = 2900000000,
                 Divider = 34
            },
                new PriceRangeModel()
            {
                 Minimum = 2900000001,
                 Maximum = 3500000000,
                 Divider = 40
            },
            };
        }
    }
}

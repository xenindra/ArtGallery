using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ArtGallery.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Years = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Style = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paintings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Author = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Style = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Materials = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Size = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paintings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaintingId = table.Column<int>(type: "integer", nullable: false),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_Paintings_PaintingId",
                        column: x => x.PaintingId,
                        principalTable: "Paintings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Bio", "Country", "Name", "PhotoUrl", "Style", "Years" },
                values: new object[,]
                {
                    { 1, "Испанский художник-сюрреалист, один из самых известных представителей этого направления. Его работы отличаются фантастическими образами и виртуозной живописной техникой.", "Испания", "Сальвадор Дали", "/images/authors/dali_author.jpg", "Сюрреализм", "1904–1989" },
                    { 2, "Русский художник-пейзажист, академик. Один из крупнейших мастеров реалистической пейзажной живописи. Его картины отличаются тщательной прорисовкой деталей природы.", "Россия", "Иван Шишкин", "/images/authors/shishkin_author.jpg", "Реализм", "1832–1898" },
                    { 3, "Нидерландский художник-постимпрессионист. За свою короткую жизнь создал более 2000 произведений. Его экспрессивная манера оказала огромное влияние на всё западное искусство.", "Нидерланды", "Винсент Ван Гог", "/images/authors/vangogh_author.jpg", "Постимпрессионизм", "1853–1890" },
                    { 4, "Русский живописец и график, мастер портрета. Один из крупнейших русских художников рубежа XIX–XX веков. Прославился портретами представителей российской интеллигенции.", "Россия", "Валентин Серов", "/images/authors/serov_author.jpg", "Реализм", "1865–1911" },
                    { 5, "Испанский художник, скульптор, график. Основатель кубизма. Один из самых известных и влиятельных художников XX века. Создал более 20 000 произведений.", "Испания", "Пабло Пикассо", "/images/authors/picasso_author.jpg", "Кубизм", "1881–1973" },
                    { 6, "Французский живописец, основоположник импрессионизма. Работал преимущественно на пленэре. Создал знаменитые серии «Стога сена», «Руанский собор», «Кувшинки».", "Франция", "Клод Моне", "/images/authors/monet_author.jpg", "Импрессионизм", "1840–1926" },
                    { 7, "Русский и советский художник, педагог, теоретик искусства. Основатель супрематизма — одного из наиболее ранних проявлений абстрактного искусства.", "Россия", "Казимир Малевич", "/images/authors/malevich_author.jpg", "Супрематизм", "1879–1935" },
                    { 8, "Норвежский живописец и график, театральный художник. Один из первых представителей экспрессионизма. Его работы оказали значительное влияние на развитие этого направления.", "Норвегия", "Эдвард Мунк", "/images/authors/munch_author.jpg", "Экспрессионизм", "1863–1944" },
                    { 9, "Итальянский художник, учёный, изобретатель, писатель, музыкант эпохи Высокого Возрождения. Гений, совместивший в себе художника и учёного.", "Италия", "Леонардо да Винчи", "/images/authors/davinci_author.jpg", "Возрождение", "1452–1519" },
                    { 10, "Русский живописец, педагог. Один из крупнейших мастеров русского реализма. Создал обширную галерею портретов современников, а также жанровые и исторические картины.", "Россия", "Илья Репин", "/images/authors/repin_author.jpg", "Реализм", "1844–1930" },
                    { 11, "Американский художник, известный своим стилем «искусственный реализм». Сочетает классические живописные техники с гротескными, юмористическими образами.", "США", "Джордж Кондо", "/images/authors/condo_avatar.jpg", "Поп-арт", "1957–н.в." },
                    { 12, "Японский художник и предприниматель. Создатель стиля «суперфлат». Работы сочетают традиционные японские мотивы с западной поп-культурой.", "Япония", "Такаси Мураками", "/images/authors/murakami_avatar.jpg", "Поп-арт", "1962–н.в." },
                    { 13, "Российский художник, представитель московского акционизма и постсоветского искусства. Работает с образами архитектурных памятников, используя нетрадиционные материалы.", "Россия", "Валерий Кошляков", "/images/authors/koshlyakov_avatar.jpg", "Современное искусство", "1962–н.в." }
                });

            migrationBuilder.InsertData(
                table: "Paintings",
                columns: new[] { "Id", "Author", "Country", "Description", "ImageUrl", "Materials", "Size", "Style", "Title", "Year" },
                values: new object[,]
                {
                    { 1, "Сальвадор Дали", "Испания", "\"Постоянство памяти\" — одна из самых известных работ Сальвадора Дали. На картине изображены мягкие, плавящиеся карманные часы на фоне пустынного пейзажа. Символ сюрреализма и размышление о гибкости времени.", "/images/paintings/collection1.jpg", "Масло, холст", "24 × 33 см", "Сюрреализм", "Постоянство памяти", 1931 },
                    { 2, "Иван Шишкин", "Россия", "Картина «Утро в сосновом лесу» — одно из самых известных произведений Ивана Шишкина. Художник мастерски изобразил утренний лес, пронизанный солнечным светом. Медведи дописаны Константином Савицким.", "/images/paintings/collection2.jpg", "Масло, холст", "139 × 213 см", "Реализм", "Утро в сосновом лесу", 1889 },
                    { 3, "Винсент Ван Гог", "Нидерланды", "\"Звёздная ночь\" — одна из самых узнаваемых картин Ван Гога. Художник изобразил ночной пейзаж с динамичными вихревыми облаками и яркими звёздами.", "/images/paintings/painting3.jpg", "Масло, холст", "73.7 × 92.1 см", "Постимпрессионизм", "Звёздная ночь", 1889 },
                    { 4, "Валентин Серов", "Россия", "\"Девочка с персиками\" — знаменитая картина Серова, написанная в усадьбе Абрамцево. На полотне изображена Вера Мамонтова. Эталон русского реализма.", "/images/paintings/painting4.jpg", "Масло, холст", "91 × 85 см", "Реализм", "Девочка с персиками", 1887 },
                    { 5, "Пабло Пикассо", "Испания", "\"Герника\" — монументальное полотно Пикассо, созданное в ответ на бомбардировку баскского города. Символ протеста против войны и насилия.", "/images/paintings/painting5.jpg", "Масло, холст", "349 × 776 см", "Кубизм", "Герника", 1937 },
                    { 6, "Клод Моне", "Франция", "Картина Клода Моне, давшая название всему направлению импрессионизма. Художник изобразил гавань Гавра в утреннем тумане.", "/images/paintings/painting6.jpg", "Масло, холст", "48 × 63 см", "Импрессионизм", "Впечатление. Восходящее солнце", 1872 },
                    { 7, "Казимир Малевич", "Россия", "\"Чёрный квадрат\" — самая известная работа Малевича, ставшая иконой супрематизма и всего русского авангарда.", "/images/paintings/painting7.jpg", "Масло, холст", "79.5 × 79.5 см", "Супрематизм", "Чёрный квадрат", 1915 },
                    { 8, "Эдвард Мунк", "Норвегия", "\"Крик\" — знаменитая картина Эдварда Мунка, выражающая экзистенциальный ужас современного человека.", "/images/paintings/painting8.jpg", "Масло, темпера, картон", "91 × 73.5 см", "Экспрессионизм", "Крик", 1893 },
                    { 9, "Леонардо да Винчи", "Италия", "\"Мона Лиза\" — всемирно известный портрет кисти Леонардо да Винчи, эталон портретной живописи эпохи Возрождения.", "/images/paintings/painting9.jpg", "Масло, тополь", "77 × 53 см", "Возрождение", "Мона Лиза", 1503 },
                    { 10, "Винсент Ван Гог", "Нидерланды", "\"Подсолнухи\" — серия картин Ван Гога с подсолнухами в вазе. Яркие цвета и эмоциональная насыщенность делают их узнаваемыми во всём мире.", "/images/paintings/painting10.jpg", "Масло, холст", "92 × 73 см", "Постимпрессионизм", "Подсолнухи", 1888 },
                    { 11, "Пабло Пикассо", "Испания", "\"Авиньонские девицы\" — новаторская работа Пикассо, предтеча кубизма. Радикальный разрыв с традиционной перспективой.", "/images/paintings/painting11.jpg", "Масло, холст", "243.9 × 233.7 см", "Кубизм", "Авиньонские девицы", 1907 },
                    { 12, "Илья Репин", "Россия", "\"Бурлаки на Волге\" — знаменитая картина Репина, изображающая группу бурлаков. Символ социальной несправедливости.", "/images/paintings/painting12.jpg", "Масло, холст", "131.5 × 281 см", "Реализм", "Бурлаки на Волге", 1873 },
                    { 13, "Джордж Кондо", "США", "\"Буги-Вуги\" — работа американского художника Джорджа Кондо в стиле «искусственный реализм». Сочетает гротескные формы с элементами поп-арта.", "/images/paintings/condo_boogie.jpg", "Акрил, холст", "203 × 183 см", "Поп-арт", "Буги-Вуги", 2018 },
                    { 14, "Такаси Мураками", "Япония", "\"Цветы 727-727\" — работа Такаси Мураками. Яркие улыбающиеся цветы сочетают японскую эстетику с современной поп-культурой.", "/images/paintings/murakami_flowers.jpg", "Акрил, холст", "100 × 100 см", "Поп-арт", "Цветы 727-727", 2022 },
                    { 15, "Валерий Кошляков", "Россия", "«Венеция. Палаццо» — работа Кошлякова. Художник использует нетрадиционные материалы для монументальных изображений архитектурных памятников.", "/images/paintings/koshlyakov_venice.jpg", "Картон, темпера", "120 × 80 см", "Современное искусство", "Венеция. Палаццо", 2019 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_PaintingId_SessionId",
                table: "Favorites",
                columns: new[] { "PaintingId", "SessionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Paintings");
        }
    }
}

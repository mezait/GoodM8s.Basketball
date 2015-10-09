using System;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace GoodM8s.Basketball {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            // Seasons
            SchemaBuilder.CreateTable("SeasonPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("Title", c => c.WithLength(50))
                .Column<bool>("IsActive"));

            ContentDefinitionManager.AlterPartDefinition("SeasonPart", part => part.Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("Season", type => type.WithPart("SeasonPart"));

            // Sports
            SchemaBuilder.CreateTable("SportPartRecord", table => table
                .ContentPartRecord()
                .Column<int>("SeasonId")
                .Column<string>("Name", c => c.WithLength(50))
                .Column<int>("FiXiId")
                .Column<DateTime>("StartDate")
                .Column<int>("WeekOffset"));

            SchemaBuilder.CreateForeignKey("Sport_Season", "SportPartRecord", new[] {"SeasonId"}, "SeasonPartRecord", new[] {"Id"});

            ContentDefinitionManager.AlterPartDefinition("SportPart", part => part.Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("Sport", type => type.WithPart("SportPart"));

            return 1;
        }

        public int UpdateFrom1() {
            // Players
            SchemaBuilder.CreateTable("PlayerPartRecord", table => table
                .ContentPartRecord()
                .Column<string>("FirstName", c => c.WithLength(50))
                .Column<string>("LastName", c => c.WithLength(50))
                .Column<int>("Number"));

            ContentDefinitionManager.AlterPartDefinition("PlayerPart", part => part.Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("Player", type => type
                .WithPart("PlayerPart")
                .WithPart("UserPart"));

            // Games
            SchemaBuilder.CreateTable("GamePartRecord", table => table
                .ContentPartRecord()
                .Column<int>("SportId")
                .Column<DateTime>("Date"));

            SchemaBuilder.CreateForeignKey("Game_Sport", "GamePartRecord", new[] {"SportId"}, "SportPartRecord", new[] {"Id"});

            ContentDefinitionManager.AlterPartDefinition("GamePart", part => part.Attachable(false));

            ContentDefinitionManager.AlterTypeDefinition("Game", type => type.WithPart("GamePart"));

            // Game statistics
            SchemaBuilder.CreateTable("GameStatisticRecord", table => table
                .Column<int>("Id", column => column.PrimaryKey().Identity())
                .Column<int>("GamePartRecord_id")
                .Column<int>("PlayerPartRecord_id")
                .Column<int>("FieldGoalsMade")
                .Column<int>("ThreeFieldGoalsMade")
                .Column<int>("FreeThrowsMade")
                .Column<int>("PersonalFouls")
                );

            SchemaBuilder.CreateForeignKey("GameStatistic_Game", "GameStatisticRecord", new[] {"GamePartRecord_id"}, "GamePartRecord", new[] {"Id"});
            SchemaBuilder.CreateForeignKey("GameStatistic_Player", "GameStatisticRecord", new[] {"PlayerPartRecord_id"}, "PlayerPartRecord", new[] {"Id"});

            return 2;
        }

        public int UpdateFrom2() {
            SchemaBuilder.AlterTable("SportPartRecord", table => table.AddColumn<int>("RoundCount"));
            SchemaBuilder.AlterTable("SportPartRecord", table => table.AddColumn<int>("IsActive"));

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable("GameStatisticRecord", table => table.AddColumn<int>("TechFouls"));

            return 4;
        }
    }
}
// <auto-generated />
using DAL.EfStructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.EfStructures.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220904130443_ReplacedManyToMany")]
    partial class ReplacedManyToMany
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Models.Department", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("DepartmentMainName")
                        .HasColumnType("character varying(50)");

                    b.HasKey("Name");

                    b.HasIndex("DepartmentMainName");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Domain.Models.Department", b =>
                {
                    b.HasOne("Domain.Models.Department", "DepartmentMain")
                        .WithMany("Subdepartments")
                        .HasForeignKey("DepartmentMainName");

                    b.Navigation("DepartmentMain");
                });

            modelBuilder.Entity("Domain.Models.Department", b =>
                {
                    b.Navigation("Subdepartments");
                });
#pragma warning restore 612, 618
        }
    }
}

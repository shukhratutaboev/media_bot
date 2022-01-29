﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using media_bot.Entities;

#nullable disable

namespace media_bot.Migrations
{
    [DbContext(typeof(BotDbContext))]
    [Migration("20220129124804_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("media_bot.Entities.User", b =>
                {
                    b.Property<long>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("InProcess")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ChatId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}

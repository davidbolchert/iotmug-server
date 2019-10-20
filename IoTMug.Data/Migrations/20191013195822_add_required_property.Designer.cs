﻿// <auto-generated />
using System;
using IoTMug.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IoTMug.Data.Migrations
{
    [DbContext(typeof(IoTMugContext))]
    [Migration("20191013195822_add_required_property")]
    partial class add_required_property
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IoTMug.Core.Device", b =>
                {
                    b.Property<Guid>("DeviceId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<Guid?>("DeviceTypeId");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("PfxCertificate");

                    b.Property<string>("TwinData");

                    b.HasKey("DeviceId");

                    b.HasIndex("DeviceTypeId");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("IoTMug.Core.DeviceType", b =>
                {
                    b.Property<Guid>("DeviceTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DefaultTwinData")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("DeviceTypeId");

                    b.ToTable("DeviceTypes");
                });

            modelBuilder.Entity("IoTMug.Core.Device", b =>
                {
                    b.HasOne("IoTMug.Core.DeviceType", "Type")
                        .WithMany()
                        .HasForeignKey("DeviceTypeId");
                });
#pragma warning restore 612, 618
        }
    }
}
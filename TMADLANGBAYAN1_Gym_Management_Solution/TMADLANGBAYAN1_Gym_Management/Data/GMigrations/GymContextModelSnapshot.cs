﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TMADLANGBAYAN1_Gym_Management.Data;

#nullable disable

namespace TMADLANGBAYAN1_Gym_Management.Data.GMigrations
{
    [DbContext(typeof(GymContext))]
    partial class GymContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.ClassTime", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("StartTime")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("ClassTimes");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Client", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DOB")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<bool>("FeePaid")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<string>("HealthCondition")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(55)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("MembershipEndDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("MembershipFee")
                        .HasColumnType("decimal(9,2)");

                    b.Property<int>("MembershipNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("MembershipStartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("MembershipTypeID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .HasMaxLength(2000)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("MembershipNumber")
                        .IsUnique();

                    b.HasIndex("MembershipTypeID");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Enrollment", b =>
                {
                    b.Property<int>("ClientID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupClassID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ClientID", "GroupClassID");

                    b.HasIndex("GroupClassID");

                    b.ToTable("Enrollments");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.FitnessCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("FitnessCategories");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.GroupClass", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ClassTimeID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("TEXT");

                    b.Property<int>("DOW")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<int>("FitnessCategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InstructorID")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("BLOB");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("FitnessCategoryID");

                    b.HasIndex("InstructorID");

                    b.HasIndex("ClassTimeID", "DOW", "InstructorID")
                        .IsUnique();

                    b.ToTable("GroupClasses");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Instructor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("HireDate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(55)
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Instructors");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.MembershipType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("StandardFee")
                        .HasColumnType("decimal(9,2)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("MembershipTypes");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Client", b =>
                {
                    b.HasOne("TMADLANGBAYAN1_Gym_Management.Models.MembershipType", "MembershipType")
                        .WithMany("Clients")
                        .HasForeignKey("MembershipTypeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MembershipType");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Enrollment", b =>
                {
                    b.HasOne("TMADLANGBAYAN1_Gym_Management.Models.Client", "Client")
                        .WithMany("Enrollments")
                        .HasForeignKey("ClientID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TMADLANGBAYAN1_Gym_Management.Models.GroupClass", "GroupClass")
                        .WithMany("Enrollments")
                        .HasForeignKey("GroupClassID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Client");

                    b.Navigation("GroupClass");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.GroupClass", b =>
                {
                    b.HasOne("TMADLANGBAYAN1_Gym_Management.Models.ClassTime", "ClassTime")
                        .WithMany("GroupClasses")
                        .HasForeignKey("ClassTimeID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TMADLANGBAYAN1_Gym_Management.Models.FitnessCategory", "FitnessCategory")
                        .WithMany("GroupClasses")
                        .HasForeignKey("FitnessCategoryID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TMADLANGBAYAN1_Gym_Management.Models.Instructor", "Instructor")
                        .WithMany("GroupClasses")
                        .HasForeignKey("InstructorID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ClassTime");

                    b.Navigation("FitnessCategory");

                    b.Navigation("Instructor");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.ClassTime", b =>
                {
                    b.Navigation("GroupClasses");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Client", b =>
                {
                    b.Navigation("Enrollments");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.FitnessCategory", b =>
                {
                    b.Navigation("GroupClasses");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.GroupClass", b =>
                {
                    b.Navigation("Enrollments");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.Instructor", b =>
                {
                    b.Navigation("GroupClasses");
                });

            modelBuilder.Entity("TMADLANGBAYAN1_Gym_Management.Models.MembershipType", b =>
                {
                    b.Navigation("Clients");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using System;
using MVS_Noticias_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MVS_Noticias_API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20250103211407_tablesname")]
    partial class tablesname
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MVS_Noticias_API.Models.Currency.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AbsoluteChange")
                        .HasPrecision(19, 6)
                        .HasColumnType("decimal(19,6)");

                    b.Property<decimal>("ExchangeRate")
                        .HasPrecision(19, 6)
                        .HasColumnType("decimal(19,6)");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("GrowthPercentage")
                        .HasPrecision(19, 6)
                        .HasColumnType("decimal(19,6)");

                    b.Property<long>("Timestamp")
                        .HasColumnType("bigint");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirebaseUid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegisterDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.News.LastNews", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "autor");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "creador");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "fecha");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "descripcion");

                    b.Property<int>("IdAuthor")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id_autor");

                    b.Property<int>("IdCreator")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id_creador");

                    b.Property<int>("IdNews")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id_noticia");

                    b.Property<int>("IdSection")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id_seccion");

                    b.Property<int>("IdSubSection")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "id_subseccion");

                    b.Property<bool>("IsSound")
                        .HasColumnType("bit")
                        .HasAnnotation("Relational:JsonPropertyName", "isSound");

                    b.Property<bool>("IsVideo")
                        .HasColumnType("bit")
                        .HasAnnotation("Relational:JsonPropertyName", "isVideo");

                    b.Property<int>("Number")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "numero");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "foto");

                    b.Property<string>("PhotoCredits")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "foto_creditos");

                    b.Property<string>("PhotoDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "foto_descripcion");

                    b.Property<string>("PhotoMobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "foto_movil");

                    b.Property<string>("Section")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "seccion");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "slug");

                    b.Property<string>("SoundUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "SoundUrl");

                    b.Property<string>("SubSection")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "subseccion");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "titulo");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "Tipo");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "url");

                    b.Property<string>("VideoUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "videoUrl");

                    b.HasKey("Id");

                    b.ToTable("LastNews");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Programming.BroadcastInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Hour")
                        .HasColumnType("int");

                    b.Property<int>("Minute")
                        .HasColumnType("int");

                    b.Property<int>("ProgrammingId")
                        .HasColumnType("int");

                    b.Property<int>("Weekday")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProgrammingId");

                    b.ToTable("BroadcastInfo");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Programming.Programming", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BroadcastDay")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BroadcastHour")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlPersonalSite")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.SavedNews", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Creator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HiddenTags")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdAuthor")
                        .HasColumnType("int");

                    b.Property<int>("IdCreator")
                        .HasColumnType("int");

                    b.Property<int>("IdNews")
                        .HasColumnType("int");

                    b.Property<int>("IdSection")
                        .HasColumnType("int");

                    b.Property<int>("IdSubSection")
                        .HasColumnType("int");

                    b.Property<bool>("IsSound")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVideo")
                        .HasColumnType("bit");

                    b.Property<int>("NewsQuantity")
                        .HasColumnType("int");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoCredits")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoMobile")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Section")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SoundUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubSection")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tags")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("VideoUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SavedNews");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.SavedPodcasts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AudioPublicUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePublicUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Index")
                        .HasColumnType("int");

                    b.Property<string>("ProgramId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProgramName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("PublishedDurationSeconds")
                        .HasColumnType("real");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SavedPodcasts");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.SavedVideos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ViewsNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SavedVideos");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.UserNotifications", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsNew")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<int>("NewsId")
                        .HasColumnType("int");

                    b.Property<string>("RegisterDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Section")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Settings.AccessibilitySettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ApareanceMode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FontSize")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AccessibilitySettings");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Settings.CustomSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CDMXOrder")
                        .HasColumnType("int");

                    b.Property<int>("CienciaTecnologiaOrder")
                        .HasColumnType("int");

                    b.Property<int>("EconomiaOrder")
                        .HasColumnType("int");

                    b.Property<int>("EntretenimientoOrder")
                        .HasColumnType("int");

                    b.Property<int>("EntrevistasOrder")
                        .HasColumnType("int");

                    b.Property<int>("EstadosOrder")
                        .HasColumnType("int");

                    b.Property<int>("MVSDeportesOrder")
                        .HasColumnType("int");

                    b.Property<int>("MasLeidasOrder")
                        .HasColumnType("int");

                    b.Property<int>("MascotasOrder")
                        .HasColumnType("int");

                    b.Property<int>("MundoOrder")
                        .HasColumnType("int");

                    b.Property<int>("NacionalOrder")
                        .HasColumnType("int");

                    b.Property<int>("NuevoLeonOrder")
                        .HasColumnType("int");

                    b.Property<int>("OpinionOrder")
                        .HasColumnType("int");

                    b.Property<int>("PodcastOrder")
                        .HasColumnType("int");

                    b.Property<int>("PoliciacaOrder")
                        .HasColumnType("int");

                    b.Property<int>("ProgramacionOrder")
                        .HasColumnType("int");

                    b.Property<int>("SaludBienestarOrder")
                        .HasColumnType("int");

                    b.Property<int>("TendenciasOrder")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("VideosOrder")
                        .HasColumnType("int");

                    b.Property<int>("ViralOrder")
                        .HasColumnType("int");

                    b.Property<bool?>("isDefaultOrder")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("CustomSettings");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Settings.NotificationsSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("CDMX")
                        .HasColumnType("bit");

                    b.Property<bool>("CienciaTecnologia")
                        .HasColumnType("bit");

                    b.Property<bool>("Deportes")
                        .HasColumnType("bit");

                    b.Property<bool>("Economia")
                        .HasColumnType("bit");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Entretenimiento")
                        .HasColumnType("bit");

                    b.Property<bool>("Entrevistas")
                        .HasColumnType("bit");

                    b.Property<bool>("Estados")
                        .HasColumnType("bit");

                    b.Property<string>("Keywords")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Mascotas")
                        .HasColumnType("bit");

                    b.Property<bool>("Mundo")
                        .HasColumnType("bit");

                    b.Property<bool>("Nacional")
                        .HasColumnType("bit");

                    b.Property<bool>("Opinion")
                        .HasColumnType("bit");

                    b.Property<bool>("Policiaca")
                        .HasColumnType("bit");

                    b.Property<bool>("Programacion")
                        .HasColumnType("bit");

                    b.Property<bool>("SaludBienestar")
                        .HasColumnType("bit");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Tendencias")
                        .HasColumnType("bit");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<bool>("Videos")
                        .HasColumnType("bit");

                    b.Property<bool>("Viral")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("NotificationsSettings");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Programming.BroadcastInfo", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Programming.Programming", "Programming")
                        .WithMany("BroadcastDates")
                        .HasForeignKey("ProgrammingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Programming");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.SavedNews", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.SavedPodcasts", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.SavedVideos", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Saved.UserNotifications", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Settings.AccessibilitySettings", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Settings.CustomSettings", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Settings.NotificationsSettings", b =>
                {
                    b.HasOne("MVS_Noticias_API.Models.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MVS_Noticias_API.Models.Programming.Programming", b =>
                {
                    b.Navigation("BroadcastDates");
                });
#pragma warning restore 612, 618
        }
    }
}

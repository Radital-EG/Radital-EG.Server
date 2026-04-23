using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestedById",
                table: "ReportingRequests",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ReportingRequests_RequestedById",
                table: "ReportingRequests",
                column: "RequestedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportingRequests_HospitalStaffMembers_RequestedById",
                table: "ReportingRequests",
                column: "RequestedById",
                principalTable: "HospitalStaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportingRequests_HospitalStaffMembers_RequestedById",
                table: "ReportingRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReportingRequests_RequestedById",
                table: "ReportingRequests");

            migrationBuilder.DropColumn(
                name: "RequestedById",
                table: "ReportingRequests");
        }
    }
}
